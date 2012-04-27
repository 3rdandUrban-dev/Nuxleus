﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperWebSocket.Protocol.FramePartReader;

namespace SuperWebSocket.Protocol
{
    class WebSocketDataFrameReader : ICommandReader<IWebSocketFragment>
    {
        private WebSocketDataFrame m_Frame;
        private IDataFramePartReader m_PartReader;
        private int m_LastPartLength = 0;

        public IAppServer AppServer { get; private set; }

        public int LeftBufferSize
        {
            get { return m_Frame.InnerData.Count; }
        }

        public ICommandReader<IWebSocketFragment> NextCommandReader
        {
            get { return this; }
        }

        public WebSocketDataFrameReader(IAppServer appServer)
        {
            AppServer = appServer;
            m_PartReader = DataFramePartReader.NewReader;
        }

        protected void AddArraySegment(ArraySegmentList segments, byte[] buffer, int offset, int length, bool isReusableBuffer)
        {
            segments.AddSegment(buffer, offset, length, isReusableBuffer);
        }

        public IWebSocketFragment FindCommandInfo(IAppSession session, byte[] readBuffer, int offset, int length, bool isReusableBuffer, out int left)
        {
            if(m_Frame == null)
                m_Frame = new WebSocketDataFrame(new ArraySegmentList());

            this.AddArraySegment(m_Frame.InnerData, readBuffer, offset, length, isReusableBuffer);

            IDataFramePartReader nextPartReader;

            int thisLength = m_PartReader.Process(m_LastPartLength, m_Frame, out nextPartReader);

            if (thisLength < 0)
            {
                left = 0;
                return null;
            }
            else
            {
                left = thisLength;

                if (left > 0)
                    m_Frame.InnerData.TrimEnd(left);

                //Means this part reader is the last one
                if (nextPartReader == null)
                {
                    m_LastPartLength = 0;
                    m_PartReader = DataFramePartReader.NewReader;

                    var frame = m_Frame;
                    m_Frame = null;
                    return frame;
                }
                else
                {
                    m_LastPartLength = m_Frame.InnerData.Count - thisLength;
                    m_PartReader = nextPartReader;

                    return null;
                }
            }
        }
    }
}
