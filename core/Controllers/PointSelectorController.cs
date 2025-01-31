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
using FreeTrain.Views;
using FreeTrain.Views.Map;
using FreeTrain.World;

namespace FreeTrain.Controllers
{
    /// <summary>
    /// Partial <c>ModalController</c> implementation that selects
    /// a particular location.
    /// </summary>
    public abstract class PointSelectorController : IModalController, IMapOverlay
    {
        /// <summary>
        /// 
        /// </summary>
        protected Location currentPos = Location.Unplaced;
        /// <summary>
        /// 
        /// </summary>
        protected readonly IControllerSite site;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_site"></param>
        public PointSelectorController(IControllerSite _site)
        {
            this.site = _site;
        }

        /// <summary>
        /// Called when a selected location is changed.
        /// Usually an application doesn't need to do anything.
        /// </summary>
        /// <param name="loc"></param>
        protected virtual void onSelectionChanged(Location loc)
        {
        }

        /// <summary>
        /// Called when the player selects a location.
        /// </summary>
        protected abstract void OnLocationSelected(Location loc);




        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="canvas"></param>
        public virtual void DrawAfter(QuarterViewDrawer view, DrawContext canvas)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="canvas"></param>
        public virtual void DrawBefore(QuarterViewDrawer view, DrawContext canvas)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="canvas"></param>
        /// <param name="loc"></param>
        /// <param name="pt"></param>
        public virtual void DrawVoxel(QuarterViewDrawer view, DrawContext canvas, Location loc, Point pt)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void OnAttached()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void OnDetached()
        {
            // clear the remaining image
            if (currentPos != Location.Unplaced)
                WorldDefinition.World.OnVoxelUpdated(currentPos);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="loc"></param>
        /// <param name="ab"></param>
        public virtual void OnMouseMove(MapViewWindow source, Location loc, Point ab)
        {
            if (currentPos != Location.Unplaced)
                WorldDefinition.World.OnVoxelUpdated(currentPos);
            currentPos = loc;
            WorldDefinition.World.OnVoxelUpdated(currentPos);

            onSelectionChanged(currentPos);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="loc"></param>
        /// <param name="ab"></param>
        public void OnRightClick(MapViewWindow source, Location loc, Point ab)
        {
            close();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="loc"></param>
        /// <param name="ab"></param>
        public void OnClick(MapViewWindow source, Location loc, Point ab)
        {
            OnLocationSelected(loc);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void close()
        {
            site.Close();
        }
        /// <summary>
        /// 
        /// </summary>
        public abstract ILocationDisambiguator Disambiguator { get; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Name { get { return site.Name; } }

        // can be overrided by a derived class to return another object.
        /// <summary>
        /// 
        /// </summary>
        public virtual IMapOverlay Overlay
        {
            get
            {
                // return this object if it implements MapOverlay by itself.
                return this as IMapOverlay;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void UpdatePreview()
        { }
    }
}
