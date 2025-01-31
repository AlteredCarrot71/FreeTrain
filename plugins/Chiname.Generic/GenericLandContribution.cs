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
using System.Diagnostics;
using System.Xml;
using System.Collections;
using FreeTrain.Framework;
using FreeTrain.Framework.Graphics;
using FreeTrain.Framework.Plugin;
using FreeTrain.Controllers;
using FreeTrain.Controllers.Structs;
using FreeTrain.Contributions;
using FreeTrain.Contributions.Common;
using FreeTrain.Contributions.Population;
using FreeTrain.Contributions.Structs;
using FreeTrain.Views;
using FreeTrain.Views.Map;
using FreeTrain.World;
using FreeTrain.World.Structs;

namespace FreeTrain.Framework.Plugin.Generic
{
    /// <summary>
    /// GenericLandContribution
    /// </summary>
    public class GenericLandContribution : GenericStructureContribution
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public GenericLandContribution(XmlElement e)
            : base(e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void loadPrimitiveParams(XmlElement e)
        {
            XmlNode xn = e.SelectSingleNode("structure");
            if (xn != null)
                Categories = new StructCategories(xn, this.Id);
            else
                Categories = new StructCategories();

            if (Categories.Count == 0)
            {
                StructCategory.Root.Entries.Add(this.Id);
                Categories.Add(StructCategory.Root);
            }

            try
            {
                Design = e.SelectSingleNode("design").InnerText;
            }
            catch
            {
                //! _design = "標準";
                Design = "default";
            }

            try
            {
                UnitPrice = int.Parse(XmlUtil.SelectSingleNode(e, "price").InnerText);
            }
            catch
            {
                UnitPrice = 0;
            }

            Size = new Size(1, 1);

            MinHeight = 2;
            MaxHeight = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="color"></param>
        /// <param name="contrib"></param>
        /// <returns></returns>
        protected override Contribution CreatePrimitiveContrib(XmlElement sprite, XmlNode color, XmlElement contrib)
        {
            sprite.AppendChild(color.Clone());
            //PluginManager manager = PluginManager;
            IContributionFactory factory = PluginManager.GetContributionFactory("land");
            XmlNode temp = contrib.Clone();
            foreach (XmlNode cn in temp.ChildNodes)
            {
                if (cn.Name.Equals("sprite") || cn.Name.Equals("picture"))
                    temp.RemoveChild(cn);
            }
            temp.AppendChild(sprite);
            contrib.AppendChild(temp);
            return factory.Load(Parent, (XmlElement)temp);
        }

    }
}
