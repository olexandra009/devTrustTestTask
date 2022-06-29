using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestTaskApi.Data.Entity;

namespace TestTaskApi.Data.Repository
{
    public interface IAddressRepository
    {
        Task<long> GetAddressId(Address address);
    }
    public class AddressRepository: IAddressRepository
    {
        protected readonly PersonDbContext DbContext;

        public AddressRepository(PersonDbContext dbContext)
        {
            DbContext = dbContext;
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

    }
}
