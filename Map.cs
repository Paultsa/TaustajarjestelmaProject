using System;
using System.Collections.Generic;

namespace Project
{
    public class Map
    {
        public List<List<MapTile>> tiles { get; set; }
        public Guid id { get; set; }
    }
}