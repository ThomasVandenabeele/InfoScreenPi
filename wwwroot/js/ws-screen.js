"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/signalr").build();

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});
connection.on("RefreshScreens", function () {
    console.log("on refresh");
    //location.reload();
});
connection.on("BroadcastSlide", function(currentSlide){
    console.log("new slide ontvangen");
    console.log(currentSlide);
    //currentSlide.Title;
    //currentSlide.background.url;
    $(document).ready(function() {
        
        var nextSlide = $('div.item').not('.active');
        nextSlide.empty();
        var soort = currentSlide.itemType;
        var item = currentSlide.item;
        
        if(soort == "CustomItem"){
            var url = item.background.url;
            if(!item.background.url.startsWith("http://") && !item.background.url.startsWith("https://")){
                url = "../images/backgrounds/" + item.background.url;
            }
            nextSlide.html( '<div class="fill" style="background-image:url(\'' + url + '\')"></div>\n' +
                            '<div class="carousel-caption customlayout">\n' +
                            $.trim(item.content) +
                            '</div>');
        }
        else if(soort == "VideoItem"){
            nextSlide.html( '<video autoplay muted loop width="1920" height="1080">\n' +
                            '<source src="videos/' + item.url + '" type="video/mp4"/>\n' +
                            'Your browser does not support the video tag.\n' +
                            '</video>\n');
        }
        else if(soort == "ClockItem"){
            nextSlide.load('clock.html');
            // nextSlide.html( '<div class="fill" style="background-color: #1FABD5;"></div>' +
            //                 'CLOCK');
        }
        else if(soort == "WeatherItem"){
            nextSlide.load('forecast.html');
            // nextSlide.html( '<div class="fill" style="background-color: #1FABD5;"></div>' +
            //                 'CLOCK');
        }
        else if(soort == "RSSItem"){
            nextSlide.html( '<div class="fill" style="background-image:url(\'' + item.background.url + '\')"></div>\n' +
                            '<div class="carousel-caption" style="font-weight: 900;">\n' +
                            '<div style="display:flex;flex-direction: column;justify-content:center;align-items:center;width:930px;height:450px;">\n' +
                            '<h2 style="font-weight: 900;">' + item.title + '</h2>\n' +
                            '<p>' + item.content + '</p>\n' +
                            '</div>\n' +
                            '<div class="test"><b>BRON:</b>' + item.rssFeed.title + '</div>\n' +
                            '</div>');
        }
        
        
        $('#screenSlider').carousel('next');
        if(firstItemReceived === false) firstItemReceived = true;
    });
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});
