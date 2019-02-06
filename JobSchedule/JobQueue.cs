using System;
using System.Collections.Generic;
using System.Threading;
namespace HuaQuant.JobSchedule
{
    public class JobQueue : List<Job>, IJob
    {
        public bool IsFinished{
            get {
                bool finished = true;
                foreach (IJob job in this)
                {
                    if (!job.IsFinished)
                    {
                        finished = false;
                        break;
                    }
                }
                return finished;
            }
        }
        
        public bool IsRunning
        {
            get
            {
                bool running = false;
                foreach (IJob job in this)
                {
                    if (job.IsRunning)
                    {
                        running = true;
                        break;
                    }
                }
                return running;
            }
        }
        private int times = 0;
        public int Times => this.times;

        public void Execute()
        {
            foreach (IJob job in this)
            {
                job.Execute();
            }
            if (this.IsFinished) Interlocked.Increment(ref times);
        }
    }
}
