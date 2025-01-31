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
using System.IO;
using System.Net;
using System.Xml;
using FreeTrain.World;

namespace FreeTrain.Framework.Plugin
{
    /// <summary>
    /// Utility methods to help parsing XML documents.
    /// </summary>
    public class XmlUtil
    {
        /// <summary>
        /// Performs a node selection and throws an exception if it's not found.
        /// </summary>
        /// <exception cref="XmlException"></exception>
        public static XmlNode SelectSingleNode(XmlNode node, string xPath)
        {
            XmlNode n = node.SelectSingleNode(xPath);
            if (n == null)
            {
                throw new XmlException("unable to find " + xPath, null);
            }
            return n;
        }

        /// <summary>
        /// Resolves a relative URI.
        /// </summary>
        public static Uri Resolve(XmlNode context, string relative)
        {
            return new Uri(new Uri(context.BaseURI), relative);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Point ParsePoint(string text)
        {
            try
            {
                int idx = text.IndexOf(',');
                return new Point(int.Parse(text.Substring(0, idx)), int.Parse(text.Substring(idx + 1)));
            }
            catch (Exception e)
            {
                throw new FormatException("Unable to parse " + text + " as point", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Size ParseSize(string text)
        {
            try
            {
                int idx = text.IndexOf(',');
                return new Size(int.Parse(text.Substring(0, idx)), int.Parse(text.Substring(idx + 1)));
            }
            catch (Exception e)
            {
                throw new FormatException("Unable to parse " + text + " as size", e);
            }
        }
    }
}
