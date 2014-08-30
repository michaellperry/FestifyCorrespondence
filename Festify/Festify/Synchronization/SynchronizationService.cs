using System.Diagnostics;
using Festify.Model;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.BinaryHTTPClient;
using UpdateControls.Correspondence.Memory;
using Festify.Logic;

namespace Festify.Synchronization
{
    public class SynchronizationService
    {
        private Device _device;

        private MemoryStorageStrategy _storage;

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
            Debug.WriteLine("Message received");
        }

        void Community_FactReceived()
        {
            Debug.WriteLine("Fact received");
        }

        void Community_FactAdded(CorrespondenceFact obj)
        {
            Debug.WriteLine("Fact added");
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
