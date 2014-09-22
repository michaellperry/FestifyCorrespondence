using Festify.Dependency;
using Festify.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

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

        public ImageSource Image
        {
            get { return Get(() => ImageSourceFrom(_session.Speaker.ImageUrl.Value)); }
        }

        public string Time
        {
            get
            {
                return Get(() => _session.CurrentSessionPlaces
                    .Select(sp => sp.Place.PlaceTime.Start.AddHours(-5).ToShortTimeString())
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

        public string Description
        {
            get { return Get(() => _session.Description.Value.JoinSegments()); }
        }

        public string Bio
        {
            get { return Get(() => _session.Speaker.Bio.Value.JoinSegments()); }
        }

        public string LikeLabel
        {
            get
            {
                return Get(() => _individual.LikedSessions.Any(s => s.Session == _session)
                    ? "Unlike"
                    : "Like");
            }
        }

        public ICommand Like
        {
            get
            {
                return MakeCommand
                    .Do(delegate
                    {
                        _individual.Community.Perform(async delegate
                        {
                            var likedSessions = (await _individual.LikedSessions.EnsureAsync())
                                .Where(s => s.Session == _session);

                            if (likedSessions.Any())
                            {
                                foreach (var likedSession in likedSessions)
                                {
                                    await likedSession.Community.AddFactAsync(new UnlikeSession(likedSession));
                                }
                            }
                            else
                            {
                                var attendee = (await _individual.Attendees.EnsureAsync()).FirstOrDefault();
                                if (attendee == null)
                                {
                                    var conference = await _session.Conference.EnsureAsync();
                                    var identifier = Guid.NewGuid().ToString();
                                    attendee = await _individual.Community.AddFactAsync(new Attendee(conference, identifier));
                                    await _individual.Community.AddFactAsync(new IndividualAttendee(_individual, attendee));
                                }

                                await _individual.Community.AddFactAsync(new LikeSession(attendee, _session));
                            }
                        });
                    });
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
