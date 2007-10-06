// Turn on logging to the event log.
#define LOGEVENTS

using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Nuxleus.Service
{

    public enum ReplicatonServiceCustomCommands { StopWorker = 128, RestartWorker, CheckWorker };

    [StructLayout(LayoutKind.Sequential)]
    public struct SERVICE_STATUS
    {
        public int serviceType;
        public int currentState;
        public int controlsAccepted;
        public int win32ExitCode;
        public int serviceSpecificExitCode;
        public int checkPoint;
        public int waitHint;
    }

    public enum State
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    public class ReplicationService : ServiceBase
    {
        private SERVICE_STATUS replicatinServiceStatus;

        private Thread workerThread = null;

        public ReplicationService()
        {
            CanPauseAndContinue = true;
            //CanHandleSessionChangeEvent = true;
            ServiceName = "ReplicationService";
        }

    }
}
