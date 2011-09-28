/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Microsoft Public
 * License. A  copy of the license can be found in the License.html file at the
 * root of this distribution. If  you cannot locate the  Microsoft Public
 * License, please send an email to  dlr@microsoft.com. By using this source
 * code in any fashion, you are agreeing to be bound by the terms of the 
 * Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System.Collections.Generic;

namespace HostingApp {
    partial class MonthAtAGlance {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.monthCalendar = new System.Windows.Forms.MonthCalendar();
            this.textBox = new System.Windows.Forms.TextBox();
            this.label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // monthCalendar
            // 
            this.monthCalendar.Location = new System.Drawing.Point(54, 18);
            this.monthCalendar.Name = "monthCalendar";
            this.monthCalendar.TabIndex = 0;
            this.monthCalendar.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar_DateChanged);
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(54, 185);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(178, 69);
            this.textBox.TabIndex = 1;
            // 
            // label
            // 
            this.label.Location = new System.Drawing.Point(12, 262);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(258, 20);
            this.label.TabIndex = 2;
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MonthAtAGlance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 291);
            this.Controls.Add(this.label);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.monthCalendar);
            this.Name = "MonthAtAGlance";
            this.Text = "Month-at-a-glance";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MonthCalendar monthCalendar;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Label label;
    }
}

