namespace Willow.Denormalizer.Repositories
{
    using Common;

    internal abstract class CommonVMRepo<VM> : DocumentDbRepository<VM>
    {
        protected CommonVMRepo(IDocumentStore db, string collectionName = null) : base(db, collectionName)
        {
        }
    }
}