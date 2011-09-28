using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Xml;
using Saxon.Api;
using Xameleon.ResultDocumentHandler;
using System.Text;
using System.Web.SessionState;

namespace Xameleon.Transform
{

    ///<summary>
    ///</summary>
    public partial class Transform
    {

        public void BeginProcess(Context context, HttpContext httpContext, XsltTransformationManager manager, TextWriter writer, TransformServiceAsyncResult result)
        {
            BeginProcess(context, httpContext, manager, writer, manager.BaseXsltName, result);
        }

        public void BeginProcess(Context context, HttpContext httpContext, XsltTransformationManager manager, TextWriter writer, String xsltName, TransformServiceAsyncResult result)
        {
            XsltTransformer transformer = manager.GetTransformer(xsltName);

            transformer.SetParameter(new QName("", "", "current-context"), new XdmValue((XdmItem)XdmAtomicValue.wrapExternalObject(httpContext)));

            if (context.XsltParams.Count > 0)
            {
                foreach (DictionaryEntry param in context.XsltParams)
                {
                    string name = (string)param.Key;
                    transformer.SetParameter(new QName("", "", name), new XdmValue((XdmItem)XdmAtomicValue.wrapExternalObject(param.Value)));
                }
            }

            Uri requestXmlUri = new Uri(context.RequestUri);

            transformer.InputXmlResolver = manager.Resolver;
            transformer.InitialContextNode = manager.GetXdmNode(context.RequestXmlETag, context.RequestUri);

            Serializer destination = manager.Serializer;
            destination.SetOutputWriter(writer);

            lock (transformer)
            {
                transformer.Run(destination);
            }

            result.CompleteCall();
        }
    }
}

