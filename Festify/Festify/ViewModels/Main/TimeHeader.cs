using Festify.Dependency;
using Festify.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Festify.ViewModels.Main
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

        public string Title
        {
            get { return "Breakout Session"; }
        }

        public string Speaker
        {
            get { return "Tap to select"; }
        }

        public string Image
        {
            get { return "http://img4.wikia.nocookie.net/__cb20130830125537/smallville/images/thumb/0/06/Question-mark.jpg/480px-Question-mark.jpg"; }
        }

        public ICommand Select
        {
            get
            {
                return MakeCommand
                    .Do(delegate
                    {

                    });
            }
        }

        public IEnumerable<SessionHeader> Sessions
        {
            get
            {
                return
                    from sessionPlace in _time.AvailableSessions
                    select new SessionHeader(sessionPlace, _individual);
            }
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
