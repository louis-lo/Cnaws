var Cnaws = typeof (Cnaws) != "undefined" ? Cnaws : {};
Cnaws.Passport = {
    _options: {},
    Init: function (options) {
        Cnaws.Passport._options = options || {};
        Cnaws.Passport._options.targetUrl = Cnaws._undef(Cnaws.Passport._options.targetUrl, Cnaws.getUrl('/ucenter'));
    },
    checkName: function (name) {
        return Cnaws.postAjax(Cnaws.getPassUrl('/register/checkname'), 'Name=' + encodeURIComponent(name), function (data, args) {
            return data.code == -200;
        }, null, false);
    },
    checkEmail: function (email) {
        return Cnaws.postAjax(Cnaws.getPassUrl('/register/checkemail'), 'Email=' + encodeURIComponent(email), function (data, args) {
            return data.code == -200;
        }, null, false);
    },
    checkMobile: function (mobile) {
        return Cnaws.postAjax(Cnaws.getPassUrl('/register/checkmobile'), 'Mobile=' + encodeURIComponent(mobile), function (data, args) {
            return data.code == -200;
        }, null, false);
    },
    sendSms: function (mobile, provider, type) {
        return Cnaws.postAjax(Cnaws.getPassUrl('/register/sendsms/' + provider + '/' + type), 'Mobile=' + encodeURIComponent(mobile), function (data, args) {
            return data.code == -200;
        }, null, false);
    },
    login: function (form, msg) {
        form = $(form);
        msg = $(msg);
        var img = null;
        if (arguments.length > 2) {
            img = $(arguments[2]);
        }
        msg.html('');
        form.attr('disabled', true);
        if (form.jqxValidator('validate')) {
            Cnaws.postAjax(form.attr('action'), form.serialize(), function (data, args) {
                if (data.code == -200) {
                    window.top.location.href = Cnaws.Passport._options.targetUrl;
                }
                else {
                    if (data.code == -1000) {
                        $(args.msg).html('用户名错误');
                    }
                    else if (data.code == -1001) {
                        $(args.msg).html('用户未审核');
                    }
                    else if (data.code == -1002) {
                        $(args.msg).html('密码错误，还剩' + data.data + '次登录机会');
                    }
                    else if (data.code == -1003) {
                        $(args.msg).html('该用户已锁定');
                    }
                    else if (data.code == -1004) {
                        $(args.msg).html('验证码错误');
                    }
                    else if (data.code == -1005) {
                        $(args.msg).html('短信验证码错误');
                    }
                    else if (data.code == -1006) {
                        window.top.location.href = Cnaws.getPassUrl('/oauth2');
                    }
                    else {
                        $(args.msg).html('未知错误');
                    }
                    if (args.img != null) {
                        $(args.img).click();
                    }
                }
                args.form.attr('disabled', false);
            }, { form: form, msg: msg, img: img });
        }
        else {
            form.attr('disabled', false);
        }
        return false;
    },
    register: function (form, msg) {
        form = $(form);
        msg = $(msg);
        var img = null;
        if (arguments.length > 2) {
            img = $(arguments[2]);
        }
        msg.html('');
        form.attr('disabled', true);
        if (form.jqxValidator('validate')) {
            Cnaws.postAjax(form.attr('action'), form.serialize(), function (data, args) {
                if (data.code == -200) {
                    window.top.location.href = Cnaws.Passport._options.targetUrl;
                }
                else {
                    if (data.code == -1) {
                        $(args.msg).html('用户名已存在');
                    }
                    else if (data.code == -2) {
                        $(args.msg).html('用户名或密码格式错误');
                    }
                    else if (data.code == -1000) {
                        $(args.msg).html('用户名错误');
                    }
                    else if (data.code == -1001) {
                        $(args.msg).html('用户未审核');
                    }
                    else if (data.code == -1002) {
                        $(args.msg).html('密码错误，还剩' + data.data + '次登录机会');
                    }
                    else if (data.code == -1003) {
                        $(args.msg).html('该用户已锁定');
                    }
                    else if (data.code == -1004) {
                        $(args.msg).html('验证码错误');
                    }
                    else if (data.code == -1005) {
                        $(args.msg).html('短信验证码错误');
                    }
                    else if (data.code == -1006) {
                        window.top.location.href = Cnaws.getPassUrl('/oauth2');
                    }
                    else {
                        $(args.msg).html('未知错误');
                    }
                    if (args.img != null) {
                        $(args.img).click();
                    }
                }
                args.form.attr('disabled', false);
            }, { form: form, msg: msg, img: img });
        }
        else {
            form.attr('disabled', false);
        }
        return false;
    }
};