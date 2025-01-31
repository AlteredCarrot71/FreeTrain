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
using System.Diagnostics;
using System.Drawing;
using FreeTrain.Framework.Graphics;
using FreeTrain.Contributions.Rail;
using FreeTrain.Framework;
using FreeTrain.World.Accounting;
using FreeTrain.Util;

namespace FreeTrain.World.Rail
{
    /// <summary>
    /// Platform that trains can stop by.
    /// </summary>
    [Serializable]
    public abstract class Platform : IPlatformHost
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="d"></param>
        /// <param name="len"></param>
        protected Platform(Location loc, Direction d, int len)
        {
            this.location = loc;
            this.direction = d;
            this.length = len;
            this.name = string.Format("Platform {0,2:d}", iota++);
            //! this.name = string.Format("ホーム{0,2:d}",iota++);
            this.bellSound = DepartureBellContribution.Default;
            WorldDefinition.World.Clock.registerRepeated(new ClockHandler(onClockPerDay), TimeLength.ONEDAY);

            // attach to the nearby station.
            foreach (IPlatformHost h in listHosts())
            {
                if (h is Station)
                {
                    host = h;
                    break;
                }
            }
        }


        /// <summary> Name of the platform if any. </summary>
        public string name;

        /// <summary> Location of the base of this platform. </summary>
        public readonly Location location;

        /// <summary> Direction of this platform. </summary>
        public readonly Direction direction;

        /// <summary> Length of the platform. </summary>
        public readonly int length;

        /// <summary> Parent host of this platform. </summary>
        private IPlatformHost _host = null;

        /// <summary>
        /// Set of child Platforms that are connected to a station through this platform.
        /// </summary>
        private readonly Set nodes = new Set();

        /// <summary> Departure bell sound. May not be null. </summary>
        public DepartureBellContribution bellSound;



        /// <summary> Other end of the platform. </summary>
        public Location otherEnd
        {
            get
            {
                Location l = location;
                l.x += direction.offsetX * length;
                l.y += direction.offsetY * length;
                return l;
            }
        }




        /// <summary> Host of this platform, or null if this is disconnected. </summary>
        internal protected IPlatformHost host
        {
            get { return _host; }
            set
            {
                if (_host == value) return;

                // remove from the current parent
                if (_host != null) _host.removeNode(this);
                _host = null;

                // notify nodes that this host is going to be destroyed.
                // we need to copy it into array because nodes will be updated
                // as we notify children
                foreach (Platform p in nodes.ToArray(typeof(Platform)))
                    p.onHostDisconnected();
                Debug.Assert(nodes.IsEmpty);

                // see if we can add to the new host.
                if (value != null && value.hostStation != null)
                {
                    _host = value;
                    if (_host != null) _host.addNode(this);
                }

                // update the warning icon
                WorldDefinition.World.OnVoxelUpdated(location);
            }
        }

        /// <summary> Host station of this platform, or null if this is isolated. </summary>
        public Station hostStation
        {
            get
            {
                if (host != null) return host.hostStation;
                else return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public abstract bool canRemove { get; }

        #region Entity implementation
        /// <summary>
        /// 
        /// </summary>
        public bool isSilentlyReclaimable { get { return false; } }
        /// <summary>
        /// 
        /// </summary>
        public bool isOwned { get { return true; } }

        // TODO: value?
        /// <summary>
        /// 
        /// </summary>
        public int EntityValue { get { return 0; } }
        /// <summary>
        /// 
        /// </summary>
        public virtual void remove()
        {
            WorldDefinition.World.Clock.unregister(new ClockHandler(onClockPerDay));
            if (onEntityRemoved != null) onEntityRemoved(this, null);
        }
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler onEntityRemoved;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public virtual object QueryInterface(Type aspect)
        {
            return null;
        }
        #endregion


        internal void onHostDisconnected()
        {
            host = null;
        }
        /// <summary>
        /// 
        /// </summary>
        public void onClockPerDay()
        {
            // charge the cost
            AccountManager.theInstance.spend(18 * length, AccountGenre.RailService);
        }





        /// <summary>
        /// Lists available platform hosts for this platform.
        /// </summary>
        internal protected abstract IPlatformHost[] listHosts();


        /// <summary> Processes a click event. </summary>
        public void onClick()
        {
            new PlatformPropertyDialog(this).ShowDialog(MainWindow.mainWindow);
        }



        /// <summary>
        /// Implementation for the listHosts() method.
        /// Lists available platform hosts for this platform.
        /// </summary>
        protected IPlatformHost[] listHosts(int range)
        {
            Set result = new Set();

            Location loc1 = Location.min(
                location + new Distance(-range, -range, 0),
                otherEnd + new Distance(-range, -range, 0));

            Location loc2 = Location.max(
                location + new Distance(range, range, 0),
                otherEnd + new Distance(range, range, 0));

            // scan the rectangle inside this region
            for (int z = location.z - 1; z <= location.z + 1; z++)
            {
                for (int y = loc1.y; y <= loc2.y; y++)
                {
                    for (int x = loc1.x; x <= loc2.x; x++)
                    {
                        Station st = Station.get(x, y, z);
                        if (st != null) result.Add(st);
                        Platform pt = Platform.get(x, y, z);
                        if (pt != null && pt != this && pt.hostStation != null)
                            result.Add(pt);
                    }
                }
            }

            // find hosts below and above this platform
            Location loc = location;
            for (int i = 0; i < length; i++, loc += direction)
            {
                for (int z = 0; z < WorldDefinition.World.Size.z; z++)
                {
                    Station st = Station.get(loc.x, loc.y, z);
                    if (st != null) result.Add(st);
                    Platform pt = Platform.get(loc.x, loc.y, z);
                    if (pt != null && pt != this && pt.hostStation != null)
                        result.Add(pt);
                }
            }


            return (IPlatformHost[])result.ToArray(typeof(IPlatformHost));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        public void addNode(Platform child)
        {
            nodes.Add(child);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        public void removeNode(Platform child)
        {
            nodes.Remove(child);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return name; }



        //
        // static methods/fields
        //


        /// <summary>
        /// sequence number generator for automatic name generation.
        /// </summary>
        private static int iota = 1;

        /// <summary> Warning icon. </summary>
        protected static readonly Surface warningIcon =
            ResourceUtil.LoadTimeIndependentSystemSurface("caution.bmp");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public static Platform get(Location loc)
        {
            Platform p = FatPlatform.get(loc);
            if (p != null) return p;

            // TODO: check slim platform
            return ThinPlatform.get(loc);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Platform get(int x, int y, int z) { return get(new Location(x, y, z)); }


    }
}
