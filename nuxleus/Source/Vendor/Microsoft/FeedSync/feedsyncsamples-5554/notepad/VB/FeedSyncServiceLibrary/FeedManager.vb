'****************************************************************************************
'   
'   Copyright (c) Microsoft Corporation. All rights reserved.
'
'   Use of this code sample is subject to the terms of the Microsoft
'   Permissive License, a copy of which should always be distributed with
'   this file.  You can also access a copy of this license agreement at:
'   http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
'
' ***************************************************************************************


Imports Microsoft.VisualBasic
Imports System
Imports System.Net
Imports System.IO

Namespace Microsoft.Samples.FeedSyncService

	Public Class FeedManager
		'  Set default timeout for web/http calls to 2 minutes
		Private Shared s_TimeoutInMilliseconds As Integer = 120000
		Private Shared s_LastStatusCode As HttpStatusCode

		Public Shared ReadOnly Property LastStatusCode() As HttpStatusCode
			Get
				Return FeedManager.s_LastStatusCode
			End Get
		End Property

		Public Shared Function IsRSSFeedURL(ByVal i_FeedURL As String) As Boolean
			Return (i_FeedURL.IndexOf(Microsoft.Samples.FeedSyncService.Constants.RSS_QUERY_FRAGMENT) <> -1)
		End Function

		Public Shared Function IsAtomFeedURL(ByVal i_FeedURL As String) As Boolean
