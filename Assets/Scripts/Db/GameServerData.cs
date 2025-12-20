using System;

namespace Db
{
    [Serializable]
    public class GameServerData
    {
        public readonly string Host;
        public readonly int Port;
        public readonly string Zone;

        public GameServerData(string host, int port, string zone)
        {
            Host = host;
            Port = port;
            Zone = zone;
        }
    }
}