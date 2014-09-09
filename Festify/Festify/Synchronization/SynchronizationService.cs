using Festify.Logic;
using Festify.Model;
using System.Diagnostics;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.BinaryHTTPClient;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.Strategy;
using System;
using System.Linq;
using UpdateControls.Correspondence.Mementos;

namespace Festify.Synchronization
{
    public class SynchronizationService
    {
        private Device _device;

        private IStorageStrategy _storage;

        public void Initialize()
        {
            _storage = new MemoryStorageStrategy();
            _device = new Device(_storage);

            var http = new HTTPConfigurationProvider();
            var communication = new BinaryHTTPAsynchronousCommunicationStrategy(http);
            _device.Community.AddAsynchronousCommunicationStrategy(communication);

            _device.Community.FactAdded += Community_FactAdded;
            _device.Community.FactReceived += Community_FactReceived;
            communication.MessageReceived += communication_MessageReceived;

            _device.Subscribe();

            CreateIndividual();

            _device.Community.BeginSending();
            _device.Community.BeginReceiving();
        }

        void communication_MessageReceived(UpdateControls.Correspondence.Mementos.FactTreeMemento obj)
        {
            Debug.WriteLine(String.Format("Message received {0}",
                string.Join(", ",
                    obj.Facts.OfType<IdentifiedFactMemento>().Select(f => f.Memento.FactType.TypeName))));
        }

        void Community_FactReceived()
        {
            Debug.WriteLine("Fact received");
        }

        void Community_FactAdded(CorrespondenceFact obj)
        {
            Debug.WriteLine(String.Format("Fact added {0}", obj.GetType()));
        }

        public Device Device
        {
            get { return _device; }
        }

        public Community Community
        {
            get { return _device.Community; }
        }

        public Individual Individual
        {
            get { return _device.Individual; }
        }

        private void CreateIndividual()
        {
            _device.CreateIndividual();
        }
    }
}
