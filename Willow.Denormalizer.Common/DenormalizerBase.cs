namespace Willow.Denormalizer.Common
{
    using System;
    using System.Threading.Tasks;

    public abstract class DenormalizerBase<T> where T : class, new()
    {
        protected readonly IDenormalizerRepository<T> repository;
        protected readonly string repositoryKey;

        protected DenormalizerBase(IDenormalizerRepository<T> repository, string viewModelKey = null)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            repositoryKey = viewModelKey ?? typeof(T).FullName;
        }

        protected virtual async Task Process<TEvent>(TEvent @event, string id)
        {
            var viewModel = await repository.GetById(id) ?? new DocumentBase<T>();

            Map(@event, viewModel.VM);

            await repository.Upsert(id, viewModel);
        }

        protected abstract T Map<TEvent>(TEvent @event, T viewModel);
    }
}