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
using System.Xml;
using FreeTrain.Contributions.Common;
using FreeTrain.Framework.Plugin;
using FreeTrain.World;
using FreeTrain.World.Rail;
using FreeTrain.World.Structs;

namespace FreeTrain.Contributions.Rail
{
    /// <summary>
    /// Stationary objects related to rail road.
    /// </summary>
    [Serializable]
    public class RailStationaryContribution : FixedSizeStructureContribution
    {
        /// <summary>
        /// Parses a commercial structure contribution from a DOM node.
        /// </summary>
        /// <exception cref="XmlException">If the parsing fails</exception>
        public RailStationaryContribution(XmlElement e) : base(e) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override StructureGroup GetGroup(string name)
        {
            return PluginManager.RailStationaryGroup[name];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wLoc"></param>
        /// <param name="initiallyOwned"></param>
        /// <returns></returns>
        public override Structure Create(WorldLocator wLoc, bool initiallyOwned)
        {
            return new RailStationaryStructure(this, wLoc);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseLoc"></param>
        /// <param name="cm"></param>
        /// <returns></returns>
        public override bool CanBeBuilt(Location baseLoc, ControlMode cm)
        {
            return RailStationaryStructure.canBeBuilt(baseLoc, Size, cm);
        }

    }
}
