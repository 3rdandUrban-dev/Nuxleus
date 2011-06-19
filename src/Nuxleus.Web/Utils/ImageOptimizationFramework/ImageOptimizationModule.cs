/*
 * Originally published as part of the "Sprite and Image Optimization Framework Preview 2" found at http://aspnet.codeplex.com/releases/view/50869
 * as part of the ASP.NET Open Source project on CodePlex.com (http://aspnet.codeplex.com).
 * As specified at http://aspnet.codeplex.com/license the code contained in the above preview release is licensed under
 * a Microsoft Source License for ASP.NET Pre-Release Components, a copy of which can be found at ~/license/MicrosoftSourceLicense-for-ASP.NET-Pre-ReleaseComponents.tx
 * in this projects directory structure.
 */

using System.Web;

namespace Nuxleus.Web.Utils {
    public class ImageOptimizationModule : IHttpModule {
        private static readonly object _lockObj = new object();
        private static bool _hasAlreadyRun;
        public const string SpriteFolderRelativePath = "~/App_Sprites/";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public void Init(System.Web.HttpApplication context) {
            lock (_lockObj) {
                if (_hasAlreadyRun)
                    return;
                else
                    _hasAlreadyRun = true;
            }

            HttpRuntime.Cache.Insert("spriteFolderRelativePath", SpriteFolderRelativePath);
            ImageOptimizations.AddCacheDependencies(context.Context.Server.MapPath(SpriteFolderRelativePath), rebuildImages: true);
            
            return;
        }

        public void Dispose() { }
    }
}
