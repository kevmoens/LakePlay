window.getElementWidth = (elementId) => {
    let elems = document.getElementsByClassName(elementId);
    for (var i = 0; i < elems.length; i++) {
        return elems[i].offsetWidth;
    }

};

window.getElementHeight = (elementId) => {
    let elems = document.getElementsByClassName(elementId);
    for (var i = 0; i < elems.length; i++) {
        return elems[i].offsetHeight;
    }
};