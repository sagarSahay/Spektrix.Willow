namespace Willow.Denormalizer.Common
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDocumentStore : IDisposable
    {
        Task CreateCollection(string collectionId);

        Task<int> Count(string collectionId);

        Task<IEnumerable<T>> ReadAll<T>(string collectionId);

        Task<IEnumerable<T>> ReadPaged<T>(string collectionId, int skip, int take);

        Task<IEnumerable<T>> Query<T>(string collectionId, Dictionary<string, object> filterProperties);

        Task<IEnumerable<T>> QueryIn<T>(string collectionId, Dictionary<string, object[]> filterProperties);

        Task<IEnumerable<T>> QueryRegex<T>(string collectionId, Dictionary<string, string> filterProperties);

        Task<DocumentBase<T>> ReadDocument<T>(string collectionId, string documentId);

        Task UpsertDocument<T>(string collectionId, string documentId, DocumentBase<T> item);

        Task DeleteDocument(string collectionId, string documentId);

        Task DeleteCollection(string collectionId);

        Task ClearCollection(string collectionId);

        bool DocumentExists(string collectionId, string documentId);

        Task<bool> CollectionExists(string collectionId);
    } 
}