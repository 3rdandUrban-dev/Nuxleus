﻿//-----------------------------------------------------------------------
// <copyright file="HttpRequestInfo.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DotNetOpenAuth.Messaging {
	using System;
	using System.Collections.Specialized;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Globalization;
	using System.IO;
	using System.Net;
	using System.ServiceModel.Channels;
	using System.Web;

	/// <summary>
	/// A property store of details of an incoming HTTP request.
	/// </summary>
	/// <remarks>
	/// This serves a very similar purpose to <see cref="HttpRequest"/>, except that
	/// ASP.NET does not let us fully initialize that class, so we have to write one
	/// of our one.
	/// </remarks>
	public class HttpRequestInfo {
		/// <summary>
		/// The key/value pairs found in the entity of a POST request.
		/// </summary>
		private NameValueCollection form;

		/// <summary>
		/// The key/value pairs found in the querystring of the incoming request.
		/// </summary>
		private NameValueCollection queryString;

		/// <summary>
		/// Backing field for the <see cref="QueryStringBeforeRewriting"/> property.
		/// </summary>
		private NameValueCollection queryStringBeforeRewriting;

		/// <summary>
		/// Backing field for the <see cref="Message"/> property.
		/// </summary>
		private IDirectedProtocolMessage message;

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpRequestInfo"/> class.
		/// </summary>
		/// <param name="request">The ASP.NET structure to copy from.</param>
		public HttpRequestInfo(HttpRequest request) {
			Contract.Requires(request != null);
			ErrorUtilities.VerifyArgumentNotNull(request, "request");

			this.HttpMethod = request.HttpMethod;
			this.Url = request.Url;
			this.UrlBeforeRewriting = GetPublicFacingUrl(request);
			this.RawUrl = request.RawUrl;
			this.Headers = GetHeaderCollection(request.Headers);
			this.InputStream = request.InputStream;

			// These values would normally be calculated, but we'll reuse them from
			// HttpRequest since they're already calculated, and there's a chance (<g>)
			// that ASP.NET does a better job of being comprehensive about gathering
			// these as well.
			this.form = request.Form;
			this.queryString = request.QueryString;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpRequestInfo"/> class.
		/// </summary>
		/// <param name="httpMethod">The HTTP method (i.e. GET or POST) of the incoming request.</param>
		/// <param name="requestUrl">The URL being requested.</param>
		/// <param name="rawUrl">The raw URL that appears immediately following the HTTP verb in the request,
		/// before any URL rewriting takes place.</param>
		/// <param name="headers">Headers in the HTTP request.</param>
		/// <param name="inputStream">The entity stream, if any.  (POST requests typically have these).  Use <c>null</c> for GET requests.</param>
		public HttpRequestInfo(string httpMethod, Uri requestUrl, string rawUrl, WebHeaderCollection headers, Stream inputStream) {
			Contract.Requires(!string.IsNullOrEmpty(httpMethod));
			Contract.Requires(requestUrl != null);
			Contract.Requires(rawUrl != null);
			Contract.Requires(headers != null);
			ErrorUtilities.VerifyNonZeroLength(httpMethod, "httpMethod");
			ErrorUtilities.VerifyArgumentNotNull(requestUrl, "requestUrl");
			ErrorUtilities.VerifyArgumentNotNull(rawUrl, "rawUrl");
			ErrorUtilities.VerifyArgumentNotNull(headers, "headers");

			this.HttpMethod = httpMethod;
			this.Url = requestUrl;
			this.UrlBeforeRewriting = requestUrl;
			this.RawUrl = rawUrl;
			this.Headers = headers;
			this.InputStream = inputStream;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpRequestInfo"/> class.
		/// </summary>
		/// <param name="listenerRequest">Details on the incoming HTTP request.</param>
		public HttpRequestInfo(HttpListenerRequest listenerRequest) {
			Contract.Requires(listenerRequest != null);
			ErrorUtilities.VerifyArgumentNotNull(listenerRequest, "listenerRequest");

			this.HttpMethod = listenerRequest.HttpMethod;
			this.Url = listenerRequest.Url;
			this.UrlBeforeRewriting = listenerRequest.Url;
			this.RawUrl = listenerRequest.RawUrl;
			this.Headers = new WebHeaderCollection();
			foreach (string key in listenerRequest.Headers) {
				this.Headers[key] = listenerRequest.Headers[key];
			}

			this.InputStream = listenerRequest.InputStream;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpRequestInfo"/> class.
		/// </summary>
		/// <param name="request">The WCF incoming request structure to get the HTTP information from.</param>
		/// <param name="requestUri">The URI of the service endpoint.</param>
		public HttpRequestInfo(HttpRequestMessageProperty request, Uri requestUri) {
			Contract.Requires(request != null);
			Contract.Requires(requestUri != null);
			ErrorUtilities.VerifyArgumentNotNull(request, "request");
			ErrorUtilities.VerifyArgumentNotNull(requestUri, "requestUri");

			this.HttpMethod = request.Method;
			this.Headers = request.Headers;
			this.Url = requestUri;
			this.UrlBeforeRewriting = requestUri;
			this.RawUrl = MakeUpRawUrlFromUrl(requestUri);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpRequestInfo"/> class.
		/// </summary>
		internal HttpRequestInfo() {
			this.HttpMethod = "GET";
			this.Headers = new WebHeaderCollection();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpRequestInfo"/> class.
		/// </summary>
		/// <param name="request">The HttpWebRequest (that was never used) to copy from.</param>
		internal HttpRequestInfo(WebRequest request) {
			Contract.Requires(request != null);
			ErrorUtilities.VerifyArgumentNotNull(request, "request");

			this.HttpMethod = request.Method;
			this.Url = request.RequestUri;
			this.UrlBeforeRewriting = request.RequestUri;
			this.RawUrl = MakeUpRawUrlFromUrl(request.RequestUri);
			this.Headers = GetHeaderCollection(request.Headers);
			this.InputStream = null;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpRequestInfo"/> class.
		/// </summary>
		/// <param name="message">The message being passed in through a mock transport.  May be null.</param>
		/// <param name="httpMethod">The HTTP method that the incoming request came in on, whether or not <paramref name="message"/> is null.</param>
		internal HttpRequestInfo(IDirectedProtocolMessage message, HttpDeliveryMethods httpMethod) {
			this.message = message;
			if ((httpMethod & HttpDeliveryMethods.GetRequest) != 0) {
				this.HttpMethod = "GET";
			} else if ((httpMethod & HttpDeliveryMethods.PostRequest) != 0) {
				this.HttpMethod = "POST";
			}
		}

		/// <summary>
		/// Gets or sets the message that is being sent over a mock transport (for testing).
		/// </summary>
		internal virtual IDirectedProtocolMessage Message {
			get { return this.message; }
			set { this.message = value; }
		}

		/// <summary>
		/// Gets or sets the verb in the request (i.e. GET, POST, etc.)
		/// </summary>
		internal string HttpMethod { get; set; }

		/// <summary>
		/// Gets or sets the entire URL of the request, after any URL rewriting.
		/// </summary>
		internal Uri Url { get; set; }

		/// <summary>
		/// Gets or sets the raw URL that appears immediately following the HTTP verb in the request,
		/// before any URL rewriting takes place.
		/// </summary>
		internal string RawUrl { get; set; }

		/// <summary>
		/// Gets or sets the full public URL used by the remote client to initiate this request,
		/// before any URL rewriting and before any changes made by web farm load distributors.
		/// </summary>
		internal Uri UrlBeforeRewriting { get; set; }

		/// <summary>
		/// Gets the query part of the URL (The ? and everything after it), after URL rewriting.
		/// </summary>
		internal string Query {
			get { return this.Url != null ? this.Url.Query : null; }
		}

		/// <summary>
		/// Gets or sets the collection of headers that came in with the request.
		/// </summary>
		internal WebHeaderCollection Headers { get; set; }

		/// <summary>
		/// Gets or sets the entity, or body of the request, if any.
		/// </summary>
		internal Stream InputStream { get; set; }

		/// <summary>
		/// Gets the key/value pairs found in the entity of a POST request.
		/// </summary>
		internal NameValueCollection Form {
			get {
				Contract.Ensures(Contract.Result<NameValueCollection>() != null);
				if (this.form == null) {
					if (this.HttpMethod == "POST" && this.Headers[HttpRequestHeader.ContentType] == Channel.HttpFormUrlEncoded) {
						StreamReader reader = new StreamReader(this.InputStream);
						long originalPosition = 0;
						if (this.InputStream.CanSeek) {
							originalPosition = this.InputStream.Position;
						}
						this.form = HttpUtility.ParseQueryString(reader.ReadToEnd());
						if (this.InputStream.CanSeek) {
							this.InputStream.Seek(originalPosition, SeekOrigin.Begin);
						}
					} else {
						this.form = new NameValueCollection();
					}
				}

				return this.form;
			}
		}

		/// <summary>
		/// Gets the key/value pairs found in the querystring of the incoming request.
		/// </summary>
		internal NameValueCollection QueryString {
			get {
				if (this.queryString == null) {
					this.queryString = this.Query != null ? HttpUtility.ParseQueryString(this.Query) : new NameValueCollection();
				}

				return this.queryString;
			}
		}

		/// <summary>
		/// Gets the query data from the original request (before any URL rewriting has occurred.)
		/// </summary>
		/// <returns>A <see cref="NameValueCollection"/> containing all the parameters in the query string.</returns>
		internal NameValueCollection QueryStringBeforeRewriting {
			get {
				if (this.queryStringBeforeRewriting == null) {
					// This request URL may have been rewritten by the host site.
					// For openid protocol purposes, we really need to look at 
					// the original query parameters before any rewriting took place.
					if (!this.IsUrlRewritten) {
						// No rewriting has taken place.
						this.queryStringBeforeRewriting = this.QueryString;
					} else {
						// Rewriting detected!  Recover the original request URI.
						ErrorUtilities.VerifyInternal(this.UrlBeforeRewriting != null, "UrlBeforeRewriting is null, so the query string cannot be determined.");
						this.queryStringBeforeRewriting = HttpUtility.ParseQueryString(this.UrlBeforeRewriting.Query);
					}
				}

				return this.queryStringBeforeRewriting;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the request's URL was rewritten by ASP.NET
		/// or some other module.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this request's URL was rewritten; otherwise, <c>false</c>.
		/// </value>
		internal bool IsUrlRewritten {
			get { return this.Url != this.UrlBeforeRewriting; }
		}

		/// <summary>
		/// Gets the query or form data from the original request (before any URL rewriting has occurred.)
		/// </summary>
		/// <returns>A set of name=value pairs.</returns>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Expensive call")]
		internal NameValueCollection GetQueryOrFormFromContext() {
			NameValueCollection query;
			if (this.HttpMethod == "GET") {
				query = this.QueryStringBeforeRewriting;
			} else {
				query = this.Form;
			}
			return query;
		}

		/// <summary>
		/// Gets the public facing URL for the given incoming HTTP request.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns>The URI that the outside world used to create this request.</returns>
		private static Uri GetPublicFacingUrl(HttpRequest request) {
			Contract.Requires(request != null);
			ErrorUtilities.VerifyArgumentNotNull(request, "request");

			// Due to URL rewriting, cloud computing (i.e. Azure)
			// and web farms, etc., we have to be VERY careful about what
			// we consider the incoming URL.  We want to see the URL as it would
			// appear on the public-facing side of the hosting web site.
			// HttpRequest.Url gives us the internal URL in a cloud environment,
			// So we use a variable that (at least from what I can tell) gives us
			// the public URL:
			if (request.ServerVariables["HTTP_HOST"] != null) {
				ErrorUtilities.VerifySupported(request.Url.Scheme == Uri.UriSchemeHttps || request.Url.Scheme == Uri.UriSchemeHttp, "Only HTTP and HTTPS are supported protocols.");
				UriBuilder publicRequestUri = new UriBuilder(request.Url);
				string[] hostAndPort = request.ServerVariables["HTTP_HOST"].Split(new[] { ':' }, 2);
				publicRequestUri.Host = hostAndPort[0];
				if (hostAndPort.Length > 1) {
					publicRequestUri.Port = Convert.ToInt32(hostAndPort[1], CultureInfo.InvariantCulture);
				} else {
					publicRequestUri.Port = publicRequestUri.Scheme == Uri.UriSchemeHttps ? 443 : 80;
				}
				return publicRequestUri.Uri;
			} else {
				// Failover to the method that works for non-web farm enviroments.
				// We use Request.Url for the full path to the server, and modify it
				// with Request.RawUrl to capture both the cookieless session "directory" if it exists
				// and the original path in case URL rewriting is going on.  We don't want to be
				// fooled by URL rewriting because we're comparing the actual URL with what's in
				// the return_to parameter in some cases.
				// Response.ApplyAppPathModifier(builder.Path) would have worked for the cookieless
				// session, but not the URL rewriting problem.
				return new Uri(request.Url, request.RawUrl);
			}
		}

		/// <summary>
		/// Makes up a reasonable guess at the raw URL from the possibly rewritten URL.
		/// </summary>
		/// <param name="url">A full URL.</param>
		/// <returns>A raw URL that might have come in on the HTTP verb.</returns>
		private static string MakeUpRawUrlFromUrl(Uri url) {
			Contract.Requires(url != null);
			return url.AbsolutePath + url.Query + url.Fragment;
		}

		/// <summary>
		/// Converts a NameValueCollection to a WebHeaderCollection.
		/// </summary>
		/// <param name="pairs">The collection a HTTP headers.</param>
		/// <returns>A new collection of the given headers.</returns>
		private static WebHeaderCollection GetHeaderCollection(NameValueCollection pairs) {
			Debug.Assert(pairs != null, "pairs == null");

			WebHeaderCollection headers = new WebHeaderCollection();
			foreach (string key in pairs) {
				headers.Add(key, pairs[key]);
			}

			return headers;
		}
	}
}
