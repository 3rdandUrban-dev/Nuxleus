//using System;
//using Extf.Net;
//using System.Xml;
//using Nuxleus.Utility.S3;

//namespace Nuxleus.IO
//{

//    public partial class GlobalClip
//    {

//        public bool Copy(ClipItem item)
//        {

//            S3Object oItem = new S3Object(item.Data, item.MetaData);
//            string copyKey = this.KeyPrefix + "-copy-" + oItem.GetHashCode().ToString(provider);

//            try
//            {
//                this.Clipboard().put(StorageBase, copyKey, oItem, null);
//                this.ClipCopy.Push(item);
//                return true;
//            }
//            catch (Exception e)
//            {
//                throw;
//            }
//        }

//    }
//}
