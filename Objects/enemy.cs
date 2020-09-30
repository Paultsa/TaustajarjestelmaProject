namespace Project
{
    public class Enemy : ICharacter
    {
        public string id { get; set; }
        public string name { get; set; }
        public int damage { get; set; }
        public int health { get; set; }

        public static implicit operator Task<object>(Enemy v)
        {
            throw new NotImplementedException();
        }
    }
}