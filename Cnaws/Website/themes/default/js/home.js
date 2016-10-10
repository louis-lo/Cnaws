$(document).ready(function () {
    $('#picBox').jqxScrollView({ width: '100%', height: 400, buttonsOffset: [0, 0], slideShow: true, slideDuration: 5000 });
    $('#picBox a').hover(function () {
        $('#picBox').jqxScrollView({ slideShow: false });
        $('#imgPrev').show();
        $('#imgNext').show();
    }, function () {
        $('#imgPrev').hide();
        $('#imgNext').hide();
        $('#picBox').jqxScrollView({ slideShow: true });
    });
    $('#imgPrev').hover(function () {
        $('#imgPrev').show();
        $('#imgNext').show();
    });
    $('#imgNext').hover(function () {
        $('#imgPrev').show();
        $('#imgNext').show();
    });
    $('#imgPrev').click(function () {
        var page = $('#picBox').jqxScrollView('currentPage');
        if (page == 0)
            $('#picBox').jqxScrollView('changePage', $('#picBox a').length - 1);
        else
            $('#picBox').jqxScrollView('back');
    });
    $('#imgNext').click(function () {
        var page = $('#picBox').jqxScrollView('currentPage');
        if (page == ($('#picBox a').length - 1))
            $('#picBox').jqxScrollView('changePage', 0);
        else
            $('#picBox').jqxScrollView('forward');
    });
});