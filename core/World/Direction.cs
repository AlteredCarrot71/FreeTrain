﻿#region LICENSE
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
using FreeTrain.Framework.Graphics;
using FreeTrain.Framework;

namespace FreeTrain.World
{
    /// <summary>
    /// 方向（８方向。速度を伴わない）
    /// </summary>
    [Serializable]
    public sealed class Direction : ISerializable
    {
        /// <summary>
        /// 方向ベクトルからオブジェクトを得る
        /// </summary>
        public static Direction get(int x, int y)
        {
            for (int i = 0; i < directions.Length; i++)
                if (directions[i].offsetX == x && directions[i].offsetY == y)
                    return directions[i];

            Debug.Assert(false);
            return null;
        }

        /// <summary>
        /// インデックスからオブジェクトを得る
        /// </summary>
        public static Direction get(int idx)
        {
            return directions[idx];
        }
        /// <summary>
        /// 
        /// </summary>
        public static Direction NORTH { get { return get(0); } }
        /// <summary>
        /// 
        /// </summary>
        public static Direction NORTHEAST { get { return get(1); } }
        /// <summary>
        /// 
        /// 
        /// </summary>
        public static Direction EAST { get { return get(2); } }
        /// <summary>
        /// 
        /// </summary>
        public static Direction SOUTHEAST { get { return get(3); } }
        /// <summary>
        /// 
        /// </summary>
        public static Direction SOUTH { get { return get(4); } }
        /// <summary>
        /// 
        /// </summary>
        public static Direction SOUTHWEST { get { return get(5); } }
        /// <summary>
        /// 
        /// </summary>
        public static Direction WEST { get { return get(6); } }
        /// <summary>
        /// 
        /// </summary>
        public static Direction NORTHWEST { get { return get(7); } }


        /// <summary>
        /// オフセット
        /// </summary>
        public readonly int offsetX, offsetY;

        /// <summary>
        /// 表示用の名称
        /// </summary>
        public readonly string displayName;

        /// <summary>
        /// Returns true if the direction is one of N,E,S, or W.
        /// </summary>
        public bool isSharp { get { return (index % 2) == 0; } }

        /// <summary>
        /// Returns true if the direction is EAST or WEST
        /// </summary>
        public bool isParallelToX { get { return (index % 4) == 2; } }

        /// <summary>
        /// Returns true if the direction is NORTH or SOUTH
        /// </summary>
        public bool isParallelToY { get { return (index % 4) == 0; } }

        /// <summary>
        /// [0,8)のインデックス。
        /// </summary>
        public readonly int index;

        /// <summary>
        /// 反時計回りに４５度回転したDirectionを得る
        /// </summary>
        public Direction left { get { return directions[(index + 7) % 8]; } }
        /// <summary>
        /// 
        /// </summary>
        public Direction left90 { get { return directions[(index + 6) % 8]; } }

        /// <summary>
        /// 時計回りに４５度回転したDirectionを得る
        /// </summary>
        public Direction right { get { return directions[(index + 1) % 8]; } }
        /// <summary>
        /// 
        /// </summary>
        public Direction right90 { get { return directions[(index + 2) % 8]; } }

        /// <summary>Gets the opposite direction.</summary>
        public Direction opposite { get { return directions[(index + 4) % 8]; } }

        /// <summary>
        /// ２つのDirectionの交差角を４５度の倍数で得る。
        /// 戻り値は[0,4]。1なら４５度で交差。
        /// </summary>
        public static int angle(Direction a, Direction b)
        {
            int d = a.index - b.index;
            if (d < 0) d = -d;
            if (d > 4) d = 8 - d;
            Debug.Assert(0 <= d && d <= 4);
            return d;
        }

        /// <summary>
        /// Cast operators
        /// </summary>
        public static implicit operator Distance(Direction d)
        {
            return new Distance(d.offsetX, d.offsetY, 0);
        }


        private static readonly Surface arrowImage = ResourceUtil.LoadTimeIndependentSystemSurface("Arrows.bmp");
        private static readonly Surface darkArrowImage = ResourceUtil.LoadTimeIndependentSystemSurface("Arrows.dark.bmp");

        /// <summary> Draws an arrow on the given surface. </summary>
        public void drawArrow(Surface display, Point pt, bool isDark)
        {
            if (isDark) drawDarkArrow(display, pt);
            else drawArrow(display, pt);
        }

        /// <summary> Draws an arrow on the given surface. </summary>
        public void drawArrow(Surface display, Point pt)
        {
            display.Blit(pt, arrowImage, new Point(32 * index, 0), new Size(32, 16));
        }

        /// <summary> Draws a dark arrow on the given surface. </summary>
        public void drawDarkArrow(Surface display, Point pt)
        {
            display.Blit(pt, darkArrowImage, new Point(32 * index, 0), new Size(32, 16));
        }



        // singleton.
        private Direction(int x, int y, string name, int idx)
        {
            this.offsetX = x;
            this.offsetY = y;
            this.index = idx;
            this.displayName = name;
        }

        /// <summary>
        /// 存在する全てのオブジェクト。北から時計回り
        /// </summary>
        public static readonly Direction[] directions = {
			new Direction( 0,-1,"North"  ,0),
			new Direction( 1,-1,"Northeast",1),
			new Direction( 1, 0,  "East",2),
			new Direction( 1, 1,"Southeast",3),
			new Direction( 0, 1,"South"  ,4),
			new Direction(-1, 1,"Southwest",5),
			new Direction(-1, 0,  "West",6),
			new Direction(-1,-1,"Northwest",7) };
        //! new Direction( 0,-1,"北"  ,0),
        //! new Direction( 1,-1,"北東",1),
        //! new Direction( 1, 0,  "東",2),
        //! new Direction( 1, 1,"南東",3),
        //! new Direction( 0, 1,"南"  ,4),
        //! new Direction(-1, 1,"南西",5),
        //! new Direction(-1, 0,  "西",6),
        //! new Direction(-1,-1,"北西",7) };


        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.SetType(typeof(ReferenceImpl));
            info.AddValue("index", index);
        }

        [Serializable]
        internal sealed class ReferenceImpl : IObjectReference
        {
            private int index = 0;
            public object GetRealObject(StreamingContext context)
            {
                return Direction.get(index);
            }
        }
    }
}
