using Festify.Dependency;
using Festify.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Festify.ViewModels.TimeSlot
{
    public class TimeSlotViewModel : ViewModelBase
    {
        private readonly Time _time;
        private readonly Individual _individual;

        public TimeSlotViewModel(Time time, Individual individual)
        {
            _time = time;
            _individual = individual;
        }

        public string Time
        {
            get { return _time.Start.ToLocalTime().ToShortTimeString(); }
        }

        public IEnumerable<SessionHeader> Sessions
        {
            get
            {
                return
                    from sessionPlace in _time.AvailableSessions
                    orderby sessionPlace.Session.Name.Value
                    select new SessionHeader(sessionPlace, _individual);
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var that = obj as TimeSlotViewModel;
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
