/*
 * Originally published as part of the "Sprite and Image Optimization Framework Preview 2" found at http://aspnet.codeplex.com/releases/view/50869
 * as part of the ASP.NET Open Source project on CodePlex.com (http://aspnet.codeplex.com).
 * As specified at http://aspnet.codeplex.com/license the code contained in the above preview release is licensed under
 * a Microsoft Source License for ASP.NET Pre-Release Components, a copy of which can be found at ~/license/MicrosoftSourceLicense-for-ASP.NET-Pre-ReleaseComponents.tx
 * in this projects directory structure.
 */

using System;
using System.ComponentModel;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Nuxleus.Web.Utils {
    public class ImageSpriteCssLink : Control {

        /// <summary>
        /// The relative path to the folder in which the CSS files are to be linked from
        /// </summary>
        [Category("Behavior")]
        [Description("The relative path to the folder in which the CSS files are to be linked from")]
        public string ImageUrl {
            get {
                return (string)ViewState["ImageUrl"];
            }
            set {
                ViewState["ImageUrl"] = value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.EndsWith(System.String)")]
        protected override void OnPreRender(EventArgs e) {

            if (Path.HasExtension(ImageUrl) || ImageUrl.EndsWith(Path.AltDirectorySeparatorChar.ToString()) || ImageUrl.EndsWith(Path.DirectorySeparatorChar.ToString())) {
                ImageUrl = Path.GetDirectoryName(ImageUrl);
            }

            string cssFileName = ImageOptimizations.LinkCompatibleCssFile(new HttpContextWrapper(Context).Request.Browser) ?? ImageOptimizations.LowCompatibilityCssFile;

            // Set up fileName and path variables
            string localPath = Context.Server.MapPath(ImageUrl);

            // Check that CSS file is accessible
            if (!File.Exists(Path.Combine(localPath, cssFileName))) {
                return;
            }

            // Have to change directory separator character because the ImageSprite method uses Path.GetDirectory, which uses backslashes
            StringWrapper key = new StringWrapper(ImageUrl.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));

            if (!Page.Items.Contains(key)) {
                Page.Items.Add(key, null);

                HtmlLink css = new HtmlLink();
                css.Href = Path.Combine(ImageUrl, cssFileName);
                css.Attributes["rel"] = "stylesheet";
                css.Attributes["type"] = "text/css";
                css.Attributes["media"] = "all";
                Page.Header.Controls.Add(css);
            }
        }
    }
}