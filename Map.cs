using System;
using System.Collections.Generic;

namespace Project
{
    public class Map
    {
        public Dictionary<string, int[]> postitions { get; set; }
        public MapTile[][] tiles { get; set; }
        public string id { get; set; }
    }
}