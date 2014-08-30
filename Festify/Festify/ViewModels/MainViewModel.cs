using Festify.Dependency;
using Festify.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Festify.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly Conference _conference;
        private readonly Individual _individual;

        public MainViewModel(Conference conference, Individual individual)
        {
            _conference = conference;
            _individual = individual;
        }

        public IEnumerable<TimeHeader> Times
        {
            get
            {
                return
                    from day in _conference.Days
                    from time in day.Times
                    orderby time.Start
                    select new TimeHeader(time, _individual);
            }
        }
    }
}
