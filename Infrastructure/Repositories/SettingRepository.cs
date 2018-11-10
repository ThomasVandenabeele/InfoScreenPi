using InfoScreenPi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoScreenPi.Infrastructure.Repositories
{
    public class SettingRepository : EntityBaseRepository<Setting>, ISettingRepository
    {
        public SettingRepository(InfoScreenContext context)
            : base(context)
        { }
    

        public string GetSettingByName(string setting)
        {
            return GetAll().Where(i => i.SettingName == setting).Select(i => i.SettingValue).First();
        }

        public void SetSettingByName(string key, string value){
            List<Setting> lijst = GetAll().ToList();
            Setting s = lijst.First(setting => setting.SettingName == key);
            s.SettingValue = value;
            Edit(s);
            Commit();
        }

    }
}