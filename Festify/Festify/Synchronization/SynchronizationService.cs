using System.Diagnostics;
using Festify.Model;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.BinaryHTTPClient;
using UpdateControls.Correspondence.Memory;

namespace Festify.Synchronization
{
    public class SynchronizationService
    {
        private SignDevice _device;
        private DateSelectionModel _dateSelectionModel;

        private MemoryStorageStrategy _storage;

        public void Initialize()
        {
            _dateSelectionModel = new DateSelectionModel();
            _storage = new MemoryStorageStrategy();
            _device = new SignDevice(_storage, _dateSelectionModel);

            var http = new HTTPConfigurationProvider();
            var communication = new BinaryHTTPAsynchronousCommunicationStrategy(http);
            _device.Community.AddAsynchronousCommunicationStrategy(communication);

            _device.Community.FactAdded += Community_FactAdded;
            _device.Community.FactReceived += Community_FactReceived;
            communication.MessageReceived += communication_MessageReceived;

            _device.Subscribe();

            CreateInstallation();

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

        public SignDevice Device
        {
            get { return _device; }
        }

        public Community Community
        {
            get { return _device.Community; }
        }

        public Installation Installation
        {
            get { return _device.Installation; }
        }

        private void CreateInstallation()
        {
            _device.CreateInstallation();
        }
    }
}
