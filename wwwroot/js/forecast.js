

var cardcounter = 1;

function capitalizeFirstLetter(string) {
    return string.charAt(0).toUpperCase() + string.slice(1);
}

$(document).ready(function() {

    var today = new Date();
    $.getJSON(
        encodeURI("/Screen/GetWeatherForecast"),
        function(data) {
            var list = [];
            
            data.list.forEach(function(el){
               var date = new Date(el.dt_txt);
               if(date.getHours() === 12 && date.getMinutes() === 0 && !(today.getTime() === date.getTime())){
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
            
            
            
        });
    
});

