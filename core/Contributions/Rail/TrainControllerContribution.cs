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
using FreeTrain.Framework.Plugin;
using FreeTrain.World.Rail;

namespace FreeTrain.Contributions.Rail
{
    /// <summary>
    /// plug-in that provides TrainController implementations
    /// </summary>
    [Serializable]
    public abstract class TrainControllerContribution : Contribution
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected TrainControllerContribution(XmlElement e)
            :
            base("trainController", e.Attributes["id"].Value) { }

        /// <summary>
        /// Creates a new instance of TrainController.
        /// </summary>
        public abstract TrainController NewController(string name);

        /// <summary>
        /// Gets the name of this train controller type.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the description of this train controller type.
        /// </summary>
        public abstract string Description { get; }
    }
}
