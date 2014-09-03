using Festify.Synchronization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Festify.ViewModels.Main
{
    public class MainViewModel
    {
        private readonly SynchronizationService _synchronizationService;
        
        public MainViewModel(SynchronizationService synchronizationService)
        {
            _synchronizationService = synchronizationService;
        }

        public string Exception
        {
            get
            {
                var exception = _synchronizationService.Community.LastException;
                if (exception == null)
                    return String.Empty;
                else
                    return exception.Message;
            }
        }

        public string Title
        {
            get { return _synchronizationService.Device.Conference.Name; }
        }

        public IEnumerable<TimeHeader> Times
        {
            get
            {
                return
                    from day in _synchronizationService.Device.Conference.Days
                    from time in day.Times
                    orderby time.Start
                    select new TimeHeader(time, _synchronizationService.Individual);
            }
        }
    }
}
