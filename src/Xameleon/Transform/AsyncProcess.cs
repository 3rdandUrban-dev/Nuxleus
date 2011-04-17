using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Xml;
using Saxon.Api;
using Nuxleus.ResultDocumentHandler;
using System.Text;
using System.Web.SessionState;
using Nuxleus.Asynchronous;
using Nuxleus.Core;

namespace Nuxleus.Transform
{
    public delegate TransformResponse TransformProcessDelegate(TransformRequest request);

    ///<summary>
    ///</summary>
    public partial class Transform
    {
        static object m_lock = new object();

        public TransformResponse BeginTransformProcess(TransformRequest request)
        {
            this.LogInfo("BeginTransformProcess reached");

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

            this.LogDebug(transformer.InitialContextNode.OuterXml);

            Serializer destination = transformationManager.Serializer;

            StringBuilder builder = new StringBuilder();

            destination.SetOutputWriter(new StringWriter(builder));

            lock (m_lock)
            {
                transformer.Run(destination);
            }

            response.TransformResult = builder.ToString();
            this.LogDebug("Output of transform: {0}", response.TransformResult);
            return response;

        }
    }
}

