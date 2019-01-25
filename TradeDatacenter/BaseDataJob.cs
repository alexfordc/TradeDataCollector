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
        
        public BaseDataJob(string methodName, string className, IEnumerable<string> symbols)
        {
            this.type = Type.GetType(className);
            this.method = this.type.GetMethod(methodName);
            this.obj = Activator.CreateInstance(this.type);
            this.symbols = symbols;
        }
        protected object invokeMethod(object[] parameters)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance;
            return this.method.Invoke(this.obj, flag, Type.DefaultBinder, parameters, null);
        }
        public abstract object Execute();
    }
}
