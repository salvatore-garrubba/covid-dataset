using System.Collections.Generic;
using System.Threading.Tasks;
using Covid.Data.Domain.Models;

namespace Covid.Data.Domain.Services
{
    public interface ICategoryService
    {
         Task<Category> GetCategoryAsync(int id);
         Task<IEnumerable<Category>> GetTop10Async();
    }
}