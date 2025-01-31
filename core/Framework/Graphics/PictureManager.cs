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
using System.Diagnostics;
using FreeTrain.World;

namespace FreeTrain.Framework.Graphics
{
    /// <summary>
    /// Repository of pictures.
    /// </summary>
    public class PictureManager
    {
        /// <summary>
        /// Event fired when a DirectDraw surface is found to be lost.
        /// </summary>
        public static EventHandler onSurfaceLost;

        /// <summary>
        /// Dictionary of id->Picture
        /// </summary>
        private static readonly IDictionary dic = new Hashtable();

        // prohibit instance creation
        private PictureManager() { }

        static PictureManager()
        {
            onSurfaceLost += new EventHandler(_onSurfaceLost);
            WorldDefinition.onNewWorld += new EventHandler(reset);
        }


        /// <summary>
        /// Get the picture with a given id, or throw an exception.
        /// </summary>
        /// <returns>
        ///   Always return a non-null valid object.
        /// </returns>
        public static Picture get(string id)
        {
            Picture pic = (Picture)dic[id];
            if (pic == null)
                throw new GraphicsException("unable to find picture of " + id);
            return pic;
        }

        /// <summary>
        /// Checks if a picture of the specified ID is already registered.
        /// </summary>
        public static bool contains(string id)
        {
            return dic[id] != null;
        }

        /// <summary>
        /// Add a new picture.
        /// </summary>
        public static void add(Picture pic)
        {
            if (dic[pic.id] != null)
                throw new GraphicsException("picture " + pic.id + " is already registered");
            dic.Add(pic.id, pic);
        }

        /// <summary>
        /// Called by Clock at sunrise and sunset.
        /// 
        /// invalidates all the surfaces so that they will be reloaded.
        /// Since this is a static method, it cannot be registered as an ordinary clock handler.
        /// </summary>
        public static void reset()
        {
            foreach (Picture pic in dic.Values)
                pic.setDirty();
        }

        private static void reset(object sender, EventArgs e)
        {
            reset();
        }


        /// <summary>
        /// Called when DirectDraw surfaces are lost. This method releases the pictures.
        /// </summary>
        private static void _onSurfaceLost(object sender, EventArgs e)
        {
            Debug.WriteLine("DirectDraw surfaces are lost");
            foreach (Picture pic in dic.Values)
                pic.release();
        }

        // TODO: priodical surface eviction
    }
}
