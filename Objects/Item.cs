using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;

namespace Project
{

    public class Item
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int damage { get; set; }
    }
}