"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/screenpiws").build();

connection.on("RefreshScreens", function () {
    //location.reload();
});

connection.on("BroadcastSlide", function(currentSlide){
    $(document).ready(function() {
        console.log(currentSlide);
        var nextSlide = $('div.item').not('.active');
        var old = $('div.item.active')
        
        
        
        var soort = currentSlide.itemType;
        var item = currentSlide.item;

        var currentId = $('div.item.active').attr("id");
        nextSlide.attr("id", item.id);
        
        if(Number(item.id) !== Number(currentId)){
            
            if(soort == "CustomItem"){
                var url = item.background.url;
                if(!item.background.url.startsWith("http://") && !item.background.url.startsWith("https://")){
                    url = "../images/backgrounds/" + item.background.url;
                }
                
                var element = nextSlide.children("div.customItem");
                element.find("div.fill").css('background-image', 'url(\'' + url + '\'');
                element.find("div.customlayout").html($.trim(item.content));

                element.show();

                /*nextSlide.html( '<div class="fill" style="background-image:url(\'' + url + '\')"></div>\n' +
                    '<div class="carousel-caption customlayout">\n' +
                    $.trim(item.content) +
                    '</div>');
                    */
            }
            else if(soort == "VideoItem"){
                var element = nextSlide.children("div.videoItem");

                var videoElement = element.find("video")[0];
                console.log(videoElement);
                console.log(element.find('source')[0]);

                element.find('source')[0].setAttribute('src', 'videos/' + item.url);
                videoElement.load();

                element.show();

                /*
                nextSlide.html( '<video id="vidCar" autoplay="1" muted="1" width="1920" height="1080">\n' +
                    '<source src="videos/' + item.url + '" type="video/mp4"/>\n' +
                    'Your browser does not support the video tag.\n' +
                    '</video>\n');
                setTimeout(function(){
                    document.getElementById("vidCar").play();
                }, 250);
                */           
 	        }
            else if(soort == "ClockItem"){
                var element = nextSlide.children("div.clockItem");
                element.show();

                // nextSlide.load('clock.html');
                // nextSlide.html( '<div class="fill" style="background-color: #1FABD5;"></div>' +
                //                 'CLOCK');
            }
            else if(soort == "WeatherItem"){
                var element = nextSlide.children("div.forecastItem");
                element.show();
                
                // nextSlide.load('forecast.html');
                // nextSlide.html( '<div class="fill" style="background-color: #1FABD5;"></div>' +
                //                 'CLOCK');
            }
            else if(soort == "RSSItem"){
                var url = item.background.url;
                if(!item.background.url.startsWith("http://") && !item.background.url.startsWith("https://")){
                    url = "../images/backgrounds/" + item.background.url;
                }

                var element = nextSlide.children("div.rssItem");
                element.find("div.fill").css('background-image', 'url(\'' + url + '\'');
                element.find("h2.rssTitle").html(item.title);
                element.find("p.rssContent").html(item.content);
                element.find("span.rssSource").html(item.rssFeed.title);

                element.show();

                /*
                nextSlide.html( '<div class="fill" style="background-image:url(\'' + url + '\')"></div>\n' +
                    '<div class="carousel-caption" style="font-weight: 900;">\n' +
                    '<div style="display:flex;flex-direction: column;justify-content:center;align-items:center;width:930px;height:450px;">\n' +
                    '<h2 style="font-weight: 900;">' + item.title + '</h2>\n' +
                    '<p>' + item.content + '</p>\n' +
                    '</div>\n' +
                    '<div class="test"><b>BRON:</b>' + item.rssFeed.title + '</div>\n' +
                    '</div>');
                */
            }


            $('#screenSlider').carousel('next');

            //cleanup old slide           
            setTimeout(function(){
                old.children("div").each(function(){
                    var element = $(this);
                    element.hide();
                });

                old.children("div.customItem").find("div.fill").css('background-image', 'none');
                old.children("div.rssItem").find("div.fill").css('background-image', 'none');

                if(old.find("video").length > 0){
                    var videoElement = old.find("video")[0];
                    videoElement.pause();
                    videoElement.removeAttribute('src'); // empty source
                    videoElement.load();
                }

            }, 750);
            

        }
        
        if(firstItemReceived === false) firstItemReceived = true;
    });
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});
