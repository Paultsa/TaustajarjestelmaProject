namespace Project
{
    public interface ICharacter
    {
        string id { get; set; }
        string name { get; set; }
        int damage { get; set; }
        int health { get; set; }
    }
}