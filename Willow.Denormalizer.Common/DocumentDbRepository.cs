namespace Willow.Denormalizer.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public abstract class DocumentDbRepository<T> : IDenormalizerRepository<T>
    {
        private readonly string collectionName;
        private readonly Lazy<Task<IDocumentStore>> db;

        protected DocumentDbRepository(IDocumentStore db, string collectionName = null)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            this.collectionName = collectionName ?? typeof(T).FullName;

            this.db = new Lazy<Task<IDocumentStore>>(async () =>
            {
                await db.CreateCollection(this.collectionName);

                return db;
            });
        }

        public async Task Delete(string id)
        {
            var db = await this.db.Value;
            await db.DeleteDocument(collectionName, id);
        }

        public async Task<DocumentBase<T>> GetById(string id)
        {
            var db = await this.db.Value;
            var doc = await db.ReadDocument<T>(collectionName, id);

            return doc;
        }

        public async Task<IEnumerable<T>> GetByExample(object example)
        {
            var db = await this.db.Value;

            var exampleData = example
                .GetType()
                .GetProperties()
                .Where(propertyInfo => !propertyInfo.PropertyType.IsGenericType)
                .ToDictionary(
                    propertyInfo => propertyInfo.Name,
                    propertyInfo => propertyInfo.GetValue(example, new object[] { }));

            // TODO fix this by using static reflection when we have time
            var data = await db.Query<T>(collectionName, exampleData);

            return data;
        }

        public async Task<IEnumerable<T>> LoadAll()
        {
            var db = await this.db.Value;
            var data = (await db.ReadAll<T>(collectionName)).ToList();

            return data;
        }

        public async Task<IEnumerable<T>> LoadPaged(int skip, int take)
        {
            var db = await this.db.Value;
            var data = (await db.ReadPaged<T>(collectionName, skip, take)).ToList();

            return data;
        }

        public async Task Upsert(string id, DocumentBase<T> item)
        {
            var db = await this.db.Value;
            await db.UpsertDocument(collectionName, id, item);
        }

        public Task SaveAsync(DocumentBase<T> item, Func<T, string> keyFunc)
        {
            return Upsert(keyFunc(item.VM), item);
        }

        public async Task<int> Count()
        {
            var db = await this.db.Value;
            return await db.Count(collectionName);
        }
    }
}