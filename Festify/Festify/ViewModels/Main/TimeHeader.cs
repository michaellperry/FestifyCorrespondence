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
        private readonly string _title;
        private readonly string _roomNumber;
        private readonly ImageSource _image;

        public TimeHeader(Time time, Individual individual)
        {
            _time = time;
            _individual = individual;

            var sessionPlaces = _time.AvailableSessions.Where(IsLiked).ToList();
            var sessionName = sessionPlaces.Count() != 1
                ? "Breakout Session"
                : sessionPlaces.Single().Session.Name.Value;
            _title = String.Format("{0}: {1}",
                _time.Start.AddHours(-5).ToShortTimeString(),
                sessionName);

            if (sessionPlaces.Count() == 0)
                _roomNumber = "Tap to select";
            else if (sessionPlaces.Count() > 1)
                _roomNumber = "More than one selected";
            else
                _roomNumber = sessionPlaces.Single().Place.Room.RoomNumber.Value;

            _image = sessionPlaces.Count() != 1
                ? ImageSourceFrom("http://icons.iconarchive.com/icons/gordon-irving/iWork-10/512/keynote-off-icon.png")
                : ImageSourceFrom(sessionPlaces.Single().Session.Speaker.ImageUrl.Value);
        }

        public Time Time
        {
            get { return _time; }
        }

        public string Title
        {
            get { return _title; }
        }

        public string RoomNumber
        {
            get { return _roomNumber; }
        }

        public ImageSource Image
        {
            get { return _image; }
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
                Object.Equals(this._individual, that._individual) &&
                Object.Equals(this._title, that._title) &&
                Object.Equals(this._roomNumber, that._roomNumber) &&
                Object.Equals(this._image, that._image);
        }

        public override int GetHashCode()
        {
            return _time.GetHashCode() + _individual.GetHashCode() + _title.GetHashCode() + _roomNumber.GetHashCode() + _image.GetHashCode();
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

        private ImageSource ImageSourceFrom(string url)
        {
            if (String.IsNullOrEmpty(url))
                return null;
            else
                return ImageSource.FromUri(new Uri(url));
        }
    }
}
