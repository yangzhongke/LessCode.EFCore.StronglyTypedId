using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example0
{
    [HasStronglyTypedId]
    class Person
    {
        public PersonId Id { get; set; }
        public string Name { get; set; }
    }

}
