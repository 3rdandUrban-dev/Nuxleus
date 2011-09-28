using System;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;
using XmlLab.nxslt;
using System.IO;
using System.Globalization;
using System.Xml.Xsl;
using System.Collections.Specialized;

namespace XmlLab.NxsltTasks.NAnt
{
    [TaskName("nxslt")]
    public class NxsltTask : Task
    {
        #region privates
        private NXsltOptions nxsltOptions = new NXsltOptions();
        private XsltParameterCollection xsltParameters = new XsltParameterCollection();
        private XsltExtensionObjectCollection xsltExtensions = new XsltExtensionObjectCollection();
        private FileSet inFiles = new FileSet();
        private Uri inFile = null;
        private FileInfo outFile = null;
        private string extension = "html";
        private DirectoryInfo destDir;
        private Uri style;
        #endregion        


        #region Properties
        /// <summary>Source XML document to be transformed.</summary>
        [TaskAttribute("in")]        
        public Uri In
        {
            get { return inFile; }
            set { inFile = value; }
        }

        /// <summary>XSLT stylesheet file. If given as path, it can
        /// be relative to the project's basedir or absolute.</summary>
        [TaskAttribute("style")]       
        public Uri Style
        {
            get { return style; }
            set { style = value; }
        }

        /// <summary>Principal output file.</summary>
        [TaskAttribute("out")]        
        public FileInfo Out
        {
            get { return outFile; }
            set { outFile = value; }
        }

        /// <summary>Strip non-significant whitespace from source and stylesheet.</summary>
        [TaskAttribute("stripwhitespace")]
        [BooleanValidator()]
        public bool StripWhitespace
        {
            get { return nxsltOptions.StripWhiteSpace; }
            set { nxsltOptions.StripWhiteSpace = value; }
        }

        /// <summary>Resolve external definitions during parse phase.</summary>
        [TaskAttribute("resolveexternals")]
        [BooleanValidator()]
        public bool ResolveExternals
        {
            get { return nxsltOptions.ResolveExternals; }
            set { nxsltOptions.ResolveExternals = value; }
        }

        /// <summary>Process XInclude during parse phase.</summary>
        [TaskAttribute("resolvexinclude")]
        [BooleanValidator()]
        public bool ResolveXInclude
        {
            get { return nxsltOptions.ProcessXInclude; }
            set { nxsltOptions.ProcessXInclude = value; }
        }

        /// <summary>Validate documents during parse phase.</summary>
        [TaskAttribute("validate")]
        [BooleanValidator()]
        public bool Validate
        {
            get { return nxsltOptions.ValidateDocs; }
            set { nxsltOptions.ValidateDocs = value; }
        }

        /// <summary>Show load and transformation timings.</summary>
        [TaskAttribute("showtimings")]
        [BooleanValidator()]
        public bool ShowTimings
        {
            get { return nxsltOptions.ShowTiming; }
            set { nxsltOptions.ShowTiming = value; }
        }

        /// <summary>Pretty-print source document.</summary>
        [TaskAttribute("prettyprint")]
        [BooleanValidator()]
        public bool PrettyPrint
        {
            get { return nxsltOptions.PrettyPrintMode; }
            set { nxsltOptions.PrettyPrintMode = value; }
        }

        /// <summary>Get stylesheet URL from xml-stylesheet PI in source document.</summary>
        [TaskAttribute("getstylesheetfrompi")]
        [BooleanValidator()]
        public bool GetStylesheetFromPI
        {
            get { return nxsltOptions.GetStylesheetFromPI; }
            set { nxsltOptions.GetStylesheetFromPI = value; }
        }

        /// <summary>Use named URI resolver class.</summary>
        [TaskAttribute("resolver")]
        public string Resolver
        {
            get { return nxsltOptions.ResolverTypeName; }
            set { nxsltOptions.ResolverTypeName = Project.ExpandProperties(value, Location); }
        }

        /// <summary>Assembly file name to look up URI resolver class.</summary>
        [TaskAttribute("assemblyfile")]
        public string AssemblyFile
        {
            get { return nxsltOptions.AssemblyFileName; }
            set { nxsltOptions.AssemblyFileName = Project.ExpandProperties(value, Location); }
        }

        /// <summary>Assembly full or partial name to look up URI resolver class.</summary>
        [TaskAttribute("assemblyname")]
        public string AssemblyName
        {
            get { return nxsltOptions.AssemblyName; }
            set { nxsltOptions.AssemblyName = Project.ExpandProperties(value, Location); }
        }

        /// <summary>Allow multiple output documents.</summary>
        [TaskAttribute("multioutput")]
        [BooleanValidator()]
        public bool MultiOutput
        {
            get { return nxsltOptions.MultiOutput; }
            set { nxsltOptions.MultiOutput = value; }
        }        

        /// <summary>
        /// Credentials in username:password@domain format to be
        /// used in Web request authentications when loading source XML.</summary>
        [TaskAttribute("xmlcredentials")]
        public string XmlCredentials
        {
            set { nxsltOptions.SourceCredential = NXsltArgumentsParser.ParseCredentials(Project.ExpandProperties(value, Location)); }
        }

