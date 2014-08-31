using Festify.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Festify.ViewModels.Main
{
    public class DayHeader
    {
        private readonly Day _day;
        private readonly Individual _individual;

        public DayHeader(Day day, Individual individual)
        {
            _day = day;
            _individual = individual;
        }

        public string Header
        {
            get { return _day.ConferenceDate.ToShortDateString(); }
        }

        public IEnumerable<TimeHeader> Times
        {
            get
            {
                return
                    from time in _day.Times
                    orderby time.Start
                    select new TimeHeader(time, _individual);
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var that = obj as DayHeader;
            if (that == null)
                return false;

            return Object.Equals(this._day, that._day);
        }

        public override int GetHashCode()
        {
            return _day.GetHashCode();
        }
    }
}
