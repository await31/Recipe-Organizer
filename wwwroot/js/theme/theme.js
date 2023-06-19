// Theme change
function addCSS1() {
    var link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = "/css/theme/darkly.css";
    document.head.appendChild(link);
}function addCSS2() {
    var link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = "/css/theme/sketchy.css";
    document.head.appendChild(link);
}function addCSS3() {
    var link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = "/css/theme/cyborg.css";
    document.head.appendChild(link);
}function addCSS4() {
    var link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = "/css/theme/vapor.css";
    document.head.appendChild(link);
}function addCSS5() {
    var link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = "/css/theme/quartz.css";
    document.head.appendChild(link);
}function addCSS6() {
    var link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = "/css/theme/superhero.css";
    document.head.appendChild(link);
}function addCSS7() {
    var link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = "/css/theme/slate.css";
    document.head.appendChild(link);
}
// Contrast toggle
// Get the button element with the id of "bd-theme-text"
const themeText = document.querySelector("#bd-theme-text");

// Get all the buttons in the dropdown menu
const themeButtons = document.querySelectorAll(".dropdown-item");

// Add an event listener to each button
themeButtons.forEach((button) => {
    button.addEventListener("click", () => {
        // Update the text of the "bd-theme-text" button to display the selected choice
        themeText.textContent = button.textContent;
    });
});