        /// <summary>
        /// Credentials in username:password@domain format to be
        /// used in Web request authentications when loading XSLT stylesheet.
        /// </summary>
        [TaskAttribute("xsltcredentials")]
        public string XsltCredentials
        {
            set { nxsltOptions.XSLTCredential = NXsltArgumentsParser.ParseCredentials(Project.ExpandProperties(value, Location)); }
        }

        /// <summary>XSLT parameters to be passed to the XSLT transformation.</summary>
        [BuildElementCollection("parameters", "parameter")]
        public XsltParameterCollection Parameters
        {
            get { return xsltParameters; }            
        }

        /// <summary>XSLT extension objects to be passed to the XSLT transformation.</summary>
        [BuildElementCollection("extensionobjects", "extensionobject")]
        public XsltExtensionObjectCollection ExtensionObjects
        {
            get { return xsltExtensions; }
        }

        /// <summary>Specifies a list of input files to be transformed.</summary>
        [BuildElement("infiles")]
        public FileSet InFiles
        {
            get { return inFiles; }            
        }

        /// <summary>
        /// Desired file extension to be used for the targets. The default is 
        /// <c>html</c>.
        /// </summary>
        [TaskAttribute("extension")]
        public string Extension
        {
            get { return extension; }
            set { extension = value; }
        }

        /// <summary>
        /// Directory in which to store the results. The default is the project
        /// base directory.
        /// </summary>
        [TaskAttribute("destdir")]
        public DirectoryInfo DestDir
        {
            get
            {
                if (destDir == null)
                {
                    return new DirectoryInfo(Project.BaseDirectory);
                }
                return destDir;
            }
            set { destDir = value; }
        }

        #endregion

        protected override void ExecuteTask()
        {            
            if (inFiles.BaseDirectory == null)
            {
                inFiles.BaseDirectory = new DirectoryInfo(Project.BaseDirectory);
            }

            TaskReporter reporter = new TaskReporter(this);
            int rc = NXsltMain.RETURN_CODE_OK;
            try
            {
                NXsltMain nxslt = new NXsltMain();
                nxslt.setReporter(reporter);
                if (xsltParameters.Count > 0)
                {
                    if (nxsltOptions.XslArgList == null)
                    {
                        nxsltOptions.XslArgList = new XsltArgumentList();
                    }
                    foreach (XsltParameter param in xsltParameters)
                    {
                        if (param.IfDefined && !param.UnlessDefined)
                        {
                            nxsltOptions.XslArgList.AddParam(param.ParameterName,
                                param.NamespaceUri, param.Value);
                        }
                    }
                }
                if (xsltExtensions.Count > 0)
                {
                    if (nxsltOptions.XslArgList == null)
                    {
                        nxsltOptions.XslArgList = new XsltArgumentList();
                    }
                    foreach (XsltExtensionObject ext in xsltExtensions)
                    {
                        if (ext.IfDefined && !ext.UnlessDefined)
                        {
                            object extInstance = ext.CreateInstance();
                            nxsltOptions.XslArgList.AddExtensionObject(
                                ext.NamespaceUri, extInstance);
                        }
                    }
                }
                nxslt.options = nxsltOptions;
                if (style != null)
                {
                    nxslt.options.Stylesheet = style.AbsoluteUri;
                }

                StringCollection srcFiles = null;
                if (inFile != null)
                {
                    srcFiles = new StringCollection();
                    srcFiles.Add(inFile.AbsoluteUri);
                }
                else if (InFiles.FileNames.Count > 0)
                {

                    if (outFile != null)
                    {
                        throw new BuildException("The 'out' attribute cannot be used when <infiles> is specified.",
                            Location);
                    }
                    srcFiles = inFiles.FileNames;
                }

                if (srcFiles == null || srcFiles.Count == 0)
                {
                    throw new BuildException("No source files indicated; use 'in' or <infiles>.",
                        Location);
                }

                if (outFile == null && destDir == null)
                {
                    throw new BuildException("'out' and 'destdir' cannot be both omitted.", Location);
                }

                foreach (string file in srcFiles)
                {
                    Log(Level.Info, "Transforming " + file);
                    nxslt.options.Source = file;
                    if (outFile != null)
                    {
                        nxslt.options.OutFile = outFile.FullName;
                    }
                    else
                    {                        
                        string destFile =  Path.GetFileNameWithoutExtension(file) + "." + extension;                        
                        nxslt.options.OutFile = Path.Combine(destDir.FullName, destFile);
                    }                     
                    rc = nxslt.Process();
                    if (rc != NXsltMain.RETURN_CODE_OK)
                    {
                        throw new BuildException(
                            string.Format(CultureInfo.InvariantCulture,
                            "nxslt task failed.", rc), Location);
                    }
                }
            }
            catch (NXsltCommandLineParsingException clpe)
            {
                //There was an exception while parsing command line
                reporter.ReportCommandLineParsingError(Reporter.GetFullMessage(clpe));
                throw new BuildException(
                        "nxslt task failed to parse parameters.", Location, clpe);
            }
            catch (NXsltException ne)
            {
                reporter.ReportError(Reporter.GetFullMessage(ne));
                throw new BuildException(
                        "nxslt task failed.", Location, ne);
            }
            catch (BuildException)
            {
                throw;
            }
            catch (Exception e)
            {
                //Some other exception
                reporter.ReportError(NXsltStrings.Error, Reporter.GetFullMessage(e));
                throw new BuildException(
                        "nxslt task failed.", Location, e);
            }           
        }       
    }
}
