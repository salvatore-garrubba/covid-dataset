using Covid.Data.Persistence.Contexts;

namespace Covid.Data.Persistence.Repositories
{
    public abstract class BaseRepository
     {
        protected readonly CovidContext _context;

        public BaseRepository(CovidContext context)
        {
            _context = context;
        }
    }
}