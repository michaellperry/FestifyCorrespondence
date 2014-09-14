using Festify.Dependency;
using Festify.Synchronization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Festify.ViewModels.Main
{
    public class MainViewModel : ViewModelBase
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
                return Get(() =>
                {
                    var exception = _synchronizationService.Community.LastException;
                    if (exception == null)
                        return String.Empty;
                    else
                        return exception.Message;
                });
            }
        }

        public string Name
        {
            get
            {
                return Get(() =>
                    _synchronizationService.Device.Conference.Name.Value);
            }
        }

        public IEnumerable<TimeHeader> Times
        {
            get
            {
                return GetCollection(() =>
                    from day in _synchronizationService.Device.Conference.Days
                    from time in day.Times
                    orderby time.Start
                    select new TimeHeader(time, _synchronizationService.Individual));
            }
        }
    }
}
