using System;
using System.Threading;
namespace HuaQuant.JobSchedule
{
    public abstract class Job : IJob
    {
        private string name;
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        private bool showDetail = false;
        public bool ShowDetail
        {
            get { return this.showDetail; }
            set { this.showDetail = value; }
        }
        private bool finished = false;
        public bool IsFinished => this.finished;
        private bool running = false;
        public bool IsRunning => this.running;
        private int times = 0;
        public int Times => this.times;

        private Job[] needJobs = null;
        public Job(string name, Job[] needJobs = null, bool showDetail = false)
        {
            this.name = name;
            this.needJobs = needJobs;
            this.showDetail = showDetail;
        }

        public void Execute()
        {
            finished = false;
            running = true;
            if (showDetail) Console.WriteLine("在时间{0},开始作业<{1}>的执行...", DateTime.Now, name);
            if (needJobs != null)
            {
                bool canDo = true;
                foreach (IJob job in needJobs)
                {
                    if (!job.IsFinished)
                    {
                        canDo = false;
                        break;
                    }
                }
                if (!canDo)
                {
                    if (showDetail) Console.WriteLine("因先行作业没有完成，作业<{0}>无法启动。", name);
                    running = false;
                    return;
                }
            }
            try
            {
                bool ret = doJob();
                if (ret)
                {
                    finished = true;
                    Interlocked.Increment(ref times);
                    if (showDetail) Console.WriteLine("在时间{0},作业<{1}>顺利完成。", DateTime.Now, name);
                }
                else
                {
                    if (showDetail) Console.WriteLine("在时间{0},作业<{1}>未能正常完成。", DateTime.Now, name);
                }
            } catch (Exception ex)
            {
                Console.WriteLine("在时间{0},作业<{1}>发生异常:{2}", DateTime.Now, name, ex.Message);
            }
            finally
            {
                running = false;
            }
        }

        protected virtual bool doJob() { return true; }
    }
}
