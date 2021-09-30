using System.Collections.Generic;
using System.Threading.Tasks;

using Covid.Data.Domain.Models;

namespace Covid.Data.Domain.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> GetCategoryAsync(int id);

        Task<IEnumerable<Category>> GetTop10Async();
    }
}