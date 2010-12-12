// SEQUENCE data type.
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

/*
 * Created by SharpDevelop.
 * User: lextm
 * Date: 2008/4/30
 * Time: 20:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lextm.SharpSnmpLib
{
    /// <summary>
    /// Array type.
    /// </summary>
    /// <remarks>Represents SMIv1 SEQUENCE.</remarks>
    public class Sequence : ISnmpData
    {
        private byte[] _raw;
        private readonly List<ISnmpData> _list = new List<ISnmpData>();

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Creates an <see cref="Sequence"/> instance with varied <see cref="ISnmpData"/> instances.
        /// </summary>
        /// <param name="items"></param>
        public Sequence(params ISnmpData[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            foreach (ISnmpData data in items.Where(data => data != null))
            {
                _list.Add(data);
            }
        }

        /// <summary>
        /// Creates an <see cref="Sequence"/> instance with varied <see cref="ISnmpData"/> instances.
        /// </summary>
        /// <param name="items"></param>
        public Sequence(IEnumerable<ISnmpData> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            foreach (ISnmpData data in items.Where(data => data != null))
            {
                _list.Add(data);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sequence"/> class.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="stream">The stream.</param>
        public Sequence(int length, Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            
            if (length == 0)
            {
                return;
            }

            long original = stream.Position;
            while (stream.Position < original + length)
            {
                _list.Add(DataFactory.CreateSnmpData(stream));
            }
        }

        /// <summary>
        /// Item count in this <see cref="Sequence"/>.
        /// </summary>
        public int Length
        {
            get { return _list.Count; }
        }

        /// <summary>
        /// Gets the <see cref="Lextm.SharpSnmpLib.ISnmpData"/> at the specified index.
        /// </summary>
        /// <value></value>
        public ISnmpData this[int index]
        {
            get { return _list[index]; }
        }
        
        /// <summary>
        /// Type code.
        /// </summary>
        public SnmpType TypeCode
        {
            get { return SnmpType.Sequence; }
        }

        /// <summary>
        /// Appends the bytes to <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void AppendBytesTo(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (_raw == null)
            {
                _raw = ByteTool.ParseItems(_list);
            }

            stream.AppendBytes(TypeCode, _raw);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this <see cref="Sequence"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder("SNMP SEQUENCE: ");
            foreach (ISnmpData item in _list)
            {
                result.Append(item).Append("; ");
            }
            
            return result.ToString();
        }
    }
}
