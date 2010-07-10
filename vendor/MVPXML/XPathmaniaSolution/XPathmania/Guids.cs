// Guids.cs
// MUST match guids.h
using System;

namespace XmlMvp.XPathmania
{
    static class GuidList
    {
        public const string guidXPathmaniaPkgString = "2e2c89a9-57e5-449f-9ad4-90080323d62c";
        public const string guidXPathmaniaCmdSetString = "ea087ddc-5244-4d7f-b1ca-e1211a815a04";
        public const string guidToolWindowPersistanceString = "9278d37d-d304-4967-952f-34a31adfcac5";

        public static readonly Guid guidXPathmaniaPkg = new Guid(guidXPathmaniaPkgString);
        public static readonly Guid guidXPathmaniaCmdSet = new Guid(guidXPathmaniaCmdSetString);
        public static readonly Guid guidToolWindowPersistance = new Guid(guidToolWindowPersistanceString);
    };
}