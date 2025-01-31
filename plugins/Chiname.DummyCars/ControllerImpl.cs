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
using System.Windows.Forms;
using FreeTrain.Framework;
using FreeTrain.Views;
using FreeTrain.Views.Map;
using FreeTrain.Controllers;


namespace FreeTrain.World.Road.DummyCars
{
    /// <summary>
    /// ModalController implementation for DummyCar contribution
    /// </summary>
    public class ControllerImpl : PointSelectorController
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly DummyCarContribution contribution;
        /// <summary>
        /// 
        /// </summary>
        protected readonly int color;
        /// <summary>
        /// 
        /// </summary>
        protected readonly bool remove;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contrib"></param>
        /// <param name="site"></param>
        /// <param name="remover"></param>
        public ControllerImpl(DummyCarContribution contrib, IControllerSite site, bool remover)
            : base(site)
        {
            this.remove = remover;
            this.contribution = contrib;
            this.color = contrib.currentColor;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loc"></param>
        protected override void OnLocationSelected(Location loc)
        {
            if (remove)
            {
                if (contribution.CanBeBuilt(loc))
                {
                    if (TrafficVoxel.get(loc).accessory != null)
                        TrafficVoxel.get(loc).accessory = null;
                }
                else
                    MessageBox.Show("Can not remove");
                //! MessageBox.Show("撤去できません");
            }
            else
            {
                if (contribution.CanBeBuilt(loc))
                    contribution.Create(loc);
                else
                    MessageBox.Show("Can not place");
                //! MessageBox.Show("設置できません");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="canvas"></param>
        /// <param name="loc"></param>
        /// <param name="pt"></param>
        public override void DrawVoxel(QuarterViewDrawer view, DrawContext canvas, Location loc, Point pt)
        {
            if (base.currentPos != loc) return;
            if (!contribution.CanBeBuilt(loc)) return;

            int x;
            RoadPattern rp = TrafficVoxel.get(loc).road.pattern;
            if (rp.hasRoad(Direction.NORTH)) x = 0;
            else x = 1;

            contribution.sprites[color, x].DrawAlpha(canvas.Surface, pt);
        }
        /// <summary>
        /// 
        /// </summary>
        public override ILocationDisambiguator Disambiguator
        {
            get
            {
                return RoadDisambiguator.theInstance;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class RoadDisambiguator : ILocationDisambiguator
    {
        // the singleton instance
        /// <summary>
        /// 
        /// </summary>
        public static ILocationDisambiguator theInstance = new RoadDisambiguator();
        private RoadDisambiguator() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public bool IsSelectable(Location loc)
        {
            // if there's any rail roads, fine
            if (BaseRoad.get(loc) != null) return true;

            // or if we hit the ground
            if (WorldDefinition.World.GetGroundLevel(loc) >= loc.z) return true;

            return false;
        }
    }
}
