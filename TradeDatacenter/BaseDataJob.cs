using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HuaQuant.JobSchedule;

namespace HuaQuant.TradeDatacenter
{
    public abstract class BaseDataJob:Job
    {
        protected Type type;
        protected MethodInfo method;
        protected object obj;
        protected IEnumerable<string> symbols;
        protected DateTime? dataDate;
        public BaseDataJob(string name,string methodName, string className,IEnumerable<string> symbols, 
            DateTime? dataDate=null, Job[] needJobs = null):base(name,needJobs)
        {
            
            this.type = Type.GetType(className, (aName) => Assembly.LoadFrom(aName.Name),
            (assem, mName, ignore) => assem == null ? Type.GetType(mName, false, ignore) :assem.GetType(mName, false, ignore));
            this.method = this.type.GetMethod(methodName);
            this.obj = Activator.CreateInstance(this.type);
            this.symbols = symbols;
            this.dataDate = dataDate;
            this.Name = string.Format("{0}[{1}-{2}]",name,type.Name,methodName);
        }
        protected object invokeMethod(object[] parameters)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance;
            return this.method.Invoke(this.obj, flag, Type.DefaultBinder, parameters, null);
        }
    }
}
