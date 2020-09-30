namespace Project
{
    public enum Type
    {
        player,
        enemy,
        item
    }

    public interface IMapObject
    {
        Type type { get; set; }
    }
}