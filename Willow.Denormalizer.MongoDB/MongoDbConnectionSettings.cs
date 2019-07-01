namespace Willow.Denormalizer.MongoDB
{
    using System;

    public class MongoDbConnectionSettings
    {
        public string ServerAddress { get; }
        public int ServerPort { get; }
        public string DatabaseName { get; }
        public string UserName { get; }
        public string UserPassword { get; }

        public MongoDbConnectionSettings(
            string serverAddress,
            int serverPort,
            string databaseName,
            string userName = null,
            string userPassword = null)
        {
            if (string.IsNullOrWhiteSpace(serverAddress)) throw new ArgumentNullException(nameof(serverAddress));
            if (string.IsNullOrWhiteSpace(databaseName)) throw new ArgumentNullException(nameof(databaseName));

            ServerAddress = serverAddress;
            ServerPort = serverPort;
            DatabaseName = databaseName;
            UserName = userName;
            UserPassword = userPassword;
        }
    }
}