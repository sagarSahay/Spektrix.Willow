namespace Willow.Denormalizer.MongoDB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common;
    using global::MongoDB.Bson;
    using global::MongoDB.Bson.Serialization.Attributes;
    using global::MongoDB.Driver;

    public class MongoDb : IDocumentStore
    {
        private readonly IMongoClient client;
        private readonly IMongoDatabase database;
        private const string VmField = "VM";

        private class MongoDocumentWrapper : IDocumentBase
        {
            [BsonId]
            public string Id { get; set; }
            public string ETag { get; set; }
            public object VM { get; set; }
        }

        public class MongoDocumentWrapper<T> : DocumentBase<T>
        {
            [BsonId]
            public string Id { get; set; }
        }

        public MongoDb(MongoDbConnectionSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            var clientSettings = new MongoClientSettings
            {
                Server = new MongoServerAddress(settings.ServerAddress, settings.ServerPort),
                MaxConnectionIdleTime = TimeSpan.FromMinutes(1)
            };

            if (!string.IsNullOrWhiteSpace(settings.UserName))
            {
                clientSettings.Credential = MongoCredential.CreateCredential(settings.DatabaseName, settings.UserName, settings.UserPassword);
            }

            client = new MongoClient(clientSettings);
            database = client.GetDatabase(settings.DatabaseName);
        }

        public async Task UpsertDocument<T>(string collectionId, string documentId, DocumentBase<T> item)
        {
            if (string.IsNullOrWhiteSpace(collectionId)) throw new ArgumentNullException(nameof(collectionId));
            if (string.IsNullOrWhiteSpace(documentId)) throw new ArgumentNullException(nameof(documentId));
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.VM == null) throw new ArgumentNullException(nameof(item.VM));

            var collection = database.GetCollection<MongoDocumentWrapper<T>>(collectionId);

            var initialCorrelationId = item.ETag;
            item.ETag = Guid.NewGuid().ToString();
            var builders = Builders<MongoDocumentWrapper<T>>.Filter;

            var filter = builders.Eq(x => x.Id, documentId) & builders.Eq(x => x.ETag, initialCorrelationId);

            var options = new FindOneAndReplaceOptions<MongoDocumentWrapper<T>, MongoDocumentWrapper<T>>
                {
                    ReturnDocument = ReturnDocument.Before,
                    IsUpsert = true
                };

            try
            {
                var toUpsert = new MongoDocumentWrapper<T>
                {
                    Id = documentId,
                    ETag = item.ETag,
                    VM = item.VM
                };

                await collection.FindOneAndReplaceAsync(filter, toUpsert, options);
            }
            catch (MongoCommandException ex) when (ex.Code == 11000)
            {
                throw new Exception(
                    $"Error upserting item with id {documentId} and etag {item.ETag}");
            }
        }

        public bool DocumentExists(string collectionId, string documentId)
        {
            if (string.IsNullOrWhiteSpace(collectionId)) throw new ArgumentNullException(nameof(collectionId));
            if (string.IsNullOrWhiteSpace(documentId)) throw new ArgumentNullException(nameof(documentId));

            var collection = database.GetCollection<MongoDocumentWrapper>(collectionId);
            var filter = Builders<MongoDocumentWrapper>.Filter.Eq(x => x.Id, documentId);

            return collection.Find(filter).CountDocuments() != 0;
        }

        public Task<DocumentBase<T>> ReadDocument<T>(string collectionId, string documentId)
        {
            if (string.IsNullOrWhiteSpace(collectionId)) throw new ArgumentNullException(nameof(collectionId));
            if (string.IsNullOrWhiteSpace(documentId)) throw new ArgumentNullException(nameof(documentId));

            var collection = database.GetCollection<MongoDocumentWrapper<T>>(collectionId);
            var filter = Builders<MongoDocumentWrapper<T>>.Filter.Eq(x => x.Id, documentId);

            return collection.Find(filter).Project(x=> x as DocumentBase<T>).FirstOrDefaultAsync();
        }

        public Task<IEnumerable<T>> ReadAll<T>(string collectionId)
        {
            if (string.IsNullOrWhiteSpace(collectionId)) throw new ArgumentNullException(nameof(collectionId));

            var collection = database.GetCollection<MongoDocumentWrapper<T>>(collectionId);
            return Task.FromResult(collection.Find(new BsonDocument())
                .Project(x=> x.VM)
                .ToEnumerable());
        }

        public Task<IEnumerable<T>> ReadPaged<T>(string collectionId, int skip, int take)
        {
            if (string.IsNullOrWhiteSpace(collectionId)) throw new ArgumentNullException(nameof(collectionId));

            var collection = database.GetCollection<MongoDocumentWrapper<T>>(collectionId);
            return Task.FromResult(collection.Find(new BsonDocument())
                .Project(x => x.VM)
                .Skip(skip)
                .Limit(take)
                .ToEnumerable());
        }

        public Task<IEnumerable<T>> Query<T>(string collectionId, Dictionary<string, object> filterProperties)
        {
            if (string.IsNullOrWhiteSpace(collectionId)) throw new ArgumentNullException(nameof(collectionId));
            if (filterProperties == null) throw new ArgumentNullException(nameof(filterProperties));

            var collection = database.GetCollection<MongoDocumentWrapper<T>>(collectionId);
            var filters = filterProperties.Select(x => Builders<MongoDocumentWrapper<T>>.Filter.Eq(VmField + "." + x.Key, x.Value));

            return Task.FromResult(collection.Find(Builders<MongoDocumentWrapper<T>>.Filter.And(filters))
                .ToEnumerable()
                .Select(x=> x.VM));
        }

        public Task<IEnumerable<T>> QueryIn<T>(string collectionId, Dictionary<string, object[]> filterProperties)
        {
            if (string.IsNullOrWhiteSpace(collectionId)) throw new ArgumentNullException(nameof(collectionId));
            if (filterProperties == null) throw new ArgumentNullException(nameof(filterProperties));

            var collection = database.GetCollection<MongoDocumentWrapper<T>>(collectionId);
            var filters = filterProperties.Select(x => Builders<MongoDocumentWrapper<T>>.Filter.In(VmField + "." + x.Key, x.Value));

            return Task.FromResult(collection.Find(Builders<MongoDocumentWrapper<T>>.Filter.And(filters))
                .ToEnumerable()
                .Select(x=> x.VM));
        }

        public Task<IEnumerable<T>> QueryRegex<T>(string collectionId, Dictionary<string, string> filterProperties)
        {
            if (string.IsNullOrWhiteSpace(collectionId)) throw new ArgumentNullException(nameof(collectionId));
            if (filterProperties == null) throw new ArgumentNullException(nameof(filterProperties));

            var collection = database.GetCollection<MongoDocumentWrapper<T>>(collectionId);
            var filters = filterProperties.Select(x => Builders<MongoDocumentWrapper<T>>.Filter.Regex(VmField + "." + x.Key, new BsonRegularExpression(x.Value)));

            return Task.FromResult(collection.Find(Builders<MongoDocumentWrapper<T>>.Filter.And(filters))
                .ToEnumerable()
                .Select(x=> x.VM));
        }

        public async Task DeleteDocument(string collectionId, string documentId)
        {
            if (string.IsNullOrWhiteSpace(collectionId)) throw new ArgumentNullException(nameof(collectionId));
            if (string.IsNullOrWhiteSpace(documentId)) throw new ArgumentNullException(nameof(documentId));

            var collection = database.GetCollection<MongoDocumentWrapper>(collectionId);
            var filter = Builders<MongoDocumentWrapper>.Filter.Eq(x=> x.Id, documentId);

            await collection.DeleteOneAsync(filter);
        }

        public async Task<bool> CollectionExists(string collectionId)
        {
            if (string.IsNullOrWhiteSpace(collectionId)) throw new ArgumentNullException(nameof(collectionId));

            var collections = await database.ListCollectionsAsync(new ListCollectionsOptions()
            {
                Filter = Builders<BsonDocument>.Filter.Eq("name", collectionId)
            });

            return collections.Any();
        }

        public async Task CreateCollection(string collectionId)
        {
            if (string.IsNullOrWhiteSpace(collectionId)) throw new ArgumentNullException(nameof(collectionId));

            if (await CollectionExists(collectionId)) return;

            await database.CreateCollectionAsync(collectionId);

            var collection = database.GetCollection<MongoDocumentWrapper>(collectionId);

            var etagKey = Builders<MongoDocumentWrapper>.IndexKeys.Ascending(x => x.Id).Ascending(x => x.ETag);

            var etagIndexTask = collection.Indexes.CreateOneAsync(new CreateIndexModel<MongoDocumentWrapper>(
                etagKey,
                new CreateIndexOptions { Unique = true }));

            await Task.WhenAll(etagIndexTask);
        }

        public Task DeleteCollection(string collectionId)
        {
            if (string.IsNullOrWhiteSpace(collectionId)) throw new ArgumentNullException(nameof(collectionId));

            return database.DropCollectionAsync(collectionId);
        }

        public Task ClearCollection(string collectionId)
        {
            if (string.IsNullOrWhiteSpace(collectionId)) throw new ArgumentNullException(nameof(collectionId));

            var collection = database.GetCollection<MongoDocumentWrapper>(collectionId);
            var filter = new BsonDocument();
            return collection.DeleteManyAsync(filter);
        }

        public async Task<int> Count(string collectionId)
        {
            if (string.IsNullOrWhiteSpace(collectionId)) throw new ArgumentNullException(nameof(collectionId));

            var collection = database.GetCollection<MongoDocumentWrapper>(collectionId);
            var filter = new BsonDocument();
            return (int)await collection.CountDocumentsAsync(filter);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}