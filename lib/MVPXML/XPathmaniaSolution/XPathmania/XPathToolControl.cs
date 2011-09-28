using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Package;
using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using XmlMvp.XPathmania.Internal;

namespace XmlMvp.XPathmania
{
    /// <summary>
    /// Summary description for MyControl.
    /// </summary>
    public partial class XPathToolControl : UserControl
    {
        private BindingList<XmlNodeInfo> results;
        private ArrayList errors;
        private int changeCount;
        private Internal.VisualStudioDocument currentDocument;
        private TabPage errorTabPage;

        public XPathToolControl()
        {
            InitializeComponent();

            this.xpathTextBox.KeyDown += new KeyEventHandler(XPathTextBox_KeyDown);

            this.resultsGridView.DoubleClick += new EventHandler(resultsGridView_DoubleClick);
            this.errorTabPage = queryTabControl.TabPages[2];
            this.queryTabControl.TabPages.Remove(errorTabPage);

            this.errors = new ArrayList();
            this.errorListGridView.AutoGenerateColumns = false;
            this.errorListGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.errorListGridView.DataSource = errors;
        }

        void resultsGridView_DoubleClick(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.resultsGridView.SelectedRows)
            {
                XmlNodeInfo info = (XmlNodeInfo)row.DataBoundItem;
                NavigateTo(info);
                return;
            }
        }

        void NavigateTo(XmlNodeInfo info)
        {
            try
            {
                this.currentDocument.Show();
                TextSpan span = info.CurrentSpan;
                CodeWindowManager mgr = this.currentDocument.Source.LanguageService.GetCodeWindowManagerForSource(this.currentDocument.Source);
                IVsTextView view;
                mgr.CodeWindow.GetLastActiveView(out view);
                view.SetSelection(span.iStartLine, span.iStartIndex, span.iEndLine, span.iEndIndex);
                view.EnsureSpanVisible(span);
            }
            catch { }
        }

        void NavigateSelection()
        {
            foreach (DataGridViewRow row in resultsGridView.SelectedRows)
            {
                XmlNodeInfo info = (XmlNodeInfo)row.DataBoundItem;
                NavigateTo(info);
                return;
            }
        }

