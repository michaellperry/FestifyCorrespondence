using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SQLite;

namespace Correspondence.MobileStorage
{
    public class NamedFactRecord
    {
        [Indexed(Unique=true)]
        public string Name { get; set; }
        public long FactID { get; set; }
    }
}