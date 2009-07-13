using Nuxleus.MetaData;

namespace Nuxleus.Extension.Aws.SimpleDb
{
    public enum SdbRequestType
    {
        [Label("Select")]
        Select,
        [Label("CreateDomain")]
        CreateDomain,
        [Label("DeleteDomain")]
        DeleteDomain,
        [Label("ListDomains")]
        ListDomains,
        [Label("DomainMetadata")]
        DomainMetadata,
        [Label("PutAttributes")]
        PutAttributes,
        [Label("BatchPutAttributes")]
        BatchPutAttributes,
        [Label("DeleteAttributes")]
        DeleteAttributes,
        [Label("GetAttributes")]
        GetAttributes
    }
}
