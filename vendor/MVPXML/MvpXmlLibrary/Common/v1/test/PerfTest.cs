using System;
using System.Runtime.InteropServices;

    public class PerfTest {

        [DllImport("kernel32.dll", EntryPoint = "QueryPerformanceCounter", CharSet = CharSet.Unicode)]
        extern static bool QueryPerformanceCounter(out long perfcount);

        [DllImport("kernel32.dll", EntryPoint = "QueryPerformanceFrequency", CharSet = CharSet.Unicode)]
        extern static bool QueryPerformanceFrequency(out long frequency);

        long startTime;
        long stopTime;

        public void Start() {
            QueryPerformanceCounter(out this.startTime);
        }
        
        public float Stop() {
            QueryPerformanceCounter(out this.stopTime);
            long frequency;
            QueryPerformanceFrequency(out frequency);
            float diff = (stopTime - startTime);
            return diff*1000f/(float)frequency;        
        }    
    }
