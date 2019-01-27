using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace TradeDatacenter
{
    public abstract class BaseDataJob
    {
        private Type type;
        private MethodInfo method;
        private object obj;
        protected IEnumerable<string> symbols;
        protected DateTime? dataDate;
        public BaseDataJob(string methodName, string className,IEnumerable<string> symbols, DateTime? dataDate=null)
        {
            this.type = Type.GetType(className, (aName) => Assembly.LoadFrom(aName.Name),
            (assem, name, ignore) => assem == null ? Type.GetType(name, false, ignore) :assem.GetType(name, false, ignore));
            this.method = this.type.GetMethod(methodName);
            this.obj = Activator.CreateInstance(this.type);
            this.symbols = symbols;
            this.dataDate = dataDate;
        }
        protected object invokeMethod(object[] parameters)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance;
            return this.method.Invoke(this.obj, flag, Type.DefaultBinder, parameters, null);
        }
    }
}
