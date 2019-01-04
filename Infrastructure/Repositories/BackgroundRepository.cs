using InfoScreenPi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace InfoScreenPi.Infrastructure.Repositories
{
    public class BackgroundRepository : EntityBaseRepository<Background>, IBackgroundRepository
    {
        InfoScreenContext _context;
        public BackgroundRepository(InfoScreenContext context)
            : base(context)
        {
            _context = context;
        }

        public IEnumerable<Background> GetAllWithoutRSS(Boolean archieved)
        {
            //IEnumerable<Background> model = null;

            return  _context.Items.Include(i => i.Background).Include(i => i.Soort)
                                  .Where(i => !(i.Soort.Description == "RSS") && archieved? true : !(i.Archieved))
                                  .Select(i => i.Background);

            /*IEnumerable<Background> rssAchtergronden = GetAll(i => i.Soort.Description == "RSS", i => i.Soort, i => i.Background)


                                      .Select(i=>i.Background);*/

            /*model = GetAll(a => !rssAchtergronden.Contains(a));

            if(!archieved){
                IEnumerable<Background> gearchiveerdeAchtergronden = _context.Items.Include(i => i.Background)
                                                                .Where(i => i.Archieved == true)
                                                                .Select(i => i.Background);

            	model = model.Where(a => !gearchiveerdeAchtergronden.Contains(a));
            }

            return model;*/
        }
    }
}
