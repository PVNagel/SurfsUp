var antiForgeryToken = document.querySelector('input[name="__RequestVerificationToken"]').value;
var submitClicked = false;

document.getElementById('submit').addEventListener('click', function () {
    submitClicked = true;
});

function getClientIP() {
    fetch("/Rentings/GetIpAddress")
        .then(response => response.json())
        .then(data => {
            var clientIP = data.clientIP;
            return clientIP;
        })
        .catch(error => {
            console.error('Fejl:', error);
            return "error";
        });
}

const beforeUnloadListener = (event) => {
    if (!submitClicked) {
        var clientIP = getClientIP();
        var data = { clientIP: clientIP };
        var jsonData = JSON.stringify(data);
        fetch("https://localhost:7022/v1/RentingsAPI/RemoveQueuePosition", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-CSRF-TOKEN': antiForgeryToken
            },
            body: jsonData
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