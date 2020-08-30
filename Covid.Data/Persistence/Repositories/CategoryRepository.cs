using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using Covid.Data.Domain.Models;
using Covid.Data.Domain.Repositories;
using Covid.Data.Persistence.Contexts;

namespace Covid.Data.Persistence.Repositories
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public CategoryRepository(CovidContext context) : base(context)
        {
        }

        public async Task<Category> GetCategoryAsync(int id)
        {
            return await _context.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Category>> GetTop10Async()
        {
            var result = await _context.Categories
            .Select(category => new
            {
                OriginalCategory = category,
                QuestionCount = category.Questions.Count
            }).OrderByDescending(c => c.QuestionCount).Take(10).ToListAsync();
            return result.Select(c => c.OriginalCategory);                        
        }
    }
}