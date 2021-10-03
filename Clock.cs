using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;


namespace tris
{
    class Clock
    {
        Thread th = null;
        DateTime lastTick = DateTime.MinValue;
        DateTime lastSubTick = DateTime.MinValue;
        public EventHandler OnTick=null;
        public EventHandler OnSubTick = null;
        public double interval = 1000.0;
        public bool stopFlag = false;
        public void Start() 
        {
            Stop();
            th = new Thread(new ParameterizedThreadStart(timerProc));
            this.stopFlag = false;
            th.Start(this);
        }

        private static void timerProc(object obj)
        {
            Clock c = (Clock)obj;
            while (!c.stopFlag)
            {

                if (c.OnSubTick != null)
                {
                    TimeSpan dtSub = DateTime.Now.Subtract(c.lastSubTick);
                    double subticks = c.interval / 8.0;
                    if (subticks < 60.0) { subticks = 60.0; }
                    dtSub = DateTime.Now.Subtract(c.lastSubTick);
                    if (dtSub.TotalMilliseconds > subticks)
                    {
                        c.lastSubTick = DateTime.Now;
                        c.OnSubTick(c, new EventArgs());
                    }
                }

                TimeSpan dt = DateTime.Now.Subtract(c.lastTick);
                if (dt.TotalMilliseconds > c.interval)
                {
                    c.lastTick = DateTime.Now;
                    if (c.OnTick != null) { c.OnTick(c, new EventArgs()); }
                }

                Thread.Sleep( (int)(c.interval / 10.0));
            }
        }


        public void Stop()
        {
            this.stopFlag = true;
            Thread.Sleep((int)(interval / 5.0));
            if (th != null)
            {
                try
                {
                    if (th.IsAlive)
                    {
                        th.Abort();
                    }
                }
                catch (Exception exc)
                { 
                    //threadabort-exception
                    Trace.WriteLine(exc.ToString());
                }
            }
            th = null;
        }
    }
}
