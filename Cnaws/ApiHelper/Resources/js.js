if (CLIENT) {
    var alert = function (v) {
        window.external.Alert(v);
    };
}
var JSON = typeof (JSON) != "undefined" ? JSON : {
    parse: function (v) {
        return (new Function("return " + v))();
    }
};
var Helper = {
    Current: null,
    LoadProject: function () {
        if (CLIENT) {
            window.external.LoadProject();
        }
    },
    LoadType: function (t, v) {
        $('#dialogt').html(t);
        var e = $('#' + v);
        var p = $('#dialogc');
        if (Helper.Current != null) {
            Helper.Current.hide();
        }
        Helper.Current = e;
        p.append(e);
        e.show();
        DoResize();
        $('#dialog').show();
    },
    CloseWindow: function () {
        $('#dialog').hide();
    },
    Save: function () {
        if (CLIENT) {
            window.external.Save();
        }
    }
};
function DoResize() {
    if (CLIENT) {
        window.external.DoResize();
    }
    else {
        var w = $(window);
        OnResize(w.width(), w.height());
    }
}
function OnResize(w, h) {
    $('#context').height(h - 50);
    $('#content').height(h - 50);
    var dlg = $('#dialog');
    dlg.css('left', (w - dlg.width()) / 2);
    dlg.css('top', (h - dlg.height()) / 2);
}
$(document).ready(function () {
    $('#project').click(Helper.LoadProject);
    $('#save').click(Helper.Save);
});