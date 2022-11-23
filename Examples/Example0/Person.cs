using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example0
{
    [HasStronglyTypedId]
    class Person:IComparable
    {
        public PersonId Id { get; set; }
        public string Name { get; set; }

        public int CompareTo(object? obj)
        {
            Person p2 = obj as Person;
            if(p2!=null)
            {
                return this.Value.CompareTo(obj.Value)
            }
        }
    }

}
