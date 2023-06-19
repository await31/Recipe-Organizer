//Jumping animation
window.addEventListener('load', function () {
    document.getElementById('burger').classList.add('drop');
});
//Img src change 
//when hover
document.getElementById('burger').addEventListener('mouseover', function () {
    this.src = '/images/cute-burger-blush.png';
});

document.getElementById('burger').addEventListener('mouseout', function () {
    this.src = '/images/cute-burger.png';
});
//when input
var passwordEntered = false;

document.getElementById('form3Example4c').addEventListener('input', function () {
    passwordEntered = true;
    document.getElementById('burger').src = '/images/cute-burger-eyecover.png';
});

var inputs = document.querySelectorAll('input');

inputs.forEach(function (input) {
    input.addEventListener('blur', function () {
        if (passwordEntered) {
            document.getElementById('burger').src = '/images/cute-burger.png';
            passwordEntered = false;
        }
    });
});
//Password reveal
document.querySelector('.password-field i').addEventListener('click', function () {
    var passwordInput = document.getElementById('form3Example4c');
    if (passwordInput.type === 'password') {
        passwordInput.type = 'text';
        this.classList.remove('fa-eye-slash');
        this.classList.add('fa-eye');
        document.getElementById('burger').src = '/images/cute-burger-1-eyecover.png';
    } else {
        passwordInput.type = 'password';
        this.classList.remove('fa-eye');
        this.classList.add('fa-eye-slash');
        document.getElementById('burger').src = '/images/cute-burger-eyecover.png';
    }
});
