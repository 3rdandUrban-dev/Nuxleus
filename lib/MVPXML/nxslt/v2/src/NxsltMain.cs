#region using
using System;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Xml.Schema;
using System.Xml;
using System.IO;
using System.Net;
using System.Text;
using System.Diagnostics;
using Mvp.Xml.XInclude;
using Mvp.Xml.Common.Xsl;
#endregion

namespace XmlLab.nxslt
{
    /// <summary>
    /// nxslt main class.
    /// </summary>
    public class NXsltMain
    {
        //Parsed command line options
        internal NXsltOptions options;
        //nxslt return codes
        internal const int RETURN_CODE_OK = 0;
        internal const int RETURN_CODE_ERROR = -1;
        //Timings
        private NXsltTimings timings;
        //reporter
        private Reporter reporter;

        /// <summary>
        /// nxslt main entry point.
        /// </summary>
        /// <param name="args">Command line args</param>
        public static int Main(string[] args)
        {
            NXsltMain nxsltMain = new NXsltMain();
            nxsltMain.setReporter(new Reporter());
            try
            {                                
                NXsltArgumentsParser clParser = new NXsltArgumentsParser();
                nxsltMain.options = clParser.ParseArguments(args);                
                //Ok, then let's process it
                return nxsltMain.Process();
            }
            catch (NXsltCommandLineParsingException clpe)
            {
                //There was an exception while parsing command line
                nxsltMain.reporter.ReportCommandLineParsingError(Reporter.GetFullMessage(clpe));
                return RETURN_CODE_ERROR;
            }
            catch (NXsltException ne)
            {
                nxsltMain.reporter.ReportError(Reporter.GetFullMessage(ne));
                return RETURN_CODE_ERROR;
            }
            catch (Exception e)
            {
                //Some other exception
                nxsltMain.reporter.ReportError(NXsltStrings.Error, Reporter.GetFullMessage(e));
                return RETURN_CODE_ERROR;
            }
        }// Main() method

        internal void setReporter(Reporter r)
        {
            this.reporter = r;
        }

        /// <summary>
        /// Process command line arguments and applies the specified stylesheet
        /// to the specified source document.
        /// </summary>
        internal int Process()
        {
            //Start timing if needed
            Stopwatch totalTimer = null;
            if (options.ShowTiming)
            {
                timings = new NXsltTimings();
                totalTimer = new Stopwatch();
                totalTimer.Start();
            }

            //Just show help
            if (options.ShowHelp)
            {
                reporter.ReportUsage();
                return RETURN_CODE_OK;
            }

            //Check that everything is in place
            if (options.Source == null && !options.LoadSourceFromStdin && !options.NoSourceXml)
            {
                reporter.ReportCommandLineParsingError(NXsltStrings.ErrorMissingSource);
                return RETURN_CODE_ERROR;
            }
            if (options.Stylesheet == null && !options.LoadStylesheetFromStdin
              && !options.GetStylesheetFromPI && !options.PrettyPrintMode)
            {
                //No stylesheet - run identity transform
                options.IdentityTransformMode = true;
            }
            if (options.PrettyPrintMode &&
              (options.Stylesheet != null || options.LoadStylesheetFromStdin || options.GetStylesheetFromPI))
            {
                reporter.ReportCommandLineParsingError(NXsltStrings.ErrorStylesheetAndPrettyPrintMode);
                return RETURN_CODE_ERROR;
            }            
            
            //Prepare source XML reader
            XmlResolver srcResolver = null;
            if (options.ResolveExternals)                
            {
                srcResolver = Utils.GetXmlResolver(options.SourceCredential, options);                
            }
            XmlReader srcReader = PrepareSourceReader(srcResolver);
            if (options.PrettyPrintMode)
            {
                //Process in pretty-print mode
                Utils.PrettyPrint(srcReader, options);
            }
            else
            {
                //Process transformation         
                XmlResolver stylesheetResolver = Utils.GetXmlResolver(options.XSLTCredential, options);

                if (options.GetStylesheetFromPI)
                {
                    //To get stylesheet from the PI we load source XML into
                    //XPathDocument (consider embedded stylesheet)
                    XPathDocument srcDoc = new XPathDocument(srcReader, XmlSpace.Preserve);
                    XPathNavigator srcNav = srcDoc.CreateNavigator();
                    //Now srcReader reads in-memory cache instead
                    srcReader = srcNav.ReadSubtree();
                    MvpXslTransform xslt = PrepareStylesheetFromPI(srcNav, stylesheetResolver);
                    Transform(srcReader, xslt, srcResolver);
                }
                else
                {
                    MvpXslTransform xslt = PrepareStylesheet(stylesheetResolver);
                    Transform(srcReader, xslt, srcResolver);
                }                
            }

            if (options.ShowTiming)
            {
                totalTimer.Stop();
                timings.TotalRunTime = totalTimer.ElapsedMilliseconds;
                reporter.ReportTimings(ref timings);
            }
            return RETURN_CODE_OK;
        }        

