using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

namespace HuaQuant.JobSchedule
{
    public class JobSchedule
    {
        private Dictionary<IJob, JobTrigger> triggers = new Dictionary<IJob, JobTrigger>();
        private Dictionary<IJob, Thread> threads = new Dictionary<IJob, Thread>();
        private System.Timers.Timer timer = null;
        private int interval = 1000;

        public JobSchedule() { }

        public void Start()
        {
            timer = new System.Timers.Timer(this.interval);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }
        public void Stop()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Elapsed -= Timer_Elapsed;
                timer = null;
            }
            foreach(KeyValuePair<IJob,Thread> kvp in threads)
            {
                if (kvp.Value != null && kvp.Value.ThreadState != ThreadState.Aborted && kvp.Value.ThreadState != ThreadState.Stopped)
                {
                    kvp.Value.Abort();
                }
            }
            threads.Clear();
        }
        private int inTimer = 0;//防止计时器事件重入
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Interlocked.Exchange(ref inTimer, 1) == 0)
            {
                List<IJob> expiredJobs = new List<IJob>();
                lock (triggers)
                {                    
                    foreach (KeyValuePair<IJob, JobTrigger> kvp in triggers)
                    {
                        if (kvp.Value.Expired) expiredJobs.Add(kvp.Key);
                        else schedule(e.SignalTime, kvp.Value, kvp.Key);
                    }
                    foreach (IJob job in expiredJobs) triggers.Remove(job);
                }
                Interlocked.Exchange(ref inTimer, 0);
            }
        }
        private void schedule(DateTime time, JobTrigger trigger, IJob job)
        {
            if (job.IsRunning) return;
            if (trigger.Triggering(time, job.Times))
            {
                Thread curThread = null;
                if (threads.TryGetValue(job, out curThread) &&
                    curThread.ThreadState != ThreadState.Stopped &&
                    curThread.ThreadState != ThreadState.Aborted) return;
                else
                {
                    curThread = new Thread(new ThreadStart(job.Execute));
                    threads[job] = curThread;
                    curThread.Start();
                }
            }
        }

        public void Add(IJob job, JobTrigger trigger)
        {
            lock (triggers)
            {
                triggers.Add(job, trigger);
            }
        }
    }
}
