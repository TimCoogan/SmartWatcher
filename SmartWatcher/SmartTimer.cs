using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SmartWatcher.Services
{
    public class SmartTimer
    {
        private bool RunTimer = false;
        private Thread thread = null;
        public readonly int MaxInterval = 60000;
        private int Interval = 100;
        public string Name = "SmartTimer";
        public event SmartTimerTickHandler EventSmartTimerTick = delegate { };
        

        public SmartTimer()
        {
            CreateThread();
        }

        public SmartTimer(int interval)
        {
            this.Interval = interval;
            CreateThread();
        }

        public SmartTimer(int interval, string name)
        {
            this.Interval = interval;
            this.Name = name;
            CreateThread();
        }

        /// <summary>
        /// Creates the thread if the interval is greater than 0 milliseconds 
        /// </summary>
        private void CreateThread()
        {
            // Normalize  the interval
            this.Interval = Math.Max(0, Math.Min(this.Interval, this.MaxInterval));
            // If the interval is 0, this indicates we don't want to monitor the path 
            // for availability.
            if (this.Interval > 0)
            {
                this.thread = new Thread(new ThreadStart(SmartTimerTick));
                this.thread.Name = this.Name;
                this.thread.IsBackground = true;
            }
        }

   
        /// <summary>
        /// start the Smart Timer thread
        /// </summary>
        public void StartTimer()
        {
            this.RunTimer = true;
            if (this.thread != null && this.thread.ThreadState != ThreadState.Running)
            {
                this.thread.Start();
            }
        }

        public void StopTimer()
        {
            this.RunTimer = false;
        }

        public void SmartTimerTick()
        {
            while (this.RunTimer)
            {
                RaiseEventSmartTimerTick();

                Thread.Sleep(this.Interval);
            }
        }

        private void RaiseEventSmartTimerTick()
        {
            EventSmartTimerTick(this, new SmartTimerEventArgs());
        }

        public class SmartTimerEventArgs : EventArgs
        {
            public SmartTimerEventArgs()
            {

            }
        }

        public delegate void SmartTimerTickHandler(object sender, SmartTimerEventArgs e);


    }
}
