using System;
using System.Collections.Generic;
using System.Text;
using Festify.Model;
using Festify.Dependency;

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

        public string Image
        {
            get { return Get(() => _sessionPlace.Session.Speaker.ImageUrl); }
        }

        public string Name
        {
            get { return Get(() => _sessionPlace.Session.Name); }
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
    }
}
