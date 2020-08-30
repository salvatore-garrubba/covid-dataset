using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Covid.Data.Domain.Services;
using Covid.Data.Domain.Models;
using AutoMapper;
using Covid.API.Domain.Models;

namespace Covid.API.Controllers
{
    //https://localhost:5001/api/categories
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        
        private readonly ILogger<CategoriesController> _logger;

        private readonly IMapper _mapper;

        public CategoriesController(ILogger<CategoriesController> logger, ICategoryService categoryService, IMapper mapper)
        {
            _logger = logger;
            _categoryService = categoryService;
            _mapper = mapper;            
        }

        [HttpGet]
        public async Task<IEnumerable<CategoryResource>> GetTop10Async()
        {            
            var categories = await _categoryService.GetTop10Async();
            
            return _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryResource>>(categories);
        }
    }
}
