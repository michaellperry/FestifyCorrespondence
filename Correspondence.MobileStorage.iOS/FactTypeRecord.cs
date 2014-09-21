using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SQLite;

namespace Correspondence.MobileStorage
{
    public class FactTypeRecord
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Indexed(Name = "NameVersion", Order = 1, Unique = true)]
        public string Name { get; set; }
        [Indexed(Name = "NameVersion", Order = 2, Unique = true)]
        public int Version { get; set; }
    }
}