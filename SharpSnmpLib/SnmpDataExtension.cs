﻿// ISnmpData extension class.
// Copyright (C) 2008-2010 Malcolm Crowe, Lex Li, and other contributors.
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.IO;

namespace Lextm.SharpSnmpLib
{
    /// <summary>
    /// ISnmpData extension class.
    /// </summary>
    public static class SnmpDataExtension
    {
        /// <summary>
        /// Converts to byte format.
        /// </summary>
        /// <returns></returns>
        public static byte[] ToBytes(this ISnmpData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
                
            using (MemoryStream result = new MemoryStream())
            {
                data.AppendBytesTo(result);
                return result.ToArray();
            }
        }
    }
}