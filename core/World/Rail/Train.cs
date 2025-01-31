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
using FreeTrain.Framework.Graphics;
using FreeTrain.Contributions.Train;
using FreeTrain.Framework;
using FreeTrain.Framework.Sound;
using FreeTrain.Framework.Plugin;
using FreeTrain.World.Accounting;
using System.Runtime.Serialization;

namespace FreeTrain.World.Rail
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="train"></param>
    public delegate void TrainHandler(Train train);

    /// <summary>
    /// Train
    /// </summary>
    [Serializable]
    public class Train : TrainItem, IDeserializationCallback
    {
        /// <summary>
        /// Function object that computes the next state for the head car.
        /// </summary>
        [NonSerialized]
        private /*readonly*/ CalcNextTrainCarState calcNextTrainCarState;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="length"></param>
        /// <param name="_type"></param>
        public Train(TrainGroup group, int length, TrainContribution _type)
            : this(group, string.Format("TR{0}", iota++), length, _type, SimpleTrainControllerImpl.theInstance)
        {

        }
        /// <summary> Sequence number generator. </summary>
        private static int iota = 1;


        /// <summary>
        /// Sound-effect of a ringing bell. Used when a train leaves a station.
        /// </summary>
        private static readonly ISoundEffect thudSound = new RepeatableSoundEffectImpl(
            ResourceUtil.LoadSystemSound("train.wav"), 1, 300);



        /// <summary>
        /// Creates a new train and assigns it to a group.
        /// </summary>
        public Train(TrainGroup group, string _name, int length, TrainContribution _type, TrainController _controller)
            : base(group, _name)
        {
            this.type = _type;
            this.controller = _controller;

            TrainCarContribution[] carTypes = type.Create(length);

            cars = new TrainCar[length];
            for (int i = 0; i < length; i++)
                cars[i] = new TrainCar(this, carTypes[i], i);

            calcNextTrainCarState = new CalcNextTrainCarState(this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        public void OnDeserialization(object sender)
        {
            calcNextTrainCarState = new CalcNextTrainCarState(this);
        }



        /// <summary> Type of this train. </summary>
        public readonly TrainContribution type;



        /// <summary>
        /// 
        /// </summary>
        public string displayName { get { return name; } }

        /// <summary>
        /// この編成を構成する車両
        /// </summary>
        private readonly TrainCar[] cars;

        /// <summary> Number of cars in this train. </summary>
        public int length { get { return cars.Length; } }

        /// <summary> The first car of this train. </summary>
        public TrainCar head { get { return cars[0]; } }

        /// <summary> Return true if this train is placed on the map </summary>
        public bool isPlaced { get { return State != TrainStates.Unplaced; } }

        /// <summary> Place a train to the specified location.</summary>
        /// <returns> false if it can't be done. </returns>
        public bool place(Location loc)
        {
            Debug.Assert(!isPlaced);

            Direction[] ds = new Direction[length];
            Location[] locs = new Location[length];

            int idx = length;

            Direction d = null;
            do
            {
                idx--;

                RailRoad rr = RailRoad.get(loc);
                if (rr == null || rr.Voxel.isOccupied)
                {
                    // can't be placed here
                    return false;
                }
                if (d == null) d = rr.Dir1;	// set the initial direction

                ds[idx] = d; locs[idx] = loc;

                // determine the next voxel
                cars[0].Place(loc, d);
                d = rr.Guide();
                loc += d;
                cars[0].Remove();
            } while (idx != 0);

            // make sure we are not forming cycles
            for (int i = 0; i < length - 1; i++)
                for (int j = i + 1; j < length; j++)
                    if (locs[i] == locs[j])
                        return false;	// can't be placed

            // can be placed. place all
            for (int i = 0; i < length; i++)
                cars[i].Place(locs[i], ds[i]);

            stopCallCount = 0;
            registerTimer();
            State = TrainStates.Moving;

            return true;
        }

        /// <summary>
        /// 配置済みの列車を撤去する
        /// </summary>
        public void remove()
        {
            Debug.Assert(isPlaced);

            foreach (TrainCar car in cars)
                car.Remove();

            // make sure that we don't have any pending event
            WorldDefinition.World.Clock.unregister(new ClockHandler(clockHandler));
            State = TrainStates.Unplaced;
        }

        /// <summary> Sell this train. </summary>
        public void sell()
        {
            if (isPlaced) remove();

            ownerGroup.items.remove(this);
            // TODO: reimberse money

            // disconnect all listeners.
            nonPersistentStateListeners = null;
            persistentStateListeners = null;
        }


        /// <summary> Possible states of a train. </summary>
        public enum TrainStates : byte
        {
            /// <summary>
            /// 
            /// </summary>
            Unplaced,			// not placed
            /// <summary>
            /// 
            /// </summary>
            Moving,				// moving normally
            /// <summary>
            /// 
            /// </summary>
            StoppingAtStation,	// stopping at a station, waiting for the time to start
            /// <summary>
            /// 
            /// </summary>
            StoppingAtSignal,	// stopping at a singal.
            /// <summary>
            /// 
            /// </summary>
            EmergencyStopping,	// stopping because of a car ahead of this train
        }

        /// <summary> State of this train. Usually updated by the clock handler. </summary>
        private TrainStates __state = TrainStates.Unplaced;
        /// <summary>
        /// 
        /// </summary>
        public TrainStates State
        {
            get
            {
                return __state;
            }
            set
            {
                if (__state == value) return;

                __state = value;
                notifyListeners();
            }
        }

        private void notifyListeners()
        {
            if (persistentStateListeners != null)
                persistentStateListeners(this);
            if (nonPersistentStateListeners != null)
                nonPersistentStateListeners(this);
        }

        /// <summary>
        /// Returns the state in its display text.
        /// </summary>
        public string stateDisplayText
        {
            get
            {
                switch (State)
                {
                    case TrainStates.Unplaced: return "Unplaced";
                    case TrainStates.Moving: return "Moving";
                    case TrainStates.StoppingAtStation: return "Stopping at station";
                    case TrainStates.StoppingAtSignal: return "Stopping at signal";
                    case TrainStates.EmergencyStopping: return "Emergency stop";
                    //! case State.Unplaced:			return "未配置";
                    //! case State.Moving:				return "進行中";
                    //! case State.StoppingAtStation:	return "発車時間待";
                    //! case State.StoppingAtSignal:	return "停止中";
                    //! case State.EmergencyStopping:	return "緊急停止";
                    default: Debug.Fail("undefined state"); return null;
                }
            }
        }

        /// <summary>
        /// Delegates that are invoked when the state of the train changes.
        /// </summary>
        public TrainHandler persistentStateListeners;
        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        public TrainHandler nonPersistentStateListeners;


        private void registerTimer()
        {
            registerTimer(TimeLength.fromMinutes(type.MinutesPerVoxel));
        }

        private void registerTimer(TimeLength time)
        {
            WorldDefinition.World.Clock.registerOneShot(new ClockHandler(clockHandler), time);
        }

        /// <summary> Counter that remembers the # of consecutive times this train is told to stop. </summary>
        private int stopCallCount = 0;

        /// <summary>
        /// Clock event handler.
        /// </summary>
        public void clockHandler()
        {
            Debug.Assert(isPlaced);	// we should have unregistered the handler when the train was removed.

            CarState.Inside ins = head.State.asInside();
            if (ins != null)
            { // this car might need to stop
                TimeLength time = ins.voxel.railRoad.getStopTimeSpan(this, stopCallCount);
                if (time.totalMinutes > 0)
                {
                    stopCallCount++;
                    // a car needs to stop here
                    registerTimer(time);	// resume after the specified time

                    // TODO: see where this train is being stopped. do something if necessary
                    State = TrainStates.StoppingAtStation;
                    return;
                }


                if (time.totalMinutes < 0)
                    reverse();	// turn around
            }

            // this car can now move
            stopCallCount = 0;
            TrainStates s = TrainStates.Moving;

            // determine the next head car state
            CarState next = calcNextTrainCarState[head.State];
            if (next != null)
            {	// if it can move forward
                if (!isBlocked[next])
                    move(next);
                else
                    // otherwise we can't move. emergency stop.
                    s = TrainStates.EmergencyStopping;
            }
            else
            {
                // we can't go forward. turn around
                reverse();
                next = calcNextTrainCarState[head.State];

                if (next != null && !isBlocked[next])
                    move(next);
                else
                    s = TrainStates.EmergencyStopping;
            }
            State = s;	// update the state
            registerTimer();
        }

        private int moveCount = 0;	// used to compute the cost of a train

        /// <summary>
        /// 1voxel動かす
        /// </summary>
        public void move(CarState next)
        {

            if (next.isOutside && next.asOutside().timeLeft == OUTSIDE_COUNTER_INITIAL_VALUE / 2)
            {
                // unload the passengers and reload them
                unloadPassengers();
                loadPassengers(null, Math.Min(100, passengerCapacity));	// TODO: compute the value seriously
            }

            for (int i = 0; i < cars.Length; i++)
                next = cars[i].moveTo(next);

            // moving a train costs money
            if (((moveCount++) & 15) == 0)
            {
                // TODO: exact amount is still under debate
                AccountManager.theInstance.spend(length * 20 + (passenger / 20), AccountGenre.RailService);
            }

            playSound(thudSound);
        }

        /// <summary>
        /// Plays a sound effect for this train if necessary.
        /// </summary>
        public void playSound(ISoundEffect se)
        {
            CarState.Inside ins = head.State.asInside();
            if (ins != null)
                se.play(ins.location);	// play the sound
        }

        /// <summary>
        /// Reverses the direction of the train.
        /// </summary>
        public void reverse()
        {
            // reverse the direction of each car.
            foreach (TrainCar car in cars)
                car.reverse();

            // swap the sequence
            for (int i = 0; i < cars.Length / 2; i++)
            {
                TrainCar t = cars[i];
                cars[i] = cars[cars.Length - (i + 1)];
                cars[cars.Length - (i + 1)] = t;
            }

            isReversed = !isReversed;
        }

        /// <summary>
        /// Returns true if the train is reversed.
        /// </summary>
        private bool isReversed;




        /// <summary>
        /// Number of passengers in this train.
        /// </summary>
        private int _passenger;
        /// <summary>
        /// 
        /// </summary>
        public int passenger
        {
            get
            {
                return _passenger;
            }
        }

        /// <summary>
        /// State of the car when the current passengers were loaded.
        /// </summary>
        private CarState.Placed passengerSourceState;

        /// <summary>
        /// Maximum number of passengers this train can hold.
        /// </summary>
        public int passengerCapacity
        {
            get
            {
                int c = 0;
                foreach (TrainCar car in this.cars)
                    c += car.type.Capacity;
                return c;
            }
        }

        /// <summary>
        /// Unloads the passengers from this train.
        /// This method should be called only by the Station.unloadPassengers() method.
        /// </summary>
        /// <returns>number of unloaded passengers</returns>
        public int unloadPassengers()
        {
            int r = _passenger;
            _passenger = 0;
            if (passengerSourceState != null)
            {
                // compute the distance between the source state and the current state
                int dist = passengerSourceState.location.
                    getDistanceTo(this.head.State.asPlaced().location);

                // record the sales
                AccountManager.theInstance.earn(
                    r * type.Fare * dist / 5000,
                    AccountGenre.RailService);

                passengerSourceState = null;
            }

            notifyListeners();

            return r;
        }
        /// <summary>
        /// Loads the passengers from this train.
        /// This method should be called only by the Station.loadPassengers() method.
        /// </summary>
        public void loadPassengers(Station from, int n)
        {
            Debug.Assert(_passenger == 0);
            Debug.Assert(n <= passengerCapacity);
            _passenger = n;
            // memorize the location where passengers are loaded
            passengerSourceState = this.head.State.asPlaced();
            notifyListeners();
        }



        private const int OUTSIDE_COUNTER_INITIAL_VALUE = 100;

        /// <summary>
        /// Determines the next car state by visiting the current state.
        /// This visitor is only applied against the head car.
        /// 
        /// The method returns null if it cannot proceed because there's no
        /// rail road in front of the head car.
        /// </summary>
        private class CalcNextTrainCarState : CarState.IVisitor
        {
            private readonly Train owner;

            internal CalcNextTrainCarState(Train _owner)
            {
                this.owner = _owner;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="s"></param>
            /// <returns></returns>
            public CarState this[CarState s]
            {
                get
                {
                    return (CarState)s.accept(this);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="state"></param>
            /// <returns></returns>
            public object onInside(CarState.Inside state)
            {
                TrainCar head = owner.head;
                RailRoad rr = RailRoad.get(state.location);

                Direction go = rr.Guide();	// angle to go
                Location newLoc = state.location + go;
                newLoc.z += rr.ZDiff(state.direction);

                if (WorldDefinition.World.IsBorderOfWorld(newLoc))
                {
                    // go outside the world
                    return new CarState.Outside(newLoc, go, OUTSIDE_COUNTER_INITIAL_VALUE);
                }
                else
                {
                    if (isConnected(newLoc, go))
                        // the rail needs to be connected
                        return new CarState.Inside(newLoc, go);
                    else
                        return null;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="state"></param>
            /// <returns></returns>
            public object onUnplaced(CarState.Unplaced state)
            {
                return state;	// remain unplaced
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="state"></param>
            /// <returns></returns>
            public object onOutsie(CarState.Outside state)
            {
                if (state.timeLeft != 0)
                    return new CarState.Outside(state.location, state.direction, state.timeLeft - 1);

                // time to get back to the world.
                CarState s = calcReturnPoint(state);
                if (s != null) return s;

                // there's no coming back. try again later
                return new CarState.Outside(state.location, state.direction, 10);
            }
        }

        /// <summary>
        /// Determines where the train should re-appear into the world.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private static CarState.Inside calcReturnPoint(CarState.Outside state)
        {
            // where do we go back?
            // for now, go back to where it comes from.
            int idx = state.direction.isSharp ? 0 : 1;
            for (int i = 0; i < 5; i++)
            {
                // compute the location
                Location newLoc = state.location + returnPointMatrixes[idx, i] * ((Distance)state.direction);
                // see if there's rail road
                if (isConnected(newLoc, state.direction.opposite))
                    // OK
                    return new CarState.Inside(newLoc, state.direction.opposite);
            }
            return null;	// non found
        }

        /// <summary>
        /// Returns true if a train car can proceed to the
        /// specified location by going the specified direction
        /// </summary>
        private static bool isConnected(Location loc, Direction d)
        {
            RailRoad rr = RailRoad.get(loc);
            return rr != null && rr.hasRail(d.opposite);
        }




        /// <summary>
        /// 45-degree rotational transformations from askew directions.
        /// (The same rotational transformations need to be doubled when
        /// applied against axis parallel directions)
        /// </summary>
        private readonly static Matrix L45 = new Matrix(0.5f, 0.5f, -0.5f, 0.5f);
        private readonly static Matrix R45 = new Matrix(0.5f, -0.5f, 0.5f, 0.5f);

        /// <summary>
        /// Determines the return location from outside the world.
        /// </summary>
        private readonly static Matrix[,] returnPointMatrixes = new Matrix[2, 5] {
			// when the direction is parallel to X/Y axis
			{
				Matrix.L90,		// in the order of precedence
				Matrix.R90,
				Matrix.L90 - 2*Matrix.E,
				Matrix.R90 - 2*Matrix.E,
				Matrix.REVERSE
			},
			// when the direction is not parallel to X/Y axis
			{
				L45 - Matrix.E,
				R45 - Matrix.E,
				L45 - 2*Matrix.E,
				R45 - 2*Matrix.E,
				Matrix.REVERSE
			}
		};



        /// <summary>
        /// Reverse the direction of the visiting car state and return it.
        /// </summary>
        private static readonly ReverseCarState reverseCarState = new ReverseCarState();
        private class ReverseCarState : CarState.IVisitor
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="state"></param>
            /// <returns></returns>
            public object onInside(CarState.Inside state)
            {
                Direction d = state.voxel.railRoad.Guide().opposite;
                return new CarState.Inside(state.location, d);
            }
            /// <summary>
            /// 
            /// 
            /// </summary>
            /// <param name="state"></param>
            /// <returns></returns>
            public object onUnplaced(CarState.Unplaced state)
            {
                return state;	// remain unchanged
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="state"></param>
            /// <returns></returns>
            public object onOutsie(CarState.Outside state)
            {
                CarState.Inside s = calcReturnPoint(state);
                if (s == null)
                    // all the usable RRs are completely removed.
                    // that means this train is fully outside the world.
                    // so it's OK to remain static.
                    return state;

                return new CarState.Outside(
                    s.location - s.direction, s.direction.opposite,
                    OUTSIDE_COUNTER_INITIAL_VALUE - state.timeLeft);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="s"></param>
            /// <returns></returns>
            public CarState this[CarState s]
            {
                get
                {
                    return (CarState)s.accept(this);
                }
            }
        }

        private static readonly IsBlocked isBlocked = new IsBlocked();
        private class IsBlocked : CarState.IVisitor
        {
            public object onInside(CarState.Inside state)
            {
                return state.voxel.isOccupied;
            }
            public object onUnplaced(CarState.Unplaced state)
            {
                return false;
            }
            public object onOutsie(CarState.Outside state)
            {
                return false;
            }
            public bool this[CarState s]
            {
                get
                {
                    return (bool)s.accept(this);
                }
            }
        }






        /// <summary>
        /// 一両の電車
        /// </summary>
        [Serializable]
        public class TrainCar : Car
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="_type"></param>
            /// <param name="idx"></param>
            public TrainCar(Train parent, TrainCarContribution _type, int idx)
            {
                this.parent = parent;
                this.type = _type;
                this.index = idx;
            }

            /// <summary> この電車を含む編成 </summary>
            public readonly Train parent;

            /// <summary> Type of this car. </summary>
            internal readonly TrainCarContribution type;


            /// <summary> Previous train car, or null. </summary>
            public TrainCar previous
            {
                get
                {
                    if (!parent.isReversed)
                    {
                        if (index == 0) return null;
                        else return parent.cars[index - 1];
                    }
                    else
                    {
                        if (index == parent.cars.Length - 1) return null;
                        else
                            return parent.cars[parent.cars.Length - index - 2];
                    }
                }
            }

            /// <summary>
            /// Index in the array. This car must be either at this
            /// position or "parent.cars.Length-index"
            /// </summary>
            private readonly int index;


            internal CarState moveTo(CarState newState)
            {
                return base.SetState(newState);
            }

            /// <summary>
            /// Reverses the direction of the car.
            /// </summary>
            public void reverse()
            {
                SetState(reverseCarState[this.State]);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="dc"></param>
            /// <param name="pt"></param>
            public override void Draw(DrawContext dc, Point pt)
            {
                Surface display = dc.Surface;

                pt.Y -= 9;	// offset

                CarState.Inside s = State.asInside();
                Debug.Assert(s != null);

                RailRoad rr = s.voxel.railRoad;
                if (rr is SlopeRailRoad)
                { // slope rail
                    SlopeRailRoad srr = (SlopeRailRoad)rr;

                    switch (srr.level)
                    {// apply slope height
                        case 0: break;
                        case 1: pt.Y -= 4; break;
                        case 2: pt.Y += 8; break;
                        case 3: pt.Y += 4; break;
                    }

                    if (!parent.isReversed)
                        type.DrawSlope(display, pt, s.direction, s.direction == srr.climbDir);
                    else
                        type.DrawSlope(display, pt, s.direction.opposite, s.direction != srr.climbDir);
                }
                else
                { // level rail road
                    int d1 = s.direction.index;
                    int d2 = s.voxel.railRoad.Guide().index;

                    int angle;
                    if (d1 == d2)
                    {
                        angle = d1 * 2;
                    }
                    else
                    {
                        int diff = (d2 - d1) & 7;
                        if (diff == 7) diff = -1;

                        int dd = (d2 * 2 + diff * 3) & 15;	// operation is on modulo 16.

                        if (2 < dd && dd < 10) pt.X += 3;
                        else pt.X -= 3;

                        if (6 < dd && dd <= 14) pt.Y += 2;
                        else pt.Y -= 2;

                        angle = (d1 * 2 + diff) & 15;
                    }

                    if (parent.isReversed)
                        angle ^= 8;

                    type.Draw(display, pt, angle);
                }
            }
        }
    }
}
