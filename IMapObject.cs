using System;

namespace Project
{
    public enum Type
    {
        player,
        enemy,
        item
    }

    public interface IMapObject
    {
        string id { get; set; }
        Type type { get; set; }
    }
}