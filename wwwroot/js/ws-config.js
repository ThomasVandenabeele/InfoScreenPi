"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/screenpiws").build();

connection.start().catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("RefreshScreen").addEventListener("click", function (event) {
    connection.invoke("RefreshScreens").catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});