'INSTANT VB NOTE: The local variable IsAtomFeedURL was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim IsAtomFeedURL_Renamed As Boolean = (i_FeedURL.IndexOf(Microsoft.Samples.FeedSyncService.Constants.ATOM_QUERY_FRAGMENT) <> -1) OrElse (i_FeedURL.IndexOf(Microsoft.Samples.FeedSyncService.Constants.RSS_QUERY_FRAGMENT) = -1)

			Return IsAtomFeedURL_Renamed
		End Function

		Public Shared Function GetRSSFeedURL(ByVal i_FeedURL As String) As String
			Dim RSSFeedURL As String = i_FeedURL

			'  Strip off Atom fragment if supplied
			If i_FeedURL.IndexOf(Microsoft.Samples.FeedSyncService.Constants.ATOM_QUERY_FRAGMENT) <> -1 Then
				RSSFeedURL = i_FeedURL.Substring(0, i_FeedURL.IndexOf(Microsoft.Samples.FeedSyncService.Constants.ATOM_QUERY_FRAGMENT))
			End If

			'  Add RSS fragment if necessary
			If RSSFeedURL.IndexOf(Microsoft.Samples.FeedSyncService.Constants.RSS_QUERY_FRAGMENT) = -1 Then
				RSSFeedURL &= Microsoft.Samples.FeedSyncService.Constants.RSS_QUERY_FRAGMENT
			End If

			Return RSSFeedURL
		End Function

		Public Shared Function GetAtomFeedURL(ByVal i_FeedURL As String) As String
			Dim AtomFeedURL As String = i_FeedURL

			'  Strip off RSS fragment if supplied
			If i_FeedURL.IndexOf(Microsoft.Samples.FeedSyncService.Constants.RSS_QUERY_FRAGMENT) <> -1 Then
				AtomFeedURL = i_FeedURL.Substring(0, i_FeedURL.IndexOf(Microsoft.Samples.FeedSyncService.Constants.RSS_QUERY_FRAGMENT))
			End If

			'  Add Atom fragment if necessary
			If AtomFeedURL.IndexOf(Microsoft.Samples.FeedSyncService.Constants.ATOM_QUERY_FRAGMENT) = -1 Then
				AtomFeedURL &= Microsoft.Samples.FeedSyncService.Constants.ATOM_QUERY_FRAGMENT
			End If

			Return AtomFeedURL
		End Function

		Public Shared Property TimeoutInMilliseconds() As Integer
			Get
				Return s_TimeoutInMilliseconds
			End Get
			Set(ByVal value As Integer)
				s_TimeoutInMilliseconds = value
			End Set
		End Property

		#Region "Generic read/write methods"

		Public Shared Function ReadFeedContents(ByVal i_FeedURL As String, ByRef i_Since As String) As String
			Dim FeedContents As String = System.String.Empty

			Dim HttpWebRequest As System.Net.HttpWebRequest = CreateWebRequest (i_FeedURL, "GET")

			HttpWebRequest.KeepAlive = False
			HttpWebRequest.Timeout = s_TimeoutInMilliseconds

			If (Not System.String.IsNullOrEmpty(i_Since)) Then
				HttpWebRequest.Headers.Add("If-None-Match", i_Since)
				HttpWebRequest.Headers.Add("A-IM", "feed")
			End If

			Dim HttpWebResponse As System.Net.HttpWebResponse = Nothing

			Try
				HttpWebResponse = CType(HttpWebRequest.GetResponse(), System.Net.HttpWebResponse)

				Using ResponseStream As System.IO.Stream = HttpWebResponse.GetResponseStream()
					Dim StreamReader As System.IO.StreamReader = New System.IO.StreamReader (ResponseStream, System.Text.Encoding.UTF8)

					FeedContents = StreamReader.ReadToEnd()
					StreamReader.Close()
				End Using

				i_Since = HttpWebResponse.Headers("ETag")
			Catch WebException As System.Net.WebException
				'  In the event that there was something returned, explicitly
				'  empty feed contents
				FeedContents = Nothing

				If WebException.Response IsNot Nothing Then
					Dim HttpWebExceptionResponse As System.Net.HttpWebResponse = CType(WebException.Response, System.Net.HttpWebResponse)

					If HttpWebExceptionResponse.StatusCode = System.Net.HttpStatusCode.Unauthorized Then
						Throw New System.Exception("Private feeds not supported!")
					ElseIf HttpWebExceptionResponse.StatusCode = System.Net.HttpStatusCode.Forbidden Then
						Throw New System.Exception("Not authorized to access feed!")
					End If

					If HttpWebExceptionResponse.StatusCode <> System.Net.HttpStatusCode.NotModified Then
						Throw
					End If
				Else
					Throw
				End If

			Finally
				If HttpWebResponse IsNot Nothing Then
					HttpWebResponse.Close()
				End If
			End Try
			Return FeedContents
		End Function

		Public Shared Sub UpdateFeedContents(ByVal i_FeedContents As String, ByVal i_CompleteFeedURL As String)
			Dim contentType As String

			If IsAtomFeedURL(i_CompleteFeedURL) Then
				contentType = "application/atom+xml;type=feed"
			ElseIf IsRSSFeedURL(i_CompleteFeedURL) Then
				contentType = "application/rss+xml"
			Else
				Throw New Exception("Invalid feed url")
			End If

			Try
                Dim webResponse As System.Net.HttpWebResponse = Nothing

				Dim encoding As System.Text.UTF8Encoding = New System.Text.UTF8Encoding()
				Dim encodedData() As Byte = encoding.GetBytes(i_FeedContents)

                CompleteRequest("POST", encodedData, i_CompleteFeedURL, contentType, Nothing, webResponse)
			Catch WebException As System.Net.WebException
				If WebException.Response IsNot Nothing Then
					Dim HttpWebResponse As System.Net.HttpWebResponse = CType(WebException.Response, System.Net.HttpWebResponse)

					If HttpWebResponse.StatusCode = System.Net.HttpStatusCode.Unauthorized Then
						Throw New System.Exception("Private feeds are unsupported!")
					ElseIf HttpWebResponse.StatusCode = System.Net.HttpStatusCode.Forbidden Then
						Throw New System.Exception("Not authorized to access feed!")
					End If
				End If

				Throw
			End Try
		End Sub


		#End Region

		#Region "Web request methods"

		Public Shared Function CompleteRequest(ByVal method As String, ByVal encodedData() As Byte, ByVal toFileUrl As String, ByVal contentType As String, ByVal headers As WebHeaderCollection, <System.Runtime.InteropServices.Out()> ByRef webResponse As HttpWebResponse) As String
			Dim wr As HttpWebRequest = CreateWebRequest(toFileUrl, method)

			If headers IsNot Nothing Then
				wr.Headers.Add(headers)
			End If

			If encodedData IsNot Nothing Then
				wr.ContentLength = encodedData.Length
				wr.ContentType = contentType

				Using s As Stream = wr.GetRequestStream()
					s.Write(encodedData, 0, encodedData.Length)
				End Using
			End If

			webResponse = Nothing
			Dim response As String = Nothing

			Try
				webResponse = CType(wr.GetResponse(), HttpWebResponse)
				Using webResponse
					s_LastStatusCode = webResponse.StatusCode
					Using sr As StreamReader = New StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8)
						response = sr.ReadToEnd()
					End Using
				End Using
			Catch ex As WebException
				If ex.Response IsNot Nothing Then
					Using sr As StreamReader = New StreamReader(ex.Response.GetResponseStream(), System.Text.Encoding.UTF8)
						response = sr.ReadToEnd()
						System.Diagnostics.Debug.Write(response)
					End Using
				End If

				Throw
			End Try

			Return response
		End Function

		' Create a webRequest to perform "method" (eg: Copy, Move, Delete etc.) on the specified file/folder.
		Public Shared Function CreateWebRequest(ByVal url As String, ByVal method As String) As HttpWebRequest
			Dim wr As HttpWebRequest = CType(WebRequest.Create(url), HttpWebRequest)

			wr.Credentials = CredentialCache.DefaultCredentials
			wr.PreAuthenticate = True
			wr.Method = method

			wr.Timeout = 10000 ' set timeout to 10 seconds.

			Return (wr)
		End Function

		#End Region
	End Class
End Namespace
