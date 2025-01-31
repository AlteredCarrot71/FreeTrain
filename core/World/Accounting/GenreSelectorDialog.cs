#region LICENSE
/*
 * Copyright (C) 2007 - 2008 FreeTrain Team (http://freetrain.sourceforge.net)
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
#endregion LICENSE

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using FreeTrain.Framework.Plugin;

namespace FreeTrain.World.Accounting
{
    /// <summary>
    /// Let the user select a list of account genre.
    /// </summary>
    public class GenreSelectorDialog : System.Windows.Forms.Form
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        public GenreSelectorDialog(AccountGenre[] current)
        {
            InitializeComponent();

            selector.availables =
                PluginManager.ListContributions(typeof(AccountGenre));
            selector.selected = current;
        }

        /// <summary>
        /// Obtain the list of selected genres in a modifiable array.
        /// </summary>
        public AccountGenre[] selected
        {
            get
            {
                IList l = selector.selected;
                AccountGenre[] r = new AccountGenre[l.Count];
                l.CopyTo(r, 0);
                return r;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private FreeTrain.Controls.SubListSelector selector;
        private System.ComponentModel.Container components = null;

        private void InitializeComponent()
        {
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.selector = new FreeTrain.Controls.SubListSelector();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okButton.Location = new System.Drawing.Point(216, 205);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(96, 26);
            this.okButton.TabIndex = 8;
            this.okButton.Text = "&OK";
            this.okButton.Click += new System.EventHandler(this.onOK);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(320, 205);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(96, 26);
            this.cancelButton.TabIndex = 9;
            this.cancelButton.Text = "&Cancel";
            // 
            // selector
            // 
            this.selector.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.selector.availables = null;
            this.selector.Location = new System.Drawing.Point(8, 9);
            this.selector.Name = "selector";
            this.selector.Size = new System.Drawing.Size(408, 188);
            this.selector.TabIndex = 1;
            this.selector.title1 = "&Available Items:";
            this.selector.title2 = "&Selected Items:";
            // 
            // GenreSelectorDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(426, 238);
            this.Controls.Add(this.selector);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GenreSelectorDialog";
            this.ShowInTaskbar = false;
            this.Text = "Display Settings";
            this.TopMost = true;
            this.ResumeLayout(false);

        }
        #endregion

        private void onOK(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

    }
}
