using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public Task<Photo> GetMainPhotoForUser(int userId)
        {
           return _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMainPhoto);
        }

        public Task<Photo> GetPhoto(int Id)
        {
            var photo = _context.Photos.FirstOrDefaultAsync(p => p.Id == Id);
            return photo;
        }

        public async Task<User> GetUser(int Id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync( u => u.Id == Id);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
             var users = _context.Users.Include(p => p.Photos);
             return await users.ToListAsync();
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}