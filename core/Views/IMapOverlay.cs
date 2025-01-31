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
using FreeTrain.Views;
using FreeTrain.World;
using FreeTrain.Framework.Graphics;

namespace FreeTrain.Views
{
    /// <summary>
    /// Modifies the image of map view window
    /// by overlaying additional data to it.
    /// </summary>
    public interface IMapOverlay
    {
        /// <summary>
        /// Called before any voxel is drawn.
        /// </summary>
        void DrawBefore(QuarterViewDrawer view, DrawContext canvas);

        /// <summary>
        /// Called for each voxel that the view is trying to draw.
        /// </summary>
        void DrawVoxel(QuarterViewDrawer view, DrawContext canvas, Location location, Point point);

        /// <summary>
        /// Called after all the images are drawn by MapView.
        /// This can be used to draw things that will never be
        /// hidden by any objects in the World.
        /// </summary>
        void DrawAfter(QuarterViewDrawer view, DrawContext canvas);
    }
}
