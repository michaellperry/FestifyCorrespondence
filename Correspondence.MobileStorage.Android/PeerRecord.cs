using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SQLite;

namespace Correspondence.MobileStorage
{
    public class PeerRecord
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Indexed(Name = "ProtocolPeer", Order = 1, Unique = true)]
        public string ProtocolName { get; set; }
        [Indexed(Name = "ProtocolPeer", Order = 2, Unique = true)]
        public string PeerName { get; set; }
    }
}