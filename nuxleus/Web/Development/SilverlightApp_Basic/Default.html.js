///////////////////////////////////////////////////////////////////////////////
//
//  Default.html.js
//
// 
// © 2007 Microsoft Corporation. All Rights Reserved.
//
// This file is licensed as part of the Silverlight 1.1 SDK, for details look here: http://go.microsoft.com/fwlink/?LinkID=89145&clcid=0x409
//
///////////////////////////////////////////////////////////////////////////////

//contains calls to silverlight.js, example below loads Page.xaml
function createSilverlight()
{
	Sys.Silverlight.createObjectEx({
		source: "Page.xaml",
		parentElement: document.getElementById("SilverlightControlHost"),
		id: "SilverlightControl",
		properties: {
			width: "1280",
			height: "800",
			version: "0.95",
			framerate: "30",
			enableHtmlAccess: true
		},
		events: {}
	});
}