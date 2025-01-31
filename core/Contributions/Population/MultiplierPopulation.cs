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
using System.Runtime.Serialization;
using FreeTrain.World;
using FreeTrain.Framework.Plugin;

namespace FreeTrain.Contributions.Population
{
    /// <summary>
    /// Multiplies another population by a constant factor.
    /// This object is not-persistent.
    /// </summary>
    [Serializable]
    public class MultiplierPopulation : BasePopulation
    {
        private readonly int factor;
        private readonly BasePopulation core;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <param name="core"></param>
        public MultiplierPopulation(int f, BasePopulation core)
        {
            this.factor = f;
            this.core = core;

        }
        /// <summary>
        /// 
        /// </summary>
        public override int Residents
        {
            get
            {
                return core.Residents * factor;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        public override int CalcPopulation(Time currentTime)
        {
            return core.CalcPopulation(currentTime) * factor;
        }
    }
}
