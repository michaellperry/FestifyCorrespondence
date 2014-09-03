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

        public string Title
        {
            get
            {
                return Get(delegate()
                {
                    var sessionPlace = SessionPlace;
                    var sessionName = sessionPlace == null
                        ? "Breakout Session"
                        : sessionPlace.Session.Name.Value;
                    return String.Format("{0}: {1}",
                        _time.Start.ToLocalTime().ToShortTimeString(),
                        sessionName);
                });
            }
        }

        public string RoomNumber
        {
            get
            {
                return Get(delegate()
                {
                    var sessionPlace = SessionPlace;
                    return sessionPlace == null
                        ? "Tap to select"
                        : sessionPlace.Place.Room.RoomNumber.Value;
                });
            }
        }

        public string Image
        {
            get
            {
                return Get(delegate()
                {
                    var sessionPlace = SessionPlace;
                    return sessionPlace == null
                        ? "https://jobs.thejobnetwork.com/Content/CandidateNet/Images/QuestionMark_IconTransparent.png"
                        : sessionPlace.Session.Speaker.ImageUrl.Value;
                });
            }
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

        private SessionPlace SessionPlace
        {
            get
            {
                var sessionPlaces = _time.AvailableSessions;
                if (sessionPlaces.Count() == 1)
                    return sessionPlaces.Single();
                else
                    return null;
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
