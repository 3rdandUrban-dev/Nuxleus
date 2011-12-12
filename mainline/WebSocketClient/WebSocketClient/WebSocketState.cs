﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperWebSocket.WebSocketClient
{
    public enum WebSocketState
    {
        None = -1,
        Connecting = 0,
        Open = 1,
        Closing = 2,
        Closed = 3
    }
}
