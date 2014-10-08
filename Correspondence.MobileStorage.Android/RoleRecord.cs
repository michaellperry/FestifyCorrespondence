using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SQLite;

namespace Correspondence.MobileStorage
{
    public class RoleRecord
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Indexed(Name = "NameDeclaringType", Order = 1, Unique = true)]
        public string Name { get; set; }
        [Indexed(Name = "NameDeclaringType", Order = 2, Unique = true)]
        public int DeclaringTypeID { get; set; }
    }
}