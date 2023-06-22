var darkmodeactive = localStorage.getItem("darkmode");

window.addEventListener('load', function () {
    if (!darkmodeactive || darkmodeactive === "") {
        localStorage.setItem('darkmode', false);
    }
});


function labelLight() {
    $(".toggle-switch").attr("alt", "Go dark");
    $(".toggle-switch").attr("title", "Go dark");
}
function goLight() {
    labelLight();
    $("body").removeClass("dark");
    //refreshFavicon();
}
function stayLight() {
    goLight();
    localStorage.setItem("darkmode", false);
    darkmodeactive = localStorage.getItem("darkmode");
}

function labelDark() {
    $(".toggle-switch").attr("alt", "Go light");
    $(".toggle-switch").attr("title", "Go light");
}
function goDark() {
    labelDark();
    $("body").addClass("dark");
    //refreshFavicon();
}
function stayDark() {
    goDark();
    localStorage.setItem("darkmode", true);
    darkmodeactive = localStorage.getItem("darkmode");
}
window.matchMedia("(prefers-color-scheme: light)").addListener(e => e.matches && stayLight());
window.matchMedia("(prefers-color-scheme: dark)").addListener(e => e.matches && stayDark());

$(".toggle-switch").click(function () {
    if ($("body").hasClass("dark")) {
        stayLight();
    } else {
        stayDark();
    }
});
$(".label-light").click(function () {
    if ($("body").hasClass("dark")) {
        stayLight();
    }
});
$(".label-dark").click(function () {
    if (!$("body").hasClass("dark")) {
        stayDark();
    }
});
window.onload = function () {
    if (localStorage.darkmode == "true") {
        goDark();
    } else if (localStorage.darkmode == "false") {
        goLight();
    } else {
        if ($("body").hasClass("dark")) {
           labelDark();
        } else {
           labelLight();
        }
    }
};
function tempDisableAnim() {
    $("*").addClass("disableEasingTemporarily");
    setTimeout(function () {
        $("*").removeClass("disableEasingTemporarily");
    }, 200);
}
setTimeout(function () {
    $(".load-flash").css("display", "none");
    $(".load-flash").css("visibility", "hidden");
    tempDisableAnim();
}, 200);
$(window).resize(function () {
    tempDisableAnim();
    setTimeout(function () {
        tempDisableAnim();
    }, 0);
});
//function refreshFavicon() {
//    if (matchMedia('(prefers-color-scheme: dark)').matches) {
//        var link = document.querySelector("link[rel*='icon']") || document.createElement('link');
//        link.type = 'image/x-icon';
//        link.rel = 'icon';
//        link.href = 'favicon-dark.svg';
//        document.getElementsByTagName('head')[0].appendChild(link);
//    } else {
//        var link = document.querySelector("link[rel*='icon']") || document.createElement('link');
//        link.type = 'image/x-icon';
//        link.rel = 'icon';
//        link.href = 'favicon.svg';
//        document.getElementsByTagName('head')[0].appendChild(link);
//    }
//}
