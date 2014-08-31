using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Festify.Model;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Fields;
using System;

namespace Festify.Logic
{
    public class Device
    {
        private const string ThisIndividual = "Festify.Individual.this";
        private const string ConferenceId = "{672F91DE-EB0C-4A1F-8982-7C99ED625E3E}";

        private Community _community;
        private Independent<Individual> _individual = new Independent<Individual>(
            Individual.GetNullInstance());
        private Independent<Conference> _conference = new Independent<Conference>(
            Conference.GetNullInstance());

        public Device(IStorageStrategy storage)
        {
            _community = new Community(storage);
            _community.Register<CorrespondenceModel>();
        }

        public Community Community
        {
            get { return _community; }
        }

        public Individual Individual
        {
            get
            {
                lock (this)
                {
                    return _individual;
                }
            }
            private set
            {
                lock (this)
                {
                    _individual.Value = value;
                }
            }
        }

        public Conference Conference
        {
            get
            {
                lock (this)
                {
                    return _conference;
                }
            }
            private set
            {
                lock (this)
                {
                    _conference.Value = value;
                }
            }
        }

        public void CreateIndividual()
        {
            Community.Perform(async delegate
            {
                var conference = await Community.AddFactAsync(new Conference(ConferenceId));
                await CreateTestData(conference);
                Conference = conference;

                var individual = await Community.LoadFactAsync<Individual>(ThisIndividual);
                if (individual == null)
                {
                    individual = await Community.AddFactAsync(new Individual());
                    await Community.SetFactAsync(ThisIndividual, individual);
                }
                Individual = individual;
            });
        }

        public void CreateIndividualDesignData()
        {
            Community.Perform(async delegate
            {
                var individual = await Community.AddFactAsync(new Individual());
                Individual = individual;

                var conference = await Community.AddFactAsync(new Conference(ConferenceId));
                await CreateTestData(conference);
                Conference = conference;
            });
        }

        public void Subscribe()
        {
            Community.Subscribe(() => Individual);
        }

        private async Task CreateTestData(Conference conference)
        {
            ICommunity community = conference.Community;

            conference.Name = "Dallas TechFest 2014";

            var day = await community.AddFactAsync(new Day(conference,
                new DateTime(2014, 10, 10, 0, 0, 0, DateTimeKind.Utc)));
            var morning = await community.AddFactAsync(new Time(day,
                new DateTime(2014, 10, 10, 9, 0, 0).ToUniversalTime()));
            await community.AddFactAsync(new Time(day,
                new DateTime(2014, 10, 10, 10, 30, 0).ToUniversalTime()));
            var noon = await community.AddFactAsync(new Time(day,
                new DateTime(2014, 10, 10, 12, 0, 0).ToUniversalTime()));
            var nap = await community.AddFactAsync(new Time(day,
                new DateTime(2014, 10, 10, 13, 0, 0).ToUniversalTime()));
            await community.AddFactAsync(new Time(day,
                new DateTime(2014, 10, 10, 14, 30, 0).ToUniversalTime()));
            await community.AddFactAsync(new Time(day,
                new DateTime(2014, 10, 10, 16, 0, 0).ToUniversalTime()));

            var room = await community.AddFactAsync(new Room(conference));
            room.RoomNumber = "Euclid";

            var morningRoom = await community.AddFactAsync(new Place(morning, room));

            var qed = await community.AddFactAsync(new Speaker(conference));
            qed.ImageUrl = "http://qedcode.com/extras/Perry_Headshot_Medium.jpg";
            qed.Name = "Michael L Perry";

            var session = await community.AddFactAsync(new Session(conference, qed, null));
            session.Name = "Modeling Settlers of Catan with Degrees of Freedom";

            await community.AddFactAsync(new SessionPlace(session, morningRoom, Enumerable.Empty<SessionPlace>()));
        }
    }
}
