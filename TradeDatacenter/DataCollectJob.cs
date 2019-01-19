using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Timers;
using TradeDataCollector;

namespace TradeDataAccess
{
    public class DataCollectJob
    {
        private string methodName;
        private string className;
        private object[] parameters;
        private int interval;
        private object obj;
        private Type type;
        private MethodInfo method;
        private System.Timers.Timer timer=null;
        public DataCollectJob(string methodName,string className, object[] parameters, int interval=0)
        {
            this.methodName = methodName;
            this.className = className;
            this.parameters = parameters;
            this.interval = interval;
            this.type = Type.GetType(className);
            this.method=this.type.GetMethod(this.methodName);
            this.obj = Activator.CreateInstance(this.type);
        }

        public void doJob()
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance;
            object returnValue = this.method.Invoke(this.obj, flag, Type.DefaultBinder, this.parameters, null);
        }
        public void Start()
        {
            if (this.interval <= 0) this.doJob();
            else 
            {
                this.timer = new System.Timers.Timer(this.interval);
                this.timer.Elapsed += Timer_Elapsed;
                this.timer.Start();
            }
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
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Interlocked.Exchange(ref this.inTimer, 1) == 0)
            {
                this.doJob();
                Interlocked.Exchange(ref this.inTimer, 0);
            }
        }
    }
}
