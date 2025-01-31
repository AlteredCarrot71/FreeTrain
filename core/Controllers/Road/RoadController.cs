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
using FreeTrain.Contributions.Road;
using FreeTrain.Framework;
using FreeTrain.Framework.Plugin;
using FreeTrain.Views;
using FreeTrain.Views.Map;
using FreeTrain.World;
using FreeTrain.World.Road;
using FreeTrain.Contributions.Common;

namespace FreeTrain.Controllers.Road
{
    /// <summary>
    /// Controller to place/remove roads
    /// </summary>
    public class RoadController : AbstractLineController
    {
        /// <summary>
        /// 
        /// </summary>
        public RoadController()
            : base()
        {

        }

        private RoadContribution currentContrib;
        private TreeNode lastValidNode;
        private int currentPattern = 0;
        /// <summary>
        /// 
        /// </summary>
        protected override LineContribution Type
        { get { return currentContrib; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="canvas"></param>
        /// <param name="pt"></param>
        protected override void Draw(Direction d, DrawContext canvas, Point pt)
        {
            ResourceUtil.EmptyChip.DrawShape(canvas.Surface, pt, Color.Blue);
            //			RoadPattern.getStraight(d).drawAlpha( canvas.surface, pt );
        }

        #region Windows Form Designer generated code
        private System.Windows.Forms.Label level;
        private System.Windows.Forms.Label description;
        private System.Windows.Forms.TreeView contribTree;

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        protected void InitializeComponent()
        {
            //base.InitializeComponent();
            this.SuspendLayout();
            this.buttonRemove.Location = new System.Drawing.Point(194, 132);
            this.buttonPlace.Location = new System.Drawing.Point(136, 132);
            this.buttonRemove.Visible = false;
            this.buttonPlace.Visible = false;
            this.buttonRemove.Size = new System.Drawing.Size(58, 25);
            this.buttonPlace.Size = new System.Drawing.Size(58, 25);
            this.picture.Location = new System.Drawing.Point(132, 56);
            this.picture.Size = new System.Drawing.Size(124, 88);
            this.picture.Click += new EventHandler(picture_Click);
            this.picture.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.toolTip.SetToolTip(this.picture, "Click to display another pattern");
            //! this.toolTip.SetToolTip(this.picture, "クリックで別のパターンを表示");
            // 
            // contribTree
            // 
            this.contribTree = new System.Windows.Forms.TreeView();
            this.contribTree.FullRowSelect = true;
            this.contribTree.HideSelection = false;
            this.contribTree.ImageIndex = -1;
            this.contribTree.Location = new System.Drawing.Point(0, 0);
            this.contribTree.Name = "contribTree";
            // set contribution tree on TreeView
            makeContribTree();
            this.contribTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)));
            this.contribTree.SelectedImageIndex = -1;
            this.contribTree.ShowLines = false;
            this.contribTree.ShowRootLines = true;
            this.contribTree.Indent = 10;
            this.contribTree.Sorted = true;
            this.contribTree.Size = new System.Drawing.Size(128, 147);
            this.contribTree.TabIndex = 0;
            this.contribTree.AfterSelect += new TreeViewEventHandler(contribTree_AfterSelect);
            // 
            // description
            // 
            this.description = new Label();
            this.description.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.description.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.description.Location = new System.Drawing.Point(132, 4);
            this.description.Name = "description";
            this.description.Size = new System.Drawing.Size(124, 32);
            this.description.TabIndex = 9;
            this.description.Text = currentContrib.OneLineDescription;
            this.description.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.description, this.description.Text);
            // 
            // level
            // 
            this.level = new System.Windows.Forms.Label();
            this.level.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.level.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.level.Location = new System.Drawing.Point(132, 38);
            this.level.Name = "level";
            this.level.Size = new System.Drawing.Size(124, 16);
            this.level.TabIndex = 10;
            this.level.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RoadController
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
            this.ClientSize = new System.Drawing.Size(260, 148);
            this.Controls.Add(this.description);
            this.Controls.Add(this.level);
            this.Controls.Add(this.contribTree);
            this.Resize += new EventHandler(this.UpdateSize);
            this.Text = "Road construction";
            //! this.Text = "道路工事";
            this.ResumeLayout(false);

        }
        #endregion

        private void picture_Click(object sender, EventArgs e)
        {
            currentPattern++;
            if (currentPattern > 2) currentPattern = 0;
            currentContrib.PreviewPatternIdx = currentPattern;
            UpdatePreview();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void UpdateSize(object sender, System.EventArgs e)
        {
            this.contribTree.Width = (this.Width - 5) / 2;
            this.description.Width = this.contribTree.Width - 10;
            this.description.Left = this.contribTree.Width + 5;
            this.level.Left = this.description.Left;
            this.level.Width = this.description.Width;
            this.picture.Width = this.description.Width;
            this.picture.Left = this.description.Left;
            UpdatePreview();
        }

        private void makeContribTree()
        {
            RoadContribution[] contribs = PluginManager.Roads;
            if (contribs.Length > 0)
            {
                currentContrib = contribs[0];

                for (int idx = 0; idx < contribs.Length; idx++)
                {
                    RoadContribution rc = contribs[idx];
                    //string[] path = rc.name.Split(new char[]{'(',')','i','j','/','\\'});
                    string[] path = rc.Name.Split(new char[] { '(', ')', '/', '\\' });
                    TreeNodeCollection parent = contribTree.Nodes;
                    TreeNode node = null;
                    int m = path.Length - 1;
                    for (int i = 0; i <= m; i++)
                    {
                        string label = path[i].Trim();
                        if (label.Length == 0) continue;
                        bool find = false;
                        foreach (TreeNode n in parent)
                        {
                            if (n.Text.Equals(label))
                            {
                                find = true;
                                node = n;
                                break;
                            }
                        }
                        if (!find)
                        {
                            node = new TreeNode(label);
                            parent.Add(node);
                        }
                        if (node.Tag == null || m == i)
                            node.Tag = rc;
                        parent = node.Nodes;
                    }
                    lastValidNode = node;
                }
            }
        }

        private void contribTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (contribTree.SelectedNode == null)
            {
                contribTree.SelectedNode = lastValidNode;
            }
            int idxnew;
            if (contribTree.SelectedNode.Tag == null)
                idxnew = contribTree.SelectedNode.Nodes[0].Index;
            else
            {
                lastValidNode = contribTree.SelectedNode;
                idxnew = lastValidNode.Index;
            }
            if (currentContrib == lastValidNode.Tag)
                return;
            currentContrib = (RoadContribution)lastValidNode.Tag;
            description.Text = currentContrib.OneLineDescription;
            level.Text = ToStyleDescription(currentContrib.Style);
            toolTip.SetToolTip(this.description, this.description.Text);
            currentContrib.PreviewPatternIdx = currentPattern;
            this.Text = this.Type.Name;
            UpdatePreview();
        }

        static private string ToStyleDescription(RoadStyle style)
        {
            string text = new string[] { "Undefined", "Path", "Street", "Highway" }[(int)style.Type];
            //! string text = new string[]{"未定義","小道","街路","高速道"}[(int)style.Type];
            if (style.Type == MajorRoadType.street || style.Type == MajorRoadType.highway)
            {
                if (style.CarLanes > 0)
                    text = string.Format("{0}-lane {1}", style.CarLanes, text);
                //! text = string.Format("{0}車線{1}",style.CarLanes,text);
                if (style.Sidewalk == SidewalkType.pavement)
                    text = " with sidewalk" + text;
                //! text = "歩道付き"+text;
            }
            return text;
        }
    }
}
