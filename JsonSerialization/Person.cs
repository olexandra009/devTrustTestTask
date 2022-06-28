using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonSerialization
{ 
    public class Person
    {
        public long Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public long? AddressId { get; set; }
        public virtual Address Address { get; set; }
    }

}
