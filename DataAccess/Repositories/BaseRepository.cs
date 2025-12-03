using DataAccess.Context;
using Interfaces.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{

    public class BaseRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
           return await _dbSet.ToListAsync();
           
        }
        public async Task<T> GetByIdAsync(int id)
        {
          return  await _dbSet.FindAsync(id);
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
           await _context.SaveChangesAsync();
        }
        public async Task Update(T entity)
        {
            _dbSet.Update(entity);
          await  _context.SaveChangesAsync();
        }
        public async Task Delete(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

}
