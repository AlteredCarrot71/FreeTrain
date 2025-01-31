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
using FreeTrain.Contributions.Structs;
using FreeTrain.Contributions.Population;
using FreeTrain.Framework;
using FreeTrain.Framework.Graphics;
using FreeTrain.Framework.Plugin;
using FreeTrain.World.Subsidiaries;

namespace FreeTrain.World.Structs
{
    /// <summary>
    /// Variable height building
    /// </summary>
    [Serializable]
    public class VarHeightBuilding : Structure, ISubsidiaryEntity
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_type"></param>
        /// <param name="wloc"></param>
        /// <param name="_height"></param>
        /// <param name="initiallyOwned"></param>
        public VarHeightBuilding(VarHeightBuildingContribution _type, WorldLocator wloc,
            int _height, bool initiallyOwned)
        {

            this.type = _type;
            this.height = _height;

            int Y = type.Size.Height;
            int X = type.Size.Width;
            int Z = height;
            this.baseLocation = wloc.location;

            voxels = new VoxelImpl[X, Y, Z];
            for (int z = 0; z < Z; z++)
                for (int y = 0; y < Y; y++)
                    for (int x = 0; x < X; x++)
                    {
                        WorldLocator wl = new WorldLocator(wloc.world, baseLocation + new Distance(x, y, z));
                        voxels[x, y, z] = new VoxelImpl(this, (byte)x, (byte)y, (byte)z, wl);
                    }
            if (wloc.world == WorldDefinition.World)
                this.subsidiary = new SubsidiaryCompany(this, initiallyOwned);

            if (type.Population != null)
                stationListener = new StationListenerImpl(
                    new MultiplierPopulation(height, type.Population), baseLocation);
        }

        /// <summary> Voxels that form this structure </summary>
        private readonly VoxelImpl[, ,] voxels;


        private readonly VarHeightBuildingContribution type;

        private readonly Location baseLocation;

        private readonly SubsidiaryCompany subsidiary;

        private readonly int height;

        /// <summary>
        /// Used to draw the structure when the height-cut mode kicks in.
        /// </summary>
        private readonly Color heightCutColor = Color.Gray;


        // don't react to the mouse click
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool onClick() { return false; }
        /// <summary>
        /// 
        /// </summary>
        public override string name { get { return type.Name; } }
        /// <summary>
        /// 
        /// </summary>
        public long structurePrice
        {
            get
            {
                return type.Price * height;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public long totalLandPrice
        {
            get
            {
                return WorldDefinition.World.LandValue[baseLocation + new Distance(type.Size, 0) / 2] * type.Size.Width * type.Size.Height;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public Location locationClue
        {
            get
            {
                return baseLocation + new Distance(type.Size, 0) / 2;
            }
        }


        #region Entity implementation
        /// <summary>
        /// 
        /// </summary>
        public override bool isSilentlyReclaimable { get { return false; } }
        /// <summary>
        /// 
        /// </summary>
        public override bool isOwned { get { return subsidiary.isOwned; } }
        /// <summary>
        /// 
        /// </summary>
        public override int EntityValue
        {
            get
            {
                return height * type.Price;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public override void remove()
        {
            if (stationListener != null)
                stationListener.OnRemoved();
            if (onEntityRemoved != null)
                onEntityRemoved(this, null);

            WorldDefinition world = WorldDefinition.World;
            foreach (VoxelImpl v in voxels)
                world.remove(v);
        }
        /// <summary>
        /// 
        /// </summary>
        public override event EventHandler onEntityRemoved;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public override object QueryInterface(Type aspect)
        {
            if (aspect == typeof(Rail.IStationListener))
                return stationListener;

            return base.QueryInterface(aspect);
        }

        /// <summary>
        /// Station to which this structure sends population to.
        /// </summary>
        private readonly StationListenerImpl stationListener;



        /// <summary>
        /// StructureVoxel with default drawing mechanism.
        /// </summary>
        [Serializable]
        protected internal class VoxelImpl : StructureVoxel
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="_owner"></param>
            /// <param name="_x"></param>
            /// <param name="_y"></param>
            /// <param name="_z"></param>
            /// <param name="wloc"></param>
            protected internal VoxelImpl(VarHeightBuilding _owner, byte _x, byte _y, byte _z, WorldLocator wloc)
                : base(_owner, wloc)
            {

                this.x = _x;
                this.y = _y;
                this.z = _z;
            }
            /// <summary>
            /// 
            /// </summary>
            protected new VarHeightBuilding owner { get { return (VarHeightBuilding)base.owner; } }

            /// <summary>The offset of the sprite.</summary>
            private readonly byte x, y, z;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="display"></param>
            /// <param name="pt"></param>
            /// <param name="heightCutDiff"></param>
            public override void Draw(DrawContext display, Point pt, int heightCutDiff)
            {
                VarHeightBuilding o = owner;

                if (heightCutDiff < 0)
                {
                    ISprite[] sps = o.type.GetSprites(x, y, z, o.height);
                    for (int i = 0; i < sps.Length; i++)
                        sps[i].Draw(display.Surface, pt);
                }
                else
                    if (z == 0)
                        ResourceUtil.EmptyChip.DrawShape(display.Surface, pt, o.heightCutColor);
            }
        }


        /// <summary>
        /// Gets the station object if one is in the specified location.
        /// </summary>
        public static VarHeightBuilding get(Location loc)
        {
            Voxel v = WorldDefinition.World[loc];
            if (!(v is VarHeightBuilding.VoxelImpl)) return null;

            return ((StructureVoxel)v).owner as VarHeightBuilding;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static VarHeightBuilding get(int x, int y, int z) { return get(new Location(x, y, z)); }
    }
}
