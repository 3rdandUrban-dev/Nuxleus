﻿//-----------------------------------------------------------------------
// <copyright file="TokenProcessingErrorEventArgs.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace DotNetOpenAuth.InfoCard {
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;

	/// <summary>
	/// Arguments for the <see cref="InfoCardSelector.TokenProcessingError"/> event.
	/// </summary>
	public class TokenProcessingErrorEventArgs : EventArgs {
		/// <summary>
		/// Initializes a new instance of the <see cref="TokenProcessingErrorEventArgs"/> class.
		/// </summary>
		/// <param name="tokenXml">The token XML.</param>
		/// <param name="exception">The exception.</param>
		internal TokenProcessingErrorEventArgs(string tokenXml, Exception exception) {
			Contract.Requires(tokenXml != null);
			Contract.Requires(exception != null);
			this.TokenXml = tokenXml;
			this.Exception = exception;
		}

		/// <summary>
		/// Gets the raw token XML.
		/// </summary>
		public string TokenXml { get; private set; }

		/// <summary>
		/// Gets the exception that was generated while processing the token.
		/// </summary>
		public Exception Exception { get; private set; }

#if CONTRACTS_FULL
		/// <summary>
		/// Verifies conditions that should be true for any valid state of this object.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by code contracts.")]
		[ContractInvariantMethod]
		protected void ObjectInvariant() {
			Contract.Invariant(this.TokenXml != null);
			Contract.Invariant(this.Exception != null);
		}
#endif
	}
}