        /// <summary>
        /// Performs XSL Transformation.
        /// </summary>
        private void Transform(XmlReader srcReader, MvpXslTransform xslt, XmlResolver resolver)
        {
            Stopwatch transformTimer = null;
            if (options.ShowTiming)
            {
                transformTimer = new Stopwatch();
                transformTimer.Start();
            }
            if (options.OutFile != null)
            {
                //Transform to a file
                FileStream fs;
                try
                {
                    fs = File.Open(options.OutFile, FileMode.Create);
                }
                catch
                {
                    throw new NXsltException(NXsltStrings.ErrorCreatingFile, options.OutFile);
                }
                try
                {
                    XmlOutput results = new XmlOutput(fs);
                    results.XmlResolver = new OutputResolver(Path.GetDirectoryName(options.OutFile));
                    TransformImpl(srcReader, xslt, resolver, results);
                }
                finally
                {
                    fs.Close();
                }
            }
            else
            {
                //Transform to Console
                XmlOutput results = new XmlOutput(Console.Out);
                results.XmlResolver = new OutputResolver(Path.GetDirectoryName(options.OutFile));
                TransformImpl(srcReader, xslt, resolver, results);
            }
            //Save transfomation time
            if (options.ShowTiming)
            {
                transformTimer.Stop();
                timings.XsltExecutionTime = transformTimer.ElapsedMilliseconds;
            }
        }

        /// <summary>
        /// Actual transformation and error handling.
        /// </summary>    
        private void TransformImpl(XmlReader srcReader, MvpXslTransform xslt, XmlResolver resolver, XmlOutput results)
        {
            xslt.MultiOutput = options.MultiOutput;            
            try
            {
                xslt.Transform(new XmlInput(srcReader, resolver), options.XslArgList, results);
            }
            catch (XmlException xe)
            {
                string uri = options.LoadSourceFromStdin ? NXsltStrings.FromStdin : xe.SourceUri;
                throw new NXsltException(NXsltStrings.ErrorParsingDoc, uri, Reporter.GetFullMessage(xe));
            }
            catch (XmlSchemaValidationException ve)
            {
                string uri = options.LoadSourceFromStdin ? NXsltStrings.FromStdin : srcReader.BaseURI;
                throw new NXsltException(NXsltStrings.ErrorParsingDoc, uri, Reporter.GetFullMessage(ve));
            }
            catch (Exception e)
            {
                throw new NXsltException(NXsltStrings.ErrorTransform, Reporter.GetFullMessage(e));
            }

        }

        /// <summary>
        /// Prepares, loads and compiles XSLT stylesheet.
        /// </summary>    
        private MvpXslTransform PrepareStylesheet(XmlResolver stylesheetResolver)
        {
            Stopwatch xsltCompileTimer = null;
            if (options.ShowTiming)
            {
                xsltCompileTimer = new Stopwatch();
                xsltCompileTimer.Start();
            }

            XmlReader stylesheetReader = PrepareStylesheetReader(stylesheetResolver);
            MvpXslTransform xslt = CreateTransform(stylesheetResolver, stylesheetReader);

            //Save stylesheet loading/compilation time
            if (options.ShowTiming)
            {
                xsltCompileTimer.Stop();
                timings.XsltCompileTime = xsltCompileTimer.ElapsedMilliseconds;
            }

            return xslt;
        }

        /// <summary>
        /// Creates XslCompiledTransform instance for given reader and resolver.
        /// </summary>                
        private MvpXslTransform CreateTransform(XmlResolver stylesheetResolver, XmlReader stylesheetReader)
        {
            MvpXslTransform xslt = new MvpXslTransform();
            try
            {
                xslt.Load(stylesheetReader, XsltSettings.TrustedXslt, stylesheetResolver);
            }
            catch (XmlException xe)
            {
                string uri = options.LoadStylesheetFromStdin ? NXsltStrings.FromStdin : xe.SourceUri;
                throw new NXsltException(NXsltStrings.ErrorParsingDoc, uri, Reporter.GetFullMessage(xe));
            }
            catch (Exception e)
            {
                string uri = options.LoadStylesheetFromStdin ? NXsltStrings.FromStdin : stylesheetReader.BaseURI;
                throw new NXsltException(NXsltStrings.ErrorCompileStyle, uri, Reporter.GetFullMessage(e));
            }
            return xslt;
        }

