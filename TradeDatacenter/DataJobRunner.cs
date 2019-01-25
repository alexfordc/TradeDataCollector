using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
namespace TradeDatacenter
{
    public class DataJobRunner
    {
        private int interval=1000;
        private DateTime? beginTime=null;
        private DateTime? endTime=null;
        private BaseDataJob job;
        private int times = 0;
        private System.Timers.Timer timer = null;
        public DataJobRunner(BaseDataJob job, int interval=1000,DateTime? beginTime = null, DateTime? endTime = null)
        {
            this.job = job;
            this.interval = interval;
            this.beginTime = beginTime;
            this.endTime = endTime;
        }
        public DataJobRunner(BaseDataJob job, int interval=1000, int times = 1, DateTime? beginTime = null)
        {
            this.job = job;
            this.interval = interval;
            this.beginTime = beginTime;
            this.times = times;
        }
        private void run()
        {
            this.job.Execute();
        }
        public void Start()
        {
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
                if (times > 0 && count > times) return;
                if (this.endTime != null && DateTime.Now > this.endTime) return;
                this.run();
                if (times > 0) count++;
                Interlocked.Exchange(ref this.inTimer, 0);
            }
        }
    }
}
