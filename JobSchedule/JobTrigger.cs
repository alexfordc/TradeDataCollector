using System;

namespace HuaQuant.JobSchedule
{
    public class JobTrigger
    {
        private DateTime? beginTime = null;
        private DateTime? endTime = null;
        private TimeSpan? timeInterval = null;
        private int timesLimit = 0;
        private DateTime? lastTriggerTime = null;

        private bool expired = false;
        public bool Expired
        {
            get { return this.expired; }
        }

        public JobTrigger(DateTime? beginTime = null, DateTime? endTime = null,int timesLimit=0, TimeSpan? timeInterval = null)
        {
            this.beginTime = beginTime;
            this.endTime = endTime;
            this.timesLimit = timesLimit;
            this.timeInterval = timeInterval;
        }
        public bool Triggering(DateTime time,int times)
        {
            if (beginTime != null && time < beginTime) return false;
            if ((timesLimit > 0 && times >= timesLimit)|| (endTime != null && time > endTime))
            {
                this.expired = true;
                return false;
            }
            
            if ((lastTriggerTime == null) || (timeInterval == null) || ((DateTime)lastTriggerTime).Add((TimeSpan)timeInterval) <= time)
            {
                lastTriggerTime = time;
                return true;
            }
            else return false;
        }
    }
}
