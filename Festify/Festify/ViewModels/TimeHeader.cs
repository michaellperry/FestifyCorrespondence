using Festify.Dependency;
using Festify.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Festify.ViewModels
{
    public class TimeHeader : ViewModelBase
    {
        private readonly Time _time;
        private readonly Individual _individual;

        public TimeHeader(Time time, Individual individual)
        {
            _time = time;
            _individual = individual;
        }

        public Time Time
        {
            get { return _time; }
        }

        public string Label
        {
            get { return _time.Start.ToLocalTime().ToShortTimeString(); }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var that = obj as TimeHeader;
            if (that == null)
                return false;

            return Object.Equals(this._time, that._time);
        }

        public override int GetHashCode()
        {
            return _time.GetHashCode();
        }
    }
}
