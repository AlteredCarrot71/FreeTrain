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
using System.Collections;
using System.Diagnostics;
using FreeTrain.Contributions.Common;
using FreeTrain.Contributions.Land;
using FreeTrain.Contributions.Structs;
using FreeTrain.Framework;
using FreeTrain.Framework.Plugin;
using FreeTrain.Util;
using FreeTrain.World.Rail;
using FreeTrain.World.Land;
using FreeTrain.World.Structs;
using FreeTrain.World.Subsidiaries;

namespace FreeTrain.World.Development
{
    /// <summary>
    /// Receives clock event and build a new structure if appropriate
    /// </summary>
    [Serializable]
    public class DevelopmentAlgorithm : IULVFactory
    {
        static internal ArrayList structs = new ArrayList();
        static internal ArrayList vhStructs = new ArrayList();

        static DevelopmentAlgorithm()
        {
            foreach (Contribution contrib in PluginManager.AllContributions)
            {
                if (contrib is IEntityBuilder)
                {
                    if (((IEntityBuilder)contrib).ComputerCannotBuild)
                        continue;
                    else
                    {
                        if (contrib is VarHeightBuildingContribution)
                            vhStructs.Add(contrib);
                        else if (contrib is CommercialStructureContribution
                            || contrib is LandBuilderContribution)
                            structs.Add(contrib);
                    }
                }
            }
            IComparer cmp = new BuilderPriceComparer();
            structs.Sort(cmp);
            vhStructs.Sort(cmp);
        }

        /// <summary>
        /// Invoked by the timer.
        /// Run the development algorithm.
        /// </summary>
        public void handleClock()
        {
            DateTime start = DateTime.Now;
            doClock();
            double d = (DateTime.Now - start).TotalMilliseconds;
            Debug.WriteLine("development: " + d + "ms");
        }


