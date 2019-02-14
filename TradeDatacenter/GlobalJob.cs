using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HuaQuant.TradeDataCollector;
using HuaQuant.TradeDataAccess;
using HuaQuant.JobSchedule;
namespace HuaQuant.TradeDatacenter
{
    public class GlobalJob:Job
    {
        private GMCollector gmc = new GMCollector();
        private Config config;
        private JobSchedule.JobSchedule jobSche;

        public GlobalJob(Config config,JobSchedule.JobSchedule jobSche):base("GlobalJob",null,true)
        {
            this.config = config;
            this.jobSche = jobSche;
            Type type = this.GetType();
        }
        protected override bool doJob()
        {
            DateTime curDay = DateTime.Today;
            Console.WriteLine("当前日期:{0}", curDay.ToLongDateString());
            DateTime tradeDay = gmc.GetNextTradingDate("SHSE", curDay.AddDays(-1));
            if (curDay == tradeDay)
            {
                List<Instrument> insts = new List<Instrument>();
                insts.AddRange(gmc.GetInstruments("SHSE", "stock"));
                insts.AddRange(gmc.GetInstruments("SZSE", "stock"));
                TradeDataAccessor.StoreInstruments(insts).Wait();
                List<string> symbols = insts.Where(e=>e.IsSuspended==false).Select(e => e.Symbol).ToList();
                Console.WriteLine("今日开市证券：{0}只",symbols.Count);
                foreach (DataJobConfig dataJobConfig in config.DataJobConfigs)
                {
                    if (dataJobConfig.SubJobs != null && dataJobConfig.SubJobs.Count > 0) {
                        JobQueue jobQueue = new JobQueue();
                        foreach(DataJobConfig subDataJobConfig in dataJobConfig.SubJobs)
                        {
                            int i = 0;
                            float weightTotal = 0;
                            foreach (DataCollector dataCollector in subDataJobConfig.DataCollectors)
                            {
                                weightTotal += dataCollector.Weight;
                                int count = (int)Math.Round(symbols.Count * weightTotal) - i;
                                object[] parameters = new object[] { dataCollector.MothedName, dataCollector.ClassName, symbols.GetRange(i, count), curDay };
                                i = i + count;
                                Type type = Type.GetType(subDataJobConfig.ClassName, (aName) => Assembly.LoadFrom(aName.Name),
                    (assem, name, ignore) => assem == null ? Type.GetType(name, false, ignore) : assem.GetType(name, false, ignore));
                                Job job = (Job)Activator.CreateInstance(type, parameters);
                                
                                jobQueue.Add(job);
                                //Console.WriteLine(job.Name);
                            }
                        }
                        
                        jobSche.Add(jobQueue, buildTrigger(dataJobConfig));
                    }
                    else
                    {
                        int i = 0;
                        float weightTotal = 0;
                        foreach (DataCollector dataCollector in dataJobConfig.DataCollectors)
                        {
                            weightTotal += dataCollector.Weight;
                            int count = (int)Math.Round(symbols.Count * weightTotal) - i;
                            object[] parameters = new object[] { dataCollector.MothedName, dataCollector.ClassName, symbols.GetRange(i, count), curDay };
                            i = i + count;
                            Type type = Type.GetType(dataJobConfig.ClassName, (aName) => Assembly.LoadFrom(aName.Name),
                (assem, name, ignore) => assem == null ? Type.GetType(name, false, ignore) : assem.GetType(name, false, ignore));
                            Job job = (Job)Activator.CreateInstance(type, parameters);

                            //job.ShowDetail = true;
                            jobSche.Add(job, buildTrigger(dataJobConfig));
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("今天不是交易日。");
            }
            return true;
        }

        private JobTrigger buildTrigger(DataJobConfig dataJobConfig)
        {
            DateTime? beginTime, endTime;
            if (dataJobConfig.BeginTime != null) beginTime = DateTime.Parse(dataJobConfig.BeginTime);
            else beginTime = null;
            if (dataJobConfig.EndTime != null) endTime = DateTime.Parse(dataJobConfig.EndTime);
            else endTime = null;
            TimeSpan? timeInterval;
            if (dataJobConfig.TimeInterval != null) timeInterval = TimeSpan.Parse(dataJobConfig.TimeInterval);
            else timeInterval = null;
            JobTrigger trigger = new JobTrigger(beginTime, endTime, dataJobConfig.Times, timeInterval);
            return trigger;
        }
    }
}
