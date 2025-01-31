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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using FreeTrain.Framework;
using FreeTrain.Views;
using FreeTrain.Views.Map;
using FreeTrain.World;

namespace FreeTrain.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class AbstractControllerImpl : Form, IModalController
    {
        /// <summary>
        /// 
        /// </summary>
        public AbstractControllerImpl()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            // Attach the control when activated.
            try
            {
                MainWindow.mainWindow.AttachController(this);
            }
            catch (NullReferenceException nre)
            {
                Debug.WriteLine(nre);
            }
        }

        /// <summary>
        /// Derived class still needs to extend this method and maintain
        /// the singleton.
        /// </summary>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Detach it when it is closed.
            if (MainWindow.mainWindow.CurrentController == this)
            {
                MainWindow.mainWindow.DetachController();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual ILocationDisambiguator Disambiguator 
        { 
            get 
            { 
                return null; 
            } 
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual IMapOverlay Overlay 
        { 
            get 
            { 
                return this as IMapOverlay; 
            } 
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnAttached() 
        { }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnDetached()
        {
            // redraw the entire surface to erase any left-over from this controller
            WorldDefinition.World.OnAllVoxelUpdated();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="loc"></param>
        /// <param name="ab"></param>
        public virtual void OnClick(MapViewWindow source, Location loc, Point ab) 
        { }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="loc"></param>
        /// <param name="ab"></param>
        public virtual void OnMouseMove(MapViewWindow view, Location loc, Point ab) 
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="loc"></param>
        /// <param name="ab"></param>
        public virtual void OnRightClick(MapViewWindow source, Location loc, Point ab)
        {
            Close();	// cancel
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void UpdatePreview()
        { }
    }
}
