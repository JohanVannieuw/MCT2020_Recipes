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

    let socketColor; //bewaren  binnen de namespace
    let username;

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
            realtime.hub.connection.invoke("ClientMessage",
                {
                    message: currentMessage,
                    user: "" || realtime.hub.username,
                    color: realtime.hub.socketColor
                })
                .catch(function (err) {
                    return console.error(err.toString());
                });
        })
    }
        document.getElementById("selectImg").addEventListener("change", (event) => {
            //meerdere beelden ontvangen onChange
            for (let i = 0; i < event.currentTarget.files.length; i++) {
                let file = event.currentTarget.files[i];  //een object "File" 
                // een base64 reader encodeert het image na volledig inlezen (loadend)
                //fetch verstuurt (POST) de sting naar een api controller (MVC)
                let reader = new FileReader();
                reader.onloadend = (evt) => {
                    let result = reader.result;
                    fetch(remoteHost + "/api/fileupload/uploadfilebyJS",
                        {
                            method: "POST",
                            body: JSON.stringify({
                                "formFile": result,
                                "fileName": file.name
                            }),
                            headers: {
                                'content-type': 'application/json'
                            }
                        })
                        .then((response) => console.log(response));//Accepted return 
                };
                reader.readAsDataURL(file); //leest in als base 64
            };
    });

    //server messages ------------------------

    //ontvangst van ServerMessages
    connection.on('Login', (jsonMsg) => {
        //user bijhouden als hub variabele
        realtime.hub.username = prompt('Kies een gebruikersnaam');
        connection.invoke("Login", { user: realtime.hub.username });
    });

    connection.on("ServerMessage", function (jsonMsg) {
        //message is een eigenschap van het ontvangen jsonMsg
        var messagesList = document.getElementById("messagesList");
        var msg = jsonMsg.message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        //message configuraties of properties 
        realtime.hub.socketColor = realtime.hub.socketColor ? realtime.hub.socketColor : jsonMsg.color;
       // realtime.hub.username = realtime.hub.username ? realtime.hub.username: jsonMsg.user;


        var li = document.createElement("li");
        li.style.color = jsonMsg.color;
        li.textContent = (jsonMsg.user != undefined ? jsonMsg.user + ": " : "") +unescape(msg);
        messagesList.insertBefore(li, messagesList.childNodes[0]);
        //input ledigen
        document.getElementById("messageInput") = "";
    });

    connection.on('UserImage', function (base64Image) {
        var imgElement = document.createElement("img");
        imgElement.src = base64Image;

        imgElement.width = 75; // zonder px    
        var div = document.createElement("div");
        div.style.position = "relative";
        div.style.display = "block";
        div.style.margin = 10 + "px";
        div.appendChild(imgElement);

        document.getElementById("messagesList").insertBefore(div, messagesList.childNodes[0]);
    });






    //publieke elementen returnen
    return {
        start: start,
        connection   //publiek bekend maken
    };
})();

realtime.hub.start; //de oproep is not a function!