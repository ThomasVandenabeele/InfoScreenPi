using System.Runtime.InteropServices.ComTypes;
using InfoScreenPi.Entities;

namespace InfoScreenPi.ViewModels
{
    public class ScreenItemViewModel
    {

        public Item item { get; set; }
        public string itemType { get; set; }


        public ScreenItemViewModel(Item i)
        {
            item = i;
            itemType = i.GetType().Name;

//            switch typeof(i)
//            {
//                case RSSItem:
//                    itemType = "RSSItem";
//                    break;
//                case CustomItem:
//                    itemType = "CustomItem";
//                    break;
//                case VideoItem:
//                    itemType = "VideoItem";
//                    break;
//                case ClockItem:
//                    itemType = "ClockItem";
//                    break;
//                case WeatherItem:
//                    itemType = "WeatherItem";
//                    break;
//                default:
//                    itemType = "CustomItem";
//                    break;
//            }
        }
        
    }
}