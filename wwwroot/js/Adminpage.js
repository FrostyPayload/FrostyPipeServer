

function SendReloadConfig() {
    const xhttp = new XMLHttpRequest();
    xhttp.onload = function () {
        GetServerConfig();
    }
    xhttp.open("GET", "/reloadconfig", true);
    xhttp.send();
}

function GetServerConfig() {
    const xhttp = new XMLHttpRequest();
    xhttp.onload = function () {
        var obj = JSON.parse(this.responseText)
        document.getElementById("mplayers").innerText = obj["Maxplayers"];
        document.getElementById("trMax").innerText = obj["TickrateMax"];
        document.getElementById("trMin").innerText = obj["TickrateMin"];
        document.getElementById("port").innerText = obj["Port"];

    }
    xhttp.open("GET", "/serverconfig", true);
    xhttp.send();

}

function GetServerStats() {
    const xhttp = new XMLHttpRequest();
    xhttp.onload = function () {
        var obj = JSON.parse(this.responseText)
        document.getElementById("ramused").innerText = obj["Maxplayers"];
        document.getElementById("cpuused").innerText = obj["TickrateMax"];
        document.getElementById("trnow").innerText = obj["TickrateMin"];
        document.getElementById("players").innerText = obj["Port"];

    }
    xhttp.open("GET", "/serverstats", true);
    xhttp.send();

}


GetServerConfig();