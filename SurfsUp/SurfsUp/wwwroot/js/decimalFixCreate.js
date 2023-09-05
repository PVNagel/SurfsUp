var dotControlElements = document.getElementsByClassName("dotControl");
var editForm = document.getElementById("createForm");
/*var createPriceError = document.getElementById("createPriceError");*/

editForm.addEventListener("submit", function (event) {
    event.preventDefault();
    var resumeEvent = true;
    for (var i = 0; i < dotControlElements.length; i++) {
        var value = dotControlElements[i].value;
        if (value.includes('.')) {
            resumeEvent = false;
            var parent = dotControlElements[i].parentElement;
            var error = parent.querySelector('.dotControlError');
            if (error == null) {
                var error = document.createElement("span");
                error.className = "text-danger dotControlError";
                error.innerHTML = "Du må ikke bruge punktum som decimal seperator";
                parent.appendChild(error);
            }
            else {
                error.style.display = "display";
            }
        }
        else {
            var parent = dotControlElements[i].parentElement;
            var error = parent.querySelector('.dotControlError');
            if (error !== null) {
                error.style.display = "none";
            }
        }
    }
    if (resumeEvent == false) {
        return false;
    }
    else {
        editForm.submit()
    }
}
    , false);