        // shouldn't be instanciated from outside
        /// <summary>
        /// 
        /// </summary>
        public DevelopmentAlgorithm()
        {
            //GlobalTrafficMonitor.TheInstance.OnPassengerTransported+=new TransportEvent(OnTransported);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        static internal protected int FindFirstIndexOf(ArrayList array, int price)
        {
            int n = array.Count >> 1;
            int p = n;
            do
            {
                n = (n + 1) >> 1;
                int ppa = (int)((IEntityBuilder)array[p]).PricePerArea;
                if (ppa >= price)
                    p = Math.Max(0, p - n);
                else
                    p = Math.Min(array.Count - 1, p + n);
            } while (n > 1);
            if (price > (int)((IEntityBuilder)array[p]).PricePerArea)
                p++;
            //			Debug.Write(string.Format("First for {0} is at {1} ;",price,p));
            //			Debug.WriteLine(string.Format("which is {0} -> {1} <- {2}",((IEntityBuilder)array[Math.Max(0,p-1)]).pricePerArea,((IEntityBuilder)array[p]).pricePerArea,((IEntityBuilder)array[Math.Min(array.Count-1,p+1)]).pricePerArea));
            return p;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        static internal protected int FindLastIndexOf(ArrayList array, int price)
        {
            int n = array.Count >> 1;
            int p = n;
            do
            {
                n = (n + 1) >> 1;
                int ppa = (int)((IEntityBuilder)array[p]).PricePerArea;
                if (ppa <= price)
                    p = Math.Min(array.Count - 1, p + n);
                else
                    p = Math.Max(0, p - n);
            } while (n > 1);
            int p1 = Math.Min(array.Count - 1, p + 1);
            if (price == (int)((IEntityBuilder)array[p1]).PricePerArea)
                p = p1;
            //			Debug.Write(string.Format("Last for {0} is at {1} ;",price,p));
            //			Debug.WriteLine(string.Format("which is {0} -> {1} <- {2}",((IEntityBuilder)array[Math.Max(0,p-1)]).pricePerArea,((IEntityBuilder)array[p]).pricePerArea,((IEntityBuilder)array[Math.Min(array.Count-1,p+1)]).pricePerArea));
            return p;
        }

        /// <summary>
        /// Dictionary from Cube to its ULV.
        /// </summary>
        private readonly IDictionary ULVs = new Hashtable();

        private Hashtable stationPlans = new Hashtable();
        /// <summary>
        /// Do the actual development algorithm.
        /// </summary>
        private void doClock()
        {
            Clock c = WorldDefinition.World.Clock;
            if (c.hour == 0)
            {
                foreach (Station s in WorldDefinition.World.Stations)
                {
                    if (s.ScoreTrains > 0)
                    {
                        if (stationPlans.ContainsKey(s))
                            ((SearchPlan)stationPlans[s]).Update();
                        else
                        {
                            stationPlans.Add(s, new SearchPlan(this, s));
                            s.onEntityRemoved += new EventHandler(onStationRemoved);
                        }
                    }
                }
            }
            else
            {
                foreach (SearchPlan sp in stationPlans.Values)
                    sp.Process();
            }
            ///////////////////////////////////////////////////////////
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onStationRemoved(object sender, EventArgs e)
        {
            stationPlans.Remove(sender);
        }

        /// <summary>
        /// Computes the "unused land value."
        /// If any structure cannot be built in this cube. returns null.
        /// </summary>
        public ULV create(Cube cube)
        {
            ULV ulv = (ULV)ULVs[cube];
            if (ulv == null)
                ULVs[cube] = ulv = ULV.create(cube);

            return ulv;
        }

    }

    [Serializable]
    internal class SearchPlan
    {
        internal static double F_StrDiffuse = 0.2;
        internal static double F_PopAmpPower = 1;
        internal static int F_PopAmpBase = 10;
        internal static double F_PopAmpScale = 0.1;
        internal static double F_LandPriceScale = 1;
        internal static double F_ReplacePriceFactor = 2.5;
        internal static double F_MaxPricePower = 0.2;
        const int F_LandAveSize = 4;
        const int F_LandFlexSize = 2;

        enum Phase { Start, SelectStruct1, SelectStruct2, FitPlace, Build, End };
        private static readonly Random rnd = new Random();
        private static int Rand(int ave, int amp)
        {
            int a = rnd.Next(amp);
            int b = rnd.Next(amp);
            return ave + a - b;
        }

        Station target;
        DevelopmentAlgorithm dev;
        private double strength = 0;
        private int vie = 0;
        private Phase phase;
        private int[] work = new int[4];
        private Location scaning;
        private int finalPrice;
        Plan plan;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="devalgo"></param>
        /// <param name="s"></param>
        public SearchPlan(DevelopmentAlgorithm devalgo, Station s)
        {
            target = s;
            dev = devalgo;
            phase = Phase.Start;
            Update();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            strength *= F_StrDiffuse;
            strength = target.ScoreExported;
            vie = (int)target.ScoreTrains;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Process()
        {
            if (vie <= 0)
                return;
            vie--;
            switch (phase)
            {
                case Phase.Start:
                    {
                        if (strength <= 0)
                            break;
                        Clock c = WorldDefinition.World.Clock;
                        if (c.hour < vie)
                            scaning = pickPlace(4);
                        if (scaning != Location.Unplaced && WorldDefinition.World.IsInsideWorld(scaning))
                            phase++;
                        else
                            strength += strength * (1 - F_StrDiffuse) / 7;
                    }
                    break;
                case Phase.SelectStruct1:
                    {
                        Debug.Assert(WorldDefinition.World.IsInsideWorld(scaning));
                        int minVal = (int)(WorldDefinition.World.LandValue[scaning] * F_LandPriceScale);
                        int maxVal = Math.Max(minVal + 15, (int)(minVal * 1.2));
                        //maxVal = Math.Max(maxVal,(int)Math.Pow(strength,F_MaxPricePower));
                        Debug.WriteLine(string.Format("target price: {0} to {1}", minVal, maxVal), "devalgo");
                        work[0] = DevelopmentAlgorithm.FindFirstIndexOf(DevelopmentAlgorithm.structs, minVal);
                        work[1] = DevelopmentAlgorithm.FindLastIndexOf(DevelopmentAlgorithm.structs, maxVal);
                        work[2] = DevelopmentAlgorithm.FindFirstIndexOf(DevelopmentAlgorithm.vhStructs, minVal);
                        work[3] = DevelopmentAlgorithm.FindLastIndexOf(DevelopmentAlgorithm.vhStructs, maxVal);
                        phase++;
                    }
                    break;
                case Phase.SelectStruct2:
                    {
                        int n1 = work[1] - work[0];
                        int n2 = work[3] - work[2];
                        int n = n1 + n2;
                        if (n <= 0)
                        {
                            phase = Phase.End;
                            return;
                        }
                        int r;
                        do
                        {
                            r = rnd.Next(n);
                        } while (r == n);
                        IEntityBuilder entity;
                        if (r < n1)
                            entity = DevelopmentAlgorithm.structs[work[0] + r] as IEntityBuilder;
                        else if (r - n1 < work[3]) //check upper bounds
                            entity = DevelopmentAlgorithm.vhStructs[work[2] + r - n1] as IEntityBuilder;
                        else
                            break; //retry
                        plan = GetPlan(entity);
                        if (plan != null)
                            phase++;
                        else
                            phase = Phase.End;
                    }
                    break;
                case Phase.FitPlace:
                    {
                        //bool OK = true;

                        if (WorldDefinition.World.IsOutsideWorld(plan.cube) || !plan.cube.IsOnGround)
                        {
                            phase = Phase.Start;
                            break;
                        }
                        IEntity[] es = plan.cube.GetEntities();
                        foreach (IEntity e in es)
                        {
                            if (!IsReplaceable(e, WorldDefinition.World.LandValue[scaning]))
                            {
                                strength += (int)Math.Pow(plan.value, 1.0 / F_MaxPricePower);
                                phase = Phase.Start;
                                return;
                            }
                        }
                        foreach (IEntity e in es)
                            e.remove();
                        phase++;
                        // 用地確保と建設の間があくと、入れ違いで他のプランが建設される可能性あり
                        //				}
                        //					break;
                        //				case Phase.Build:{
                        plan.build();
                        Debug.Write("building reduces strength " + strength);
                        double p = Math.Pow((double)finalPrice, 1.0 / F_MaxPricePower);
                        strength -= Math.Min(p, strength);
                        Debug.WriteLine("-> " + strength);
                        phase++;
                    }
                    break;
                case Phase.End:
                    {
                        phase = Phase.Start;
                    }
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected Plan GetPlan(IEntityBuilder entity)
        {
            Plan p = null;
            finalPrice = 0;
            if (entity is LandBuilderContribution)
            {
                LandBuilderContribution lbc = entity as LandBuilderContribution;
                Size size = new Size(Rand(F_LandAveSize, F_LandFlexSize), Rand(F_LandAveSize, F_LandFlexSize));
                p = new LandPlan(lbc, dev, scaning, size);
                finalPrice = lbc.Price * size.Width * size.Height;
            }
            else if (entity is CommercialStructureContribution)
            {
                CommercialStructureContribution csb = entity as CommercialStructureContribution;
                p = new CommercialStructurePlan(csb, dev, scaning);
                finalPrice = entity.Price;
            }
            else if (entity is VarHeightBuildingContribution)
            {
                VarHeightBuildingContribution vhbc = entity as VarHeightBuildingContribution;
                int h = vhbc.MinHeight;
                int h2 = vhbc.MaxHeight;
                int price = vhbc.Price * h;
                Cube tmp = new Cube(scaning, vhbc.Size, h);
                int cost = 0;
                foreach (IEntity e in tmp.GetEntities())
                    cost += e.EntityValue;
                while (price < cost && h < h2)
                {
                    if (price < cost)
                        price += vhbc.Price;
                    h++;
                }
                p = new VarHeightBuildingPlan(vhbc, dev, scaning, h);
                finalPrice = price;
            }
            return p;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        protected Location pickPlace(int count)
        {
            while (count-- > 0)
            {
                Location loc = target.baseLocation;
                WorldDefinition w = WorldDefinition.World;
                int amp = F_PopAmpBase + (int)(Math.Pow(w[loc].LandPrice, F_PopAmpPower) * F_PopAmpScale);
                // then randomly pick nearby voxel
                loc.x = Rand(loc.x, amp);
                loc.y = Rand(loc.y, amp);
                loc.z = w.GetGroundLevel(loc);
                if (w.IsOutsideWorld(loc))
                    continue;
                if (loc.z < w.WaterLevel) continue;// below water
                Voxel v = w[loc];
                if (v != null)
                {
                    if (!IsReplaceable(v.Entity, v.LandPrice))
                        continue;
                }
                return loc;
            }
            return Location.Unplaced;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="comPrice"></param>
        /// <returns></returns>
        protected bool IsReplaceable(IEntity e, int comPrice)
        {
            comPrice = (int)(comPrice * F_ReplacePriceFactor);
            if (e.isOwned || e is ConstructionSite)
                return false;
            if (e is ISubsidiaryEntity)
            {
                ISubsidiaryEntity se = e as ISubsidiaryEntity;
                bool b = (se.structurePrice < comPrice || se.structurePrice < se.totalLandPrice);
                return b;
            }
            else if (e is LandVoxel)
            {
                Debug.WriteLine("LandPrice:" + e.EntityValue + " (>" + comPrice);
                if (e.EntityValue > comPrice)
                    return false;
            }
            return true;
        }
    }


    class BuilderPriceComparer : IComparer
    {
        #region IComparer o
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            try
            {
                IEntityBuilder e1 = x as IEntityBuilder;
                IEntityBuilder e2 = y as IEntityBuilder;
                if (e1 != null && e2 != null)
                {
                    return Math.Sign(e1.PricePerArea - e2.PricePerArea);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine("caused by " + e.Source);
            }
            return 0;
        }

        #endregion
    }

}