        void XPathTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            { // enter is same as clicking the query button
                this.XPathQueryButton_Click(this, new EventArgs());
            }
        }

        /// <summary> 
        /// Let this control process the mnemonics.
        /// </summary>
        protected override bool ProcessDialogChar(char charCode)
        {
            // If we're the top-level form or control, we need to do the mnemonic handling
            if (charCode != ' ' && ProcessMnemonic(charCode))
            {
                return true;
            }
            return base.ProcessDialogChar(charCode);
        }

        private void GetCurrentSource()
        {
            try
            {
                IVsMonitorSelection selection = (IVsMonitorSelection)Package.GetGlobalService(typeof(SVsShellMonitorSelection));
                object pvar = null;
                if (!ErrorHandler.Succeeded(selection.GetCurrentElementValue((uint)VSConstants.VSSELELEMID.SEID_DocumentFrame, out pvar)))
                {
                    this.currentDocument = null;
                    return;
                }
                IVsWindowFrame frame = pvar as IVsWindowFrame;
                if (frame == null)
                {
                    this.currentDocument = null;
                    return;
                }

                object docData = null;
                if (!ErrorHandler.Succeeded(frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocData, out docData)))
                {
                    this.currentDocument = null;
                    return;
                }
                object docViewServiceObject;
                if (!ErrorHandler.Succeeded(frame.GetProperty((int)Microsoft.VisualStudio.Shell.Interop.__VSFPROPID.VSFPROPID_SPFrame, out docViewServiceObject)))
                {
                    this.currentDocument = null;
                    return;
                }

                IVsTextLines buffer = docData as IVsTextLines;
                if (buffer == null)
                {
                    IVsTextBufferProvider tb = docData as IVsTextBufferProvider;
                    if (tb != null)
                    {
                        tb.GetTextBuffer(out buffer);
                    }
                }
                if (buffer == null)
                {
                    this.currentDocument = null;
                    return;
                }

                IOleServiceProvider docViewService = (IOleServiceProvider)docViewServiceObject;
                if (this.currentDocument == null || buffer != this.currentDocument.TextEditorBuffer)
                {
                    this.currentDocument = new VisualStudioDocument(frame,buffer, docViewService);
                    this.changeCount = this.currentDocument.Source.ChangeCount;
                }
                else
                {
                    if (this.changeCount != this.currentDocument.Source.ChangeCount)
                    {
                        this.currentDocument.Reload();
                        this.changeCount = this.currentDocument.Source.ChangeCount;
                    }
                }
                return;

            }
            catch (Exception e)
            {
                ReportError(e);
            }
        }

        private void XPathQueryButton_Click(object sender, EventArgs e)
        {
            try
            {
                //ErrorListBox.Items.Clear();
                this.errors.Clear(); ;
                this.resultsGridView.DataSource = null;
                ClearResults();

                GetCurrentSource();

                if (!this.currentDocument.IsValidXmlDocument())
                {
                    ReportError(new ErrorInfoLine("Invalid XML Document – see Error List Window for details", errors.Count + 1, ErrorInfoLine.ErrorType.Warning));
                    return;
                }

                try
                {
                    foreach (DataGridViewRow CurrentRow in namespaceGridView.Rows)
                    {
                        if (CurrentRow.Cells[0].Value == null && CurrentRow.Cells[1].Value == null)
                        {
                            //skip empty rows
                            continue;
                        }
                        else
                        {
                            if (CurrentRow.Cells[0].Value == null)
                            {
                                ReportError(new ErrorInfoLine("Invalid Namespace Table - a prefix is required for all namespaces", errors.Count + 1, ErrorInfoLine.ErrorType.Warning));
                                return;
                            }
                            else
                            {
                                if (CurrentRow.Cells[1].Value == null)
                                {
                                    this.currentDocument.XmlNamespaceManager.AddNamespace((string)CurrentRow.Cells[0].Value, string.Empty);
                                }
                                else
                                {
                                    this.currentDocument.XmlNamespaceManager.AddNamespace((string)CurrentRow.Cells[0].Value, (string)CurrentRow.Cells[1].Value);
                                }
                            }
                        }


                    }
                }
                catch (Exception ex)
                {
                    //We should never get here
                    //ReportError(ex);
                    return;
                }

                XmlNodeList XPathResultNodeList = null;
                try
                {
                    XPathResultNodeList = this.currentDocument.Query(xpathTextBox.Text);
                }
                catch (XPathException ex)
                {
                    ReportError(ex);
                    return;
                }

                results = new BindingList<XmlNodeInfo>();

                if (XPathResultNodeList != null)
                {
                    foreach (XmlNode Node in XPathResultNodeList)
                    {
                        LineInfo CurrentLineInfo = this.currentDocument.GetLineInfo(Node);
                        if (CurrentLineInfo != null)
                        {
                            XmlNodeInfo info = new XmlNodeInfo(Node, CurrentLineInfo.LineNumber, CurrentLineInfo.LinePosition);
                            results.Add(info);
                            TextSpan span = GetNodeSpan(info);
                            IVsTextLineMarker[] amark = new IVsTextLineMarker[1];
                            int hr = this.currentDocument.TextEditorBuffer.CreateLineMarker((int)MARKERTYPE2.MARKER_EXSTENCIL,
                                span.iStartLine, span.iStartIndex, span.iEndLine, span.iEndIndex,
                                info, amark);
                            info.Marker = amark[0];
                            info.MarkerChanged += new EventHandler(OnMarkerChanged);
                            info.MarkerDeleted += new EventHandler(OnMarkerDeleted);
                        }
                    }
                }

                this.resultsGridView.AutoGenerateColumns = false;
                this.resultsGridView.DataSource = results;

                if (this.results.Count > 0)
                {
                    this.resultsGridView.Focus();
                    this.queryTabControl.SelectedTab = this.resultsTabPage;
                }
                else
                {
                    // If no results and XmlDoc has a default namespace, display a warning
                    if (!String.IsNullOrEmpty(this.currentDocument.DocumentElementDefaultNamespace) && !(this.currentDocument.DocumentElementDefaultNamespace == ""))
                    {
                        this.ReportError(new ErrorInfoLine("Document has a default namespace.  Did you make sure to add it to the Namespace Table and use its prefix in your XPath query?",errors.Count + 1,ErrorInfoLine.ErrorType.Warning));
                        return;
                    }
                }
            }
            catch { }
        }

        private void ReportError(Exception ex)
        {
            ErrorInfoLine line = new ErrorInfoLine(ex.Message, errors.Count + 1, ErrorInfoLine.ErrorType.Serious);
            ReportError(line);
        }

        private void ReportError(ErrorInfoLine line)
        {
            this.errors.Add(line);
            //this.errors.Add(line);


            if (!this.queryTabControl.TabPages.Contains(errorTabPage))
            {
                this.queryTabControl.TabPages.Add(errorTabPage);
            }
            this.queryTabControl.SelectedTab = errorTabPage;
            this.errorListGridView.Focus();


        }

        void OnMarkerChanged(object sender, EventArgs a)
        {
            this.resultsGridView.Invalidate();
        }

        void OnMarkerDeleted(object sender, EventArgs a)
        {
            XmlNodeInfo old = sender as XmlNodeInfo;
            old.MarkerDeleted -= new EventHandler(OnMarkerDeleted);
            old.MarkerChanged -= new EventHandler(OnMarkerChanged);

            this.results.Remove(old);
            this.resultsGridView.Invalidate();          
        }

        void ClearResults()
        {
            if (results != null)
            {
                foreach (XmlNodeInfo old in results)
                {
                    old.MarkerChanged -= new EventHandler(OnMarkerChanged);
                    old.MarkerDeleted -= new EventHandler(OnMarkerDeleted);
                    old.Dispose();
                }
            }
            results = null;

            if (queryTabControl.TabPages.Contains(errorTabPage))
            {
                queryTabControl.TabPages.Remove(errorTabPage);
            }
        }


        private TextSpan GetNodeSpan(XmlNodeInfo info)
        {
            int line = info.Line;
            int col = info.Column + 1; // skip '<'.
            TokenInfo token = this.currentDocument.Source.GetTokenInfo(line, col);
            TextSpan span = new TextSpan();
            if (token != null)
            {
                span.iStartLine = span.iEndLine = line;
                span.iStartIndex = token.StartIndex;
                span.iEndIndex = token.EndIndex + 1; // include last character of the token.
            }
            return span;
        }

        private void resultsGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                NavigateSelection();
                e.Handled = true;
            }
        }
    }
}
