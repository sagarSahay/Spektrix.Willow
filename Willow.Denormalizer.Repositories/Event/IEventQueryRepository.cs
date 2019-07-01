namespace Willow.Denormalizer.Repositories.Event
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common;
    using ViewModels;

    public interface IEventQueryRepository
    {
        Task<DocumentBase<EventVM>> GetById(string id);
        Task<IEnumerable<EventVM>> LoadAll();
    }
}