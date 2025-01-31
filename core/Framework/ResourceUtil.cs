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
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;

using SdlDotNet.Audio;
using FreeTrain.Util;
using FreeTrain.Framework.Graphics;
using FreeTrain.World;

namespace FreeTrain.Framework
{
    /// <summary>
    /// Simplified resource manager.
    /// </summary>
    public abstract class ResourceUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string FindSystemResource(string name)
        {
            string path;
            string resourcesDirectory = "Resources";

            path = Path.Combine(Core.InstallationDirectory, Path.Combine(resourcesDirectory, name));
            if (File.Exists(path))
            {
                return path;
            }

            path = Path.Combine(Core.InstallationDirectory, Path.Combine("..", Path.Combine("..", Path.Combine("core", Path.Combine(resourcesDirectory, name)))));
            if (File.Exists(path))
            {
                return path;
            }

            path = Path.Combine("..", Path.Combine("..", Path.Combine(resourcesDirectory, name)));
            if (File.Exists(path))
            {
                return path;
            }

            path = Path.Combine("..", Path.Combine(resourcesDirectory, name));
            if (File.Exists(path))
            {
                return path;
            }
            Assembly assembly = Assembly.GetAssembly(typeof(ResourceUtil));
            path = Path.Combine(Path.Combine(assembly.Location, ".."), Path.Combine(resourcesDirectory, name));
            if (File.Exists(path))
            {
                return path;
            }

            throw new FileNotFoundException("System resource: " + path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Bitmap LoadSystemBitmap(string name)
        {
            return new Bitmap(FindSystemResource(name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SdlDotNet.Audio.Sound LoadSystemSound(String name)
        {
            // can't read from stream
            return new SdlDotNet.Audio.Sound(FindSystemResource(name));
        }

        // using URI is essentially dangerous as Segment only support file names.
        // I should limit it to file names only.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static SdlDotNet.Audio.Sound LoadSound(Uri uri)
        {
            return new SdlDotNet.Audio.Sound(uri.LocalPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Picture LoadSystemPicture(string name)
        {
            string id = "{8AD4EF28-CBEF-4C73-A8FF-5772B87EF005}:" + name;

            // check if it has already been loaded
            if (PictureManager.contains(id))
            {
                return PictureManager.get(id);
            }

            // otherwise load a new picture
            return new Picture(id, FindSystemResource(name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dayName"></param>
        /// <param name="nightName"></param>
        /// <returns></returns>
        public static Picture LoadSystemPicture(string dayName, string nightName)
        {
            string id = "{8AD4EF28-CBEF-4C73-A8FF-5772B87EF005}:" + dayName;

            // check if it has already been loaded
            if (PictureManager.contains(id))
            {
                return PictureManager.get(id);
            }

            // otherwise load a new picture
            return new Picture(id, FindSystemResource(dayName), FindSystemResource(nightName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Surface LoadTimeIndependentSystemSurface(string name)
        {
            //using(Bitmap bmp=loadSystemBitmap(name))
            //	return directDraw.createSprite(bmp);
            return new Surface(FindSystemResource(name));
        }

        /// <summary>
        /// DirectDraw instance for loading surface objects.
        /// </summary>
        //public static readonly DirectDraw directDraw = new DirectDraw();

        private static Picture emptyChips = LoadSystemPicture("EmptyChip.bmp", "EmptyChip_n.bmp");
        private static Picture cursorChips = LoadSystemPicture("cursorChip.bmp", "cursorChip.bmp");

        /// <summary>
        /// 
        /// </summary>
        public static ISprite EmptyChip
        {
            get
            {
                return groundChips[0];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        public static ISprite GetGroundChip(WorldDefinition world)
        {
            if (world.Clock.season != Season.Winter)
            {
                return groundChips[0];
            }
            else
            {
                return groundChips[1];
            }
        }

        private static ISprite[] groundChips = new ISprite[]{
			new SimpleSprite(emptyChips,new Point(0,0),new Point( 0,0),new Size(32,16)),
			new SimpleSprite(emptyChips,new Point(0,0),new Point(32,0),new Size(32,16))
		};

        /// <summary>
        /// 
        /// </summary>
        private static ISprite removerChip =
            new SimpleSprite(cursorChips, new Point(0, 0), new Point(0, 0), new Size(32, 16));

        /// <summary>
        /// 
        /// </summary>
        public static ISprite RemoverChip
        {
            get { return ResourceUtil.removerChip; }
            set { ResourceUtil.removerChip = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        private static ISprite underWaterChip =
            new SimpleSprite(emptyChips, new Point(0, 0), new Point(64, 0), new Size(32, 16));

        /// <summary>
        /// 
        /// </summary>
        public static ISprite UnderWaterChip
        {
            get { return ResourceUtil.underWaterChip; }
            set { ResourceUtil.underWaterChip = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        private static ISprite underGroundChip =
            new SimpleSprite(emptyChips, new Point(0, 0), new Point(96, 0), new Size(32, 16));

        /// <summary>
        /// 
        /// </summary>
        public static ISprite UnderGroundChip
        {
            get { return ResourceUtil.underGroundChip; }
            set { ResourceUtil.underGroundChip = value; }
        }
    }
}
