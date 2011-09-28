using System;
using NUnit.Framework;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.IO;
using System.Text;
using Mvp.Xml.Common.Xsl;

namespace Mvp.Xml.Tests
{
    [TestFixture]
    public class MvpXslTransformTests 
    {
        byte[] standardResult;
        byte[] resolverTestStandardResult;
        MvpXslTransform xslt, xslt2;
        XsltArgumentList args;

        public MvpXslTransformTests()
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load("../../MvpXslTransformTests/test1.xslt");
            MemoryStream ms = new MemoryStream();
            using (XmlReader r = GetReader(Globals.NorthwindResource))
            {                
                xslt.Transform(r, Arguments, ms);
            }
            standardResult = ms.ToArray();
            XslCompiledTransform xslt2 = new XslCompiledTransform();
            xslt2.Load("../../MvpXslTransformTests/resolver-test.xslt", XsltSettings.TrustedXslt, null);
            MemoryStream ms2 = new MemoryStream();
            XmlWriter w = XmlWriter.Create(ms2);
            using (XmlReader r2 = XmlReader.Create("../../MvpXslTransformTests/test.xml"))
            {
                xslt2.Transform(r2, Arguments, w, new MyXmlResolver());
            }
            w.Close();
            resolverTestStandardResult = ms2.ToArray();              
        }

        private XsltArgumentList Arguments
        {
            get {
                if (args == null)
                {
                    args = new XsltArgumentList();
                    args.AddParam("prm1", "", "value1");
                }
                return args;
            }
        }

        private MvpXslTransform GetMvpXslTransform()
        {
            if (xslt == null)
            {
                xslt = new MvpXslTransform();
                xslt.Load("../../MvpXslTransformTests/test1.xslt");
            }
            return xslt;
        }

        private MvpXslTransform GetMvpXslTransform2()
        {
            if (xslt2 == null)
            {
                xslt2 = new MvpXslTransform();
                xslt2.Load("../../MvpXslTransformTests/resolver-test.xslt", XsltSettings.TrustedXslt, null);
            }
            return xslt2;
        }        

        private static void CompareResults(byte[] standard, byte[] test)
        {
            Assert.AreEqual(standard.Length, test.Length, string.Format("Lengths are different: {0}, {1}", standard.Length, test.Length));
            for (int i = 0; i < standard.Length; i++)
            {
                Assert.IsTrue(standard[i] == test[i], string.Format("Values aren't equal: {0}, {1}, positoin {2}", standard[i], test[i], i));
            }
        }

        private static XmlReader GetReader(string xml)
        {
            XmlReaderSettings s = new XmlReaderSettings();
            s.ProhibitDtd = false;
            return XmlReader.Create(Globals.GetResource(xml), s);
        }

