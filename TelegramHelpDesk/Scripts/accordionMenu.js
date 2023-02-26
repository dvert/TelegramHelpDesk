$(document).ready(function () {
    var checkCookie = $.cookie("sub-nav");
    if (checkCookie !== "") {
        $('#submenu>li.sub > a:eq(' + checkCookie + ')').addClass('active').next().show();
    }
    $('#submenu>li.sub>a').click(function() {
        var navIndex = $('#submenu>li.sub>a').index(this);
        $.cookie("sub-nav", navIndex);
        $('#submenu li ul').slideUp();
        if ($(this).next().is(":visible")) {
            $(this).next().slideUp();
        } else {
            $(this).next().slideToggle();
        }
        $('#submenu li a').removeClass('active');
        $(this).addClass('active');
        return false;
    });
    checkCookie = $.cookie("sub-link");
    if (checkCookie !== "") {
        $('#submenu > li.sub > ul li a:eq(' + checkCookie + ')').addClass('active');
    }
    $('.sub ul li a').click(function() {
        var subIndex = $('.sub ul li a').index(this);
        $.cookie("sub-link", subIndex);
        $('.sub ul li a').removeClass('active');
        $(this).addClass('active');
    });
})