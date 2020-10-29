//import in Common.js style
import { config as config } from './config.js';
import * as utilities  from './Utilities/Utilities.js';


var hub = {};


hub.recipesDB = (() => {

    let messagesList = document.getElementById("messagesList");

    //1. Initialisatie vd connectie ------------------------------------------------------
    let remoteHost = config.realTime.scheme + "://" + config.realTime.host + ":" + config.realTime.externalPortRecipes;
    let connection = new signalR.HubConnectionBuilder()
        .withUrl(remoteHost + "/repohub")
        .configureLogging(signalR.LogLevel.Error)
        //.withAutomaticReconnect()
        .build();

    //nodig indien niet in de connection state
    connection.start().catch(function (err) {
        return console.error(err.toString());
    })

    var start = document.addEventListener("DOMContentLoaded", function (evt) {

    });

    //2. HUB SERVER Messages --------------------------------------------------------
    connection.on("ServerMessage", function (jsonMsg) { 
        var parsedJSON = JSON.parse(jsonMsg.message);
        utilities.createTableAsync(parsedJSON, function (error, tableResult) {
            if (error != null) { console.log(error);}
            utilities.insertInDom(document.getElementById("notification") , tableResult);
        });

    });
    return { start };
})();

hub.recipesDB.start;