/*
 * Originally published as part of the "Sprite and Image Optimization Framework Preview 2" found at http://aspnet.codeplex.com/releases/view/50869
 * as part of the ASP.NET Open Source project on CodePlex.com (http://aspnet.codeplex.com).
 * As specified at http://aspnet.codeplex.com/license the code contained in the above preview release is licensed under
 * a Microsoft Source License for ASP.NET Pre-Release Components, a copy of which can be found at ~/license/MicrosoftSourceLicense-for-ASP.NET-Pre-ReleaseComponents.tx
 * in this projects directory structure.
 */

using System.Drawing;

namespace Nuxleus.Web.Utils
{
	/// <summary>
	/// ImageSettings is a class used to store the settings retrieved from the sprite settings file
	/// </summary>
	internal class ImageSettings
	{
		/// <summary>
		/// The output image file format
		/// </summary>
		public string Format { get; set; }
		/// <summary>
		/// The quality level of the format, if the format supports quality settings (such as jpg)
		/// </summary>
		public int Quality { get; set; }
		/// <summary>
		/// The maximum size of a sprite before it will be split into multiple images
		/// </summary>
		public int MaxSize { get; set; }
		/// <summary>
		/// The background color of the output sprite
		/// </summary>
		public Color BackgroundColor { get; set; }
		/// <summary>
		/// Controls whether base64 inlining should be used for high-compatibility browsers
		/// </summary>
		public bool Base64 { get; set; }
		/// <summary>
		/// Controls whether the application will tile images along the X or Y axis
		/// </summary>
		public bool TileInYAxis { get; set; }

		// Constructor inputs default values
		public ImageSettings ()
		{
			Format = "png";
			Quality = 90;
			MaxSize = 500;
			BackgroundColor = Color.Transparent;
			Base64 = true;
		}
	}
}
