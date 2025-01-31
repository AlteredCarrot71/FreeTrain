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
using FreeTrain.World;

namespace FreeTrain.World.Road.DummyCars
{
    /// <summary>
    /// Accessory implementation.
    /// </summary>
    [Serializable]
    public class DummyCar : TrafficVoxel.IAccessory
    {
        private readonly byte index;
        private readonly DummyCarContribution contrib;
        private readonly int color;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="contrib"></param>
        /// <param name="color"></param>
        /// <param name="index"></param>
        public DummyCar(TrafficVoxel target, DummyCarContribution contrib, int color, int index)
        {
            this.index = (byte)index;
            this.contrib = contrib;
            this.color = color;
            target.accessory = this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="display"></param>
        /// <param name="pt"></param>
        public void DrawBefore(DrawContext display, Point pt)
        {
            //contrib.sprites[index].draw( display.surface, pt );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="display"></param>
        /// <param name="pt"></param>
        public void DrawAfter(DrawContext display, Point pt)
        {
            contrib.sprites[color, index].Draw(display.Surface, pt);
        }
        /// <summary>
        /// 
        /// </summary>
        public void OnRemoved() { }
    }
}
