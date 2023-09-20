var antiForgeryToken = document.querySelector('input[name="__RequestVerificationToken"]').value;
var submitClicked = false;

document.getElementById('submit').addEventListener('click', function () {
    submitClicked = true;
});

const beforeUnloadListener = (event) => {
    if (!submitClicked) {

        fetch("/Rentings/RemoveQueuePosition", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-CSRF-TOKEN': antiForgeryToken
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error("Netværksfejl: " + response.status);
                }
                return response.json();
            })
            .then(data => {
                console.log("success: " + data);
            })
            .catch(error => {
                console.error("Fejl: " + error);
            });
    }
};

window.addEventListener("beforeunload", beforeUnloadListener);