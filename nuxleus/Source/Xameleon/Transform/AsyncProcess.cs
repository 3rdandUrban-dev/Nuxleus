using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Xml;
using Saxon.Api;
using Nuxleus.ResultDocumentHandler;
using System.Text;
using System.Web.SessionState;
using Nuxleus.Async;
using Nuxleus.Agent;

namespace Nuxleus.Transform
{

    ///<summary>
    ///</summary>
    public partial class Transform
    {

        public void BeginTransformProcess (TransformRequest request, AsyncCallback callback, Nuxleus.Agent.NuxleusAsyncResult asyncResult)
        {
            TransformContext transformContext = (TransformContext)request.TransformContext;
            XsltTransformationManager transformationManager = transformContext.XsltTransformationManager;
            TransformResponse response = transformContext.Response;

            XsltTransformer transformer = transformationManager.GetTransformer(transformContext.XsltName);

            transformer.SetParameter(new QName("", "", "current-context"), new XdmValue((XdmItem)XdmAtomicValue.wrapExternalObject(transformContext.HttpContext)));

            if (transformContext.Context.XsltParams.Count > 0)
            {
                foreach (DictionaryEntry param in transformContext.Context.XsltParams)
                {
                    string name = (string)param.Key;
                    transformer.SetParameter(new QName("", "", name), new XdmValue((XdmItem)XdmAtomicValue.wrapExternalObject(param.Value)));
                }
            }

            Uri requestXmlUri = new Uri(transformContext.Context.RequestUri);

            transformer.InputXmlResolver = transformationManager.Resolver;
            transformer.InitialContextNode = transformationManager.GetXdmNode(transformContext.Context.RequestXmlETag, transformContext.Context.RequestUri);

            Serializer destination = transformationManager.Serializer;

            destination.SetOutputWriter(transformContext.Writer);

            lock (transformer)
            {
                transformer.Run(destination);
            }
            //Console.WriteLine("Output of transform: {0}", builder.ToString());
            //response.TransformResult = builder.ToString();
            //callback.Invoke(asyncResult);
            //asyncResult.CompleteCall();
            //return response;

        }

        public void EndTransformProcess (IAsyncResult ar) {
            ///TODO: Process and serialize result.
            ///TODO: Add result to Result Hashtable
        }
    }
}

