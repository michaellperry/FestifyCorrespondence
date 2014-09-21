using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Correspondence.MobileStorage
{
    public class PredecessorRecord
    {
        public long FactID { get; set; }
        public int RoleID { get; set; }
        public long PredecessorFactID { get; set; }
        public bool IsPivot { get; set; }
    }
}