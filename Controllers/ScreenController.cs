using System;
using System.Collections.Generic;
using System.Linq;
using InfoScreenPi.Entities;
using InfoScreenPi.ViewModels;
using InfoScreenPi.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InfoScreenPi.Extensions;
using InfoScreenPi.Infrastructure.Repositories;

namespace InfoScreenPi.Controllers
{
    public class ScreenController : BaseController
    {
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly ISettingRepository _settingRepository;
        private readonly IItemRepository _itemRepository;

        public ScreenController(IHostingEnvironment hostEnvironment,
                                ISettingRepository settingRepository,
                                IItemRepository itemRepository)
        {
            _hostEnvironment = hostEnvironment;
            _settingRepository = settingRepository;
            _itemRepository = itemRepository;
        }

        public IActionResult Index()
        {
            List<string> tickerData = new List<string>(System.IO.File.ReadAllLines(_hostEnvironment.WebRootPath + "/data/ticker.txt"));
            TempData["TickerData"] = tickerData;

            //List<string> lst= TempData["TickerData"] as List<string>;

            /*List<ItemViewModel> lijst = _context.Items
                            .Include(i => i.Soort)
                            .Include(i => i.Background)
                            .Include(i => i.RssFeed)
                            .Where(i => i.Active == true && i.Archieved == false).ToList<Item>()
                            .Select(item => new ItemViewModel(item)).ToList<ItemViewModel>();*/

            List<Item> lijstRss = _itemRepository.GetAllActiveRSSItems().ToList();
            List<Item> lijstCustom = _itemRepository.GetAllActiveCustomItems().ToList();
            List<Item> lijstCustomOld = lijstCustom;

            double deling = (double)lijstRss.Count()/(double)lijstCustom.Count();
            int multiplier = (int)Math.Floor(deling)-3;

            if(multiplier < 0) multiplier = 2;

            for(int i = 1; i < (int)multiplier/2; i++){
                lijstCustom = lijstCustom.Concat(lijstCustomOld).ToList();
                lijstCustom.Shuffle();
            }

            lijstCustom = lijstCustom.Concat(lijstRss).ToList();

            for(int i = (int)multiplier/2; i < multiplier; i++){
                lijstCustom = lijstCustom.Concat(lijstCustomOld).ToList();
                lijstCustom.Shuffle(); //Dit is juiste versie
            }

            List<Item> lijst = lijstCustom;
            lijst.Shuffle();
            lijst.CleanList();

            List<ItemViewModel> model = lijst.Select(item => new ItemViewModel(item)).ToList<ItemViewModel>();

            //Instellingen
            ViewBag.SlideTime = _settingRepository.GetSettingByName("SlideTime");
            ViewBag.TickerTime = _settingRepository.GetSettingByName("TickerTime");
            ViewBag.TickerEffect = _settingRepository.GetSettingByName("TickerEffect");
            ViewBag.ShowClock = Convert.ToBoolean(_settingRepository.GetSettingByName("ShowClock"));
            ViewBag.ShowTicker = Convert.ToBoolean(_settingRepository.GetSettingByName("ShowTicker"));
            ViewBag.ShowWeather = Convert.ToBoolean(_settingRepository.GetSettingByName("ShowWeather"));
            ViewBag.WeatherLocation = _settingRepository.GetSettingByName("WeatherLocation");


            return View(model);
        }
    }
}
