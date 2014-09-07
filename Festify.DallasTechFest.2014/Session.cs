using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Festify.DallasTechFest._2014
{
    public class Session
    {
        public string id { get; set; }
        public string title { get; set; }
        public string @abstract { get; set; }
        public string timeSlot { get; set; }
        public string room { get; set; }
        public string speakerName { get; set; }
        public string speakerBio { get; set; }
        public string speakerPhotoUrl { get; set; }
    }
}
