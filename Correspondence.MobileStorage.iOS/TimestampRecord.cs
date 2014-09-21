using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SQLite;

namespace Correspondence.MobileStorage
{
    public class TimestampRecord
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Indexed(Name = "PeerPivot", Order = 1, Unique = true)]
        public int PeerID { get; set; }
        [Indexed(Name = "PeerPivot", Order = 2, Unique = true)]
        public long PivotID { get; set; }

        public long DatabaseID { get; set; }
        public long FactID { get; set; }
    }
}