using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SQLite;

namespace Correspondence.MobileStorage
{
    public class FactRecord
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Indexed(Name = "TypeHashCode", Order = 1)]
        public int FactTypeID { get; set; }
        [Indexed(Name = "TypeHashCode", Order = 2)]
        public int HashCode { get; set; }

        public byte[] Data { get; set; }
    }
}