$(document).on('click', '#forgotPasswordLink', function (e) {
    e.preventDefault();
    var email = $("#email").val();
    var url = `Auth/ForgotPassword`;
    if (email != "") {
        url += `?email=${encodeURIComponent(email)}`;
    }
    window.location.href = url;
});

$(document).ready(function () {
    const passField = document.getElementById('password');
    togglePassword.addEventListener('click', function (e) {
        const type = passField.getAttribute('type') === 'password' ? 'text' : 'password';
        passField.setAttribute('type', type);
        this.classList.toggle('fa-eye');
        this.classList.toggle('fa-eye-slash');
    });
});