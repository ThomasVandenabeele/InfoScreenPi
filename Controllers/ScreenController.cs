using System;
using System.Collections.Generic;
using System.Linq;
using InfoScreenPi.Entities;
using InfoScreenPi.ViewModels;
using InfoScreenPi.Infrastructure;
using InfoScreenPi.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InfoScreenPi.Extensions;

namespace InfoScreenPi.Controllers
{
    public class ScreenController : BaseController
    {
        public ScreenController(IHostingEnvironment hostEnvironment, IDataService dataService)
        : base(dataService, hostEnvironment){}

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

            List<Item> lijstRss = _data.GetAllActiveRSSItems().ToList();
            List<Item> lijstCustom = _data.GetAllActiveCustomItems().ToList();
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
            ViewBag.SlideTime = _data.GetSettingByName("SlideTime");
            ViewBag.TickerTime = _data.GetSettingByName("TickerTime");
            ViewBag.TickerEffect = _data.GetSettingByName("TickerEffect");
            ViewBag.ShowClock = Convert.ToBoolean(_data.GetSettingByName("ShowClock"));
            ViewBag.ShowTicker = Convert.ToBoolean(_data.GetSettingByName("ShowTicker"));
            ViewBag.ShowWeather = Convert.ToBoolean(_data.GetSettingByName("ShowWeather"));
            ViewBag.WeatherLocation = _data.GetSettingByName("WeatherLocation");


            return View(model);
        }
    }
}
