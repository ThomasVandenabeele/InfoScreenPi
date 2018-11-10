using InfoScreenPi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoScreenPi.Infrastructure.Repositories
{
    public class ItemKindRepository : EntityBaseRepository<ItemKind>, IItemKindRepository
    {
        public ItemKindRepository(InfoScreenContext context)
            : base(context)
        { }
    }
}