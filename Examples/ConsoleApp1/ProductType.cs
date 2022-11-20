using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture;

namespace ConsoleApp1
{
    public sealed partial class ProductType : IEnum<string>
    {
        public static readonly ProductType Groceries = new("Groceries");
        public static readonly ProductType Housewares = new("Housewares");
    }
}
