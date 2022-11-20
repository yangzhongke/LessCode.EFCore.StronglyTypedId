using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    [HasStronglyTypedId]
    internal class Dog
    {
        public DogId Id { get; set; }
    }
}
