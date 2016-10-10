function initGotop() {
    if ($(window).scrollTop() > 0)
        $('#gotop').show();
    else
        $('#gotop').hide();
}
$(window).scroll(function () {
    initGotop();
});
$('#gotoplink').click(function () {
    $(window).scrollTop(0);
});
initGotop();