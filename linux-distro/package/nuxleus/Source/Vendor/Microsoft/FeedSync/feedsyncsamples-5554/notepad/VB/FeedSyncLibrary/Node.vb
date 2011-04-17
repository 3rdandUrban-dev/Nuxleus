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
	Public MustInherit Class Node
		Protected m_XmlElement As System.Xml.XmlElement

		Public ReadOnly Property XmlElement() As System.Xml.XmlElement
			Get
				Return m_XmlElement
			End Get
		End Property
	End Class
End Namespace
