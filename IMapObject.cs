using System;
using System.ComponentModel.DataAnnotations;

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

        [EnumDataType(typeof(Type))]
        Type type { get; set; }
    }
}