using System;
using XmlLab.nxslt;
using System.IO;
using System.Globalization;
using System.Xml.Xsl;
using System.Collections.Specialized;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Xml;
using System.Reflection;

namespace XmlLab.NxsltTasks.MSBuild
{   
    public class Nxslt : Task
    {
        #region privates
        private NXsltOptions nxsltOptions = new NXsltOptions();
        private string xsltParameters;
        private string xsltExtensions;        
        private ITaskItem[] inFiles = null;
        private string outFile = null;
        private string extension = "html";
        private string destDir;
        private string style;
        #endregion


        #region Properties
        /// <summary>Source XML document to be transformed.</summary>        
        public ITaskItem[] In
        {
            get { return inFiles; }
            set { inFiles = value; }
        }

        /// <summary>XSLT stylesheet file. If given as path, it can
        /// be relative to the project's basedir or absolute.</summary>        
        public string Style
        {
            get { return style; }
            set { style = value; }
        }

        /// <summary>Principal output file.</summary>        
        public string Out
        {
            get { return outFile; }
            set { outFile = value; }
        }

        /// <summary>Strip non-significant whitespace from source and stylesheet.</summary>                
        public bool StripWhitespace
        {
            get { return nxsltOptions.StripWhiteSpace; }
            set { nxsltOptions.StripWhiteSpace = value; }
        }

        /// <summary>Resolve external definitions during parse phase.</summary>        
        public bool ResolveExternals
        {
            get { return nxsltOptions.ResolveExternals; }
            set { nxsltOptions.ResolveExternals = value; }
        }

        /// <summary>Process XInclude during parse phase.</summary>        
        public bool ResolveXInclude
        {
            get { return nxsltOptions.ProcessXInclude; }
            set { nxsltOptions.ProcessXInclude = value; }
        }

        /// <summary>Validate documents during parse phase.</summary>        
        public bool Validate
        {
            get { return nxsltOptions.ValidateDocs; }
            set { nxsltOptions.ValidateDocs = value; }
        }

        /// <summary>Show load and transformation timings.</summary>        
        public bool ShowTimings
        {
            get { return nxsltOptions.ShowTiming; }
            set { nxsltOptions.ShowTiming = value; }
        }

        /// <summary>Pretty-print source document.</summary>        
        public bool PrettyPrint
        {
            get { return nxsltOptions.PrettyPrintMode; }
            set { nxsltOptions.PrettyPrintMode = value; }
        }

        /// <summary>Get stylesheet URL from xml-stylesheet PI in source document.</summary>        
        public bool GetStylesheetFromPI
        {
            get { return nxsltOptions.GetStylesheetFromPI; }
            set { nxsltOptions.GetStylesheetFromPI = value; }
        }

        /// <summary>Use named URI resolver class.</summary>        
        public string Resolver
        {
            get { return nxsltOptions.ResolverTypeName; }
            set { nxsltOptions.ResolverTypeName = value; }
        }

        /// <summary>Assembly file name to look up URI resolver class.</summary>        
        public string AssemblyFile
        {
            get { return nxsltOptions.AssemblyFileName; }
            set { nxsltOptions.AssemblyFileName = value; }
        }

        /// <summary>Assembly full or partial name to look up URI resolver class.</summary>        
        public string AssemblyName
        {
            get { return nxsltOptions.AssemblyName; }
            set { nxsltOptions.AssemblyName = value; }
        }

        /// <summary>Allow multiple output documents.</summary>        
        public bool MultiOutput
        {
            get { return nxsltOptions.MultiOutput; }
            set { nxsltOptions.MultiOutput = value; }
        }

        /// <summary>
        /// Credentials in username:password@domain format to be
        /// used in Web request authentications when loading source XML.</summary>        
        public string XmlCredentials
        {
            set { nxsltOptions.SourceCredential = NXsltArgumentsParser.ParseCredentials(value); }
        }

        /// <summary>
        /// Credentials in username:password@domain format to be
        /// used in Web request authentications when loading XSLT stylesheet.
        /// </summary>        
        public string XsltCredentials
        {
            set { nxsltOptions.XSLTCredential = NXsltArgumentsParser.ParseCredentials(value); }
        }

        /// <summary>XSLT parameters to be passed to the XSLT transformation.</summary>        
        public string Parameters
        {
            get { return xsltParameters; }
            set { xsltParameters = value; }
        }

        /// <summary>XSLT extension objects to be passed to the XSLT transformation.</summary>        
        public string ExtensionObjects
        {
            get { return xsltExtensions; }
            set { xsltExtensions = value; }
        }       

        /// <summary>
        /// Desired file extension to be used for the targets. The default is 
        /// <c>html</c>.
        /// </summary>        
        public string Extension
        {
            get { return extension; }
            set { extension = value; }
        }

        /// <summary>
        /// Directory in which to store the results. The default is the project
        /// base directory.
        /// </summary>        
        public string DestDir
        {
            get
            {
                if (destDir == null)
                {
                    return Path.GetDirectoryName(this.BuildEngine.ProjectFileOfTaskNode);
                }
                return destDir;
            }
            set { destDir = value; }
        }

        #endregion

