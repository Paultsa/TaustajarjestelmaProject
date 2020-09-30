using System;
using System.Collections.Generic;

namespace Project
{
    public class Map
    {
        List<List<MapTile>> tiles { get; set; }
        Guid id { get; set; }
    }
}