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
using System.Runtime.InteropServices;
using System.Resources;
using System.Reflection;
using Org.Mentalis.SecurityServices.Resources;
using Org.Mentalis.SecurityServices.Authentication;

namespace Org.Mentalis.SecurityServices.Smartcard.UI {
	/// <summary>
	/// Represents a dialog that allows the user to input a PIN number.
	/// </summary>
	public sealed class InputPinForm : System.Windows.Forms.Form {
		private System.Windows.Forms.Label DescriptionLabel;
		private System.Windows.Forms.PictureBox SmartcardImage;
		private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Button AbortButton;
		private System.Windows.Forms.TextBox PinText;

		private string m_PIN;
        private string m_Description;
        private PasswordValidator m_Validator;
		private System.Windows.Forms.Label PinLabel;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Initializes a new instance of the InputPinForm.
		/// </summary>
        public InputPinForm() : this(null, null) { }
        /// <summary>
        /// Initializes a new instance of the InputPinForm.
        /// </summary>
        /// <param name="validator">The validator that checks the PIN code for validity.</param>
        public InputPinForm(PasswordValidator validator)
            : this(validator, null) {
        }
        /// <summary>
        /// Initializes a new instance of the InputPinForm.
        /// </summary>
        /// <param name="validator">The validator that checks the PIN code for validity.</param>
        /// <param name="description">A description that's shown to the user. If this parameter is a null reference, a default description will be shown.</param>
        public InputPinForm(PasswordValidator validator, string description) {
			InitializeComponent();
            m_Description = description;
            if (validator == null)
                m_Validator = new PasswordValidator(new IValidator[] { new NumericValidator(4, false) });
            else
                m_Validator = validator;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputPinForm));
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.SmartcardImage = new System.Windows.Forms.PictureBox();
            this.OkButton = new System.Windows.Forms.Button();
            this.AbortButton = new System.Windows.Forms.Button();
            this.PinText = new System.Windows.Forms.TextBox();
            this.PinLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.SmartcardImage)).BeginInit();
            this.SuspendLayout();
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.Location = new System.Drawing.Point(72, 24);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Size = new System.Drawing.Size(224, 40);
            this.DescriptionLabel.TabIndex = 3;
            this.DescriptionLabel.Text = "#Description#";
            // 
            // SmartcardImage
            // 
            this.SmartcardImage.Image = ((System.Drawing.Image)(resources.GetObject("SmartcardImage.Image")));
            this.SmartcardImage.Location = new System.Drawing.Point(16, 16);
            this.SmartcardImage.Name = "SmartcardImage";
            this.SmartcardImage.Size = new System.Drawing.Size(48, 48);
            this.SmartcardImage.TabIndex = 2;
            this.SmartcardImage.TabStop = false;
            // 
            // OkButton
            // 
            this.OkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.OkButton.Location = new System.Drawing.Point(128, 112);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(80, 24);
            this.OkButton.TabIndex = 6;
            this.OkButton.Text = "#OK#";
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // AbortButton
            // 
            this.AbortButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.AbortButton.Location = new System.Drawing.Point(216, 112);
            this.AbortButton.Name = "AbortButton";
            this.AbortButton.Size = new System.Drawing.Size(80, 24);
            this.AbortButton.TabIndex = 5;
            this.AbortButton.Text = "#Cancel#";
            this.AbortButton.Click += new System.EventHandler(this.AbortButton_Click);
            // 
            // PinText
            // 
            this.PinText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PinText.Location = new System.Drawing.Point(128, 64);
            this.PinText.Name = "PinText";
            this.PinText.PasswordChar = 'X';
            this.PinText.Size = new System.Drawing.Size(152, 20);
            this.PinText.TabIndex = 7;
            this.PinText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PinText_KeyPress);
            // 
            // PinLabel
            // 
            this.PinLabel.Location = new System.Drawing.Point(72, 64);
            this.PinLabel.Name = "PinLabel";
            this.PinLabel.Size = new System.Drawing.Size(50, 20);
            this.PinLabel.TabIndex = 8;
            this.PinLabel.Text = "#PIN#";
            this.PinLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // InputPinForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(306, 143);
            this.Controls.Add(this.PinLabel);
            this.Controls.Add(this.PinText);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.AbortButton);
            this.Controls.Add(this.DescriptionLabel);
            this.Controls.Add(this.SmartcardImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputPinForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "#Caption#";
            this.Load += new System.EventHandler(this.InputPinForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.SmartcardImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// Gets the PIN entered by the user.
		/// </summary>
		/// <value>A string that contains the PIN.</value>
		public string PIN {
			get {
				return m_PIN;
			}
		}

		private void InputPinForm_Load(object sender, System.EventArgs e) {
			m_PIN = null;
            SetLabels();
            PinText.Select();
		}

		private void SetLabels() {
            this.Text = ResourceController.GetString("PinForm_Caption");
            if (m_Description == null)
                DescriptionLabel.Text = ResourceController.GetString("PinForm_Description");
            else
                DescriptionLabel.Text = m_Description;
            PinLabel.Text = ResourceController.GetString("PinForm_PIN");
            OkButton.Text = ResourceController.GetString("PinForm_OK");
            AbortButton.Text = ResourceController.GetString("PinForm_Cancel");
		}

		private void PinText_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == 13)
                OkButton_Click(null, null);
		}

		private void OkButton_Click(object sender, System.EventArgs e) {
            if (!m_Validator.Validate(PinText.Text)) {
                MessageBox.Show(this, ResourceController.GetString("PinForm_InvalidPin"), ResourceController.GetString("PinForm_InvalidPinTitle"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            m_PIN = PinText.Text;
			this.Close();
		}

		private void AbortButton_Click(object sender, System.EventArgs e) {
			m_PIN = null;
			this.Close();
		}
	}
}