        public override bool  Execute()
        {            
            TaskReporter reporter = new TaskReporter(this);
            int rc = NXsltMain.RETURN_CODE_OK;
            try
            {
                try
                {
                    NXsltMain nxslt = new NXsltMain();
                    nxslt.setReporter(reporter);

                    if (xsltParameters != null)
                    {                        
                        if (nxsltOptions.XslArgList == null)
                        {
                            nxsltOptions.XslArgList = new XsltArgumentList();
                        }
                        ParseParameters();                                                
                    }

                    if (xsltExtensions != null)
                    {
                        if (nxsltOptions.XslArgList == null)
                        {
                            nxsltOptions.XslArgList = new XsltArgumentList();
                        }

                        ParseExtensions();                                                
                    }

                    nxslt.options = nxsltOptions;
                    if (style != null)
                    {
                        nxslt.options.Stylesheet = style;
                    }                                                           
                                        
                    if (inFiles == null || inFiles.Length == 0)
                    {
                        throw new NxsltTaskException("No source files indicated; use 'in' or <infiles>.");
                    }

                    if (outFile == null && destDir == null)
                    {
                        throw new NxsltTaskException("'out' and 'destdir' cannot be both omitted.");
                    }

                    foreach (ITaskItem file in inFiles)
                    {
                        Log.LogMessage(MessageImportance.Normal, "Transforming " + file.ItemSpec);
                        nxslt.options.Source = file.ItemSpec;
                        if (outFile != null)
                        {
                            nxslt.options.OutFile = outFile;
                        }
                        else
                        {
                            string destFile = Path.GetFileNameWithoutExtension(file.ItemSpec) + "." + extension;                            
                            nxslt.options.OutFile = Path.Combine(destDir, destFile);
                        }
                        rc = nxslt.Process();
                        if (rc != NXsltMain.RETURN_CODE_OK)
                        {
                            throw new NxsltTaskException(
                                string.Format(CultureInfo.InvariantCulture,
                                "nxslt task failed.", rc));
                        }
                    }
                }
                catch (NXsltCommandLineParsingException clpe)
                {
                    //There was an exception while parsing command line
                    reporter.ReportCommandLineParsingError(Reporter.GetFullMessage(clpe));
                    throw new NxsltTaskException(
                            "nxslt task failed to parse parameters.", clpe);
                }
                catch (NXsltException ne)
                {
                    reporter.ReportError(Reporter.GetFullMessage(ne));
                    throw new NxsltTaskException(
                            "nxslt task failed.", ne);
                }
            }
            catch (Exception e)
            {                
                reporter.ReportError(NXsltStrings.Error, Reporter.GetFullMessage(e));
                return false;
            }
            return true;
        }

        private void ParseExtensions()
        {            
            XmlReaderSettings rs = new XmlReaderSettings();
            rs.ConformanceLevel = ConformanceLevel.Fragment;
            using (XmlReader r = XmlReader.Create(new StringReader(xsltExtensions), rs))
            {
                while (r.Read())
                {
                    if (r.NodeType == XmlNodeType.Element &&
                        r.LocalName == "ExtensionObject")
                    {
                        string extTypeName = r["TypeName"];
                        if (extTypeName == null)
                        {
                            throw new NxsltTaskException("'TypeName' attribute of the <ExtensionObject> element is required.");
                        }
                        string extNamespaceUri = r["NamespaceUri"];

                        string extAssemblyName = r["Assembly"];
                        if (extAssemblyName == null)
                        {
                            throw new NxsltTaskException("'Assembly' attribute of the <ExtensionObject> element is required.");
                        }
                        
                        Uri baseUri = new Uri(this.BuildEngine.ProjectFileOfTaskNode);
                        Uri assemblyFullPath = new Uri(baseUri, extAssemblyName);
                        Assembly extAssembly = null;
                        try
                        {
                            extAssembly = Assembly.LoadFile(assemblyFullPath.AbsolutePath);
                        } catch 
                        {                        
                            throw new NxsltTaskException("Unable to load assembly '" + assemblyFullPath + "'.");
                        }
                        Type extObjType = extAssembly.GetType(extTypeName);
                        if (extObjType == null)
                        {
                            throw new NxsltTaskException("Unable to find type '" + extTypeName + "'.");
                        }
                        object extInstance = null;
                        try
                        {
                            extInstance = Activator.CreateInstance(extObjType);
                        }
                        catch
                        {
                            throw new NxsltTaskException("Unable to create an instance of the type '" + extTypeName + "'.");
                        }
                        nxsltOptions.XslArgList.AddExtensionObject(
                            extNamespaceUri==null?"":extNamespaceUri, extInstance);

                        Log.LogMessage(MessageImportance.Low, "Adding XSLT extension object {0}:{1}, from \"{2}\"",
                            extNamespaceUri == null ? "" : "{" + extNamespaceUri + "}", extTypeName, extAssemblyName);
                    }
                }
            }           
        }

        private void ParseParameters()
        {
            XmlReaderSettings rs = new XmlReaderSettings();
            rs.ConformanceLevel = ConformanceLevel.Fragment;
            using (XmlReader r = XmlReader.Create(new StringReader(xsltParameters), rs))
            {
                while (r.Read())
                {
                    if (r.NodeType == XmlNodeType.Element &&
                        r.LocalName == "Parameter")
                    {
                        string name = r["Name"];
                        if (name == null)
                        {
                            throw new NxsltTaskException("'Name' attribute of the <Parameter> element is required.");
                        }
                        string ns = r["NamespaceUri"];
                        string val = r["Value"];
                        nxsltOptions.XslArgList.AddParam(
                            name, ns == null ? "" : ns, val == null ? "" : val);
                        Log.LogMessage(MessageImportance.Low, "Adding XSLT parameter {0}{1}=\"{2}\"",
                            ns == null ? "" : "{" + ns + "}", name, val);
                    }
                }
            }
        }
    }
}
