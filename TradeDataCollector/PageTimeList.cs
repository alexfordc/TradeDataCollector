using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeDataCollector
{
    public class PageTimeList:SortedList<int,DateTime>
    {
        public int LastPage
        {
            get {
                if (this.Count > 0) return this.Keys.Last();
                else return 0;
            }
        }
        public int FirstPage
        {
            get
            {
                if (this.Count > 0) return this.Keys.First();
                else return 100;
            }
        }
        public int FindPageByTime(DateTime time)
        {
            if (this.Count <= 0) return -1;
            int left = this.FirstPage;
            int right = this.LastPage;

            while (left < right)
            {
                int mid = (left + right) / 2;
                DateTime cur = this[mid];
                if (time < cur) right = mid - 1;
                else
                {
                    DateTime next = this[mid + 1];
                    if (time < next) return mid;
                    else left = mid + 1;
                }
            }
            if (time < this[left]) return -1;
            else return left;
        }
    }
}
