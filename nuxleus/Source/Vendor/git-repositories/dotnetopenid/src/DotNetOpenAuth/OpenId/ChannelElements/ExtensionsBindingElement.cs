﻿//-----------------------------------------------------------------------
// <copyright file="ExtensionsBindingElement.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DotNetOpenAuth.OpenId.ChannelElements {
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Text;
	using DotNetOpenAuth.Messaging;
	using DotNetOpenAuth.Messaging.Reflection;
	using DotNetOpenAuth.OpenId.Extensions;
	using DotNetOpenAuth.OpenId.Messages;
	using DotNetOpenAuth.OpenId.Provider;
	using DotNetOpenAuth.OpenId.RelyingParty;

	/// <summary>
	/// The binding element that serializes/deserializes OpenID extensions to/from
	/// their carrying OpenID messages.
	/// </summary>
	internal class ExtensionsBindingElement : IChannelBindingElement {
		/// <summary>
		/// The security settings that apply to this binding element.
		/// </summary>
		private readonly SecuritySettings securitySettings;

		/// <summary>
		/// The security settings that apply to this relying party, if it is a relying party.
		/// </summary>
		private readonly RelyingPartySecuritySettings relyingPartySecuritySettings;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExtensionsBindingElement"/> class.
		/// </summary>
		/// <param name="extensionFactory">The extension factory.</param>
		/// <param name="securitySettings">The security settings.</param>
		internal ExtensionsBindingElement(IOpenIdExtensionFactory extensionFactory, SecuritySettings securitySettings) {
			ErrorUtilities.VerifyArgumentNotNull(extensionFactory, "extensionFactory");
			ErrorUtilities.VerifyArgumentNotNull(securitySettings, "securitySettings");

			this.ExtensionFactory = extensionFactory;
			this.securitySettings = securitySettings;
			this.relyingPartySecuritySettings = securitySettings as RelyingPartySecuritySettings;
		}

		#region IChannelBindingElement Members

		/// <summary>
		/// Gets or sets the channel that this binding element belongs to.
		/// </summary>
		/// <value></value>
		/// <remarks>
		/// This property is set by the channel when it is first constructed.
		/// </remarks>
		public Channel Channel { get; set; }

		/// <summary>
		/// Gets the extension factory.
		/// </summary>
		public IOpenIdExtensionFactory ExtensionFactory { get; private set; }

		/// <summary>
		/// Gets the protection offered (if any) by this binding element.
		/// </summary>
		/// <value><see cref="MessageProtections.None"/></value>
		public MessageProtections Protection {
			get { return MessageProtections.None; }
		}

		/// <summary>
		/// Prepares a message for sending based on the rules of this channel binding element.
		/// </summary>
		/// <param name="message">The message to prepare for sending.</param>
		/// <returns>
		/// The protections (if any) that this binding element applied to the message.
		/// Null if this binding element did not even apply to this binding element.
		/// </returns>
		/// <remarks>
		/// Implementations that provide message protection must honor the
		/// <see cref="MessagePartAttribute.RequiredProtection"/> properties where applicable.
		/// </remarks>
		public MessageProtections? ProcessOutgoingMessage(IProtocolMessage message) {
			ErrorUtilities.VerifyArgumentNotNull(message, "message");
			ErrorUtilities.VerifyOperation(this.Channel != null, "Channel property has not been set.");

			var extendableMessage = message as IProtocolMessageWithExtensions;
			if (extendableMessage != null) {
				Protocol protocol = Protocol.Lookup(message.Version);
				MessageDictionary baseMessageDictionary = this.Channel.MessageDescriptions.GetAccessor(message);

				// We have a helper class that will do all the heavy-lifting of organizing
				// all the extensions, their aliases, and their parameters.
				var extensionManager = ExtensionArgumentsManager.CreateOutgoingExtensions(protocol);
				foreach (IExtensionMessage protocolExtension in extendableMessage.Extensions) {
					var extension = protocolExtension as IOpenIdMessageExtension;
					if (extension != null) {
						// Give extensions that require custom serialization a chance to do their work.
						var customSerializingExtension = extension as IMessageWithEvents;
						if (customSerializingExtension != null) {
							customSerializingExtension.OnSending();
						}

						// OpenID 2.0 Section 12 forbids two extensions with the same TypeURI in the same message.
						ErrorUtilities.VerifyProtocol(!extensionManager.ContainsExtension(extension.TypeUri), OpenIdStrings.ExtensionAlreadyAddedWithSameTypeURI, extension.TypeUri);

						var extensionDictionary = this.Channel.MessageDescriptions.GetAccessor(extension).Serialize();
						extensionManager.AddExtensionArguments(extension.TypeUri, extensionDictionary);
					} else {
						Logger.OpenId.WarnFormat("Unexpected extension type {0} did not implement {1}.", protocolExtension.GetType(), typeof(IOpenIdMessageExtension).Name);
					}
				}

				// We use a cheap trick (for now at least) to determine whether the 'openid.' prefix
				// belongs on the parameters by just looking at what other parameters do.
				// Technically, direct message responses from Provider to Relying Party are the only
				// messages that leave off the 'openid.' prefix.
				bool includeOpenIdPrefix = baseMessageDictionary.Keys.Any(key => key.StartsWith(protocol.openid.Prefix, StringComparison.Ordinal));

				// Add the extension parameters to the base message for transmission.
				baseMessageDictionary.AddExtraParameters(extensionManager.GetArgumentsToSend(includeOpenIdPrefix));
				return MessageProtections.None;
			}

			return null;
		}

		/// <summary>
		/// Performs any transformation on an incoming message that may be necessary and/or
		/// validates an incoming message based on the rules of this channel binding element.
		/// </summary>
		/// <param name="message">The incoming message to process.</param>
		/// <returns>
		/// The protections (if any) that this binding element applied to the message.
		/// Null if this binding element did not even apply to this binding element.
		/// </returns>
		/// <exception cref="ProtocolException">
		/// Thrown when the binding element rules indicate that this message is invalid and should
		/// NOT be processed.
		/// </exception>
		/// <remarks>
		/// Implementations that provide message protection must honor the
		/// <see cref="MessagePartAttribute.RequiredProtection"/> properties where applicable.
		/// </remarks>
		public MessageProtections? ProcessIncomingMessage(IProtocolMessage message) {
			var extendableMessage = message as IProtocolMessageWithExtensions;
			if (extendableMessage != null) {
				// First add the extensions that are signed by the Provider.
				foreach (IOpenIdMessageExtension signedExtension in this.GetExtensions(extendableMessage, true, null)) {
					signedExtension.IsSignedByRemoteParty = true;
					extendableMessage.Extensions.Add(signedExtension);
				}

				// Now search again, considering ALL extensions whether they are signed or not,
				// skipping the signed ones and adding the new ones as unsigned extensions.
				if (this.relyingPartySecuritySettings == null || !this.relyingPartySecuritySettings.IgnoreUnsignedExtensions) {
					Func<string, bool> isNotSigned = typeUri => !extendableMessage.Extensions.Cast<IOpenIdMessageExtension>().Any(ext => ext.TypeUri == typeUri);
					foreach (IOpenIdMessageExtension unsignedExtension in this.GetExtensions(extendableMessage, false, isNotSigned)) {
						unsignedExtension.IsSignedByRemoteParty = false;
						extendableMessage.Extensions.Add(unsignedExtension);
					}
				}

				return MessageProtections.None;
			}

			return null;
		}

		#endregion

		/// <summary>
		/// Gets the extensions on a message.
		/// </summary>
		/// <param name="message">The carrier of the extensions.</param>
		/// <param name="ignoreUnsigned">If set to <c>true</c> only signed extensions will be available.</param>
		/// <param name="extensionFilter">A optional filter that takes an extension type URI and 
		/// returns a value indicating whether that extension should be deserialized and 
		/// returned in the sequence.  May be null.</param>
		/// <returns>A sequence of extensions in the message.</returns>
		private IEnumerable<IOpenIdMessageExtension> GetExtensions(IProtocolMessageWithExtensions message, bool ignoreUnsigned, Func<string, bool> extensionFilter) {
			bool isAtProvider = message is SignedResponseRequest;

			// We have a helper class that will do all the heavy-lifting of organizing
			// all the extensions, their aliases, and their parameters.
			var extensionManager = ExtensionArgumentsManager.CreateIncomingExtensions(this.GetExtensionsDictionary(message, ignoreUnsigned));
			foreach (string typeUri in extensionManager.GetExtensionTypeUris()) {
				// Our caller may have already obtained a signed version of this extension,
				// so skip it if they don't want this one.
				if (extensionFilter != null && !extensionFilter(typeUri)) {
					continue;
				}

				var extensionData = extensionManager.GetExtensionArguments(typeUri);

				// Initialize this particular extension.
				IOpenIdMessageExtension extension = this.ExtensionFactory.Create(typeUri, extensionData, message, isAtProvider);
				if (extension != null) {
					MessageDictionary extensionDictionary = this.Channel.MessageDescriptions.GetAccessor(extension);
					foreach (var pair in extensionData) {
						extensionDictionary[pair.Key] = pair.Value;
					}

					// Give extensions that require custom serialization a chance to do their work.
					var customSerializingExtension = extension as IMessageWithEvents;
					if (customSerializingExtension != null) {
						customSerializingExtension.OnReceiving();
					}

					yield return extension;
				} else {
					Logger.OpenId.WarnFormat("Extension with type URI '{0}' ignored because it is not a recognized extension.", typeUri);
				}
			}
		}

		/// <summary>
		/// Gets the dictionary of message parts that should be deserialized into extensions.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="ignoreUnsigned">If set to <c>true</c> only signed extensions will be available.</param>
		/// <returns>
		/// A dictionary of message parts, including only signed parts when appropriate.
		/// </returns>
		private IDictionary<string, string> GetExtensionsDictionary(IProtocolMessage message, bool ignoreUnsigned) {
			Contract.Requires(this.Channel != null);
			ErrorUtilities.VerifyOperation(this.Channel != null, "Channel property has not been set.");

			IndirectSignedResponse signedResponse = message as IndirectSignedResponse;
			if (signedResponse != null && ignoreUnsigned) {
				return signedResponse.GetSignedMessageParts(this.Channel);
			} else {
				return this.Channel.MessageDescriptions.GetAccessor(message);
			}
		}
	}
}
