﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SuperSocket.Common;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase.Security;

namespace SuperSocket.SocketBase
{
    /// <summary>
    /// AppServer basic class
    /// </summary>
    public abstract class AppServer : AppServer<AppSession>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppServer"/> class.
        /// </summary>
        public AppServer()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServer"/> class.
        /// </summary>
        /// <param name="requestFilterFactory">The request filter factory.</param>
        public AppServer(IRequestFilterFactory<StringRequestInfo> requestFilterFactory)
            : base(requestFilterFactory)
        {

        }
    }

    /// <summary>
    /// AppServer basic class
    /// </summary>
    /// <typeparam name="TAppSession">The type of the app session.</typeparam>
    public abstract class AppServer<TAppSession> : AppServer<TAppSession, StringRequestInfo>
        where TAppSession : AppSession<TAppSession, StringRequestInfo>, IAppSession, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppServer&lt;TAppSession&gt;"/> class.
        /// </summary>
        public AppServer()
            : base(new CommandLineRequestFilterFactory())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServer&lt;TAppSession&gt;"/> class.
        /// </summary>
        /// <param name="requestFilterFactory">The request filter factory.</param>
        public AppServer(IRequestFilterFactory<StringRequestInfo> requestFilterFactory)
            : base(requestFilterFactory)
        {

        }
    }


    /// <summary>
    /// AppServer basic class
    /// </summary>
    /// <typeparam name="TAppSession">The type of the app session.</typeparam>
    /// <typeparam name="TRequestInfo">The type of the request info.</typeparam>
    public abstract class AppServer<TAppSession, TRequestInfo> : AppServerBase<TAppSession, TRequestInfo>, IPerformanceDataSource
        where TRequestInfo : class, IRequestInfo
        where TAppSession : AppSession<TAppSession, TRequestInfo>, IAppSession, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppServer&lt;TAppSession, TRequestInfo&gt;"/> class.
        /// </summary>
        public AppServer()
            : base()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServer&lt;TAppSession, TRequestInfo&gt;"/> class.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        protected AppServer(IRequestFilterFactory<TRequestInfo> protocol)
            : base(protocol)
        {
   
        }

        /// <summary>
        /// Starts this AppServer instance.
        /// </summary>
        /// <returns></returns>
        public override bool Start()
        {
            if (!base.Start())
                return false;

            if (!Config.DisableSessionSnapshot)
                StartSessionSnapshotTimer();

            if (Config.ClearIdleSession)
                StartClearSessionTimer();

            return true;
        }

        private ConcurrentDictionary<string, TAppSession> m_SessionDict = new ConcurrentDictionary<string, TAppSession>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Creates the app session base one socketSession.
        /// </summary>
        /// <param name="socketSession">The socket session.</param>
        /// <returns></returns>
        public override IAppSession CreateAppSession(ISocketSession socketSession)
        {
            var baseAppSession = base.CreateAppSession(socketSession);

            if (baseAppSession == null)
                return null;

            var appSession = (TAppSession)baseAppSession;

            if (m_SessionDict.TryAdd(appSession.SessionID, appSession))
            {
                if(!Logger.IsInfoEnabled)
                    Logger.Info(appSession, "New SocketSession was accepted!");
                return appSession;
            }
            else
            {
                if(Logger.IsErrorEnabled)
                    Logger.Error(appSession, "SocketSession was refused because the session's IdentityKey already exists!");
                return NullAppSession;
            }
        }

        /// <summary>
        /// Gets the app session by ID internal.
        /// </summary>
        /// <param name="sessionID">The session ID.</param>
        /// <returns></returns>
        protected override IAppSession GetAppSessionByIDInternal(string sessionID)
        {
            return GetAppSessionByID(sessionID);
        }

        /// <summary>
        /// Gets the app session by ID.
        /// </summary>
        /// <param name="sessionID">The session ID.</param>
        /// <returns></returns>
        public TAppSession GetAppSessionByID(string sessionID)
        {
            if (string.IsNullOrEmpty(sessionID))
                return NullAppSession;

            TAppSession targetSession;
            m_SessionDict.TryGetValue(sessionID, out targetSession);
            return targetSession;
        }

        /// <summary>
        /// Called when [socket session closed].
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="reason">The reason.</param>
        protected override void OnSessionClosed(TAppSession session, CloseReason reason)
        {
            string sessionID = session.SessionID;

            if (!string.IsNullOrEmpty(sessionID))
            {
                TAppSession removedSession;
                if (!m_SessionDict.TryRemove(sessionID, out removedSession))
                {
                    if (Logger.IsErrorEnabled)
                        Logger.Error(removedSession, "Failed to remove this session, Because it haven't been in session container!");
                }
            }

            base.OnSessionClosed(session, reason);
        }

        /// <summary>
        /// Gets the total session count.
        /// </summary>
        public override int SessionCount
        {
            get
            {
                return m_SessionDict.Count;
            }
        }

        #region Clear idle sessions

        private System.Threading.Timer m_ClearIdleSessionTimer = null;

        private void StartClearSessionTimer()
        {
            int interval = Config.ClearIdleSessionInterval * 1000;//in milliseconds
            m_ClearIdleSessionTimer = new System.Threading.Timer(ClearIdleSession, new object(), interval, interval);
        }

        /// <summary>
        /// Clears the idle session.
        /// </summary>
        /// <param name="state">The state.</param>
        private void ClearIdleSession(object state)
        {
            if (Monitor.TryEnter(state))
            {
                try
                {
                    DateTime now = DateTime.Now;
                    DateTime timeOut = now.AddSeconds(0 - Config.IdleSessionTimeOut);

                    var timeOutSessions = SessionSource.Where(s => s.Value.LastActiveTime <= timeOut).Select(s => s.Value);
                    System.Threading.Tasks.Parallel.ForEach(timeOutSessions, s =>
                        {
                            if (Logger.IsInfoEnabled)
                                Logger.Info(s, string.Format("The socket session has been closed for {0} timeout, last active time: {1}!", now.Subtract(s.LastActiveTime).TotalSeconds, s.LastActiveTime));
                            s.Close(CloseReason.TimeOut);
                        });
                }
                catch (Exception e)
                {
                    if(Logger.IsErrorEnabled)
                        Logger.Error("Clear idle session error!", e);
                }
                finally
                {
                    Monitor.Exit(state);
                }
            }
        }

        private KeyValuePair<string, TAppSession>[] SessionSource
        {
            get
            {
                if (Config.DisableSessionSnapshot)
                    return m_SessionDict.ToArray();
                else
                    return m_SessionsSnapshot;
            }
        }

        #endregion

        #region Take session snapshot

        private System.Threading.Timer m_SessionSnapshotTimer = null;

        private KeyValuePair<string, TAppSession>[] m_SessionsSnapshot = new KeyValuePair<string, TAppSession>[0];

        private void StartSessionSnapshotTimer()
        {
            int interval = Math.Max(Config.SessionSnapshotInterval, 1) * 1000;//in milliseconds
            m_SessionSnapshotTimer = new System.Threading.Timer(TakeSessionSnapshot, new object(), interval, interval);
        }

        private void TakeSessionSnapshot(object state)
        {
            if (Monitor.TryEnter(state))
            {
                Interlocked.Exchange(ref m_SessionsSnapshot, m_SessionDict.ToArray());
                Monitor.Exit(state);
            }
        }

        #endregion

        #region Search session utils

        /// <summary>
        /// Gets the matched sessions from sessions snapshot.
        /// </summary>
        /// <param name="critera">The prediction critera.</param>
        /// <returns></returns>
        public override IEnumerable<TAppSession> GetSessions(Func<TAppSession, bool> critera)
        {
            return SessionSource.Select(p => p.Value).Where(critera);
        }

        /// <summary>
        /// Gets all sessions in sessions snapshot.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<TAppSession> GetAllSessions()
        {
            return SessionSource.Select(p => p.Value);
        }

        #endregion

        #region Performance logging

        private PerformanceData m_PerformanceData = new PerformanceData();

        /// <summary>
        /// Collects the performance data.
        /// </summary>
        /// <param name="globalPerfData">The global perf data.</param>
        /// <returns></returns>
        public PerformanceData CollectPerformanceData(GlobalPerformanceData globalPerfData)
        {
            m_PerformanceData.PushRecord(new PerformanceRecord
                {
                    TotalConnections = m_SessionDict.Count,
                    TotalHandledRequests = TotalHandledRequests
                });

            //User can process the performance data by self
            Async.Run(() => OnPerformanceDataCollected(globalPerfData, m_PerformanceData), e => Logger.Error(e));

            return m_PerformanceData;
        }

        /// <summary>
        /// Called when [performance data collected], you can override this method to get collected performance data
        /// </summary>
        /// <param name="globalPerfData">The global perf data.</param>
        /// <param name="performanceData">The performance data.</param>
        protected virtual void OnPerformanceDataCollected(GlobalPerformanceData globalPerfData, PerformanceData performanceData)
        {

        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {                
                if (m_SessionSnapshotTimer != null)
                {
                    m_SessionSnapshotTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    m_SessionSnapshotTimer.Dispose();
                    m_SessionSnapshotTimer = null;
                }

                if (m_ClearIdleSessionTimer != null)
                {
                    m_ClearIdleSessionTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    m_ClearIdleSessionTimer.Dispose();
                    m_ClearIdleSessionTimer = null;
                }

                var sessions = m_SessionDict.ToArray();

                if(sessions.Length > 0)
                {
                    var tasks = new Task[sessions.Length];
                    
                    for(var i = 0; i < tasks.Length; i++)
                    {
                        tasks[i] = Task.Factory.StartNew((s) =>
                            {
                                ((TAppSession)s).Close(CloseReason.ServerShutdown);
                            }, sessions[i].Value);
                    }

                    Task.WaitAll(tasks);
                }
            }
        }

        #endregion
    }
}
