namespace Project
{
    public interface ICharacter : IMapObject
    {
        string id { get; set; }
        string name { get; set; }
        int damage { get; set; }
        int health { get; set; }
    }
}