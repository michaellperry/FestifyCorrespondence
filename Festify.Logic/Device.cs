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
                var individual = await Community.LoadFactAsync<Individual>(ThisIndividual);
                if (individual == null)
                {
                    individual = await Community.AddFactAsync(new Individual());
                    await Community.SetFactAsync(ThisIndividual, individual);
                }
                Individual = individual;

                var conference = await Community.AddFactAsync(new Conference(ConferenceId));
                await CreateTestData(conference);
                Conference = conference;
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
            var day = await conference.Community.AddFactAsync(new Day(conference,
                new DateTime(2014, 9, 8, 0, 0, 0, DateTimeKind.Utc)));
            await day.Community.AddFactAsync(new Time(day,
                new DateTime(2014, 9, 8, 9, 0, 0).ToUniversalTime()));
            await day.Community.AddFactAsync(new Time(day,
                new DateTime(2014, 9, 8, 10, 30, 0).ToUniversalTime()));
            await day.Community.AddFactAsync(new Time(day,
                new DateTime(2014, 9, 8, 12, 0, 0).ToUniversalTime()));
            await day.Community.AddFactAsync(new Time(day,
                new DateTime(2014, 9, 8, 13, 0, 0).ToUniversalTime()));
            await day.Community.AddFactAsync(new Time(day,
                new DateTime(2014, 9, 8, 14, 30, 0).ToUniversalTime()));
            await day.Community.AddFactAsync(new Time(day,
                new DateTime(2014, 9, 8, 16, 0, 0).ToUniversalTime()));
        }
    }
}
