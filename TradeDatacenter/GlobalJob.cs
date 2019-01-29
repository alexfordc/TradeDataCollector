using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeDataCollector;
using System.Reflection;
using TradeDataAccess;

namespace TradeDatacenter
{
    public class GlobalJob : IJob
    {
        private GMCollector gmc = new GMCollector();
        private Config config;
        private List<JobRunner> jRunners = new List<JobRunner>();
        public List<JobRunner> JobRunners
        {
            get { return this.jRunners; }
        }
        public GlobalJob(Config config)
        {
            this.config = config;
            Type type = this.GetType();
        }
        public bool Execute()
        {
            DateTime curDay = DateTime.Today;
            Console.WriteLine("当前日期:{0}", curDay.ToLongDateString());
            DateTime tradeDay = gmc.GetNextTradingDate("SHSE", curDay.AddDays(-1));
            if (curDay == tradeDay)
            {
                List<Instrument> insts = new List<Instrument>();
                insts.AddRange(gmc.GetInstruments("SHSE", "stock"));
                insts.AddRange(gmc.GetInstruments("SZSE", "stock"));
                TradeDataAccessor.StoreInstruments(insts);
                List<string> symbols = insts.Where(e=>e.IsSuspended==false).Select(e => e.Symbol).ToList();
                Console.WriteLine("今日开市证券：{0}只",symbols.Count);
                foreach (DataJobConfig dataJobConfig in config.DataJobConfigs)
                {
                    int i = 0;
                    float weightTotal = 0;
                    foreach (DataCollector dataCollector in dataJobConfig.DataCollectors)
                    {
                        weightTotal += dataCollector.Weight;
                        int count =(int)Math.Round(symbols.Count * weightTotal)-i;
                        object[] parameters = new object[] { dataCollector.MothedName, dataCollector.ClassName, symbols.GetRange(i, count) , curDay };    
                        i = i + count;
                        Type type = Type.GetType(dataJobConfig.ClassName, (aName) => Assembly.LoadFrom(aName.Name),
            (assem, name, ignore) => assem == null ? Type.GetType(name, false, ignore) : assem.GetType(name, false, ignore));
                        IJob job = (IJob)Activator.CreateInstance(type, parameters);
                        JobRunner jRunner;
                        DateTime? beginTime,endTime;
                        if (dataJobConfig.BeginTime != null) beginTime=DateTime.Parse(dataJobConfig.BeginTime);
                        else beginTime = null;
                        if (dataJobConfig.EndTime != null) endTime = DateTime.Parse(dataJobConfig.EndTime);
                        else endTime = null;
                        TimeSpan? timeSpan;
                        if (dataJobConfig.TimeSpan != null) timeSpan = TimeSpan.Parse(dataJobConfig.TimeSpan);
                        else timeSpan = null;
                        if (dataJobConfig.Times > 0) {
                            jRunner=new JobRunner(dataJobConfig.CallInterval, dataJobConfig.Times, beginTime, timeSpan);
                        }else
                        {
                            jRunner = new JobRunner(dataJobConfig.CallInterval, beginTime, endTime, timeSpan);
                        }
                        jRunner.AddJob(job);
                        this.jRunners.Add(jRunner);
                    }
                }
                foreach (JobRunner jRunner in this.jRunners) jRunner.Start();
            }
            else
            {
                Console.WriteLine("今天不是交易日。");
            }
            return true;
        }
        public void StopAllJobs()
        {
            foreach(JobRunner jRunner in this.jRunners)
            {
                jRunner.Stop();
            }
            this.jRunners.RemoveAll(e=> { return true; });
        }
    }
}
