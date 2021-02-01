// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


// Function to write a token to browser storage
//function login(loginElement, passwordElement) {
//    const payload = JSON.stringify({
//        "Email": loginElement.value,
//        "Password": passwordElement.value
//    });  

//    const postOptions = {
//        method: "POST",
//        headers: { "Content-Type": "application/json" },
//        body: payload
//    }

//    fetch("https://localhost:44317/user/login", postOptions)
//        .then((res) => {
//            if (res.status == 200) {
//                return res.json()
//            } else {
//                throw Error(res.statusText)
//            }
//        })
//        .then(data => {
//            localStorage.setItem("token", data.token)
//            logResponse("loginResponse", `localStorage set with token value: ${data.token}`)
//        })
//        .catch(console.error)
//}

//const loginElement = document.querySelector(".login-input");
//const passwordElement = document.querySelector(".password-input");


//const loginButton = document.querySelector("button").addEventListener("click", () => { login(loginElement, passwordElement) });



// Modal Config
const modal = document.getElementById("myModal");
const modalMessage = document.getElementById("modalMessage").textContent;

// Get the <span> element that closes the modal
const span = document.getElementsByClassName("close")[0];

// When the user clicks on <span> (x), close the modal
span.onclick = function () {
    modal.style.display = "none";
}

// When the user clicks anywhere outside of the modal, close it
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}

// When message to show is set open modal window
if (modalMessage) {
    modal.style.display = "block";
}
