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
using FreeTrain.World;

namespace FreeTrain.Controllers
{
    /// <summary>
    /// Used by the MapViewController to disambiguate
    /// stacked voxels.
    /// 
    /// When an user clicks a screen, there are many locations
    /// that can match. Depending on the context, the program needs
    /// to select one of them. For example, when an user is placing
    /// a train, we'd like to select a voxel with a railroad.
    /// 
    /// This interface does this.
    /// </summary>
    public interface ILocationDisambiguator
    {
        /// <summary>
        /// Returns true if the callee prefers this location
        /// to be selected.
        /// </summary>
        bool IsSelectable(Location loc);
    }
}
