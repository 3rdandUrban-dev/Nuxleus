using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;


using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Package;
using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;


namespace XmlMvp.XPathmania.Internal
{
    internal class VisualStudioDocument
    {
        #region Private Fields
        private IVsTextLines textEditorBuffer;
        private XmlDocument currentXmlDocument;
        private DomLoader currentDomloader;
        private XmlNamespaceManager currentNamespaceManager;
        static private readonly Guid xmlLanguageServiceGuid = new Guid("f6819a78-a205-47b5-be1c-675b3c7f0b8e");
        IVsLanguageInfo currentDocumentLanguageInfo;
        private Source currentSource;
        private IVsWindowFrame currentWindowFrame;

        #endregion

        internal VisualStudioDocument(IVsWindowFrame frame, IVsTextLines buffer, IOleServiceProvider docViewService)
        {
            this.textEditorBuffer = buffer;
            this.currentWindowFrame = frame;
            if (GetSource(docViewService))
            {
                LoadDocument();
            }
        }


        #region Properties
        /// <summary>
        /// Provides read and write access to the text buffer of this document using two-dimensional coordinates.
        /// </summary>
        internal IVsTextLines TextEditorBuffer
        {
            get { return this.textEditorBuffer; }
        }

        internal Source Source
        {
            get { return this.currentSource; }
        }

        internal XmlNamespaceManager XmlNamespaceManager
        {
            get
            {
                if (this.currentNamespaceManager == null)
                {
                    this.currentNamespaceManager = new XmlNamespaceManager(this.currentXmlDocument.NameTable);
                }
                return this.currentNamespaceManager;
            }
        }


        internal string DocumentElementDefaultNamespace
        {
            get 
            {
                if (this.currentXmlDocument == null)
                {
                    return null;
                }
                else
                {
                    if (this.currentXmlDocument.DocumentElement.Prefix == "" && this.currentXmlDocument.DocumentElement.NamespaceURI != "")
                    {
                        return this.currentXmlDocument.DocumentElement.NamespaceURI;
                    }
                    else
                    {
                        return null;
                    }
                } 
            }
        }
	
        #endregion


        #region Internal Methods

        internal void Reload()
        {
            this.LoadDocument();
        }

        internal bool IsValidXmlDocument()
        {
            if (this.currentXmlDocument == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        internal XmlNodeList Query(string xpath)
        {
            return this.currentXmlDocument.SelectNodes(xpath,this.currentNamespaceManager);
        }

        internal LineInfo GetLineInfo(XmlNode node)
        {
            return this.currentDomloader.GetLineInfo(node);
        }

        internal void Show()
        {
            if (!Convert.ToBoolean(this.currentWindowFrame.IsVisible()))
            {
                this.currentWindowFrame.Show();
            }
        }


        #endregion

        private bool GetSource(IOleServiceProvider docViewService)
        {
            Guid languageServiceGuid;
            this.textEditorBuffer.GetLanguageServiceID(out languageServiceGuid);
            if (languageServiceGuid != VisualStudioDocument.xmlLanguageServiceGuid)
            {
                return false;
            }

            //IOleServiceProvider docViewService = (IOleServiceProvider)docViewServiceObject;
            IntPtr ptr;
            Guid guid = VisualStudioDocument.xmlLanguageServiceGuid;
            Guid iid = typeof(IVsLanguageInfo).GUID;
            if (!ErrorHandler.Succeeded(docViewService.QueryService(ref guid, ref iid, out ptr)))
            {
                return false;
            }

            this.currentDocumentLanguageInfo = (IVsLanguageInfo)Marshal.GetObjectForIUnknown(ptr);
            Marshal.Release(ptr);

            LanguageService langsvc = this.currentDocumentLanguageInfo as LanguageService;
            this.currentSource = langsvc.GetSource(this.textEditorBuffer);
            return true;
        }

        private void LoadDocument()
        {

            string xml = this.currentSource.GetText();

            try
            {
                StringReader sr = new StringReader(xml);
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ProhibitDtd = false;
                settings.IgnoreWhitespace = false;
                settings.CheckCharacters = false;
                settings.ValidationType = ValidationType.None;
                settings.ValidationEventHandler += new ValidationEventHandler(OnValidationEvent);
                XmlReader reader = XmlReader.Create(sr, settings);

                this.currentDomloader = new DomLoader();
                using (reader)
                {
                    this.currentXmlDocument = this.currentDomloader.Load(reader);
                }
                this.currentNamespaceManager = new System.Xml.XmlNamespaceManager(this.currentXmlDocument.NameTable);

                //if (this.currentXmlDocument.DocumentElement.NamespaceURI != string.Empty)
                //{
                //    int NewRowId = NamespaceGridView.Rows.Add();
                //    NamespaceGridView.Rows[NewRowId].Cells[0].Value = "ns1";
                //    NamespaceGridView.Rows[NewRowId].Cells[1].Value = this.currentXmlDocument.DocumentElement.NamespaceURI;

                //}

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                this.currentXmlDocument = null;
                this.currentDomloader = null;
                //throw;
            }
            return;
        }



        #region Xml Schema Validation Routines
        void OnValidationEvent(object sender, System.Xml.Schema.ValidationEventArgs e)
        {
            // todo: log errors in error list window.
        }
        #endregion
    }
}
