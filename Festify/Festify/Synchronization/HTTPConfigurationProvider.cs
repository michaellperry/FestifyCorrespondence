using System;
using System.Collections.Generic;
using System.Text;
using UpdateControls.Correspondence.BinaryHTTPClient;

namespace Festify.Synchronization
{
    public class HTTPConfigurationProvider : IHTTPConfigurationProvider
    {
        public HTTPConfiguration Configuration
        {
            get
            {
                string address = "https://api.facetedworlds.com/correspondence_server_web/bin";
                string apiKey = "B6F6EFEADE474A038143B0ECB40CDCB6";
                int timeoutSeconds = 30;
                return new HTTPConfiguration(address, "Festify", apiKey, timeoutSeconds);
            }
        }
    }
}
