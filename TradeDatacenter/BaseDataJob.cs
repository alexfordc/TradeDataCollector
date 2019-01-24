using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace TradeDatacenter
{
    public class BaseDataJob
    {
        private Type type;
        private MethodInfo method;
        private object obj;
        private object[] parameters;
        
        public BaseDataJob(string methodName, string className, object[] parameters)
        {
            this.type = Type.GetType(className);
            this.method = this.type.GetMethod(methodName);
            this.obj = Activator.CreateInstance(this.type);
            this.parameters = parameters;
        }
        public virtual object Do()
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance;
            return this.method.Invoke(this.obj, flag, Type.DefaultBinder, this.parameters, null);
        }
    }
}
