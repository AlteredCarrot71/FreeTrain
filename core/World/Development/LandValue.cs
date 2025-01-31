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
using FreeTrain.World.Road;

namespace FreeTrain.World.Development
{
    /// <summary>
    /// Computes and maintains land value.
    /// 
    /// This algorithm is based on the heat conductivity model,
    /// where the source of value is considered as a heat source,
    /// and temprature is in turn considered as land value.
    /// </summary>
    [Serializable]
    public sealed class LandValue
    {
        /// <summary>
        /// 
        /// </summary>
        public static float RHO_BARE_LAND = 0.80f;
        const float RHO_ROAD = 0.999f;
        /// <summary>
        /// Creates a new object and associates that with the world.
        /// </summary>
        /// <param name="w"></param>
        public LandValue(WorldDefinition w)
        {
            w.OtherObjects["{51CD7E24-4296-4043-B58D-A654AB71F121}"] = this;

            H = w.Size.x;
            V = w.Size.y;
            q = new float[H + 2, V + 2];
            back = new float[H + 2, V + 2];
            rho = new float[H + 2, V + 2];

            // fill the array by 1.
            for (int h = H; h > 0; h--)
                for (int v = V; v > 0; v--)
                    rho[h, v] = RHO_BARE_LAND;

            // register the event notification so that we can update rho correctly
            w.onVoxelChanged += new VoxelChangeListener(updateRho);
            w.Clock.registerRepeated(new ClockHandler(next), TimeLength.fromHours(UPDATE_FREQUENCY));
        }

        /// <summary> "tempratures" for each (h,v) </summary>
        private float[,] q;

        /// <summary> back buffer </summary>
        private float[,] back;

        /// <summary> heat conductivity (0-1) </summary>
        private float[,] rho;

        // size of the world
        private readonly int H;
        private readonly int V;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public float Rho(Location loc)
        {
            int h, v;
            WorldDefinition.World.toHV(loc.x, loc.y, out h, out v);

            return rho[h, v];
        }

        /// <summary>
        /// Returns the land value for the given voxel.
        /// </summary>
        public int this[int h, int v]
        {
            get
            {
                return (int)Math.Pow(q[h + 1, v + 1], LAND_VAL_POWER) * 10;
            }
        }

        /// <summary>
        /// Returns the land value for the given voxel.
        /// </summary>
        public int this[Location loc]
        {
            get
            {
                int h, v;
                WorldDefinition.World.toHV(loc.x, loc.y, out h, out v);
                return this[h, v];
            }
        }

        /// <summary>
        /// Made public just because of a bug in .NET.
        /// Compute the next step.
        /// </summary>
        public void next()
        {
            {// flip the buffer
                float[,] t = q;
                q = back;
                back = t;
            }

            // compute next
            for (int h = H; h > 0; h--)
            {
                for (int v = V; v > 0; v--)
                {
                    float t = back[h, v];
                    float tr;
                    if (RHO_BARE_LAND < rho[h, v])
                    {
                        // apply special enforcement for road
                        float dt;
                        if ((v % 2) == 0)
                        {
                            dt = Math.Max(0, back[h, v - 1] - t); tr = dt * dt;
                            dt = Math.Max(0, back[h, v + 1] - t); tr += dt * dt;
                            dt = Math.Max(0, back[h + 1, v - 1] - t); tr += dt * dt;
                            dt = Math.Max(0, back[h + 1, v + 1] - t); tr += dt * dt;
                        }
                        else
                        {
                            dt = Math.Max(0, back[h, v - 1] - t); tr = dt * dt;
                            dt = Math.Max(0, back[h, v + 1] - t); tr += dt * dt;
                            dt = Math.Max(0, back[h - 1, v - 1] - t); tr += dt * dt;
                            dt = Math.Max(0, back[h - 1, v + 1] - t); tr += dt * dt;
                        }
                        t = back[h, v] * RHO_ROAD + (float)Math.Sqrt(tr * ALPHA);
                    }
                    else
                    {
                        if ((v % 2) == 0)
                        {
                            tr = (back[h, v - 1] - t) * rho[h, v - 1];
                            tr += (back[h, v + 1] - t) * rho[h, v + 1];
                            tr += (back[h + 1, v - 1] - t) * rho[h + 1, v - 1];
                            tr += (back[h + 1, v + 1] - t) * rho[h + 1, v + 1];
                        }
                        else
                        {
                            tr = (back[h, v - 1] - t) * rho[h, v - 1];
                            tr += (back[h, v + 1] - t) * rho[h, v + 1];
                            tr += (back[h - 1, v - 1] - t) * rho[h - 1, v - 1];
                            tr += (back[h - 1, v + 1] - t) * rho[h - 1, v + 1];
                        }
                        //t = (back[h,v] + tr * ALPHA) * rho[h,v];

                        t = back[h, v] * DIFF + tr * ALPHA * rho[h, v];
                    }
                    if (t < 0) t = 0;	// try to save the algorithm just in case something goes terribly wrong
                    q[h, v] = t;
                }
            }
        }

        /// <summary>
        /// Deposites "heat".
        /// </summary>
        public void addQ(Location loc, float deltaQ)
        {
            int h, v;
            WorldDefinition.World.toHV(loc, out h, out v);
            q[h, v] += deltaQ * UPDATE_FREQUENCY / 4;
        }

        /// <summary>
        /// Public simply because of a bug in .NET
        /// Updates the heat conductivity according to the voxels we have.
        /// </summary>
        public void updateRho(Location loc)
        {
            int h, v;
            WorldDefinition.World.toHV(loc.x, loc.y, out h, out v);

            BaseRoad roadFound = null;
            bool hasMountain = false;

            // FIXME: this code shouldn't have the knowledge of any particular voxel type.
            for (int z = 0; z < WorldDefinition.World.Size.z; z++)
            {
                Voxel vxl = WorldDefinition.World[loc.x, loc.y, z];
                if (vxl is TrafficVoxel)
                {
                    roadFound = ((TrafficVoxel)vxl).road;
                }
                if (vxl is Terrain.MountainVoxel)
                    hasMountain = true;
            }

            bool hasSea = WorldDefinition.World.getGroundLevelFromHV(h, v) < WorldDefinition.World.WaterLevel;

            if (roadFound != null)
            {
                if (roadFound.Style.Type >= MajorRoadType.street)
                    if (roadFound.Style.Sidewalk == SidewalkType.pavement)
                        rho[h, v] = RHO_ROAD;
                    else
                        rho[h, v] = (RHO_ROAD + RHO_BARE_LAND) / 2;
                else
                    rho[h, v] = RHO_BARE_LAND;
            }
            else if (hasSea)
            {
                rho[h, v] = 0.03f;
            }
            else if (hasMountain)
            {
                rho[h, v] = 0.4f;
            }
            else
            {
                rho[h, v] = RHO_BARE_LAND;
            }
        }

        /// <summary>
        /// Heat conductivity factor.
        /// The larger the value, the faster heat spreads.
        /// No more than 0.25. Otherwise the model becomes chaotic.
        /// </summary>
        public static float ALPHA = 0.240f;

        /// <summary>
        /// Diffusion. 1-epsilon.
        /// The larger the epsilon, the more heat evaporates.
        /// </summary>
        public static float DIFF = 1f - 0.01f;

        /// <summary>
        /// N where the land values are recomputed for every N hours.
        /// </summary>
        public static int UPDATE_FREQUENCY = 10;
        /// <summary>
        /// 
        /// </summary>
        public static double LAND_VAL_POWER = 0.3f;
    }
}
