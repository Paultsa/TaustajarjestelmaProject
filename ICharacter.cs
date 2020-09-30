namespace Project
{
    public interface ICharacter
    {
        string Id { get; set; }
        string Name { get; set; }
        int Damage { get; set; }
        int X { get; set; }
        int Y { get; set; }
    }
}