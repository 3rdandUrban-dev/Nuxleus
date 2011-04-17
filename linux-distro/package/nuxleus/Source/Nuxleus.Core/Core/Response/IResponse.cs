using System.Net;

namespace Nuxleus.Core
{
    public interface IResponse
    {
        WebHeaderCollection Headers {
            get;
            set;
        }
        IResult Result {
            get;
            set;
        }
        //object Result {
        //    get;
        //    set;
        //}
        //object GetResult();
        //T ToObject<T>();
        //XElement ToXElement();
        //string ToXmlString();
    }
}
