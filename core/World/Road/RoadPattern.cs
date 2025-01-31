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
using System.Runtime.Serialization;
using FreeTrain.Framework;
using FreeTrain.Framework.Graphics;

namespace FreeTrain.World.Road
{
    /// <summary>
    /// Road pattern.
    /// 
    /// This object provides the structure of road network.
    /// The actual presentation (the outlook) of roads are encapsulated in
    /// derived classes of <code>Road</code>.
    /// 
    /// Implemented as a fly-weight pattern.
    /// 
    /// Because of a mysterious problem in serialization, this object is
    /// implemented as a value type, but to save the memory most of the data
    /// is stored statically. .NET often just sucks, and this is one of
    /// such examples.
    /// </summary>
    [Serializable]
    public struct RoadPattern //  : ISerializable
    {
        // TODO: slopes

        /// <summary>
        /// Gets the flat road pattern that has roads to the given directions.
        /// </summary>
        public static RoadPattern get(bool n, bool e, bool s, bool w)
        {
            Debug.Assert(n || e || s || w);	// at least one must be true
            return get((byte)((n ? 1 : 0) | (e ? 2 : 0) | (s ? 4 : 0) | (w ? 8 : 0)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dirs"></param>
        /// <returns></returns>
        public static RoadPattern get(byte dirs)
        {
            return new RoadPattern(dirs);
        }

        /// <summary>
        /// Gets the straight road pattern toward the given direction.
        /// </summary>
        public static RoadPattern getStraight(Direction d)
        {
            switch (d.index)
            {
                case 0:
                case 4:
                    return get(5);
                case 2:
                case 6:
                    return get(10);
                default:
                    Debug.Assert(false);
                    return get(0);
            }
        }


        //		/// <summary>
        //		/// Gets the pattern that has the specified direction to climb and the height.
        //		/// </summary>
        //		/// <param name="climb"></param>
        //		/// <param name="n"></param>
        //		/// <returns></returns>
        //		public static RailPattern getSlope( Direction climb, int height ) {
        //			Debug.Assert( 0<=height && height<4 );
        //			Debug.Assert( climb.isSharp );
        //
        //			return slopePatterns[climb.index*2+height];
        //		}

        private RoadPattern(byte dirs)
        {
            //			this.id = id;
            this.dirs = dirs;
        }


        /// <summary>
        /// If the correponding bit is 1, then the pattern has a road to the
        /// specified direction. (N:1,E:2,W:4,S:8)
        /// </summary>
        public readonly byte dirs;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public bool hasRoad(Direction d) { return hasRoad(d.index); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool hasRoad(int index) { return (dirs & (1 << (index / 2))) != 0; }

        //		private readonly int id;

        //		/// <summary>
        //		/// this rail road has up/down in z-axis to this direction.
        //		/// </summary>
        //		private int _zdiff;
        //		private Direction _zangle;
        //
        //		private Direction _climbDir =null;
        //
        //		/// <summary>
        //		/// If this is a slope rail, the rail is climbing to
        //		/// this direcion. Otherwise null.
        //		/// </summary>
        //		public Direction climbDir { get { return _climbDir; } }
        //
        //		private int _level;
        //		/// <summary>
        //		/// If this is a slope rail, this property holds the height
        //		/// of the rail.
        //		/// </summary>
        //		public int level { get { return _level; } }
        //
        //		public int zdiff  { get { return _zdiff; } }
        //		public Direction zangle { get { return _zangle; } }




        //		/// <summary>
        //		/// These objects are serialized as singleton.
        //		/// </summary>
        //		public void GetObjectData( SerializationInfo info, StreamingContext context) {
        //			info.SetType(typeof(ReferenceImpl));
        //			info.AddValue("id",id);
        //		}
        //		
        //		[Serializable]
        //		internal sealed class ReferenceImpl : IObjectReference {
        //			private int id=0;
        //			public object GetRealObject(StreamingContext context) {
        //				foreach( RoadPattern rp in flatPatterns )
        //					if(rp.id==id)	return rp;
        //				
        //				Debug.Assert(false);
        //				return null;
        //			}
        //		}
    }
}
