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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using FreeTrain.Contributions.Common;
using FreeTrain.Framework;
using FreeTrain.Framework.Graphics;
using FreeTrain.Framework.Plugin;
using FreeTrain.Views.Map;
using FreeTrain.World;
using FreeTrain.World.Structs;

namespace FreeTrain.Controllers.Structs
{
    /// <summary>
    /// FixedSizeStructController の概要の説明です。
    /// </summary>
    public abstract class FixedSizeStructController : StructPlacementController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupGroup"></param>
        protected FixedSizeStructController(StructureGroupGroup groupGroup) : base(groupGroup) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="loc"></param>
        public abstract void Remove(MapViewWindow view, Location loc);

        // TODO: extend StructureContribution and Structure so that 
        // the this method can be implemented here.
        /// <summary>
        /// 
        /// </summary>
        protected new FixedSizeStructureContribution SelectedType
        {
            get
            {
                return (FixedSizeStructureContribution)base.SelectedType;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="loc"></param>
        /// <param name="ab"></param>
        public override void OnClick(MapViewWindow view, Location loc, Point ab)
        {
            if (IsPlacing)
            {
                if (!SelectedType.CanBeBuilt(loc, ControlMode.Player))
                {
                    MessageBox.Show("Can not build");
                    //! MessageBox.Show("設置できません");
                }
                else
                {
                    CompletionHandler handler = new CompletionHandler(SelectedType, loc, true);
                    new ConstructionSite(loc, new EventHandler(handler.Handle), SelectedType.Size);
                }
            }
            else
            {
                Remove(view, loc);
            }
        }

        [Serializable]
        private class CompletionHandler
        {
            internal CompletionHandler(FixedSizeStructureContribution contribution, Location loc, bool owned)
            {
                this.contribution = contribution;
                this.loc = loc;
                this.owned = owned;
            }
            private readonly FixedSizeStructureContribution contribution;
            private readonly Location loc;
            private readonly bool owned;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="args"></param>
            public void Handle(object sender, EventArgs args)
            {
                Structure s = contribution.Create(loc, owned);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override AlphaBlendSpriteSet CreateAlphaSprites()
        {
            if (SelectedType != null) return new AlphaBlendSpriteSet(SelectedType.Sprites);
            else return null;
        }
    }
}
