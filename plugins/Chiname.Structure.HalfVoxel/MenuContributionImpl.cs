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
using System.Windows.Forms;
using FreeTrain.Framework;
using FreeTrain.Contributions.Others;

namespace FreeTrain.World.Structs.HalfVoxelStructure
{
    /// <summary>
    /// Adds "half voxel" menu to the main window
    /// </summary>
    public class MenuContributionImpl : MenuContribution
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public MenuContributionImpl(XmlElement e) : base(e) { }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="containerMenu"></param>
        public override void MergeMenu(MainMenu containerMenu)
        {
            MenuItem item = new MenuItem("&Half-tile Construction...");
            item.Click += new System.EventHandler(OnClick);

            containerMenu.MenuItems[4].MenuItems.Add(item);
        }

        private void OnClick(object sender, EventArgs e)
        {
            new ControllerForm().Show();
        }
    }
}
