export function insertInDom(parent,DOMelement) {
    if (!parent) {
        console.error('Unable to add this to the DOM.');
    }
    parent.append(DOMelement);
}

export function removeFromDom(item, cb) {
    var elem = document.querySelector(item.nodeName);
    elem.parentNode.removeChild(elem);
};


export function createLink(url, cssclass, rel, txt) {
    //<a href="#" class="linkDetailsUser" rel="JohanV">details</a>
    var a = document.createElement("a");
    a.href = url + rel;
    a.setAttribute("class", cssclass);
    a.rel = rel;
    a.append(document.createTextNode(txt));
    return a;
};

export  function createButton(value, cssclass, rel) {
    var btn = document.createElement("button");
    btn.innerHTML = value;
    btn.setAttribute("class", cssclass);
    btn.setAttribute("rel", rel);
    btn.addEventListener("click", function (evt) {
        Utilities.deleteItem(rel);
    });
    return btn;
};


export function createTableAsync(arrJSON, cb) {
    let table = document.createElement("table");
    let tbody = document.createElement("tbody");

    for (var obj in arrJSON) {
        var tr = document.createElement("tr");
        for (var prop in arrJSON[obj]) {
            var td = document.createElement("td");
            td.style.border = "silver solid 1px";
            td.style.padding = "0 5px";
            td.innerHTML = arrJSON[obj][prop];
            tr.append(td);
        }
        tbody.append(tr);
    }
    table.append(tbody);
    cb(null, table);
}