using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project
{
    public enum Direction
    {
        up,
        right,
        down,
        left
    };
    public class Player : ICharacter
    {
        public string id { get; set; }
        public string name { get; set; }
        public int damage { get; set; }
        public int health { get; set; }
        public int score { get; set; }
        public int level { get; set; }
        public DateTime creationTime { get; set; }
        public List<Item> list_items { get; set; } = new List<Item>();
        public Type type { get; set; }

        public static implicit operator Task<object>(Player v)
        {
            throw new NotImplementedException();
        }
    }
}