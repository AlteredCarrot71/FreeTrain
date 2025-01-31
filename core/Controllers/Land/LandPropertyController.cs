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
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using FreeTrain.Views;
using FreeTrain.Views.Map;
using FreeTrain.World;
using FreeTrain.World.Accounting;
using FreeTrain.World.Land;
using FreeTrain.Framework;
using FreeTrain.Framework.Graphics;
using FreeTrain.Util;

namespace FreeTrain.Controllers.Land
{
    /// <summary>
    /// Controller that allows the user buy/sell land properties.
    /// </summary>
    public class LandPropertyController : AbstractControllerImpl, IControllerSite
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly IControllerSite siteImpl;

        /// <summary>
        /// 
        /// </summary>
        public LandPropertyController()
        {
            InitializeComponent();
            // create preview
            UpdatePreview();
            //this.currentController = new Logic(this);
        }
        /// <summary>
        /// 
        /// </summary>
        public override void UpdatePreview()
        {
            using (PreviewDrawer drawer = new PreviewDrawer(preview.Size, new Size(3, 3), 0))
            {
                for (int x = 0; x < 3; x++)
                    for (int y = 0; y < 3; y++)
                        drawer.Draw(LandPropertyVoxel.sprite, x, y);
                if (preview.Image != null) preview.Image.Dispose();
                preview.Image = drawer.CreateBitmap();
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

            preview.Image.Dispose();
        }

        #region Designer generated code
        private System.Windows.Forms.PictureBox preview;
        private System.ComponentModel.IContainer components = null;
        private FreeTrain.Controls.CostBox costBox;
        private System.Windows.Forms.RadioButton buttonRemove;
        private System.Windows.Forms.RadioButton buttonPlace;

        private void InitializeComponent()
        {
            this.preview = new System.Windows.Forms.PictureBox();
            this.costBox = new FreeTrain.Controls.CostBox();
            this.buttonRemove = new System.Windows.Forms.RadioButton();
            this.buttonPlace = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.preview)).BeginInit();
            this.SuspendLayout();
            // 
            // preview
            // 
            this.preview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.preview.Location = new System.Drawing.Point(8, 9);
            this.preview.Name = "preview";
            this.preview.Size = new System.Drawing.Size(96, 86);
            this.preview.TabIndex = 1;
            this.preview.TabStop = false;
            // 
            // costBox
            // 
            this.costBox.cost = 0;
            this.costBox.label = "Cost:";
            this.costBox.Location = new System.Drawing.Point(8, 95);
            this.costBox.Name = "costBox";
            this.costBox.Size = new System.Drawing.Size(96, 35);
            this.costBox.TabIndex = 7;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Appearance = System.Windows.Forms.Appearance.Button;
            this.buttonRemove.Location = new System.Drawing.Point(56, 130);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(48, 26);
            this.buttonRemove.TabIndex = 6;
            this.buttonRemove.Text = "Sell";
            this.buttonRemove.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonPlace
            // 
            this.buttonPlace.Appearance = System.Windows.Forms.Appearance.Button;
            this.buttonPlace.Checked = true;
            this.buttonPlace.Location = new System.Drawing.Point(8, 130);
            this.buttonPlace.Name = "buttonPlace";
            this.buttonPlace.Size = new System.Drawing.Size(48, 26);
            this.buttonPlace.TabIndex = 5;
            this.buttonPlace.TabStop = true;
            this.buttonPlace.Text = "Buy";
            this.buttonPlace.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LandPropertyController
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(171, 214);
            this.Controls.Add(this.costBox);
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.buttonPlace);
            this.Controls.Add(this.preview);
            this.Name = "LandPropertyController";
            this.Text = "Trade Land";
            ((System.ComponentModel.ISupportInitialize)(this.preview)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private bool isPlacing { get { return buttonPlace.Checked; } }


        /// <summary>
        /// Controller logic
        /// </summary>
        private class Logic : RectSelectorController, IMapOverlay
        {
            protected readonly LandPropertyController owner;

            internal Logic(LandPropertyController owner)
                : base(owner.siteImpl)
            {
                this.owner = owner;
            }


            protected override void OnRectSelected(Location loc1, Location loc2)
            {
                if (owner.isPlacing)
                    buy(loc1, loc2);
                else
                    sell(loc1, loc2);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="loc1"></param>
            /// <param name="loc2"></param>
            protected override void OnRectUpdated(Location loc1, Location loc2)
            {
                if (owner.isPlacing)
                    owner.costBox.cost = computePriceForBuy(loc1, loc2);
                else
                    owner.costBox.cost = computePriceForSell(loc1, loc2);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="view"></param>
            /// <param name="surface"></param>
            public void DrawBefore(QuarterViewDrawer view, DrawContext surface) { }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="view"></param>
            /// <param name="canvas"></param>
            /// <param name="loc"></param>
            /// <param name="pt"></param>
            public void DrawVoxel(QuarterViewDrawer view, DrawContext canvas, Location loc, Point pt)
            {
                if (loc.z != Anchor.z) return;

                if (Anchor != Unplaced && loc.inBetween(Anchor, CurrentLocation))
                {
                    if (owner.isPlacing)
                        LandPropertyVoxel.sprite.DrawAlpha(canvas.Surface, pt);
                    else
                        ResourceUtil.EmptyChip.DrawAlpha(canvas.Surface, pt);
                }
            }

            public void DrawAfter(QuarterViewDrawer view, DrawContext surface) { }
        }


        /// <summary>
        /// Buys region [loc1,loc2] and turn them into the privately owned property.
        /// </summary>
        public static void buy(Location loc1, Location loc2)
        {
            Debug.Assert(loc1.z == loc2.z);
            int z = loc1.z;

            for (int x = loc1.x; x <= loc2.x; x++)
            {
                for (int y = loc1.y; y <= loc2.y; y++)
                {
                    Voxel v = WorldDefinition.World[x, y, z];
                    if (v != null && !v.Entity.isOwned && v.Entity.isSilentlyReclaimable)
                    {
                        // remove the old structure if possible
                        AccountGenre.Subsidiaries.Spend(v.Entity.EntityValue);
                        v.Entity.remove();
                    }
                    v = WorldDefinition.World[x, y, z];

                    if (v == null)
                    {
                        // buy it
                        AccountGenre.Subsidiaries.Spend(WorldDefinition.World.LandValue[new Location(x, y, z)]);
                        new LandPropertyVoxel(new Location(x, y, z));
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loc1"></param>
        /// <param name="loc2"></param>
        /// <returns></returns>
        public static int computePriceForBuy(Location loc1, Location loc2)
        {
            int r = 0;
            Set s = new Set();
            int z = loc1.z;

            for (int x = loc1.x; x <= loc2.x; x++)
            {
                for (int y = loc1.y; y <= loc2.y; y++)
                {
                    Voxel v = WorldDefinition.World[x, y, z];
                    if (v != null && !v.Entity.isOwned && v.Entity.isSilentlyReclaimable)
                    {
                        // cost for removing this structure
                        if (s.Add(v.Entity))
                            r += v.Entity.EntityValue;
                    }
                    v = WorldDefinition.World[x, y, z];

                    if (v == null)
                        // cost for the land
                        r += WorldDefinition.World.LandValue[new Location(x, y, z)];
                }
            }
            return r;
        }

        /// <summary>
        /// Sells land properties of the region [loc1,loc2].
        /// </summary>
        public static void sell(Location loc1, Location loc2)
        {
            Debug.Assert(loc1.z == loc2.z);
            int z = loc1.z;

            for (int x = loc1.x; x <= loc2.x; x++)
            {
                for (int y = loc1.y; y <= loc2.y; y++)
                {
                    LandPropertyVoxel v = WorldDefinition.World[x, y, z] as LandPropertyVoxel;
                    if (v != null)
                    {
                        AccountGenre.Subsidiaries.Earn(v.LandPrice);
                        v.remove();
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loc1"></param>
        /// <param name="loc2"></param>
        /// <returns></returns>
        public static int computePriceForSell(Location loc1, Location loc2)
        {
            int r = 0;
            int z = loc1.z;

            for (int x = loc1.x; x <= loc2.x; x++)
            {
                for (int y = loc1.y; y <= loc2.y; y++)
                {
                    LandPropertyVoxel v = WorldDefinition.World[x, y, z] as LandPropertyVoxel;
                    if (v != null)
                        r += v.LandPrice;
                }
            }
            return r;
        }
    }
}

