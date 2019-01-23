


var loc = "Diepenbeek,Belgium";
var appid = "e9e81d9533487c8075cd2f47af1ef9ae";
//var appid = "&appid=" + api;
var weatherUrl = "https://api.openweathermap.org/data/2.5/forecast?q=" + loc + "&units=metric&lang=nl&appid=" + appid;
var iconMap = {
    200: "cloud_lightning_sun",
    201: "cloud_lightning_sun",
    202: "cloud_lightning_sun",
    210: "cloud_lightning_sun",
    211: "cloud_lightning_sun",
    212: "cloud_lightning_sun",
    221: "cloud_lightning_sun",
    230: "cloud_lightning_sun",
    231: "cloud_lightning_sun",
    232: "cloud_lightning_sun",
    300: "cloud_drizzle_sun",
    301: "cloud_drizzle_sun",
    302: "cloud_drizzle_sun",
    310: "cloud_drizzle_sun",
    311: "cloud_drizzle_sun",
    312: "cloud_drizzle_sun",
    313: "cloud_drizzle_sun",
    314: "cloud_drizzle_sun",
    321: "cloud_drizzle_sun",
    500: "cloud_rain_sun",
    501: "cloud_rain_sun",
    502: "cloud_rain_sun",
    503: "cloud_rain_sun",
    504: "cloud_rain_sun",
    511: "cloud_hail_sun",
    520: "cloud_rain_alt_sun",
    521: "cloud_rain_alt_sun",
    522: "cloud_rain_alt_sun",
    531: "cloud_rain_alt_sun",
    600: "cloud_snow_sun",
    601: "cloud_snow_alt_sun",
    602: "cloud_snow_alt_sun",
    611: "cloud_hail_alt_sun",
    612: "cloud_hail_alt_sun",
    615: "cloud_hail_alt_sun",
    616: "cloud_hail_alt_sun",
    620: "cloud_snow_sun",
    621: "cloud_snow_sun",
    622: "cloud_snow_alt_sun",
    701: "cloud_fog_sun",
    711: "cloud_fog_alt_sun",
    721: "cloud_fog_sun",
    731: "tornado",
    741: "cloud_fog_sun",
    751: "cloud_fog_alt_sun",
    761: "cloud_fog_alt_sun",
    762: "cloud_fog_alt_sun",
    771: "cloud_fog_alt_sun",
    781: "tornado",
    800: "sun",
    801: "cloud_sun",
    802: "cloud_sun",
    803: "cloud_sun",
    804: "cloud_sun"
}
var cardcounter = 1;

function capitalizeFirstLetter(string) {
    return string.charAt(0).toUpperCase() + string.slice(1);
}

$(document).ready(function() {

    var dateToTest = new Date();
    var today = new Date();
    dateToTest.setDate(dateToTest.getDate() + 1);
    dateToTest.setHours(12);
    dateToTest.setMinutes(0);
    dateToTest.setSeconds(0);
    dateToTest.setMilliseconds(0);
    console.log(dateToTest);
    
    console.log(encodeURI(weatherUrl));
    $.getJSON(
        encodeURI(weatherUrl),
        function(data) {
            var list = [];
            
            data.list.forEach(function(el){
               var date = new Date(el.dt_txt);
               if(date.getHours() === 12 && date.getMinutes() === 0 && !(today.getTime() === date.getTime())){
                   console.log("gevonden");
                   list.push(el);
                   
                   var fc = "anim-flip-card-" + cardcounter;
                   if(cardcounter === 1) { fc = "anim-flip" }

                   var dd = date.getDate();

                   var mm = date.getMonth()+1;
                   var yyyy = date.getFullYear();
                   if(dd<10)
                   {
                       dd='0'+dd;
                   }

                   if(mm<10)
                   {
                       mm='0'+mm;
                   }

                   var newCard = "" +
                       "<section class=\"card " + fc + "\">\n" +
                       "            <header>\n" +
                       "                <h1 class=\"card-header\">" + date.toLocaleString('nl-BE', {weekday: 'long'}).toUpperCase() + "</h1>\n" +
                       "                <h2>" + dd + "/" + mm + "</h2>\n" +
                       "            </header>\n" +
                       "            <p class=\"card-temp box-highlight\">" + Math.ceil(el.main.temp) + "</p>\n" +
                       "            <div class=\"icon\">\n" +
                       "                <i class=\"wi wi-owm-" + el.weather[0].id + "\"></i>" +
                       "            </div>\n" +
                       "            <p class=\"card-info\">" + capitalizeFirstLetter(el.weather[0].description) + "</p>\n" +
                       "        </section>";
                   
                   $(".card-wrapper").append(newCard);

                    
                   
                   
                   
               }
               
            });
            
            console.log(list);
            
            
        });
    
});


// Eerste nog beste keuze: 3-h interval maar neem ergens rond middaguur dan
// https://api.openweathermap.org/data/2.5/forecast?q=Alken,Belgium&units=metric&lang=nl&appid=e9e81d9533487c8075cd2f47af1ef9ae
//
//
// YAHOO
//
// App ID
// syLL8C3e
// Client ID (Consumer Key)
// dj0yJmk9NklUbXhWWnc5V3VEJnM9Y29uc3VtZXJzZWNyZXQmc3Y9MCZ4PTBk
// Client Secret (Consumer Secret)
// d3962733ed5e1e81c77865232d2ae957c9181295


// weatherbit: e5078a9aad79470581c631998b388655
// https://api.weatherbit.io/v2.0/forecast/daily?city=Diepenbeek,Belgium&key=e5078a9aad79470581c631998b388655&lang=nl&days=5