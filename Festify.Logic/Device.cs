using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Festify.Model;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Fields;

namespace Festify.Logic
{
    public class Device
    {
        private const string ThisIndividual = "Festify.Individual.this";
        private static readonly Regex Punctuation = new Regex(@"[{}\-]");

        private Community _community;
        private Independent<Individual> _individual = new Independent<Individual>(
            Individual.GetNullInstance());

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
            });
        }

        public void CreateIndividualDesignData()
        {
            Community.Perform(async delegate
            {
                var individual = await Community.AddFactAsync(new Individual());
                Individual = individual;
            });
        }

        public void Subscribe()
        {
            Community.Subscribe(() => Individual);
        }
    }
}
