using System;
using System.Collections.Generic;
using System.Text;
using Festify.Model;
using Festify.Dependency;
using System.Linq;
using Xamarin.Forms;

namespace Festify.ViewModels.TimeSlot
{
    public class SessionHeader : ViewModelBase
    {
        private readonly SessionPlace _sessionPlace;
        private readonly Individual _individual;

        public SessionHeader(SessionPlace sessionPlace, Individual individual)
        {
            _sessionPlace = sessionPlace;
            _individual = individual;
        }

        public Session Session
        {
            get { return _sessionPlace.Session; }
        }

        public ImageSource Image
        {
            get { return Get(() => ImageSourceFrom(_sessionPlace.Session.Speaker.ImageUrl)); }
        }

        public string Name
        {
            get { return Get(() => (IsLiked ? "<3 " : "") + _sessionPlace.Session.Name); }
        }

        public Color TextColor
        {
            get { return Get(() => IsLiked ? Color.Accent : Color.Default); }
        }

        public string Speaker
        {
            get { return Get(() => _sessionPlace.Session.Speaker.Name); }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var that = obj as SessionHeader;
            if (that == null)
                return false;

            return Object.Equals(this._sessionPlace, that._sessionPlace);
        }

        public override int GetHashCode()
        {
            return _sessionPlace.GetHashCode();
        }

        private bool IsLiked
        {
            get
            {
                return _individual.LikedSessions.Any(s => s.Session == _sessionPlace.Session);
            }
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
