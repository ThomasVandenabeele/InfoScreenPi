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
        
        if(currentSlide.soort.description == "CUSTOM"){
            var url = currentSlide.background.url;
            if(!currentSlide.background.url.startsWith("http://") && !currentSlide.background.url.startsWith("https://")){
                url = "../images/backgrounds/" + currentSlide.background.url;
            }
            nextSlide.html( '<div class="fill" style="background-image:url(\'' + url + '\')"></div>\n' +
                            '<div class="carousel-caption customlayout">\n' +
                            $.trim(currentSlide.content) +
                            '</div>');
        }
        else if(currentSlide.soort.description == "VIDEO"){
            nextSlide.html( '<video autoplay muted loop width="1920" height="1080">\n' +
                            '<source src="videos/' + currentSlide.content + '" type="video/mp4"/>\n' +
                            'Your browser does not support the video tag.\n' +
                            '</video>\n');
        }
        else if(currentSlide.soort.description == "CLOCK"){
            nextSlide.load('clock.html');
            // nextSlide.html( '<div class="fill" style="background-color: #1FABD5;"></div>' +
            //                 'CLOCK');
        }
        else if(currentSlide.soort.description == "WEATHER"){
            nextSlide.load('forecast.html');
            // nextSlide.html( '<div class="fill" style="background-color: #1FABD5;"></div>' +
            //                 'CLOCK');
        }
        else if(currentSlide.soort.description == "RSS"){
            nextSlide.html( '<div class="fill" style="background-image:url(\'' + currentSlide.background.url + '\')"></div>\n' +
                            '<div class="carousel-caption" style="font-weight: 900;">\n' +
                            '<div style="display:flex;flex-direction: column;justify-content:center;align-items:center;width:930px;height:450px;">\n' +
                            '<h2 style="font-weight: 900;">' + currentSlide.title + '</h2>\n' +
                            '<p>' + currentSlide.content + '</p>\n' +
                            '</div>\n' +
                            '<div class="test"><b>BRON:</b>' + currentSlide.rssFeed.title + '</div>\n' +
                            '</div>');
        }
        
        
        $('#screenSlider').carousel('next');
        if(firstItemReceived === false) firstItemReceived = true;
    });
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});
