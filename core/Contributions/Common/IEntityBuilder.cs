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
using System.Drawing;
using FreeTrain.Framework.Graphics;
using FreeTrain.Controllers;
using FreeTrain.Contributions.Population;

namespace FreeTrain.Contributions.Common
{
    /// <summary>
    /// IEntityBuilder の概要の説明です。
    /// </summary>
    public interface IEntityBuilder
    {

        /// <summary> 
        /// Population of this structure, or null if this structure is not populated. 
        /// </summary>
        BasePopulation Population { get; }

        /// <summary>
        /// True if the computer (the development algorithm) is not allowed to
        /// build this structure.
        /// </summary>
        // TODO: make IEntityBuilder responsible for creating a new Plan object.
        bool ComputerCannotBuild { get; }

        /// <summary>
        /// True if the player is not allowed to build this structure.
        /// </summary>
        bool PlayerCannotBuild { get; }

        /// <summary>
        /// Name of this entity builder. Primarily used as the display name.
        /// Doesn't need to be unique.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 
        /// </summary>
        int Price { get; set;}

        /// <summary>
        /// price par area (minimum).
        /// </summary>
        double PricePerArea { get; set;}

        /// <summary>
        /// Creates a preview
        /// </summary>
        /// <param name="pixelSize"></param>
        /// <returns></returns>
        PreviewDrawer CreatePreview(Size pixelSize);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        IModalController CreateBuilder(IControllerSite site);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        IModalController CreateRemover(IControllerSite site);
    }
}
