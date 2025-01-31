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
using System.Xml;
using FreeTrain.Contributions.Common;
using FreeTrain.Contributions.Population;
using FreeTrain.Framework;
using FreeTrain.Framework.Plugin;
using FreeTrain.Framework.Plugin.Graphics;
using FreeTrain.Framework.Graphics;
using FreeTrain.World;
using FreeTrain.World.Structs;
using FreeTrain.Controllers;
using FreeTrain.Contributions.Structs;

namespace FreeTrain.World.Structs.HalfVoxelStructure
{
    /// <summary>
    /// 
    /// </summary>
    public enum PlaceSide : int 
    { 
        /// <summary>
        /// 
        /// </summary>
        Fore, 
        /// <summary>
        /// 
        /// </summary>
        Back 
    };
    /// <summary>
    /// 
    /// </summary>
    public enum SideStored : int 
    { 
        /// <summary>
        /// 
        /// </summary>
        None, 
        /// <summary>
        /// 
        /// </summary>
        Fore, 
        /// <summary>
        /// 
        /// </summary>
        Back, 
        /// <summary>
        /// 
        /// </summary>
        Both 
    };
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HalfVoxelContribution : StructureContribution
    {
        /// <summary>
        /// 
        /// </summary>
        static private readonly int hl_patterns = 6;

        /// <summary>
        /// 
        /// </summary>
        protected static int HighlightPatterns
        {
            get { return HalfVoxelContribution.hl_patterns; }
        } 

        /// <summary>
        /// 
        /// </summary>
        private StructureGroup _group = new StructureGroup("HalfVoxel");

        /// <summary>
        /// 
        /// </summary>
        public override StructureGroup Group
        {
            get { return _group; }
            //set { _group = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        static private readonly Point[] offsets = new Point[]
		{
			new Point(0,-8), new Point(-8,-8),new Point(0,-8),new Point(-8,-8),
			new Point(-8,-4), new Point(0,-4),new Point(-8,-4),new Point(0,-4)
		};
        
        /// <summary>
        /// 
        /// </summary>
        protected static Point[] Offsets
        {
            get { return HalfVoxelContribution.offsets; }
        } 


        /// <summary>
        /// Parses a commercial structure contribution from a DOM node.
        /// </summary>
        /// <exception cref="XmlException">If the parsing fails</exception>
        public HalfVoxelContribution(XmlElement e)
            : base(e)
        {
            _price = int.Parse(XmlUtil.SelectSingleNode(e, "price").InnerText);
            height = int.Parse(XmlUtil.SelectSingleNode(e, "height").InnerText);
            subgroup = XmlUtil.SelectSingleNode(e, "subgroup").InnerText;
            XmlElement spr = (XmlElement)XmlUtil.SelectSingleNode(e, "sprite");
            XmlElement pic = (XmlElement)XmlUtil.SelectSingleNode(spr, "picture");
            variation = spr.SelectSingleNode("map");
            if (variation != null)
            {
                String idc = variation.Attributes["to"].Value;
                colors = PluginManager.GetContribution(idc) as ColorLibrary;
                sprites = new SpriteSet[colors.size];
                for (int i = 0; i < colors.size; i++)
                    sprites[i] = new SpriteSet(8);
            }
            else
            {
                colors = ColorLibrary.NullLibrary;
                sprites = new SpriteSet[1];
                sprites[0] = new SpriteSet(8);
            }
            LoadSprites(spr, pic);
            XmlElement hle = (XmlElement)spr.SelectSingleNode("highlight");
            if (hle != null)
            {
                hilights = new SpriteSet[hl_patterns];
                LoadHighSprites(spr, hle);
            }
            else
                hilights = null;
        }

        #region helper methods used on reading XML
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="ep"></param>
        protected virtual void LoadSprites(XmlElement e, XmlElement ep)
        {
            Picture pic = GetPicture(ep, null);
            XmlNode cn = e.FirstChild;
            while (cn != null)
            {
                if (cn.Name.Equals("pattern"))
                {
                    SideStored ss = ParseSide(cn);
                    Direction d = ParseDirection(cn);
                    Point orgn = XmlUtil.ParsePoint(cn.Attributes["origin"].Value);
                    Point offF = GetOffset(d, PlaceSide.Fore);
                    Point offB = GetOffset(d, PlaceSide.Back);
                    Size sz = new Size(24, 8 + height * 16);
                    if (variation != null)
                    {
                        for (int i = 0; i < colors.size; i++)
                        {
                            Color c = colors[i];
                            string v = c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString();
                            variation.Attributes["to"].Value = v;
                            SpriteFactory factory = new HueTransformSpriteFactory(e);
                            if ((ss & SideStored.Fore) != 0)
                                sprites[i][d, PlaceSide.Fore] = factory.CreateSprite(pic, offF, orgn, sz);
                            if ((ss & SideStored.Back) != 0)
                                sprites[i][d, PlaceSide.Back] = factory.CreateSprite(pic, offB, orgn, sz);
                        }
                    }
                    else
                    {
                        SpriteFactory factory = new SimpleSpriteFactory();
                        if ((ss & SideStored.Fore) != 0)
                            sprites[0][d, PlaceSide.Fore] = factory.CreateSprite(pic, offF, orgn, sz);
                        if ((ss & SideStored.Back) != 0)
                            sprites[0][d, PlaceSide.Back] = factory.CreateSprite(pic, offB, orgn, sz);
                    }
                }
                cn = cn.NextSibling;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="hle"></param>
        protected virtual void LoadHighSprites(XmlElement e, XmlElement hle)
        {
            Picture pic = GetPicture(hle, "HL");
            if (pic == null || hle.Attributes["src"] == null)
                throw new FormatException("highlight picture not found.");
            string baseFileName = XmlUtil.Resolve(hle, hle.Attributes["src"].Value).LocalPath;
            using (Bitmap bit = new Bitmap(baseFileName))
            {
                for (int i = 0; i < hl_patterns; i++)
                    hilights[i] = new SpriteSet(8);

                XmlNode cn = e.FirstChild;
                while (cn != null)
                {
                    if (cn.Name.Equals("pattern"))
                    {
                        SideStored ss = ParseSide(cn);
                        Direction d = ParseDirection(cn);
                        Point orgn = XmlUtil.ParsePoint(cn.Attributes["origin"].Value);
                        Point offF = GetOffset(d, PlaceSide.Fore);
                        Point offB = GetOffset(d, PlaceSide.Back);
                        Size sz = new Size(24, 8 + height * 16);

                        // create highlight patterns
                        XmlNode hlp = cn.SelectSingleNode("highlight");
                        if (hlp != null)
                        {
                            HueShiftSpriteFactory factory = new HueShiftSpriteFactory(hl_patterns);
                            if ((ss & SideStored.Fore) != 0)
                            {
                                ISprite[] arr = factory.CreateSprites(bit, pic, offF, orgn, sz);
                                for (int i = 0; i < hl_patterns; i++)
                                    hilights[i][d, PlaceSide.Fore] = arr[i];
                            }
                            if ((ss & SideStored.Back) != 0)
                            {
                                ISprite[] arr = factory.CreateSprites(bit, pic, offB, orgn, sz);
                                for (int i = 0; i < hl_patterns; i++)
                                    hilights[i][d, PlaceSide.Back] = arr[i];
                            }
                        }
                    }//if(cn.Name.Equals("pattern"))
                    cn = cn.NextSibling;
                }//while
            }//using
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        protected Point GetOffset(Direction d, PlaceSide s)
        {
            Point o = offsets[d.index / 2 + (int)s * 4];
            return new Point(o.X, o.Y + height * 16);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        protected SideStored ParseSide(XmlNode n)
        {
            String s = n.Attributes["side"].Value;
            if (s == null || s.Equals("either"))
                return SideStored.Both;
            if (s.Equals("fore"))
                return SideStored.Fore;
            if (s.Equals("back"))
                return SideStored.Back;
            return SideStored.None;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        protected Direction ParseDirection(XmlNode n)
        {
            String s = n.Attributes["direction"].Value;
            if (s == null)
                throw new FormatException("missing direction attribute.");
            if (s.Equals("north"))
                return Direction.NORTH;
            if (s.Equals("south"))
                return Direction.SOUTH;
            if (s.Equals("west"))
                return Direction.WEST;
            if (s.Equals("east"))
                return Direction.EAST;
            throw new FormatException("invalid direction attribute.");
            //return null;
        }

        internal static Picture GetPicture(XmlElement pic, string suffix)
        {
            //XmlElement pic = (XmlElement)XmlUtil.selectSingleNode(sprite,suffix);			
            XmlAttribute r = pic.Attributes["ref"];
            if (r != null)
                // reference to externally defined pictures.
                return PictureManager.get(r.Value);

            // otherwise look for local picture definition
            XmlAttribute s = pic.Attributes["src"];
            if (s == null)
                return null;
            if (suffix != null)
                return new Picture(pic,
                    pic.SelectSingleNode("ancestor-or-self::contribution/@id").InnerText + "#" + suffix);
            else
                return new Picture(pic,
                    pic.SelectSingleNode("ancestor-or-self::contribution/@id").InnerText);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetHighlightPatternCount()
        {
            if (hilights == null) return 1;
            else return hl_patterns;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="s"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public ISprite GetSprite(Direction d, PlaceSide s, int col)
        {
            return sprites[col][d, s];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="s"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public ISprite GetHighLightSprite(Direction d, PlaceSide s, int col)
        {
            if (hilights != null)
                return hilights[col][d, s];
            else
                return null;
        }


        internal SpriteSet[] sprites;
        internal SpriteSet[] hilights;

        readonly int _price;
        /// <summary>
        /// 
        /// </summary>
        public override int Price { get { return _price; } }
        /// <summary>
        /// 
        /// </summary>
        public override double PricePerArea { get { return _price << 1; } }

        /// <summary>
        /// 
        /// </summary>
        private readonly int height;

        /// <summary>
        /// 
        /// </summary>
        public int Height
        {
            get { return height; }
        } 

        readonly string subgroup;

        /// <summary>
        /// 
        /// </summary>
        public string Subgroup
        {
            get { return subgroup; }
        } 

        /// <summary>
        /// 
        /// 
        /// </summary>
        readonly ColorLibrary colors;

        /// <summary>
        /// 
        /// </summary>
        public ColorLibrary Colors
        {
            get { return colors; }
        } 

        /// <summary>
        /// 
        /// </summary>
        readonly XmlNode variation;

        /// <summary>
        /// 
        /// </summary>
        protected XmlNode Variation
        {
            get { return variation; }
        } 

        /// <summary>
        /// 
        /// </summary>
        int _currentCol;
        /// <summary>
        /// 
        /// </summary>
        public int currentColor
        {
            get { return _currentCol; }
            set
            {
                if (value >= 0 && value < colors.size)
                    _currentCol = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        int _currentHLIdx;
        /// <summary>
        /// 
        /// </summary>
        public int currentHighlight
        {
            get { return _currentHLIdx; }
            set
            {
                if (value >= 0 && value < hl_patterns)
                    _currentHLIdx = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>]
        protected override StructureGroup GetGroup(string name)
        {
            return _group;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        public override IModalController CreateBuilder(IControllerSite site)
        {
            return new HVControllerImpl(this, site, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        public override IModalController CreateRemover(IControllerSite site)
        {
            return new HVControllerImpl(this, site, true);
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="baseLoc"></param>
        /// <param name="front"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public Structure Create(Location baseLoc, Direction front, PlaceSide side)
        {
            ContributionReference reffer = new ContributionReference(this, currentColor, currentHighlight, front, side);
            HalfDividedVoxel v = WorldDefinition.World[baseLoc] as HalfDividedVoxel;
            if (v == null)
                return new HVStructure(reffer, baseLoc);
            else
            {
                if (!v.owner.add(reffer))
                {
                    MessageBox.Show("Not enough space or no fit");
                }
                
                return v.owner;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseLoc"></param>
        /// <param name="front"></param>
        /// <param name="side"></param>
        public void Destroy(Location baseLoc, Direction front, PlaceSide side)
        {
            HalfDividedVoxel v = WorldDefinition.World[baseLoc] as HalfDividedVoxel;
            if (v != null)
                v.owner.remove(side);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseLoc"></param>
        /// <returns></returns>
        public static bool CanBeBuilt(Location baseLoc)
        {
            Voxel v = WorldDefinition.World[baseLoc];
            if (v != null)
            {
                HalfDividedVoxel hv = v as HalfDividedVoxel;
                if (hv != null)
                {
                    return hv.hasSpace;
                }
                else
                    return false;
            }
            else
                return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pixelSize"></param>
        /// <returns></returns>
        public override PreviewDrawer CreatePreview(Size pixelSize)
        {
            PreviewDrawer drawer = new PreviewDrawer(pixelSize, new Size(7, 1), 1);
            drawer.Draw(sprites[currentColor][Direction.WEST, PlaceSide.Fore], 3, 1);
            drawer.Draw(sprites[currentColor][Direction.EAST, PlaceSide.Back], 2, 0);
            if (hilights != null)
            {
                drawer.Draw(hilights[currentHighlight][Direction.WEST, PlaceSide.Fore], 3, 1);
                drawer.Draw(hilights[currentHighlight][Direction.EAST, PlaceSide.Back], 2, 0);
            }
            return drawer;
        }

    }
}
