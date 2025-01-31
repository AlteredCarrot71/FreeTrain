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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace FreeTrain.Controls
{
    /// <summary>
    /// Displays a price.
    /// </summary>
    public class CostBox : System.Windows.Forms.UserControl
    {
        private int _cost;

        /// <summary>
        /// 
        /// </summary>
        public CostBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        [
            Description("The caption of the price"),
            Category("custom")
        ]
        public string label
        {
            get { return labelTextBox.Text; }
            set { labelTextBox.Text = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [
            Description("The displayed price"),
            Category("custom")
        ]
        public int cost
        {
            get { return _cost; }
            set
            {
                _cost = value;
                costTextBox.Text = value.ToString();
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

        #region Component Designer generated code

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelTextBox;
        private System.Windows.Forms.Label costTextBox;
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.labelTextBox = new System.Windows.Forms.Label();
            this.costTextBox = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // labelTextBox
            // 
            this.labelTextBox.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.labelTextBox.BackColor = System.Drawing.Color.Transparent;
            this.labelTextBox.Location = new System.Drawing.Point(0, 8);
            this.labelTextBox.Name = "labelTextBox";
            this.labelTextBox.Size = new System.Drawing.Size(36, 16);
            this.labelTextBox.TabIndex = 0;
            this.labelTextBox.Text = "Cost:";
            //! this.labelTextBox.Text = "費用：";
            this.labelTextBox.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // costTextBox
            // 
            this.costTextBox.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.costTextBox.BackColor = System.Drawing.Color.Transparent;
            this.costTextBox.Location = new System.Drawing.Point(32, 0);
            this.costTextBox.Name = "costTextBox";
            this.costTextBox.Size = new System.Drawing.Size(60, 24);
            this.costTextBox.TabIndex = 1;
            this.costTextBox.Text = "1,200";
            this.costTextBox.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.Location = new System.Drawing.Point(0, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(104, 4);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // CostBox
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox1,
																		  this.costTextBox,
																		  this.labelTextBox});
            this.Name = "CostBox";
            this.Size = new System.Drawing.Size(96, 32);
            this.ResumeLayout(false);

        }
        #endregion
    }
}
