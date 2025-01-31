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
using System.Xml;
using FreeTrain.Framework.Plugin;

namespace FreeTrain.Framework.Graphics
{
    /// <summary>
    /// DefaultSpriteLoaderContributionImpl の概要の説明です。
    /// </summary>
    public class DefaultSpriteLoaderContributionImpl : SpriteLoaderContribution
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public DefaultSpriteLoaderContributionImpl(XmlElement e) : base(e) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        public override ISprite load0D(XmlElement sprite)
        {

            int h = int.Parse(sprite.Attributes["offset"].Value);

            XmlAttribute size = sprite.Attributes["size"];

            return SpriteFactory.GetSpriteFactory(sprite).CreateSprite(
                GetPicture(sprite),
                new Point(0, h),
                XmlUtil.ParsePoint(XmlUtil.SelectSingleNode(sprite, "@origin").InnerText),
                size == null ? new Size(32, 32) : XmlUtil.ParseSize(size.Value));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public override ISprite[,] load2D(XmlElement sprite, int X, int Y, int height)
        {
            Picture picture = GetPicture(sprite);
            SpriteFactory spriteFactory = SpriteFactory.GetSpriteFactory(sprite);

            ISprite[,] sprites = new ISprite[X, Y];

            Point origin = XmlUtil.ParsePoint(sprite.Attributes["origin"].Value);
            int h = height;
            XmlAttribute att = sprite.Attributes["offset"];
            if (att != null)
                h = int.Parse(att.Value);
            int maxh = int.MaxValue;
            if (sprite.Attributes["height"] != null)
                maxh = int.Parse(sprite.Attributes["height"].Value);

            for (int y = 0; y < Y; y++)
            {
                for (int x = 0; x < X; x++)
                {
                    Point sprOrigin = new Point((x + y) * 16 + origin.X, origin.Y);
                    Size sprSize = new Size(32, Math.Min(maxh, h + 16 + (y - x) * 8));

                    if (sprSize.Height == 0)
                        sprites[x, y] = NullSprite.theInstance;
                    else
                        sprites[x, y] = spriteFactory.CreateSprite(
                            picture, new Point(0, h + (y - x) * 8), sprOrigin, sprSize);
                }
            }

            return sprites;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        /// <returns></returns>
        public override ISprite[, ,] load3D(XmlElement sprite, int X, int Y, int Z)
        {
            Picture picture = GetPicture(sprite);
            SpriteFactory spriteFactory = SpriteFactory.GetSpriteFactory(sprite);

            ISprite[, ,] sprites = new ISprite[X, Y, Z];

            Point origin = XmlUtil.ParsePoint(sprite.Attributes["origin"].Value);
            int h = ((Z << 1) + (X - 1)) << 3; // calculate default offset
            XmlAttribute att = sprite.Attributes["offset"];
            if (att != null)
                h = int.Parse(att.Value);

            // top-floor
            for (int y = 0; y < Y; y++)
            {
                for (int x = 0; x < X; x++)
                {
                    Point sprOrigin = new Point(
                        (x + y) * 16 + origin.X, origin.Y + h - 16 * (Z - 1) + (y - x) * 8);

                    Size sprSize = new Size(32, 16);
                    Point voxelOrigin = sprOrigin;

                    if (y == 0 || x == X - 1)
                    {
                        sprOrigin.Y -= 16;
                        sprSize.Height += 16;
                        if (y == 0 && x == X - 1)
                        {// top of the "hat"
                            ;
                        }
                        else
                            if (y == 0 && Y > 1)
                            {// top-left edge
                                sprSize.Width = 16;
                            }
                            else
                                if (x == X - 1 && X > 1)
                                {// top-right edge
                                    sprOrigin.X += 16;
                                    sprSize.Width -= 16;
                                }
                    }

                    if (sprOrigin.Y < 0)
                    {
                        sprSize.Height += sprOrigin.Y;
                        sprOrigin.Y = 0;
                    }

                    if (sprSize.Height == 0)
                        sprites[x, y, Z - 1] = NullSprite.theInstance;
                    else
                        sprites[x, y, Z - 1] = spriteFactory.CreateSprite(
                            picture,
                            new Point(voxelOrigin.X - sprOrigin.X,
                                        voxelOrigin.Y - sprOrigin.Y),
                            sprOrigin, sprSize);
                }
            }

            // bottom-front
            if (Z > 1)
            {
                for (int y = 0; y < Y; y++)
                {
                    for (int x = 0; x < X; x++)
                    {
                        Point voxelOrigin = new Point(
                            (x + y) * 16 + origin.X, origin.Y + h + (y - x) * 8);

                        Point sprOrigin = voxelOrigin;
                        sprOrigin.Y -= (Z - 2) * 16 + 8;
                        Size sprSize;

                        if (x == 0 && y == Y - 1)
                        {// bottom
                            sprSize = new Size(32, 16 * (Z - 1) + 8);
                        }
                        else
                            if (x == 0)
                            { // left edge
                                sprSize = new Size(16, 16 * (Z - 1) + 8);
                            }
                            else
                                if (y == Y - 1)
                                {// right edge
                                    sprSize = new Size(16, 16 * (Z - 1) + 8);
                                    sprOrigin.X += 16;
                                }
                                else
                                    continue;	// invisible

                        sprites[x, y, 0] = spriteFactory.CreateSprite(
                            picture,
                            new Point(voxelOrigin.X - sprOrigin.X,
                                        voxelOrigin.Y - sprOrigin.Y),
                            sprOrigin, sprSize);
                    }
                }
            }

            // fill-in invisible cells by NullSprite
            for (int z = 0; z < Z; z++)
                for (int y = 0; y < Y; y++)
                    for (int x = 0; x < X; x++)
                        if (sprites[x, y, z] == null)
                            sprites[x, y, z] = NullSprite.theInstance;

            return sprites;
        }
    }
}
