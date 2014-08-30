using Festify.Dependency;
using Festify.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Festify.ViewModels
{
    public class MainViewModel
    {
        private readonly Conference _conference;
        private readonly Individual _individual;

        public MainViewModel(Conference conference, Individual individual)
        {
            _conference = conference;
            _individual = individual;
        }

        public IEnumerable<DayHeader> Days
        {
            get
            {
                return
                    from day in _conference.Days
                    orderby day.ConferenceDate
                    select new DayHeader(day, _individual);
            }
        }
    }
}
