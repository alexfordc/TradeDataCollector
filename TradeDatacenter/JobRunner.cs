using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
namespace TradeDatacenter
{
    public class JobRunner
    {
        private int interval=1000;
        private DateTime? beginTime=null;
        private DateTime? endTime=null;
        private TimeSpan? timeSpan = null;
        private List<IJob> jobs=new List<IJob>();
        private int times = 0;
        private System.Timers.Timer timer = null;
        public JobRunner(int interval=1000,DateTime? beginTime = null, DateTime? endTime = null,TimeSpan? timeSpan=null)
        {
            this.interval = interval;
            this.beginTime = beginTime;
            this.endTime = endTime;
            this.timeSpan = timeSpan;
        }
        public JobRunner( int interval = 1000, int times = 1, DateTime? beginTime = null, TimeSpan ?timeSpan = null)
        {
            this.interval = interval;
            this.beginTime = beginTime;
            this.times = times;
            this.timeSpan = timeSpan;
        }
        public void AddJob(IJob job)
        {
            this.jobs.Add(job);
        }
        private void run()
        {
            bool success = true;
            foreach(IJob job in this.jobs)
            {
                Console.WriteLine("job {0} begin...", job.GetType().Name);
                if (!job.Execute())
                {
                    success = false;
                    break;
                }
            }
            if (times > 0 && success) count++;
        }
        public void Start()
        {
            this.count = 0;
            this.timer = new System.Timers.Timer(this.interval);
            this.timer.Elapsed += Timer_Elapsed;
            this.timer.Start();  
        }
        public void Stop()
        {
            if (this.timer != null)
            {
                this.timer.Stop();
                this.timer.Elapsed -= Timer_Elapsed;
                this.timer = null;
            }
        }
        private int inTimer = 0;//防止计时器事件重入
        private int count = 0;
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Interlocked.Exchange(ref this.inTimer, 1) == 0)
            {

                if (this.beginTime != null && DateTime.Now < this.beginTime) return;
                if (times > 0 && count >= times)
                {
                    if (this.timeSpan != null)
                    {
                        if (this.beginTime != null) this.beginTime = ((DateTime)this.beginTime).Add((TimeSpan)this.timeSpan);
                    }
                    else
                    {
                        this.Stop();
                    }
                    return;
                }
                if (this.endTime != null && DateTime.Now > this.endTime)
                {
                    if (this.timeSpan != null)
                    {
                        if (this.beginTime != null) this.beginTime = ((DateTime)this.beginTime).Add((TimeSpan)this.timeSpan);
                        if (this.endTime != null) this.endTime = ((DateTime)this.endTime).Add((TimeSpan)this.timeSpan);
                    }
                    else
                    {
                        this.Stop();
                    }
                    return;
                }
                this.run();
                Interlocked.Exchange(ref this.inTimer, 0);
            }
        }
    }
}
