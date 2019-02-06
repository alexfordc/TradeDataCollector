using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaQuant.JobSchedule
{
    public interface IJob
    {
        void Execute();
        bool IsFinished { get; }
        bool IsRunning { get; }
        int Times { get; }
    }
}
