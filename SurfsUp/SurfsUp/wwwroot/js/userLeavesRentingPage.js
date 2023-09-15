var antiForgeryToken = document.querySelector('input[name="__RequestVerificationToken"]').value;
var submitClicked = false;

document.getElementById('submit').addEventListener('click', function () {
    submitClicked = true;
});

const beforeUnloadListener = (event) => {
    if (!submitClicked) {
        var data = {
            __RequestVerificationToken: antiForgeryToken
        };

        fetch("/Rentings/RemoveQueuePosition", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
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