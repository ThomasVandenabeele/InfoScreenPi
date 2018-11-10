using InfoScreenPi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace InfoScreenPi.Infrastructure.Repositories
{
    public class ItemRepository : EntityBaseRepository<Item>, IItemRepository
    {
        InfoScreenContext _context;

        public ItemRepository(InfoScreenContext context)
            : base(context)
        {
            _context = context;
        }

        public IEnumerable<Item> GetAllCustomItems()
        {
            return _context.Items.Include(i => i.Soort).Include(b => b.Background).Where(i => i.Soort.Description == "CUSTOM");
        }

        public IEnumerable<Item> GetAllActiveCustomItems()
        {
            return _context.Items.Include(i => i.Soort).Include(b => b.Background).Where(i => i.Soort.Description == "CUSTOM" && i.Active == true && i.Archieved == false);
        }

        public IEnumerable<Item> GetAllActiveRSSItems()
        {
            return _context.Items.Include(i => i.Soort).Include(b => b.Background).Include(r => r.RssFeed).Where(i => i.Soort.Description == "RSS" && i.Active == true && i.Archieved == false);
        }
    }
}