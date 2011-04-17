/*
 * Originally published as part of the "Sprite and Image Optimization Framework Preview 2" found at http://aspnet.codeplex.com/releases/view/50869
 * as part of the ASP.NET Open Source project on CodePlex.com (http://aspnet.codeplex.com).
 * As specified at http://aspnet.codeplex.com/license the code contained in the above preview release is licensed under
 * a Microsoft Source License for ASP.NET Pre-Release Components, a copy of which can be found at ~/license/MicrosoftSourceLicense-for-ASP.NET-Pre-ReleaseComponents.tx
 * in this projects directory structure.
 */


namespace Nuxleus.Web.Utils {
    internal class StringWrapper {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public string Value { get; private set; }

        public StringWrapper(string input) {
            Value = input;
        }

        public override bool Equals(object obj) {
            var otherWrapper = obj as StringWrapper;
            return ((otherWrapper != null) && (otherWrapper.Value.Equals(Value)));
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }
    }
}
