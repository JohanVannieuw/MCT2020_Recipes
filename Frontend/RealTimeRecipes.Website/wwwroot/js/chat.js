//wwwroot/js/chat.js

//import in Common.js style
import { config as config } from './config.js';

let realtime = {}; //namespace

realtime.hub = (() => {
    let remoteHost = config.realTime.scheme + "://" + config.realTime.host + ":" + config.realTime.port;
   // remoteHost = "http://localhost:26133";  //naar config.js
    //cors instellen op client samen met AllowCredentials()!!!
    let connection =
        new signalR.HubConnectionBuilder().withUrl(remoteHost + "/chatHub")
            .configureLogging(signalR.LogLevel.Error)
            .build();

    connection.start().catch(function (err) {
        return console.error(err.toString());
    })

    let start = document.addEventListener("DOMContentLoaded", (event) => {
        console.log("DOM fully loaded and parsed");
        addHandlers();
    });

    let addHandlers = () => {
        var sendButton = document.getElementById("sendButton");

        sendButton.addEventListener("click", (event) => {
            var messageInput = document.getElementById("messageInput");
            var currentMessage = escape(messageInput.value);
            event.preventDefault();
            //message is een property bij het server object.(ook user)
            realtime.hub.connection.invoke("ClientMessage", { message: currentMessage })
                .catch(function (err) {
                    return console.error(err.toString());
                });
        })
    };

    //server messages ------------------------ 
    connection.on("ServerMessage", function (jsonMsg) {
        //message is een eigenschap van het ontvangen jsonMsg
        var messagesList = document.getElementById("messagesList");
        var msg = jsonMsg.message.replace(/&/g, "&amp;")
            .replace(/</g, "&lt;").replace(/>/g, "&gt;");

        var li = document.createElement("li");
        li.textContent = unescape(msg);
        messagesList.appendChild(li);
    });







    //publieke elementen returnen
    return {
        start: start,
        connection   //publiek bekend maken
    };
})();

realtime.hub.start; //de oproep is not a function!