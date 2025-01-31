﻿#region LICENSE
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
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using FreeTrain.Contributions.Land;
using FreeTrain.Contributions.Common;
using FreeTrain.Contributions.Sound;
using FreeTrain.Views;
using FreeTrain.Views.Map;
using FreeTrain.World;
using FreeTrain.World.Terrain;
using FreeTrain.Framework;
using FreeTrain.Framework.Graphics;
using FreeTrain.Framework.Plugin;
using FreeTrain.Util;

namespace FreeTrain.Controllers.Land
{
    /// <summary>
    /// Controller that allows the user to
    /// place/remove lands.
    /// </summary>
    public class BulldozeController : AbstractControllerImpl
    {
        private Bitmap previewBitmap;
        /// <summary>
        /// 
        /// </summary>
        public BulldozeController()
        {
            InitializeComponent();
            previewBitmap = ResourceUtil.LoadSystemBitmap("bulldozer.bmp");
            preview.Image = previewBitmap;
            LandBuilderContribution builder = (LandBuilderContribution)PluginManager.GetContribution("{AE43E6DB-39F0-49FE-BE18-EE3FAC248FDE}");
            //TODO: fix this
            //currentController = builder.createBuilder(new ControllerSiteImpl(this));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            preview.Image = null;
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);

            if (previewBitmap != null)
                previewBitmap.Dispose();
        }

        #region Designer generated code
        private System.Windows.Forms.PictureBox preview;
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.preview = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.preview)).BeginInit();
            this.SuspendLayout();
            // 
            // preview
            // 
            this.preview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.preview.Location = new System.Drawing.Point(0, 0);
            this.preview.Name = "preview";
            this.preview.Size = new System.Drawing.Size(112, 87);
            this.preview.TabIndex = 1;
            this.preview.TabStop = false;
            // 
            // BulldozeController
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(165, 146);
            this.Controls.Add(this.preview);
            this.Name = "BulldozeController";
            this.Text = "Bulldozer";
            ((System.ComponentModel.ISupportInitialize)(this.preview)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion
    }
}

