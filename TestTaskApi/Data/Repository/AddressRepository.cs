using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestTaskApi.Data.Entity;

namespace TestTaskApi.Data.Repository
{
    public interface IAddressRepository
    {
        Task<long> GetAddressId(Address address);
        Task<List<Address>> List();
        Task<Address> Get(long id);
        Task DeleteRange(List<long> ids);
    }
    public class AddressRepository: IAddressRepository
    {
        protected readonly PersonDbContext DbContext;

        public AddressRepository(PersonDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<Address> Get(long id)
        {
            var keys = new object[] { id };
            return await DbContext.Set<Address>().FindAsync(keys);
        }
        public async Task<List<Address>> List()
        {
            return await DbContext.Set<Address>().ToListAsync();
        }

        public async Task<long> GetAddressId(Address address)
        {
            var query = DbContext.Set<Address>()
                                                    .Where(a => a.City == address.City)
                                                    .Where(a => a.AddressLine == address.AddressLine);
            var results = await query.ToListAsync();
            if (results.Count == 0) return -1;
            return results[0].Id;
        }

        public async Task DeleteRange(List<long> ids)
        {
            var query = await DbContext.Set<Address>().Where(p => ids.Contains(p.Id)).ToListAsync();
            DbContext.Set<Address>().RemoveRange(query);
            await DbContext.SaveChangesAsync();

        }
    }
}
