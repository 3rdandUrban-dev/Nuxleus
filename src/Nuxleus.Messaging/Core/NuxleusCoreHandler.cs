using System;
using System.Collections;
using System.Collections.Generic;
using ALAZ.SystemEx.NetEx.SocketsEx;
using ALAZ.SystemEx.ThreadingEx;

namespace Nuxleus.Messaging.Core
{
    public class NuxleusCoreHandler
    {
        ReceiverHandler _receiver = null;
        DispatchHandler _dispatcher = null;

        public NuxleusCoreHandler ()
        {
            _receiver = new ReceiverHandler();
            _dispatcher = new DispatchHandler();
            PostOffice m_postOffice = new PostOffice();
            _receiver.PostOffice = m_postOffice;
            _dispatcher.PostOffice = m_postOffice;
        }

        /// <summary>
        /// Gets or sets the service handling events on the connections
        /// between clients to the publisher and the publisher handler.
        /// </summary>
        public MessageService ReceiverService
        {
            get
            {
                return _receiver.Service;
            }
            set
            {
                _receiver.Service = value;
            }
        }

        /// <summary>
        /// Gets or sets the service handling events on the connections
        /// between the publisher and routers connected to it.
        /// </summary>
        public MessageService DispatcherService
        {
            get
            {
                return _dispatcher.Service;
            }
            set
            {
                _dispatcher.Service = value;
            }
        }

        /// <summary>
        /// PostOffice to synchronise receiver and dispatcher.
        /// Set internally by the constructor but can be changed to 
        /// different instance.
        /// </summary>
        public PostOffice PostOffice
        {
            set
            {
                _receiver.PostOffice = value;
                _dispatcher.PostOffice = value;
            }
        }
    }
}