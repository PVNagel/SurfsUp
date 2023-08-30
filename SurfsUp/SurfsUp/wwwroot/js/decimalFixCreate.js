var createPrice = document.getElementById("createPrice");
var createForm = document.getElementById("createForm");
var createPriceError = document.getElementById("createPriceError");
createForm.addEventListener("submit", function (event) {
    event.preventDefault();
    var value = createPrice.value;
    if (value.includes('.')) {
        createPriceError.style.display = "block";
        createPriceError.innerText = "Du må ikke bruge punktum som decimal seperator";
        return false;
    }
    else {
        createForm.submit()
    }
}
    , false);


