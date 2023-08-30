var editPrice = document.getElementById("editPrice");
var editForm = document.getElementById("editForm");
var editPriceError = document.getElementById("editPriceError");
editForm.addEventListener("submit", function (event) {
    event.preventDefault();
    var value = editPrice.value;
    if (value.includes('.')) {
        editPriceError.style.display = "block";
        editPriceError.innerText = "Du må ikke bruge punktum som decimal seperator";
        return false;
    }
    else {
        editForm.submit()
    }
}
    , false);
