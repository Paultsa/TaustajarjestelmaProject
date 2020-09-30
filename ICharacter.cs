using System;

namespace Project
{
    public interface ICharacter : IMapObject
    {
        string name { get; set; }
        int damage { get; set; }
        int health { get; set; }
    }
}