using Expose_data.Data;
using Expose_data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;

namespace Expose_data.Controllers
{
    public class HomeController : Controller
    {
        private DatabaseContext _db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DatabaseContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // Get files from db and convert to a workable structure.
            var files = await _db.Files.Select(x => ByteArrayToString(x.FileBytes)).ToListAsync();
            var filesSplitted = files
                .Select(x => x.Split(new string[] { "\r\n", "\r", "\n", "|" }, StringSplitOptions.None))
                .ToList();

            // Get top 10 categories.
            var top10Categories = Count(filesSplitted, 1);

            // Get top 10 questions.
            var top10Questions = Count(filesSplitted, 2);

            return View(new ResultModel { MostUsedCategories = top10Categories, MostAskedQuestions = top10Questions});
        }

        public static Dictionary<string, int> Count(List<string[]>listOfStrings, int key)
        {
            Dictionary<string, int> top10 = new();
            listOfStrings.ForEach(x =>
            {
                for (int i = key; i < x.Length; i += 5)
                {
                    if (top10.ContainsKey(x[i]))
                        top10[x[i]]++;
                    else
                        top10.Add(x[i], 1);
                }
            });

            return top10.OrderByDescending(x => x.Value).Take(10).ToDictionary(x => x.Key, x => x.Value); ;
        }

        public static string ByteArrayToString(byte[] array)
        {
            return Encoding.Default.GetString(array);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}