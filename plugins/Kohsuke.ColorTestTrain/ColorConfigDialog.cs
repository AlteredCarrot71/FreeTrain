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
using FreeTrain.Contributions.Train;

namespace FreeTrain.World.Rail.ColorTestTrain
{
    /// <summary>
    /// ColorConfigDialog
    /// </summary>
    public class ColorConfigDialog : Form
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="car"></param>
        public ColorConfigDialog(ColorTestTrainCar car)
        {
            InitializeComponent();
            this.car = car;
            comboType.DataSource = ColoredTrainPictureContribution.list();
            picture = ColoredTrainPictureContribution.list()[0];
            colors = car.colors;
            updateDialog();
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

        private readonly ColorTestTrainCar car;
        private ColoredTrainPictureContribution picture;
        private Color[] colors;

        #region Windows Form Designer generated code
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox boxBase;
        private System.Windows.Forms.Button buttonBase;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonCopy;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Button buttonLine1;
        private System.Windows.Forms.PictureBox boxLine1;
        private System.Windows.Forms.Button buttonLine2;
        private System.Windows.Forms.PictureBox boxLine2;
        private System.Windows.Forms.Button buttonLine3;
        private System.Windows.Forms.PictureBox boxLine3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboType;
        private System.ComponentModel.Container components = null;
        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.boxBase = new System.Windows.Forms.PictureBox();
            this.buttonBase = new System.Windows.Forms.Button();
            this.buttonLine1 = new System.Windows.Forms.Button();
            this.boxLine1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonLine2 = new System.Windows.Forms.Button();
            this.boxLine2 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.buttonLine3 = new System.Windows.Forms.Button();
            this.boxLine3 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.boxBase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.boxLine1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.boxLine2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.boxLine3)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Body color:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // boxBase
            // 
            this.boxBase.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.boxBase.Location = new System.Drawing.Point(64, 43);
            this.boxBase.Name = "boxBase";
            this.boxBase.Size = new System.Drawing.Size(64, 26);
            this.boxBase.TabIndex = 1;
            this.boxBase.TabStop = false;
            // 
            // buttonBase
            // 
            this.buttonBase.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonBase.Location = new System.Drawing.Point(136, 43);
            this.buttonBase.Name = "buttonBase";
            this.buttonBase.Size = new System.Drawing.Size(64, 26);
            this.buttonBase.TabIndex = 2;
            this.buttonBase.Text = "&Base";
            this.buttonBase.Click += new System.EventHandler(this.buttonBase_Click);
            // 
            // buttonLine1
            // 
            this.buttonLine1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonLine1.Location = new System.Drawing.Point(136, 78);
            this.buttonLine1.Name = "buttonLine1";
            this.buttonLine1.Size = new System.Drawing.Size(64, 26);
            this.buttonLine1.TabIndex = 5;
            this.buttonLine1.Text = "Set (&1)";
            this.buttonLine1.Click += new System.EventHandler(this.buttonLine1_Click);
            // 
            // boxLine1
            // 
            this.boxLine1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.boxLine1.Location = new System.Drawing.Point(64, 78);
            this.boxLine1.Name = "boxLine1";
            this.boxLine1.Size = new System.Drawing.Size(64, 26);
            this.boxLine1.TabIndex = 4;
            this.boxLine1.TabStop = false;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 26);
            this.label2.TabIndex = 3;
            this.label2.Text = "Belt color 1:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // buttonLine2
            // 
            this.buttonLine2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonLine2.Location = new System.Drawing.Point(136, 113);
            this.buttonLine2.Name = "buttonLine2";
            this.buttonLine2.Size = new System.Drawing.Size(64, 26);
            this.buttonLine2.TabIndex = 8;
            this.buttonLine2.Text = "Set (&2)";
            this.buttonLine2.Click += new System.EventHandler(this.buttonLine2_Click);
            // 
            // boxLine2
            // 
            this.boxLine2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.boxLine2.Location = new System.Drawing.Point(64, 113);
            this.boxLine2.Name = "boxLine2";
            this.boxLine2.Size = new System.Drawing.Size(64, 26);
            this.boxLine2.TabIndex = 7;
            this.boxLine2.TabStop = false;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 26);
            this.label3.TabIndex = 6;
            this.label3.Text = "Belt color 2:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // buttonCopy
            // 
            this.buttonCopy.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonCopy.Location = new System.Drawing.Point(32, 182);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(168, 26);
            this.buttonCopy.TabIndex = 9;
            this.buttonCopy.Text = "Copy color setting to clipboard";
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // colorDialog
            // 
            this.colorDialog.AnyColor = true;
            this.colorDialog.FullOpen = true;
            // 
            // buttonLine3
            // 
            this.buttonLine3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonLine3.Location = new System.Drawing.Point(136, 147);
            this.buttonLine3.Name = "buttonLine3";
            this.buttonLine3.Size = new System.Drawing.Size(64, 26);
            this.buttonLine3.TabIndex = 12;
            this.buttonLine3.Text = "Set (&3)";
            this.buttonLine3.Click += new System.EventHandler(this.buttonLine3_Click);
            // 
            // boxLine3
            // 
            this.boxLine3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.boxLine3.Location = new System.Drawing.Point(64, 147);
            this.boxLine3.Name = "boxLine3";
            this.boxLine3.Size = new System.Drawing.Size(64, 26);
            this.boxLine3.TabIndex = 11;
            this.boxLine3.TabStop = false;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 147);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 26);
            this.label4.TabIndex = 10;
            this.label4.Text = "Belt color 3:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // comboType
            // 
            this.comboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboType.Location = new System.Drawing.Point(64, 9);
            this.comboType.Name = "comboType";
            this.comboType.Size = new System.Drawing.Size(136, 21);
            this.comboType.TabIndex = 13;
            this.comboType.SelectedIndexChanged += new System.EventHandler(this.comboType_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 26);
            this.label5.TabIndex = 14;
            this.label5.Text = "Image:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ColorConfigDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(223, 225);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboType);
            this.Controls.Add(this.buttonLine3);
            this.Controls.Add(this.boxLine3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonCopy);
            this.Controls.Add(this.buttonLine2);
            this.Controls.Add(this.boxLine2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonLine1);
            this.Controls.Add(this.boxLine1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonBase);
            this.Controls.Add(this.boxBase);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColorConfigDialog";
            this.ShowInTaskbar = false;
            this.Text = "Color Test Train Settings";
            ((System.ComponentModel.ISupportInitialize)(this.boxBase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.boxLine1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.boxLine2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.boxLine3)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void updateDialog()
        {
            boxBase.BackColor = colors[0];
            boxLine1.BackColor = colors[1];
            boxLine2.BackColor = colors[2];
            boxLine3.BackColor = colors[3];
            car.picture = picture;
            car.colors = colors;
            WorldDefinition.World.OnAllVoxelUpdated();	// redraw
        }

        private void buttonBase_Click(object sender, EventArgs e)
        {
            colors[0] = selectColor(colors[0]);
            updateDialog();
        }
        private void buttonLine1_Click(object sender, EventArgs e)
        {
            colors[1] = selectColor(colors[1]);
            updateDialog();
        }
        private void buttonLine2_Click(object sender, EventArgs e)
        {
            colors[2] = selectColor(colors[2]);
            updateDialog();
        }
        private void buttonLine3_Click(object sender, EventArgs e)
        {
            colors[3] = selectColor(colors[3]);
            updateDialog();
        }

        private Color selectColor(Color org)
        {
            colorDialog.Color = org;
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
                return colorDialog.Color;
            else
                return org;
        }

        private void buttonCopy_Click(object sender, System.EventArgs e)
        {
            Clipboard.SetDataObject(
                string.Format("<colorMap picture=\"{0}\" base=\"{1}\" line1=\"{2}\" line2=\"{3}\" line3=\"{4}\" />",
                    picture.Id,
                    displayName(colors[0]),
                    displayName(colors[1]),
                    displayName(colors[2]),
                    displayName(colors[3])));
        }

        private string displayName(Color c)
        {
            return string.Format("{0},{1},{2}", c.R, c.G, c.B);
        }

        private void comboType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            picture = (ColoredTrainPictureContribution)comboType.SelectedItem;
            updateDialog();
        }

    }
}
