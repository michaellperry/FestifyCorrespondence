using Festify.Dependency;
using Festify.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Collections;
using UpdateControls.Fields;
using Xamarin.Forms;

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
                    if (_sessionPlaces.Count() == 0)
                        return "Tap to select";
                    else if (_sessionPlaces.Count() > 1)
                        return "More than one selected";
                    else
                        return _sessionPlaces.Single().Place.Room.RoomNumber.Value;
                });
            }
        }

        public ImageSource Image
        {
            get
            {
                return Get(delegate()
                {
                    return _sessionPlaces.Count() != 1
                        ? ImageSource.FromUri(new Uri("http://icons.iconarchive.com/icons/gordon-irving/iWork-10/512/keynote-off-icon.png"))
                        : ImageSource.FromUri(new Uri(_sessionPlaces.Single().Session.Speaker.ImageUrl.Value));
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
                bool similar = likedSession.GetHashCode() == listedSession.GetHashCode();
                if (!similar)
                    return false;
                bool same = Object.Equals(likedSession, listedSession);
                return same;
            });
        }

        protected override void OnDisposed()
        {
            var hash = _time.GetHashCode();
            System.Diagnostics.Debug.WriteLine(String.Format("Disposed {0}", hash));
        }
    }
}
