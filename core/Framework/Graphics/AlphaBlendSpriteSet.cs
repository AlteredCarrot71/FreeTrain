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
using FreeTrain.Framework;
using FreeTrain.World;
using FreeTrain.Framework.Graphics;

namespace FreeTrain.Framework.Graphics
{
    /// <summary>
    /// Builds a set of sprites for alpha-blending blit
    /// from a set of ordinary sprites.
    /// 
    /// This object keeps a reference to DirectDraw surfaces,
    /// so it needs to be disposed.
    /// 
    /// The crux is that we need to avoid the overlap of sprites.
    /// </summary>
    public class AlphaBlendSpriteSet : IDisposable
    {
        /// <summary>
        /// DirectDraw surface.
        /// </summary>
        private Surface[, ,] surfaces;

        /// <summary>
        /// Sprites built for alpha-blending.
        /// </summary>
        public readonly ISprite[, ,] sprites;
        /// <summary>
        /// 
        /// </summary>
        public readonly Distance size;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        public AlphaBlendSpriteSet(ISprite[, ,] src)
        {
            int X = src.GetLength(0);
            int Y = src.GetLength(1);
            int Z = src.GetLength(2);
            surfaces = new Surface[X, Y, Z];
            sprites = new ISprite[X, Y, Z];
            size = new Distance(X, Y, Z);

            for (int z = 0; z < Z; z++)
            {
                for (int y = 0; y < Y; y++)
                {
                    for (int x = 0; x < X; x++)
                    {
                        Size sz = src[x, y, z].Size;
                        if (sz.Height <= 0 || sz.Width <= 0)
                        {
                            sprites[x, y, z] = NullSprite.theInstance;
                            continue;	// this voxel is invisible
                        }

                        Surface surface = new Surface(sz.Width, sz.Height);// ResourceUtil.directDraw.createOffscreenSurface(sz);
                        surfaces[x, y, z] = surface;
                        surface.Fill(Color.Magenta);
                        surface.SourceColorKey = Color.Magenta;

                        Point offset = src[x, y, z].Offset;

                        // first copy the sprite
                        src[x, y, z].Draw(surface, offset);

                        // then mask areas that will be hidden by other sprites
                        for (int xx = 0; xx <= x; xx++)
                        {
                            for (int yy = y; yy < Y; yy++)
                            {
                                for (int zz = z; zz < Z; zz++)
                                {
                                    if (xx == x && yy == y && zz == z)
                                        continue;	// skip this sprite

                                    Point pt = offset;
                                    pt.X += 16 * ((xx - x) + (yy - y));
                                    pt.Y += 8 * (-(xx - x) + (yy - y) - (zz - z) * 2);
                                    src[xx, yy, zz].DrawShape(surface, pt, Color.Magenta);
                                }
                            }
                        }

                        sprites[x, y, z] = new DirectSprite(surface, offset, new Point(0, 0), surface.Size);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            foreach (Surface s in surfaces)
                if (s != null) s.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public ISprite getSprite(Distance d)
        {
            return sprites[d.x, d.y, d.z];
        }
    }
}