        [Test]
        public void TestStringInput()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
            XmlInput input = new XmlInput("../../northwind.xml");
            MemoryStream ms = new MemoryStream();
            xslt.Transform(input, Arguments, new XmlOutput(ms));
            CompareResults(standardResult, ms.ToArray());
        }

        [Test]
        public void TestStreamInput()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
            using (FileStream fs = File.OpenRead("../../northwind.xml"))
            {
                XmlInput input = new XmlInput(fs);
                MemoryStream ms = new MemoryStream();
                xslt.Transform(input, Arguments, new XmlOutput(ms));
                CompareResults(standardResult, ms.ToArray());
            }            
        }

        [Test]
        public void TestTextReaderInput()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
            XmlInput input = new XmlInput(new StreamReader("../../northwind.xml", Encoding.GetEncoding("windows-1252")));
            MemoryStream ms = new MemoryStream();
            xslt.Transform(input, Arguments, new XmlOutput(ms));
            CompareResults(standardResult, ms.ToArray());
        }

        [Test]
        public void TestXmlReaderInput()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
            XmlInput input = new XmlInput(XmlReader.Create("../../northwind.xml"));
            MemoryStream ms = new MemoryStream();
            xslt.Transform(input, Arguments, new XmlOutput(ms));
            CompareResults(standardResult, ms.ToArray());
        }


        [Test]
        public void TestIXPathNavigableInput()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
            XmlInput input = new XmlInput(new XPathDocument("../../northwind.xml", XmlSpace.Preserve));
            MemoryStream ms = new MemoryStream();
            xslt.Transform(input, Arguments, new XmlOutput(ms));
            CompareResults(standardResult, ms.ToArray());
        }

        [Test]
        public void TestStringInput2()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
            XmlInput input = new XmlInput("../../northwind.xml");
            MemoryStream ms = new MemoryStream();
            XmlReader r = xslt.Transform(input, Arguments);
            XmlWriter w = XmlWriter.Create(ms);
            w.WriteNode(r, false);
            w.Close();
            CompareResults(standardResult, ms.ToArray());
        }

        [Test]
        public void TestStreamInput2()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
            using (FileStream fs = File.OpenRead("../../northwind.xml"))
            {
                XmlInput input = new XmlInput(fs);
                MemoryStream ms = new MemoryStream();
                XmlReader r = xslt.Transform(input, Arguments);
                XmlWriter w = XmlWriter.Create(ms);
                w.WriteNode(r, false);
                w.Close();
                CompareResults(standardResult, ms.ToArray());
            }
        }

        [Test]
        public void TestTextReaderInput2()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
            XmlInput input = new XmlInput(new StreamReader("../../northwind.xml", Encoding.GetEncoding("windows-1252")));
            MemoryStream ms = new MemoryStream();
            XmlReader r = xslt.Transform(input, Arguments);
            XmlWriter w = XmlWriter.Create(ms);
            w.WriteNode(r, false);
            w.Close();
            CompareResults(standardResult, ms.ToArray());
        }

        [Test]
        public void TestXmlReaderInput2()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
            XmlInput input = new XmlInput(XmlReader.Create("../../northwind.xml"));
            MemoryStream ms = new MemoryStream();
            XmlReader r = xslt.Transform(input, Arguments);
            XmlWriter w = XmlWriter.Create(ms);
            w.WriteNode(r, false);
            w.Close();
            CompareResults(standardResult, ms.ToArray());
        }


        [Test]
        public void TestIXPathNavigableInput2()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
            XmlInput input = new XmlInput(new XPathDocument("../../northwind.xml", XmlSpace.Preserve));
            MemoryStream ms = new MemoryStream();
            XmlReader r = xslt.Transform(input, Arguments);
            XmlWriter w = XmlWriter.Create(ms);
            w.WriteNode(r, false);
            w.Close();
            CompareResults(standardResult, ms.ToArray());
        }

        [Test]
        public void TestStringOutput()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
            XmlInput input = new XmlInput("../../northwind.xml");
            xslt.Transform(input, Arguments, new XmlOutput("../../MvpXslTransformTests/out.xml"));
            using (FileStream fs = File.OpenRead("../../MvpXslTransformTests/out.xml"))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                CompareResults(standardResult, bytes);
            }            
        }

        [Test]
        public void TestStreamOutput()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
            XmlInput input = new XmlInput("../../northwind.xml");
            using (FileStream outStrm = File.OpenWrite("../../MvpXslTransformTests/out.xml")) {
                xslt.Transform(input, Arguments, new XmlOutput(outStrm));
            }
            using (FileStream fs = File.OpenRead("../../MvpXslTransformTests/out.xml"))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                CompareResults(standardResult, bytes);
            }
        }

        [Test]
        public void TestTextWriterOutput()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
            XmlInput input = new XmlInput("../../northwind.xml");
            TextWriter w = new StreamWriter("../../MvpXslTransformTests/out.xml", false, Encoding.UTF8);
            xslt.Transform(input, Arguments, new XmlOutput(w));
            w.Close();
            using (FileStream fs = File.OpenRead("../../MvpXslTransformTests/out.xml"))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                CompareResults(standardResult, bytes);
            }
        }

        [Test]
        public void TestXmlWriterOutput()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
            XmlInput input = new XmlInput("../../northwind.xml");
            XmlWriter w = XmlWriter.Create("../../MvpXslTransformTests/out.xml");
            xslt.Transform(input, Arguments, new XmlOutput(w));
            w.Close();
            using (FileStream fs = File.OpenRead("../../MvpXslTransformTests/out.xml"))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                CompareResults(standardResult, bytes);
            }
        }


        [Test]        
        public void ResolverTestStringInput()
        {
            MvpXslTransform xslt = GetMvpXslTransform2();
            XmlInput input = new XmlInput("../../MvpXslTransformTests/test.xml", new MyXmlResolver());
            MemoryStream ms = new MemoryStream();
            xslt.Transform(input, Arguments, new XmlOutput(ms));
            CompareResults(resolverTestStandardResult, ms.ToArray());            
        }

        [Test]
        public void ResolverTestStreamInput()
        {
            MvpXslTransform xslt = GetMvpXslTransform2();
            using (FileStream fs = File.OpenRead("../../MvpXslTransformTests/test.xml"))
            {
                XmlInput input = new XmlInput(fs, new MyXmlResolver());
                MemoryStream ms = new MemoryStream();
                xslt.Transform(input, Arguments, new XmlOutput(ms));
                CompareResults(resolverTestStandardResult, ms.ToArray());
            }
        }

        [Test]
        public void ResolverTestTextReaderInput()
        {
            MvpXslTransform xslt = GetMvpXslTransform2();
            XmlInput input = new XmlInput(new StreamReader("../../MvpXslTransformTests/test.xml"), new MyXmlResolver());
            MemoryStream ms = new MemoryStream();
            xslt.Transform(input, Arguments, new XmlOutput(ms));
            CompareResults(resolverTestStandardResult, ms.ToArray());
        }

        [Test]
        public void ResolverTestXmlReaderInput()
        {
            MvpXslTransform xslt = GetMvpXslTransform2();
            XmlInput input = new XmlInput(XmlReader.Create("../../MvpXslTransformTests/test.xml"), new MyXmlResolver());
            MemoryStream ms = new MemoryStream();
            xslt.Transform(input, Arguments, new XmlOutput(ms));
            CompareResults(resolverTestStandardResult, ms.ToArray());
        }

        [Test]
        public void ResolverTestIXPathNavigableInput()
        {
            MvpXslTransform xslt = GetMvpXslTransform2();
            XmlInput input = new XmlInput(new XPathDocument("../../MvpXslTransformTests/test.xml"), new MyXmlResolver());
            MemoryStream ms = new MemoryStream();
            xslt.Transform(input, Arguments, new XmlOutput(ms));
            CompareResults(resolverTestStandardResult, ms.ToArray());
        }

        [Test]
        public void ExsltTest()
        {
            MvpXslTransform xslt = new MvpXslTransform();
            xslt.Load("../../MvpXslTransformTests/exslt-test.xslt");
            XmlInput input = new XmlInput("../../MvpXslTransformTests/test.xml");
            MemoryStream ms = new MemoryStream();
            xslt.Transform(input, Arguments, new XmlOutput(ms));
            string expected = "<out>3</out>";            
            CompareResults(Encoding.ASCII.GetBytes(expected), ms.ToArray());
        }

        [Test]       
        public void NoExsltTest()
        {
            MvpXslTransform xslt = new MvpXslTransform();
            xslt.Load("../../MvpXslTransformTests/exslt-test.xslt");
            XmlInput input = new XmlInput("../../MvpXslTransformTests/test.xml");
            MemoryStream ms = new MemoryStream();
            xslt.SupportedFunctions = Mvp.Xml.Exslt.ExsltFunctionNamespace.None;
            try
            {
                xslt.Transform(input, Arguments, new XmlOutput(ms));
            }
            catch (XsltException) { return; }
            Assert.Fail("Shoudn't be here.");
        }
    }

    public class MyXmlResolver : XmlUrlResolver
    {
        public MyXmlResolver() {}

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri.Scheme == "my")
            {
                string xml = "<resolver>data</resolver>";
                return XmlReader.Create(new StringReader(xml));
            }
            else
            {
                return base.GetEntity(absoluteUri, role, ofObjectToReturn);
            }
        }
    }
}
