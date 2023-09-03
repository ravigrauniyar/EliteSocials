﻿function GetElement(id) {
    return document.getElementById(id);
}
function ToggleFormTo(formType) {

    var loginFormContainer = GetElement("loginFormContainer");
    var signUpFormContainer = GetElement("signUpFormContainer");
    var loginFormFooter = GetElement("loginFormFooter");
    var signUpFormFooter = GetElement("signUpFormFooter");

    loginFormContainer.classList.toggle('d-none', formType !== "login");
    signUpFormContainer.classList.toggle('d-none', formType === "login");

    loginFormFooter.classList.toggle('d-none', formType !== "login");
    signUpFormFooter.classList.toggle('d-none', formType === "login");
}