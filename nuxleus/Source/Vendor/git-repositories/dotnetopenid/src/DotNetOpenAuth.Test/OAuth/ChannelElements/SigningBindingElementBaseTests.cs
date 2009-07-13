﻿//-----------------------------------------------------------------------
// <copyright file="SigningBindingElementBaseTests.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DotNetOpenAuth.Test.ChannelElements {
	using DotNetOpenAuth.Messaging;
	using DotNetOpenAuth.Messaging.Reflection;
	using DotNetOpenAuth.OAuth;
	using DotNetOpenAuth.OAuth.ChannelElements;
	using DotNetOpenAuth.OAuth.Messages;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class SigningBindingElementBaseTests : MessagingTestBase {
		[TestMethod]
		public void BaseSignatureStringTest() {
			// Tests a message sent by HTTP GET, with no query string included in the endpoint.
			UnauthorizedTokenRequest message = CreateTestRequestTokenMessage(
				this.MessageDescriptions,
				new MessageReceivingEndpoint("https://www.google.com/accounts/OAuthGetRequestToken", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest));
			Assert.AreEqual(
				"GET&https%3A%2F%2Fwww.google.com%2Faccounts%2FOAuthGetRequestToken&oauth_consumer_key%3Dnerdbank.org%26oauth_nonce%3Dfe4045a3f0efdd1e019fa8f8ae3f5c38%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1222665749%26oauth_version%3D1.0%26scope%3Dhttp%253A%252F%252Fwww.google.com%252Fm8%252Ffeeds%252F",
				SigningBindingElementBase_Accessor.ConstructSignatureBaseString(message, MessageDictionary_Accessor.AttachShadow(this.MessageDescriptions.GetAccessor(message))));

			// Test HTTP GET with an attached query string.  We're elevating the scope parameter to the query string
			// and removing it from the extradata dictionary.  This should NOT affect the base signature string.
			message = CreateTestRequestTokenMessage(
				this.MessageDescriptions,
				new MessageReceivingEndpoint("https://www.google.com/accounts/OAuthGetRequestToken?scope=http://www.google.com/m8/feeds/", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest));
			message.ExtraData.Remove("scope"); // remove it from ExtraData since we put it in the URL
			Assert.AreEqual(
				"GET&https%3A%2F%2Fwww.google.com%2Faccounts%2FOAuthGetRequestToken&oauth_consumer_key%3Dnerdbank.org%26oauth_nonce%3Dfe4045a3f0efdd1e019fa8f8ae3f5c38%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1222665749%26oauth_version%3D1.0%26scope%3Dhttp%253A%252F%252Fwww.google.com%252Fm8%252Ffeeds%252F",
				SigningBindingElementBase_Accessor.ConstructSignatureBaseString(message, MessageDictionary_Accessor.AttachShadow(this.MessageDescriptions.GetAccessor(message))));

			// Test HTTP POST, with query string as well
			message = CreateTestRequestTokenMessage(
				this.MessageDescriptions,
				new MessageReceivingEndpoint("https://www.google.com/accounts/OAuthGetRequestToken?scope=http://www.google.com/m8/feeds/", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.PostRequest));
			message.ExtraData.Remove("scope"); // remove it from ExtraData since we put it in the URL
			Assert.AreEqual(
				"GET&https%3A%2F%2Fwww.google.com%2Faccounts%2FOAuthGetRequestToken&oauth_consumer_key%3Dnerdbank.org%26oauth_nonce%3Dfe4045a3f0efdd1e019fa8f8ae3f5c38%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1222665749%26oauth_version%3D1.0%26scope%3Dhttp%253A%252F%252Fwww.google.com%252Fm8%252Ffeeds%252F",
				SigningBindingElementBase_Accessor.ConstructSignatureBaseString(message, MessageDictionary_Accessor.AttachShadow(this.MessageDescriptions.GetAccessor(message))));

			// Test HTTP POST, with query string, but not using the Authorization header
			message = CreateTestRequestTokenMessage(
				this.MessageDescriptions,
				new MessageReceivingEndpoint("https://www.google.com/accounts/OAuthGetRequestToken?scope=http://www.google.com/m8/feeds/", HttpDeliveryMethods.PostRequest));
			message.ExtraData.Remove("scope"); // remove it from ExtraData since we put it in the URL
			Assert.AreEqual(
				"GET&https%3A%2F%2Fwww.google.com%2Faccounts%2FOAuthGetRequestToken&oauth_consumer_key%3Dnerdbank.org%26oauth_nonce%3Dfe4045a3f0efdd1e019fa8f8ae3f5c38%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1222665749%26oauth_version%3D1.0%26scope%3Dhttp%253A%252F%252Fwww.google.com%252Fm8%252Ffeeds%252F",
				SigningBindingElementBase_Accessor.ConstructSignatureBaseString(message, MessageDictionary_Accessor.AttachShadow(this.MessageDescriptions.GetAccessor(message))));

			// This is a simulation of receiving the message, where the query string is still in the URL,
			// but has been read into ExtraData, so parameters in the query string appear twice.
			message = CreateTestRequestTokenMessage(
				this.MessageDescriptions,
				new MessageReceivingEndpoint("https://www.google.com/accounts/OAuthGetRequestToken?scope=http://www.google.com/m8/feeds/", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.PostRequest));
			Assert.AreEqual(
				"GET&https%3A%2F%2Fwww.google.com%2Faccounts%2FOAuthGetRequestToken&oauth_consumer_key%3Dnerdbank.org%26oauth_nonce%3Dfe4045a3f0efdd1e019fa8f8ae3f5c38%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1222665749%26oauth_version%3D1.0%26scope%3Dhttp%253A%252F%252Fwww.google.com%252Fm8%252Ffeeds%252F",
				SigningBindingElementBase_Accessor.ConstructSignatureBaseString(message, MessageDictionary_Accessor.AttachShadow(this.MessageDescriptions.GetAccessor(message))));
		}

		internal static UnauthorizedTokenRequest CreateTestRequestTokenMessage(MessageDescriptionCollection messageDescriptions, MessageReceivingEndpoint endpoint) {
			endpoint = endpoint ?? new MessageReceivingEndpoint("https://www.google.com/accounts/OAuthGetRequestToken", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest);
			UnauthorizedTokenRequest message = new UnauthorizedTokenRequest(endpoint, Protocol.V10.Version);
			message.ConsumerKey = "nerdbank.org";
			((ITamperResistantOAuthMessage)message).ConsumerSecret = "nerdbanksecret";
			var signedMessage = (ITamperResistantOAuthMessage)message;
			signedMessage.HttpMethod = "GET";
			signedMessage.SignatureMethod = "HMAC-SHA1";
			MessageDictionary dictionary = messageDescriptions.GetAccessor(message);
			dictionary["oauth_timestamp"] = "1222665749";
			dictionary["oauth_nonce"] = "fe4045a3f0efdd1e019fa8f8ae3f5c38";
			dictionary["scope"] = "http://www.google.com/m8/feeds/";
			return message;
		}
	}
}
