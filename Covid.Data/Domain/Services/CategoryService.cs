using System.Collections.Generic;
using System.Threading.Tasks;

using Covid.Data.Domain.Repositories;
using Covid.Data.Domain.Models;

namespace Covid.Data.Domain.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            this._categoryRepository = categoryRepository;
        }

        public async Task<Category> GetCategoryAsync(int id)
        {
            return await this._categoryRepository.GetCategoryAsync(id);
        }

        public async Task<IEnumerable<Category>> GetTop10Async()
        {
            return await this._categoryRepository.GetTop10Async();
        }
    }
}