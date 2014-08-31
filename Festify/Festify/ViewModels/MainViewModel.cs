using Festify.Dependency;
using Festify.Model;
using Festify.Synchronization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Festify.ViewModels
{
    public class MainViewModel
    {
        private readonly SynchronizationService _synchronizationService;

        public MainViewModel(SynchronizationService synchronizationService)
        {
            _synchronizationService = synchronizationService;
        }

        public string Title
        {
            get { return _synchronizationService.Device.Conference.Name; }
        }

        public IEnumerable<DayHeader> Days
        {
            get
            {
                return
                    from day in _synchronizationService.Device.Conference.Days
                    orderby day.ConferenceDate
                    select new DayHeader(day, _synchronizationService.Individual);
            }
        }
    }
}
