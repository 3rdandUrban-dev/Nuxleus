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

Namespace Microsoft.Samples.FeedSync
	Public Class MiscHelpers
		Public Shared Function XMLEncode(ByVal i_String As String) As String
			Dim StringBuilder As System.Text.StringBuilder = New System.Text.StringBuilder(i_String)

			StringBuilder = StringBuilder.Replace("&", "&amp;")
			StringBuilder = StringBuilder.Replace(">", "&gt;")
			StringBuilder = StringBuilder.Replace("<", "&lt;")
			StringBuilder = StringBuilder.Replace("""", "&quot;")
			StringBuilder = StringBuilder.Replace("'", "&#39;")

			Return StringBuilder.ToString()
		End Function
	End Class
End Namespace
