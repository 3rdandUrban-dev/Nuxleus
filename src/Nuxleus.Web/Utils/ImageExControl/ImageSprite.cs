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
using System.Web.UI.WebControls;

namespace Nuxleus.Web.Utils {
    public class ImageSprite : Image {

        /// <summary>
        /// The "EnableSprites" property (enabled by default) instructs the application to use an optimized sprite or inlined image in place of
        /// a normal image tag (from which this inherits).
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("The EnableSprites property (enabled by default) instructs the application to use an optimized sprite or inlined image in place of a normal image tag (from which this inherits).")]
        public bool EnableSprites {
            get {
                bool? value = (bool?)ViewState["EnableSprites"] ?? true;
                return value.Value;
            }
            set {
                ViewState["EnableSprites"] = value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected override void OnPreRender(EventArgs e) {

            if (!EnableSprites) {
                return;
            }

            string cssFileName = ImageOptimizations.LinkCompatibleCssFile(new HttpContextWrapper(Context).Request.Browser);
            if (cssFileName == null) {
                return;
            }

            try {
                string webPath = Path.GetDirectoryName(ImageUrl);

                // Check that CSS file is accessible
                if (!File.Exists(Path.Combine(Context.Server.MapPath(webPath), cssFileName))) {
                    return;
                }

                // A new class is used to avoid conflicts in Page.Items
                StringWrapper key = new StringWrapper(webPath);

                if (!Page.Items.Contains(key)) {
                    Page.Items.Add(key, null);

                    HtmlLink css = new HtmlLink();
                    css.Href = Path.Combine(webPath, cssFileName);
                    css.Attributes["rel"] = "stylesheet";
                    css.Attributes["type"] = "text/css";
                    css.Attributes["media"] = "all";
                    Page.Header.Controls.Add(css);
                }

                string imagefileName = Path.GetFileName(ImageUrl);
                CssClass = ImageOptimizations.MakeCssClassName(Path.Combine(webPath, imagefileName), (string)HttpRuntime.Cache.Get("spriteFolderRelativePath"));
                ImageUrl = ImageOptimizations.InlinedTransparentGif;
            }
            catch (Exception) {
                // If an exception occured, use a normal image tag in place of the sprite
            }
        }
    }
}
