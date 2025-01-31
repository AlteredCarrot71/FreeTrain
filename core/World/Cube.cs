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
using System.Drawing;
using FreeTrain.Util;
using FreeTrain.World.Terrain;

namespace FreeTrain.World
{
    /// <summary>
    /// Cubic space in the world.
    /// </summary>
    [Serializable]
    public struct Cube
    {
        /// <summary>
        /// The north-western bottom corner of the cube.
        /// The location of the voxel that has the smallest (x,y,z)
        /// value in the cube.
        /// </summary>
        private Location corner;

        /// <summary>
        /// 
        /// </summary>
        public Location Corner
        {
            get { return corner; }
            set { corner = value; }
        }

        /// <summary>
        /// Size of the cube.
        /// </summary>
        private int sx;
        /// <summary>
        /// 
        /// </summary>
        public int SizeX
        {
            get { return sx; }
            set { sx = value; }
        }

        private int sy;
        /// <summary>
        /// 
        /// </summary>
        public int SizeY
        {
            get { return sy; }
            set { sy = value; }
        }

        private int sz;
        /// <summary>
        /// 
        /// </summary>
        public int SizeZ
        {
            get { return sz; }
            set { sz = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="_sx"></param>
        /// <param name="_sy"></param>
        /// <param name="_sz"></param>
        public Cube(int x, int y, int z, int _sx, int _sy, int _sz)
            : this(new Location(x, y, z), _sx, _sy, _sz) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_corner"></param>
        /// <param name="_sx"></param>
        /// <param name="_sy"></param>
        /// <param name="_sz"></param>
        public Cube(Location _corner, int _sx, int _sy, int _sz)
        {
            this.corner = _corner;
            this.sx = _sx; this.sy = _sy; this.sz = _sz;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_corner"></param>
        /// <param name="sz"></param>
        /// <param name="z"></param>
        public Cube(Location _corner, Size sz, int z)
            : this(_corner, sz.Width, sz.Height, z) { }

        #region factory methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Cube CreateExclusive(Location loc, Distance d)
        {
            return CreateExclusive(loc, loc + d);
        }

        /// <summary>
        /// Create a cube represented by two locations [loc1,loc2)
        /// The voxel pointed by loc1 is inside the cube but that by loc2
        /// is not. (Hence the name "exclusive")
        /// </summary>
        public static Cube CreateExclusive(Location loc1, Location loc2)
        {
            int x, y, z;

            if (loc1.x <= loc2.x) x = loc1.x;
            else x = loc2.x + 1;

            if (loc1.y <= loc2.y) y = loc1.y;
            else y = loc2.y + 1;

            if (loc1.z <= loc2.z) z = loc1.z;
            else z = loc2.z + 1;

            return new Cube(x, y, z,
                Math.Abs(loc2.x - loc1.x),
                Math.Abs(loc2.y - loc1.y),
                Math.Abs(loc2.z - loc1.z));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loc1"></param>
        /// <param name="loc2"></param>
        /// <returns></returns>
        public static Cube CreateInclusive(Location loc1, Location loc2)
        {
            Debug.Assert(loc1 != Location.Unplaced && loc2 != Location.Unplaced);

            return new Cube(
                Math.Min(loc1.x, loc2.x),
                Math.Min(loc1.y, loc2.y),
                Math.Min(loc1.z, loc2.z),

                Math.Abs(loc2.x - loc1.x) + 1,
                Math.Abs(loc2.y - loc1.y) + 1,
                Math.Abs(loc2.z - loc1.z) + 1);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public int x1 { get { return corner.x; } }

        /// <summary>
        /// 
        /// </summary>
        public int y1 { get { return corner.y; } }

        /// <summary>
        /// 
        /// </summary>
        public int z1 { get { return corner.z; } }

        /// <summary>
        /// 
        /// </summary>
        public int x2 { get { return corner.x + sx; } }

        /// <summary>
        /// 
        /// </summary>
        public int y2 { get { return corner.y + sy; } }

        /// <summary>
        /// 
        /// 
        /// </summary>
        public int z2 { get { return corner.z + sz; } }

        /// <summary>
        /// 
        /// </summary>
        public Distance Size { get { return new Distance(sx, sy, sz); } }

        /// <summary>
        /// 
        /// </summary>
        public int Volume { get { return sx * sy * sz; } }

        /// <summary>
        /// Return true if this cube is on the ground.
        /// This property can be used to check if a structure can be built
        /// in this cube.
        /// </summary>
        public bool IsOnGround
        {
            get
            {
                int mx = x2;
                int my = y2;
                for (int x = x1; x < mx; x++)
                {
                    for (int y = y1; y < my; y++)
                    {
                        if (WorldDefinition.World.GetGroundLevel(x, y) != z1)
                            return false;
                        if (WorldDefinition.World[x, y, z1] is MountainVoxel)
                            return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Checks if this cube contains the given location.
        /// </summary>
        public bool Contains(Location loc)
        {
            return corner.x <= loc.x && loc.x < corner.x + sx
                && corner.y <= loc.y && loc.y < corner.y + sy
                && corner.z <= loc.z && loc.z < corner.z + sz;
        }

        /// <summary>
        /// Computes the rectangle in the (A,B) axis that completely contains
        /// all the voxels in this cube.
        /// </summary>
        public Rectangle BoundingABRect
        {
            get
            {
                // calculate the correct top left corner.
                int a1 = WorldDefinition.World.fromXYZToAB(corner).X;
                int b1 = WorldDefinition.World.fromXYZToAB(x2 - 1, y1, z2 - 1).Y - 16;

                int xyDiff = sx + sy;

                int width = xyDiff * 16;
                int height = (xyDiff + sz * 2) * 8;

                return new Rectangle(a1, b1, width, height);
            }
        }

        /// <summary>
        /// Lists up all the entities whose voxels intersect with this cube.
        /// </summary>
        public IEntity[] GetEntities()
        {
            int mx = x2;
            int my = y2;
            int mz = z2;

            Set r = new Set();

            for (int x = corner.x; x < mx; x++)
            {
                for (int y = corner.y; y < my; y++)
                {
                    for (int z = corner.z; z < mz; z++)
                    {
                        Voxel v = WorldDefinition.World[x, y, z];
                        if (v != null) r.Add(v.Entity);
                    }
                }
            }

            return (IEntity[])r.ToArray(typeof(IEntity));
        }

        /// <summary>
        /// Enumerates all the voxels inside a cube.
        /// </summary>
        public ICollection Voxels
        {
            get
            {
                ArrayList a = new ArrayList(this.Volume);
                for (int x = 0; x < sx; x++)
                {
                    for (int y = 0; y < sy; y++)
                    {
                        for (int z = 0; z < sz; z++)
                        {
                            Voxel v = WorldDefinition.World[corner.x + x, corner.y + y, corner.z + z];
                            if (v != null) a.Add(v);
                        }
                    }
                }
                return a;
            }
        }

        //		/// <summary>
        //		/// Computes the sume of entity values in this cube.
        //		/// </summary>
        //		public int getEntityValueSum() {
        //			int r=0;
        //			foreach( Entity e in getEntities() )
        //				r += e.entityValue;
        //			return r;
        //		}
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return corner.GetHashCode() ^ Size.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override bool Equals(object o)
        {
            if (!(o is Cube)) return false;
            Cube rhs = (Cube)o;
            return this.corner == rhs.corner
                && this.Size == rhs.Size;
        }
    }
}
