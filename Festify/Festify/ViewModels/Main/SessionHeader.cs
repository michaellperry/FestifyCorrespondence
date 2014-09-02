using System;
using System.Collections.Generic;
using System.Text;
using Festify.Model;
using System.Windows.Input;
using Festify.Dependency;

namespace Festify.ViewModels.Main
{
    public class SessionHeader
    {
        private readonly SessionPlace _sessionPlace;
        private readonly Individual _individual;

        public SessionHeader(SessionPlace sessionPlace, Individual individual)
        {
            _sessionPlace = sessionPlace;
            _individual = individual;
        }

        public SessionPlace SessionPlace
        {
            get { return _sessionPlace; }
        }

        public Individual Individual
        {
            get { return _individual; }
        }

        public string Image
        {
            get { return _sessionPlace.Session.Speaker.ImageUrl; }
        }

        public string Title
        {
            get { return _sessionPlace.Session.Name; }
        }

        public string Speaker
        {
            get { return _sessionPlace.Session.Speaker.Name; }
        }

        public string Room
        {
            get { return _sessionPlace.Place.Room.RoomNumber; }
        }

        public ICommand Select
        {
            get
            {
                return MakeCommand
                    .Do(() => System.Diagnostics.Debug.WriteLine("Navigate"));
            }
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
