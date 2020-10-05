using System;

namespace Project
{
    public class PlayerNotFoundException : Exception
    {

        public string playerId { get; set; }
        public string mapId { get; set; }
        public PlayerNotFoundException(string playerId, string mapId)
        {
            this.playerId = playerId;
            this.mapId = mapId;
            Console.WriteLine("\nPlayerNotFoundException thrown!");
        }
    }
}