        /// <summary>
        /// Prepares, loads and compiles XSLT stylesheet referenced in the PI.
        /// </summary>    
        private MvpXslTransform PrepareStylesheetFromPI(XPathNavigator srcNav, XmlResolver stylesheetResolver)
        {
            Stopwatch xsltCompileTimer = null;
            if (options.ShowTiming)
            {
                xsltCompileTimer = new Stopwatch();
                xsltCompileTimer.Start();
            }
            XPathNavigator pi = srcNav.SelectSingleNode("/processing-instruction('xml-stylesheet')");
            if (pi == null)
            {
                //Absent xml-stylesheet PI
                throw new NXsltException(NXsltStrings.ErrorInvalidPI);
            }
            //Found PI node, look for the href pseudo attribute
            string href = Utils.ExtractStylsheetHrefFromPI(pi);

            MvpXslTransform xslt = null;

            if (href.StartsWith("#"))
            {
                //Embedded stylesheet
                string id = href.Remove(0, 1);
                XPathNavigator embStylesheet = srcNav.SelectSingleNode("id('" + id + "')|/descendant::*[@id='" + id + "']");
                if (embStylesheet == null)
                {
                    //Unresolvable stylesheet URI                    
                    throw new NXsltException(NXsltStrings.ErrorPIStylesheetNotFound, href);                    
                }                
                xslt = CreateTransform(stylesheetResolver, embStylesheet.ReadSubtree());
            }
            else
            {
                //External stylesheet
                options.Stylesheet = href;
                xslt = PrepareStylesheet(stylesheetResolver);
            }            
            //Save stylesheet loading/compilation time
            if (options.ShowTiming)
            {
                xsltCompileTimer.Stop();
                timings.XsltCompileTime = xsltCompileTimer.ElapsedMilliseconds;
            }
            return xslt;
        }

        

        /// <summary>
        /// Prepares source XML reader.
        /// </summary>
        /// <returns>XmlReader over source XML</returns>
        private XmlReader PrepareSourceReader(XmlResolver srcResolver)
        {
            XmlReaderSettings srcReaderSettings = new XmlReaderSettings();            
            srcReaderSettings.ProhibitDtd = false;            
            if (options.StripWhiteSpace || options.PrettyPrintMode)
            {
                srcReaderSettings.IgnoreWhitespace = true;
            }
            if (options.ValidateDocs)
            {
                srcReaderSettings.ValidationType = ValidationType.DTD;
            }            
            srcReaderSettings.XmlResolver = srcResolver;            
            XmlReader srcReader;
            if (options.NoSourceXml)
            {
                //No source XML - create dummy one
                srcReader = XmlReader.Create(new StringReader("<dummy/>"), srcReaderSettings);
            }
            else if (options.LoadSourceFromStdin)
            {
                //Get source from stdin 
                srcReader = Utils.CreateReader(Console.OpenStandardInput(), srcReaderSettings, options, srcResolver);
            }
            else
            {
                //Get source from URI
                srcReader = Utils.CreateReader(options.Source, srcReaderSettings, options, srcResolver);
            }
            //Chain schema validaring reader on top
            if (options.ValidateDocs)
            {
                srcReaderSettings.ValidationType = ValidationType.Schema;
                srcReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
                return XmlReader.Create(srcReader, srcReaderSettings);
            }
            return srcReader;
        }

        /// <summary>
        /// Prepares stylesheet XML reader.
        /// </summary>
        /// <returns>XmlReader over source XML</returns>
        private XmlReader PrepareStylesheetReader(XmlResolver stylesheetResolver)
        {
            XmlReaderSettings stylesheetReaderSettings = new XmlReaderSettings();
            stylesheetReaderSettings.ProhibitDtd = false;
            if (options.ValidateDocs)
            {
                stylesheetReaderSettings.ValidationType = ValidationType.DTD;
            }
            if (options.StripWhiteSpace)
            {
                stylesheetReaderSettings.IgnoreWhitespace = true;
            }
            if (!options.ResolveExternals)
                stylesheetReaderSettings.XmlResolver = null;
            else
            {
                stylesheetReaderSettings.XmlResolver = stylesheetResolver;
            }
            XmlReader stylesheetReader;            
            if (options.IdentityTransformMode)
            {
                //No XSLT - use identity transformation
                stylesheetReader = XmlReader.Create(new StringReader(NXsltStrings.IdentityTransformation), stylesheetReaderSettings);
            }
            else if (options.LoadStylesheetFromStdin)
            {
                //Get stylesheet from stdin                 
                stylesheetReader = Utils.CreateReader(Console.OpenStandardInput(), stylesheetReaderSettings, options, stylesheetResolver);
            }
            else
            {
                //Get source from URI
                stylesheetReader = Utils.CreateReader(options.Stylesheet, stylesheetReaderSettings, options, stylesheetResolver);
            }
            //Chain schema validaring reader on top
            if (options.ValidateDocs)
            {
                stylesheetReaderSettings.ValidationType = ValidationType.Schema;
                stylesheetReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
                return XmlReader.Create(stylesheetReader, stylesheetReaderSettings);
            }
            return stylesheetReader;
        }


    }// NXsltMain class    
}// XmlLab.nxslt namespace
