using Microsoft.EntityFrameworkCore;
using Plooto.Payments.Domain.Interfaces;
using Plooto.Payments.Infrastructure.Storage.Configuration;

namespace Plooto.Payments.Infrastructure.Storage.Repositories
{
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        protected readonly PaymentDbContext Context;

        public Repository(PaymentDbContext context)
        {
            Context = context;
        }

        public async Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken)
        {
            return await Context.Set<TEntity>().FindAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await Context.Set<TEntity>().ToListAsync(cancellationToken);
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await Context.Set<TEntity>().AddAsync(entity, cancellationToken);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            Context.Set<TEntity>().Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
            await Task.CompletedTask;
        }
    }
}