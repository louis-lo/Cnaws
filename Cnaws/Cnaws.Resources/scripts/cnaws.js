var Cnaws = {
    _options: {},
    _notify: false,
    _undef: function (val, defaultVal) {
        return val === undefined ? defaultVal : val;
    },
    Init: function (options) {
        Cnaws._options = options || {};
        Cnaws._options.urlExt = Cnaws._undef(Cnaws._options.urlExt, '.html');
        Cnaws._options.resourcesUrl = Cnaws._undef(Cnaws._options.resourcesUrl, '');
        Cnaws._options.passportUrl = Cnaws._undef(Cnaws._options.passportUrl, '');
    },
    InitNotify: function () {
        $(document.body).append($('<div id="infoNotify" style="display:none;"><div id="infoNotifyContent"></div></div>'));
        $(document.body).append($('<div id="warningNotify" style="display:none;"><div id="warningNotifyContent"></div></div>'));
        $(document.body).append($('<div id="successNotify" style="display:none;"><div id="successNotifyContent"></div></div>'));
        $(document.body).append($('<div id="errorNotify" style="display:none;"><div id="errorNotifyContent"></div></div>'));
        $("#infoNotify").jqxNotification({
            position: "top-right", opacity: 0.9,
            autoOpen: false, animationOpenDelay: 800, autoClose: true, autoCloseDelay: 5000, template: "info"
        });
        $("#warningNotify").jqxNotification({
            position: "top-right", opacity: 0.9,
            autoOpen: false, animationOpenDelay: 800, autoClose: true, autoCloseDelay: 5000, template: "warning"
        });
        $("#successNotify").jqxNotification({
            position: "top-right", opacity: 0.9,
            autoOpen: false, animationOpenDelay: 800, autoClose: true, autoCloseDelay: 5000, template: "success"
        });
        $("#errorNotify").jqxNotification({
            position: "top-right", opacity: 0.9,
            autoOpen: false, animationOpenDelay: 800, autoClose: true, autoCloseDelay: 5000, template: "error"
        });
        Cnaws._notify = true;
    },
    getUrl: function (url) {
        return url + Cnaws._options.urlExt;
    },
    getResUrl: function (url) {
        return Cnaws._options.resourcesUrl + Cnaws.getUrl(url);
    },
    getPassUrl: function (url) {
        return Cnaws._options.passportUrl + Cnaws.getUrl(url);
    },
    getAjax: function (url, func) {
        var args = null;
        var async = true;
        var type = 'json';
        if (arguments.length > 2)
            args = arguments[2];
        if (arguments.length > 3)
            async = arguments[3];
        if (arguments.length > 4)
            type = arguments[4];
        if (async) {
            $.ajax({
                type: "GET",
                url: url,
                dataType: type,
                __func: func,
                __args: args,
                success: function (result) {
                    this.__func(result, this.__args);
                }
            });
        }
        else {
            var result = null;
            var xhr = $.ajax({
                type: "GET",
                url: url,
                data: data,
                dataType: type,
                async: false
            });
            if (type == 'json')
                result = xhr.responseJSON;
            else if (type == 'xml')
                result = xhr.responseXML;
            else
                result = xhr.responseText;
            return func(result, args);
        }
    },
    postAjax: function (url, data, func) {
        var args = null;
        var async = true;
        var type = 'json';
        if (arguments.length > 3)
            args = arguments[3];
        if (arguments.length > 4)
            async = arguments[4];
        if (arguments.length > 5)
            type = arguments[5];
        if (async) {
            $.ajax({
                type: "POST",
                url: url,
                data: data,
                dataType: type,
                __func: func,
                __args: args,
                success: function (result) {
                    this.__func(result, this.__args);
                }
            });
        }
        else {
            var result = null;
            var xhr = $.ajax({
                type: "POST",
                url: url,
                data: data,
                dataType: type,
                async: false
            });
            if (type == 'json')
                result = xhr.responseJSON;
            else if (type == 'xml')
                result = responseXML;
            else
                result = responseText;
            return func(result, args);
        }
    },
    checkCaptcha: function (name, captcha, func) {
        Cnaws.postAjax(Cnaws.getResUrl('/captcha/checkcaptcha'), 'name=' + encodeURIComponent(name) + '&captcha=' + encodeURIComponent(captcha), function (data, args) {
            func && func(data, args);
        });
        return false;
    },
    changeCaptcha: function (img, name) {
        $(img).attr('src', Cnaws.getResUrl('/captcha/custom/' + name) + '?' + parseInt(Math.random() * 10000));
        return false;
    },
    showNotify: function (type, msg) {
        if (!Cnaws._notify)
            Cnaws.InitNotify();
        $('#' + type + 'NotifyContent').html(msg);
        $('#' + type + 'Notify').jqxNotification("open");
        var w = $(window).width();
        var h = $(window).height();
        var div = $('#' + type + 'Notify');
        $('#jqxNotificationDefaultContainer-top-right').css({ position: 'fixed', top: ((h - div.height()) / 2), right: ((w - div.width()) / 2) });
    },
    showInfo: function (msg) {
        Cnaws.showNotify('info', msg);
    },
    showSuccess: function (msg) {
        Cnaws.showNotify('success', msg);
    },
    showWarning: function (msg) {
        Cnaws.showNotify('warning', msg);
    },
    showError: function (msg) {
        Cnaws.showNotify('error', msg);
    },
    closeAllNotify: function () {
        if (Cnaws._notify) {
            $("#infoNotify").jqxNotification("closeAll");
            $("#warningNotify").jqxNotification("closeAll");
            $("#successNotify").jqxNotification("closeAll");
            $("#errorNotify").jqxNotification("closeAll");
        }
    }
};