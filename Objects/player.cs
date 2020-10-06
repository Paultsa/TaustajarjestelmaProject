using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [ValidateCreationDateAttribute]
        public DateTime creationTime { get; set; }
        public List<Item> list_items { get; set; } = new List<Item>();
        public Type type { get; set; }

        public static implicit operator Task<object>(Player v)
        {
            throw new NotImplementedException();
        }
    }

    public class ValidateCreationDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Player player = (Player)validationContext.ObjectInstance;
            Console.WriteLine("\nValidating player: \"" + player.name + "\" CreationTime: " + player.creationTime);
            if (player.creationTime > DateTime.UtcNow)
            {
                Console.WriteLine("Validation error: CreationTime must be in the past");
                return new ValidationResult("CreationTime must be in the past");
            }
            Console.WriteLine("Player " + player.name + " CreationTime validated");
            return ValidationResult.Success;
        }
    }
}