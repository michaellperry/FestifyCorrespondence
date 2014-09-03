using Festify.Dependency;
using Festify.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Festify.ViewModels.Detail
{
    public class DetailViewModel : ViewModelBase
    {
        private readonly Session _session;
        private readonly Individual _individual;
        
        public DetailViewModel(Session session, Individual individual)
        {
            _session = session;
            _individual = individual;
        }

        public string Name
        {
            get { return Get(() => _session.Name.Value); }
        }

        public string Image
        {
            get { return Get(() => _session.Speaker.ImageUrl.Value); }
        }

        public string Time
        {
            get
            {
                return Get(() => _session.CurrentSessionPlaces
                    .Select(sp => sp.Place.PlaceTime.Start.ToLocalTime().ToShortTimeString())
                    .FirstOrDefault());
            }
        }

        public string Speaker
        {
            get { return Get(() => _session.Speaker.Name.Value); }
        }

        public string Room
        {
            get
            {
                return Get(() => _session.CurrentSessionPlaces
                    .Select(sp => sp.Place.Room.RoomNumber.Value)
                    .FirstOrDefault());
            }
        }
    }
}
