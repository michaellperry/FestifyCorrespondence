using Festify.Dependency;
using Festify.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Collections;
using UpdateControls.Fields;

namespace Festify.ViewModels.Main
{
    public class TimeHeader : ViewModelBase
    {
        private readonly Time _time;
        private readonly Individual _individual;

        private DependentList<SessionPlace> _sessionPlaces;

        public TimeHeader(Time time, Individual individual)
        {
            _time = time;
            _individual = individual;

            _sessionPlaces = new DependentList<SessionPlace>(() =>
                _time.AvailableSessions.Where(IsLiked));
        }

        public Time Time
        {
            get { return _time; }
        }

        public string Title
        {
            get
            {
                return Get(delegate()
                {
                    var sessionName = _sessionPlaces.Count() != 1
                        ? "Breakout Session"
                        : _sessionPlaces.Single().Session.Name.Value;
                    return String.Format("{0}: {1}",
                        _time.Start.AddHours(-5).ToShortTimeString(),
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
                    return
                        _sessionPlaces.Count() == 0
                            ? "Tap to select" :
                        _sessionPlaces.Count() > 1
                            ? "More than one selected"
                            : _sessionPlaces.Single().Place.Room.RoomNumber.Value;
                });
            }
        }

        public string Image
        {
            get
            {
                return Get(delegate()
                {
                    return _sessionPlaces.Count() != 1
                        ? "https://jobs.thejobnetwork.com/Content/CandidateNet/Images/QuestionMark_IconTransparent.png"
                        : _sessionPlaces.Single().Session.Speaker.ImageUrl.Value;
                });
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var that = obj as TimeHeader;
            if (that == null)
                return false;

            return
                Object.Equals(this._time, that._time) &&
                Object.Equals(this._individual, that._individual);
        }

        public override int GetHashCode()
        {
            return _time.GetHashCode() + _individual.GetHashCode();
        }


        private bool IsLiked(SessionPlace sessionPlace)
        {
            return _individual.LikedSessions.Any(delegate(LikeSession s)
            {
                Session likedSession = s.Session;
                Session listedSession = sessionPlace.Session;
                bool same = Object.Equals(likedSession, listedSession);
                return same;
            });
        }
    }
}
