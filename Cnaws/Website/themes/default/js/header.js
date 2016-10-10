$(document).ready(function () {
    (function () {
        var nav = '/';
        $('#funcTab li').each(function () {
            var iter = $(this).attr('nav');
            if (typeof (iter) == 'string' && iter.length > 1) {
                if (location.pathname.indexOf(iter) === 0)
                    nav = iter;
            }
        });
        $('li[nav="' + nav + '"]').addClass('active');
    })();
    $('a[user=true]').click(function () {
        showLogin();
        return false;
    });
    $('.mcDropMenuBox').hover(function () {
        $(this).addClass('dropMenuBoxActive');
    }, function () {
        $(this).removeClass('dropMenuBoxActive');
    });
    $('#addFavBtn').click(function () {
        try {
            window.external.addFavorite(window.location.href, document.title);
        } catch (e) {
            try {
                window.sidebar.addPanel(document.title, window.location.href, "");
            } catch (e) {
                alert("请使用 Ctrl + D 进行添加");
            }
        }
    });
});