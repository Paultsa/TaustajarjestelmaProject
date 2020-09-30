namespace Project
{
    public interface ICharacter : IMapObjcet
    {
        string id { get; set; }
        string name { get; set; }
        int damage { get; set; }
        int health { get; set; }
        Type type { get; set; }

    }
}