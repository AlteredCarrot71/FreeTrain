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
using System.Windows.Forms;
using FreeTrain.Contributions.Rail;
using FreeTrain.Framework;
using FreeTrain.Framework.Plugin;
using FreeTrain.Util;
using FreeTrain.World.Accounting;
using FreeTrain.World.Structs;
using FreeTrain.World.Development;

namespace FreeTrain.World.Rail
{
    /// <summary>
    /// Station
    /// </summary>
    [Serializable]
    public class Station : PThreeDimStructure, IPlatformHost, ITrainHarbor
    {
        /// <summary>
        /// Creates a new station object with its left-top corner at
        /// the specified location.
        /// </summary>
        /// <param name="_type">
        /// Type of the station to be built.
        /// </param>
        /// <param name="wloc"></param>
        public Station(StationContribution _type, WorldLocator wloc)
            : base(_type, wloc)
        {
            this.type = _type;
            this._name = string.Format("ST{0,2:d}", iota++);
            if (wloc.world == WorldDefinition.World)
            {
                WorldDefinition.World.Stations.add(this);
                WorldDefinition.World.Clock.registerRepeated(new ClockHandler(clockHandlerHour), TimeLength.fromHours(1));
                WorldDefinition.World.Clock.registerRepeated(new ClockHandler(clockHandlerDay), TimeLength.fromHours(24));
            }
            Distance r = new Distance(REACH_RANGE, REACH_RANGE, REACH_RANGE);

            // advertise listeners in the neighborhood that a new station is available
            foreach (IEntity e in Cube.CreateInclusive(baseLocation - r, baseLocation + r).GetEntities())
            {
                IStationListener l = (IStationListener)e.QueryInterface(typeof(IStationListener));
                if (l != null)
                    l.advertiseStation(this);
            }
        }

        private new readonly StationContribution type;

        /// <summary>
        /// sequence number generator for automatic name generation.
        /// </summary>
        private static int iota = 1;

