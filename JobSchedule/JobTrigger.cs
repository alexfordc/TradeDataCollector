using System;

namespace HuaQuant.JobSchedule
{
    public class JobTrigger
    {
        private DateTime? beginTime = null;
        private DateTime? endTime = null;
        private TimeSpan? timeInterval = null;
        private int timesLimit = 0;
        private DateTime? nextTriggerTime = null;

        private bool expired = false;
        public bool Expired
        {
            get { return this.expired; }
        }

        private bool intervalBaseOnBeginTime = false;
        public bool IntervalBaseOnBeginTime
        {
            get { return this.intervalBaseOnBeginTime; }
            set { this.intervalBaseOnBeginTime = value; }
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
            if (timeInterval == null) return true;
            if (nextTriggerTime == null)
            {
                if (intervalBaseOnBeginTime) nextTriggerTime = ((DateTime)beginTime).Add((TimeSpan)timeInterval);
                else nextTriggerTime = ((DateTime)time).Add((TimeSpan)timeInterval);
                return true;
            }
            if (time >= nextTriggerTime)
            {
                nextTriggerTime = ((DateTime)time).Add((TimeSpan)timeInterval);
                return true;
            }else return false;
        }
    }
}
