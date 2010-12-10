﻿// SNMP engine class.
// Copyright (C) 2009-2010 Lex Li
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
 * Date: 11/28/2009
 * Time: 12:40 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Lextm.SharpSnmpLib.Messaging;

namespace Lextm.SharpSnmpLib.Pipeline
{
    /// <summary>
    /// SNMP engine, who is the core of an SNMP entity (manager or agent).
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    public sealed class SnmpEngine : IDisposable
    {
        private readonly SnmpApplicationFactory _factory;
        private readonly EngineGroup _group;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnmpEngine"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="listener">The listener.</param>
        /// <param name="group">Engine core group.</param>
        public SnmpEngine(SnmpApplicationFactory factory, Listener listener, EngineGroup group)
        {
            _factory = factory;
            Listener = listener;
            _group = group;
        }
        
        /// <summary>
        /// Disposes resources in use.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="SnmpEngine"/> is reclaimed by garbage collection.
        /// </summary>
        ~SnmpEngine()
        {
            Dispose(false);
        }
        
        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Component"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            
            if (disposing)
            {
                if (Listener != null)
                {
                    Listener.Dispose();
                    Listener = null;
                }
            }
            
            _disposed = true;
        }
        
        /// <summary>
        /// Gets or sets the listener.
        /// </summary>
        /// <value>The listener.</value>
        public Listener Listener { get; private set; }

        private void ListenerMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            ISnmpMessage request = e.Message;
            var context = SnmpContextFactory.Create(request, e.Sender, Listener.Users, _group, e.Binding);
            SnmpApplication application = _factory.Create(context);
            application.Process();
        }

        /// <summary>
        /// Starts the engine.
        /// </summary>
        public void Start()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            
            Listener.ExceptionRaised += ListenerExceptionRaised;
            Listener.MessageReceived += ListenerMessageReceived;
            Listener.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            
            Listener.Stop();
            Listener.ExceptionRaised -= ListenerExceptionRaised;
            Listener.MessageReceived -= ListenerMessageReceived;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="SnmpEngine"/> is active.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        public bool Active
        {
            get
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                
                return Listener.Active;
            }
        }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void ListenerExceptionRaised(object sender, ExceptionRaisedEventArgs e)
        {
            var handler = ExceptionRaised;
            if (handler != null)
            {
                handler(sender, e);
            }
        }        
        
        /// <summary>
        /// Occurs when an exception is raised.
        /// </summary>
        public event EventHandler<ExceptionRaisedEventArgs> ExceptionRaised;
    }
}