        /// <summary> Name of this station. </summary>
        private string _name;
        /// <summary>
        /// 
        /// </summary>
        public override string name { get { return _name; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void setName(string name)
        {
            this._name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public Location location { get { return baseLocation; } }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool onClick()
        {
            new StationPropertyDialog(this).ShowDialog(MainWindow.mainWindow);
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return name; }


        #region Entity implementation
        /// <summary>
        /// 
        /// </summary>
        public override bool isSilentlyReclaimable { get { return false; } }
        /// <summary>
        /// 
        /// </summary>
        public override bool isOwned { get { return true; } }

        // TODO: value?
        /// <summary>
        /// 
        /// </summary>
        public override int EntityValue { get { return 0; } }
        /// <summary>
        /// 
        /// </summary>
        public override void remove()
        {

            WorldDefinition.World.Clock.unregister(new ClockHandler(clockHandlerHour));
            WorldDefinition.World.Clock.unregister(new ClockHandler(clockHandlerDay));

            // first, remove this station from the list of all stations.
            // this will allow disconnected structures to find the next
            // nearest station.
            WorldDefinition.World.Stations.remove(this);

            // notify listeners
            foreach (IStationListener l in listeners)
                l.onStationRemoved(this);

            // notify nodes that this host is going to be destroyed.
            // we need to copy it into array because nodes will be updated
            // as we notify children
            foreach (Platform p in nodes.ToArray(typeof(Platform)))
                p.onHostDisconnected();
            Debug.Assert(nodes.IsEmpty);

            base.remove();
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aspect"></param>
        /// <returns></returns>
        public override object QueryInterface(Type aspect)
        {
            if (aspect == typeof(ITrainHarbor))
                return this;

            return base.QueryInterface(aspect);
        }


        private readonly Set nodes = new Set();
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
        public Station hostStation
        {
            get
            {
                return this;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal protected override Color heightCutColor { get { return Color.Gray; } }


        private void onDayClock()
        {
            // called once a day. charge the operation cost
            AccountManager.theInstance.spend(type.OperationCost, AccountGenre.RailService);
        }


        #region listeners
        //
        //
        // Listener handling
        //
        //
        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class ListenerSet
        {
            private readonly Set core = new Set();
            /// <summary>
            /// 
            /// </summary>
            /// <param name="listener"></param>
            public void add(IStationListener listener)
            {
                core.Add(listener);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="listener"></param>
            public void remove(IStationListener listener)
            {
                core.Remove(listener);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public System.Collections.IEnumerator GetEnumerator()
            {
                return core.GetEnumerator();
            }
        }

        /// <summary> StationListeners that are attached to this staion. </summary>
        public readonly ListenerSet listeners = new ListenerSet();

        /// <summary>
        /// Gets the total sum of the population of this station.
        /// </summary>
        public int population
        {
            get
            {
                int p = 0;
                foreach (IStationListener l in listeners)
                    p += l.GetPopulation(this);
                return p;
            }
        }

        // FIXME: probably there's no need to maintain the average values any longer


        /// <summary>
        /// The number of passengers that is "gone".
        /// Those are people that live in this station but are on the road.
        /// </summary>
        private int gonePassengers = 0;

        /// <summary>
        /// Weighted average of # of people that are unloaded in this station.
        /// Multiplied by AVERAGE_PASSENGER_RATIO for every hour.
        /// </summary>
        private int accumulatedUnloadedPassengers = 0;
        /// <summary>
        /// 
        /// </summary>
        public int averageUnloadedPassengers
        {
            get
            {
                return (int)(accumulatedUnloadedPassengers * AVERAGE_PASSENGER_PER_DAY_FACTOR);
            }
        }

        /// <summary>
        /// Weighted average of # of people that are loaded in this station.
        /// Multiplied by AVERAGE_PASSENGER_RATIO for every hour.
        /// </summary>
        private int accumulatedLoadedPassengers = 0;
        /// <summary>
        /// 
        /// </summary>
        public int averageLoadedPassengers
        {
            get
            {
                return (int)(accumulatedLoadedPassengers * AVERAGE_PASSENGER_PER_DAY_FACTOR);
            }
        }

        /// <summary>
        /// Factor that we apply to averageLoaded/UnloadedPassengers every hour.
        /// </summary>
        const float AVERAGE_PASSENGER_RATIO = 0.9996f;

        /// <summary>
        /// Factor that we need to apply to obtain average passengers per day.
        /// obtained by 24*(1-RATIO)
        /// 
        /// Justification of the above equation is that if you always carry 1 passenger
        /// for every hour, thie accumulated value should converge to C
        /// where C = C*RATIO + 1. Such C = \frac{1}{1-RATIO}
        /// </summary>
        const float AVERAGE_PASSENGER_PER_DAY_FACTOR = 24.0f * (1.0f - AVERAGE_PASSENGER_RATIO);

        /// <summary>
        /// 
        /// </summary>
        protected TransportLog trains = new TransportLog(); // train numbers arrive and depart today.
        /// <summary>
        /// 
        /// </summary>
        protected TransportLog import = new TransportLog(); // passengers unloaded today.
        /// <summary>
        /// 
        /// </summary>
        protected TransportLog export = new TransportLog(); // passengers can be exported today.
        /// <summary>
        /// 
        /// </summary>
        public double ScoreImported { get { return import.LastWeek; } }
        /// <summary>
        /// 
        /// </summary>
        public double ScoreExported { get { return export.LastWeek; } }
        /// <summary>
        /// 
        /// </summary>
        public double ScoreTrains { get { return trains.LastWeek; } }
        /// <summary>
        /// 
        /// </summary>
        public int UnloadedToday { get { return import.Today; } }
        /// <summary>
        /// 
        /// </summary>
        public int UnloadedYesterday { get { return import.Yesterday; } }
        /// <summary>
        /// 
        /// 
        /// </summary>
        public int TrainsToday { get { return trains.Today; } }
        /// <summary>
        /// 
        /// </summary>
        public int TrainsYesterday { get { return trains.Yesterday; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tr"></param>
        public void unloadPassengers(Train tr)
        {
            // TODO: do something with unloaded passengers
            int r = tr.unloadPassengers();
            import.AddAmount(r);
            trains.AddAmount(1);
            Debug.WriteLine(string.Format("devQ on unload v={0} for {1} passengers.", import.LastWeek / 24, r));
            WorldDefinition.World.LandValue.addQ(location, Math.Min((float)(import.LastWeek / 24), r));
            accumulatedUnloadedPassengers += r;
            GlobalTrafficMonitor.TheInstance.NotifyPassengerTransport(this, r);
        }

        /// <summary>
        /// Obtains the number of the passenger for the train
        /// that is going to depart.
        /// </summary>
        /// <param name="tr">train to put passengers in</param>
        public void loadPassengers(Train tr)
        {
            int total = this.population;
            if (total == 0) return;		// avoid division by 0

            int avail = Math.Max(0, total - gonePassengers);

            // one train can't have 100% of available populations. (the number is arbitrarily set to 30%)
            int pass = Math.Min(tr.passengerCapacity, (int)(avail * 0.3f));
            export.AddAmount(tr.passengerCapacity - pass);
            trains.AddAmount(1);

            gonePassengers += pass;
            accumulatedLoadedPassengers += pass;
            WorldDefinition.World.LandValue.addQ(location, pass);
            Debug.WriteLine(name + ": # of passengers gone (up to) " + gonePassengers);

            tr.loadPassengers(this, pass);
        }
        /// <summary>
        /// 
        /// </summary>
        public void clockHandlerHour()
        {
            // increase the passenger ratio
            gonePassengers = (int)(gonePassengers * 0.8f);
            Debug.WriteLine(name + ": # of passengers gone (down to) " + gonePassengers);

            // update those statistics
            accumulatedLoadedPassengers = (int)(accumulatedLoadedPassengers * AVERAGE_PASSENGER_RATIO);
            accumulatedUnloadedPassengers = (int)(accumulatedUnloadedPassengers * AVERAGE_PASSENGER_RATIO);
        }
        /// <summary>
        /// 
        /// </summary>
        public void clockHandlerDay()
        {
            // called once a day. charge the operation cost
            AccountManager.theInstance.spend(type.OperationCost, AccountGenre.RailService);
            import.DailyReset();
            export.DailyReset();
            trains.DailyReset();
        }
        /// <summary>
        /// 
        /// </summary>
        public const int REACH_RANGE = 10;

        /// <summary>
        /// Returns true if a listener at the given location can use this station.
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public bool withinReach(Location loc)
        {
            // TODO: maybe it's better to take Listener as a parameter
            return distanceTo(loc) < REACH_RANGE;
        }
        #endregion


        /// <summary>
        /// Gets the station object if one is in the specified location.
        /// </summary>
        public static Station get(Location loc)
        {
            return WorldDefinition.World.GetEntityAt(loc) as Station;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Station get(int x, int y, int z) { return get(new Location(x, y, z)); }
    }
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class TransportLog
    {
        private const double LogFactor = 5;

        private int today = 0;
        private int yesterday = 0;
        private double thisweek = 0;
        private double lastweek = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public void AddAmount(int amount)
        {
            today += amount;
        }
        /// <summary>
        /// 
        /// </summary>
        public void DailyReset()
        {
            thisweek += Math.Pow(today, 1 / LogFactor);
            yesterday = today;
            today = 0;
            Clock c = WorldDefinition.World.Clock;
            if (c.dayOfWeek == 6)
            {
                lastweek = Math.Pow(thisweek / 7, LogFactor);
                thisweek = 0;
                Debug.WriteLine("report " + lastweek);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Yesterday { get { return yesterday; } }
        /// <summary>
        /// 
        /// </summary>
        public int Today { get { return today; } }
        /// <summary>
        /// 
        /// </summary>
        public double ThisWeek
        {
            get { return Math.Pow(thisweek / (WorldDefinition.World.Clock.dayOfWeek + 1), LogFactor); }
        }
        /// <summary>
        /// 
        /// </summary>
        public double LastWeek { get { return lastweek; } }
    }
}
