namespace Project
{
    public class Player : ICharacter
    {
        public string id { get; set; }
        public string name { get; set; }
        public int damage { get; set; }
        public int score { get; set; }
        public int level { get; set; }
        public DateTime creationTime { get; set; }
        public List<Item> list_items { get; set; } = new List<Item>();

        public static implicit operator Task<object>(Player v)
        {
            throw new NotImplementedException();
        }
    }
}