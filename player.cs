namespace GameWebApi
{
    public class Player : ICharacter
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public int Damage { get; set; }
        public int Score { get; set; }
        public int Level { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
        public DateTime CreationTime { get; set; }

        public List<Item> List_items = new List<Item>();

        //public string[] Tags;

        public static implicit operator Task<object>(Player v)
        {
            throw new NotImplementedException();
        }
    }
}