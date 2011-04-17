/*
 *   Mentalis.org Security Services for .NET 2.0
 * 
 *     Copyright © 2006, The Mentalis.org Team
 *     All rights reserved.
 *     http://www.mentalis.org/
 *
 *
 *   Redistribution and use in source and binary forms, with or without
 *   modification, are permitted provided that the following conditions
 *   are met:
 *
 *     - Redistributions of source code must retain the above copyright
 *        notice, this list of conditions and the following disclaimer. 
 *
 *     - Neither the name of the Mentalis.org Team, nor the names of its contributors
 *        may be used to endorse or promote products derived from this
 *        software without specific prior written permission. 
 *
 *   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 *   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 *   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 *   FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
 *   THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 *   INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 *   (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 *   SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 *   HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 *   STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 *   ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 *   OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Resources;
using System.Reflection;
using Org.Mentalis.SecurityServices.Win32;
using Org.Mentalis.SecurityServices.Smartcard;
using Org.Mentalis.SecurityServices.Resources;

namespace Org.Mentalis.SecurityServices.Smartcard.UI {
	/// <summary>
	/// Represents a dialog that allows the user to select a card reader to use.
	/// </summary>
	public sealed class SelectReaderForm : System.Windows.Forms.Form {
		private System.Windows.Forms.PictureBox SmartcardImage;
		private System.Windows.Forms.GroupBox ReadersGroup;
		private System.Windows.Forms.ListView CardReaderList;
		private System.Windows.Forms.ImageList CardImageList;
		private System.Windows.Forms.TextBox CardTypeText;
		private System.Windows.Forms.TextBox CardStatusText;
		private System.Windows.Forms.Button OkButton;
		private System.Windows.Forms.Label DescriptionLabel;
		private System.Windows.Forms.Label ReadersLabel;
		private System.Windows.Forms.Label InsertedLabel;
		private System.Windows.Forms.Label StatusLabel;
		private System.Windows.Forms.Button AbortButton;
		private System.Windows.Forms.Timer UpdateTimer;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Initializes a new instance of the SelectReaderForm.
		/// </summary>
        /// <remarks>All cards are considered valid.</remarks>
		public SelectReaderForm() : this(null) { }
		/// <summary>
		/// Initializes a new instance of the SelectReaderForm.
		/// </summary>
        /// <param name="atrs">A list of ATRs that are considered valid.</param>
        /// <exception cref="ArgumentException"><i>atrs</i> is not a null reference, but it is empty or contains null references.</exception>
        public SelectReaderForm(Atr[] atrs) {
            if (atrs != null && atrs.Length == 0)
                throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"), "atrs");
            if (atrs != null) {
                for (int i = 0; i < atrs.Length; i++) {
                    if (atrs[i] == null)
                        throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"), "atrs");
                }
            }
            InitializeComponent();

            m_Atrs = atrs;
            int ret = NativeMethods.SCardEstablishContext(NativeMethods.SCARD_SCOPE_USER, IntPtr.Zero, IntPtr.Zero, out m_Context);
            if (ret != NativeMethods.SCARD_S_SUCCESS)
                throw new SmartcardException(ResourceController.GetString("Error_SmartcardEstablishContext"));
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
        /// <param name="disposing"><b>true</b> to release both managed and unmanaged resources; <b>false</b> to release only unmanaged resources.</param>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			if (m_Context != IntPtr.Zero) {
                NativeMethods.SCardReleaseContext(m_Context);
				m_Context = IntPtr.Zero;
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectReaderForm));
            this.SmartcardImage = new System.Windows.Forms.PictureBox();
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.ReadersGroup = new System.Windows.Forms.GroupBox();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.CardStatusText = new System.Windows.Forms.TextBox();
            this.CardTypeText = new System.Windows.Forms.TextBox();
            this.InsertedLabel = new System.Windows.Forms.Label();
            this.CardReaderList = new System.Windows.Forms.ListView();
            this.CardImageList = new System.Windows.Forms.ImageList(this.components);
            this.ReadersLabel = new System.Windows.Forms.Label();
            this.AbortButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.SmartcardImage)).BeginInit();
            this.ReadersGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // SmartcardImage
            // 
            this.SmartcardImage.Image = ((System.Drawing.Image)(resources.GetObject("SmartcardImage.Image")));
            this.SmartcardImage.Location = new System.Drawing.Point(8, 8);
            this.SmartcardImage.Name = "SmartcardImage";
            this.SmartcardImage.Size = new System.Drawing.Size(48, 48);
            this.SmartcardImage.TabIndex = 0;
            this.SmartcardImage.TabStop = false;
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.Location = new System.Drawing.Point(72, 24);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Size = new System.Drawing.Size(264, 69);
            this.DescriptionLabel.TabIndex = 1;
            this.DescriptionLabel.Text = "#Description#";
            // 
            // ReadersGroup
            // 
            this.ReadersGroup.Controls.Add(this.StatusLabel);
            this.ReadersGroup.Controls.Add(this.CardStatusText);
            this.ReadersGroup.Controls.Add(this.CardTypeText);
            this.ReadersGroup.Controls.Add(this.InsertedLabel);
            this.ReadersGroup.Controls.Add(this.CardReaderList);
            this.ReadersGroup.Controls.Add(this.ReadersLabel);
            this.ReadersGroup.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ReadersGroup.Location = new System.Drawing.Point(8, 96);
            this.ReadersGroup.Name = "ReadersGroup";
            this.ReadersGroup.Size = new System.Drawing.Size(328, 176);
            this.ReadersGroup.TabIndex = 2;
            this.ReadersGroup.TabStop = false;
            this.ReadersGroup.Text = "#";
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Location = new System.Drawing.Point(136, 72);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(51, 13);
            this.StatusLabel.TabIndex = 5;
            this.StatusLabel.Text = "#Status#";
            // 
            // CardStatusText
            // 
            this.CardStatusText.Location = new System.Drawing.Point(136, 88);
            this.CardStatusText.Multiline = true;
            this.CardStatusText.Name = "CardStatusText";
            this.CardStatusText.ReadOnly = true;
            this.CardStatusText.Size = new System.Drawing.Size(176, 72);
            this.CardStatusText.TabIndex = 4;
            // 
            // CardTypeText
            // 
            this.CardTypeText.Location = new System.Drawing.Point(136, 40);
            this.CardTypeText.Name = "CardTypeText";
            this.CardTypeText.ReadOnly = true;
            this.CardTypeText.Size = new System.Drawing.Size(176, 20);
            this.CardTypeText.TabIndex = 3;
            // 
            // InsertedLabel
            // 
            this.InsertedLabel.AutoSize = true;
            this.InsertedLabel.Location = new System.Drawing.Point(136, 24);
            this.InsertedLabel.Name = "InsertedLabel";
            this.InsertedLabel.Size = new System.Drawing.Size(59, 13);
            this.InsertedLabel.TabIndex = 2;
            this.InsertedLabel.Text = "#Inserted#";
            // 
            // CardReaderList
            // 
            this.CardReaderList.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.CardReaderList.HideSelection = false;
            this.CardReaderList.LargeImageList = this.CardImageList;
            this.CardReaderList.Location = new System.Drawing.Point(16, 40);
            this.CardReaderList.MultiSelect = false;
            this.CardReaderList.Name = "CardReaderList";
            this.CardReaderList.Size = new System.Drawing.Size(104, 120);
            this.CardReaderList.TabIndex = 1;
            this.CardReaderList.UseCompatibleStateImageBehavior = false;
            this.CardReaderList.SelectedIndexChanged += new System.EventHandler(this.CardReaderList_SelectedIndexChanged);
            // 
            // CardImageList
            // 
            this.CardImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("CardImageList.ImageStream")));
            this.CardImageList.TransparentColor = System.Drawing.Color.Magenta;
            this.CardImageList.Images.SetKeyName(0, "");
            this.CardImageList.Images.SetKeyName(1, "");
            this.CardImageList.Images.SetKeyName(2, "");
            this.CardImageList.Images.SetKeyName(3, "");
            // 
            // ReadersLabel
            // 
            this.ReadersLabel.AutoSize = true;
            this.ReadersLabel.Location = new System.Drawing.Point(16, 24);
            this.ReadersLabel.Name = "ReadersLabel";
            this.ReadersLabel.Size = new System.Drawing.Size(61, 13);
            this.ReadersLabel.TabIndex = 0;
            this.ReadersLabel.Text = "#Readers#";
            // 
            // AbortButton
            // 
            this.AbortButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.AbortButton.Location = new System.Drawing.Point(256, 280);
            this.AbortButton.Name = "AbortButton";
            this.AbortButton.Size = new System.Drawing.Size(80, 24);
            this.AbortButton.TabIndex = 3;
            this.AbortButton.Text = "#Cancel#";
            this.AbortButton.Click += new System.EventHandler(this.AbortButton_Click);
            // 
            // OkButton
            // 
            this.OkButton.Enabled = false;
            this.OkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.OkButton.Location = new System.Drawing.Point(168, 280);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(80, 24);
            this.OkButton.TabIndex = 4;
            this.OkButton.Text = "#OK#";
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Interval = 500;
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
            // 
            // SelectReaderForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(346, 311);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.AbortButton);
            this.Controls.Add(this.ReadersGroup);
            this.Controls.Add(this.DescriptionLabel);
            this.Controls.Add(this.SmartcardImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectReaderForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "#Caption#";
            this.Closed += new System.EventHandler(this.SelectReaderForm_Closed);
            this.Activated += new System.EventHandler(this.SelectReaderForm_Activated);
            this.Load += new System.EventHandler(this.SelectReaderForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.SmartcardImage)).EndInit();
            this.ReadersGroup.ResumeLayout(false);
            this.ReadersGroup.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void SelectReaderForm_Activated(object sender, System.EventArgs e) {
			CardReaderList.Select();
		}

        private void SelectReaderForm_Load(object sender, System.EventArgs e) {
            m_Selected = null;
            string[] readers = SmartcardReader.InternalGetReaders(m_Context);
            if (readers == null || readers.Length == 0) {
                MessageBox.Show(this, ResourceController.GetString("SelectReaderForm_NoReaders"), ResourceController.GetString("SelectReaderForm_NoReadersTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                this.Close();
                return;
            } else {
                m_Readers = new ReaderItem[readers.Length];
                m_States = new SCARD_READERSTATE[readers.Length];
                CardReaderList.Items.Clear();
                for (int i = 0; i < readers.Length; i++) {
                    m_Readers[i].item = CardReaderList.Items.Add(readers[i], 0);
                    m_Readers[i].name = readers[i];
                    m_States[i].szReader = readers[i];
                    m_States[i].dwCurrentState = NativeMethods.SCARD_STATE_UNAWARE;
                }
                UpdateTimer_Tick(null, null);
                UpdateTimer.Enabled = true;
            }
            SetLabels();
        }
		private void SetLabels() {
            this.Text = ResourceController.GetString("SelectReaderForm_Caption");
            DescriptionLabel.Text = ResourceController.GetString("SelectReaderForm_Description");
            ReadersGroup.Text = ResourceController.GetString("SelectReaderForm_Details");
            ReadersLabel.Text = ResourceController.GetString("SelectReaderForm_Readers");
            InsertedLabel.Text = ResourceController.GetString("SelectReaderForm_Inserted");
            StatusLabel.Text = ResourceController.GetString("SelectReaderForm_Status");
            OkButton.Text = ResourceController.GetString("SelectReaderForm_OK");
            AbortButton.Text = ResourceController.GetString("SelectReaderForm_Cancel");
		}
		private void SelectReaderForm_Closed(object sender, System.EventArgs e) {
			UpdateTimer.Enabled = false;
		}
		/// <summary>
		/// Gets the selected reader.
		/// </summary>
		/// <value>A string with the name of the selected reader. This will be a null reference if the user clicked the cancel button.</value>
		public string SelectedReader {
			get {
				return m_Selected;
			}
		}

		private void UpdateTimer_Tick(object sender, System.EventArgs e) {
			// check for updates
            if (NativeMethods.SCardGetStatusChange(m_Context, 0, m_States, m_States.Length) != NativeMethods.SCARD_S_SUCCESS) {
				return;
			}
			// update info
			bool update;
			bool ok = false;
			for(int i = 0; i < m_Readers.Length; i++) {
				update = (CardReaderList.FocusedItem == m_Readers[i].item);
                if ((m_States[i].dwEventState & NativeMethods.SCARD_STATE_PRESENT) != 0) {
                    Atr insertedAtr = new Atr(m_States[i].rgbAtr, m_States[i].cbAtr);
                    bool isValid = false;
                    if (m_Atrs == null) {
                        isValid = true;
                    } else {
                        for (int j = 0; j < m_Atrs.Length; j++) {
                            if (m_Atrs[j].Match(insertedAtr)) {
                                isValid = true;
                                break;
                            }
                        }
                    }

                    if (isValid) {
						m_Readers[i].item.ImageIndex = 2;
						if (update) {
                            CardTypeText.Text = ResourceController.GetString("SelectReaderForm_ValidCard");
                            CardStatusText.Text = ResourceController.GetString("SelectReaderForm_ValidCardDesc");
							ok = true;
						}
					} else {
						m_Readers[i].item.ImageIndex = 1;
						if (update) {
                            CardTypeText.Text = ResourceController.GetString("SelectReaderForm_UnknownCard");
                            CardStatusText.Text = ResourceController.GetString("SelectReaderForm_UnknownCardDesc");
						}
					}
                } else if ((m_States[i].dwEventState & NativeMethods.SCARD_STATE_EMPTY) != 0) {
					m_Readers[i].item.ImageIndex = 0;
					if (update) {
                        CardTypeText.Text = ResourceController.GetString("SelectReaderForm_NoCard");
                        CardStatusText.Text = ResourceController.GetString("SelectReaderForm_NoCardDesc");
					}
				} else {
					m_Readers[i].item.ImageIndex = 3;
					if (update) {
                        CardTypeText.Text = ResourceController.GetString("SelectReaderForm_UnknownStatus");
                        CardStatusText.Text = ResourceController.GetString("SelectReaderForm_UnknownStatusDesc");
					}
				}
			}
			if (OkButton.Enabled != ok)
				OkButton.Enabled = ok;
		}
		private void AbortButton_Click(object sender, System.EventArgs e) {
			m_Selected = null;
			this.Close();
		}

		private void OkButton_Click(object sender, System.EventArgs e) {
			m_Selected = this.CardReaderList.FocusedItem.Text;
			this.Close();
		}

		private void CardReaderList_SelectedIndexChanged(object sender, System.EventArgs e) {
			UpdateTimer_Tick(null, null);
		}

		private SCARD_READERSTATE[] m_States;
		private ReaderItem[] m_Readers;
		private IntPtr m_Context;
		private string m_Selected;
        private Atr[] m_Atrs;
	}

	internal struct ReaderItem {
		public string name;
		public ListViewItem item;
	}
}