Date.prototype.format = function (format) {
    var o = {
        'M+': this.getMonth() + 1,
        'd+': this.getDate(),
        'h+': this.getHours(),
        'm+': this.getMinutes(),
        's+': this.getSeconds(),
        'q+': Math.floor((this.getMonth() + 3) / 3),
        'S': this.getMilliseconds()
    };
    if (/(y+)/.test(format) || /(Y+)/.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear() + '').substr(4 - RegExp.$1.length));
    }
    for (var k in o) {
        if (new RegExp('(' + k + ')').test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ('00' + o[k]).substr(('' + o[k]).length));
        }
    }
    return format;
};
function getTextElementByColor(color) {
    if (color == 'transparent' || color.hex == "") {
        return $("<div style='text-shadow:none;position:relative;padding-bottom:2px;margin-top:2px;'>transparent</div>");
    }
    var element = $("<div style='text-shadow:none;position:relative;padding-bottom:2px;margin-top:2px;'>#" + color.hex + "</div>");
    var nThreshold = 105;
    var bgDelta = (color.r * 0.299) + (color.g * 0.587) + (color.b * 0.114);
    var foreColor = (255 - bgDelta < nThreshold) ? 'Black' : 'White';
    element.css('color', foreColor);
    element.css('background', "#" + color.hex);
    element.addClass('jqx-rc-all');
    return element;
}
function getUrl(url) {
    return url + Url_Ext;
}
function showNotify(type, msg) {
    $('#' + type + 'NotifyContent').html(msg);
    $('#' + type + 'Notify').jqxNotification("open");
}
function showResult(data, s, f, c) {
    if (data.code == -200) {
        if (s != null)
            showNotify('success', s);
        return true;
    }
    if (data.code == -401) {
        showNotify('warning', '权限不够');
    }
    else if (data.code == -404) {
        showNotify('warning', '未安装"' + data.data + '"模块');
    }
    else {
        if (c != null) {
            var r = c(data.code);
            if (r != null) {
                showNotify(r.type, r.message);
                return false;
            }
        }
        if (data.data != undefined && typeof (data.data) == 'string') {
            showNotify('error', data.data);
        }
        else {
            if (f != null)
                showNotify('error', f);
        }
    }
    return false;
}
function loadUrl(url) {
    __window.destroy();
    __dataTable.destroy();
    $('#context form').each(function () {
        try { $(this).jqxValidator('hide'); }
        catch (e) { }
    });
    $('#context').empty();
    $.ajax({
        type: "GET",
        url: url,
        dataType: 'html',
        success: function (data) {
            $('#context').html(data);
        }
    });
}
function enableWindow(enabled) {
    $('#ok').attr('disabled', !enabled);
    $('#cancel').attr('disabled', !enabled);
}
function getAjax(url, func, args) {
    $.ajax({
        type: "GET",
        url: url,
        dataType: 'json',
        __func: func,
        __args: args,
        success: function (data) {
            this.__func(data, this.__args);
        }
    });
}
function postAjax(url, data, func, args) {
    $.ajax({
        type: "POST",
        url: url,
        data: data,
        dataType: 'json',
        __func: func,
        __args: args,
        success: function (data) {
            this.__func(data, this.__args);
        }
    });
}
function doPassword() {
    $('#oldpass').val('');
    $('#newpass').val('');
    $('#conpass').val('');
    $('#winpass').jqxWindow('open');
}
function loadHome() {
    if (SELECTED_MENU != null) {
        $('#' + SELECTED_MENU).removeClass('selected');
        SELECTED_MENU = null;
    }
    $('#cnav').html('首页');
    loadUrl(Welcome_Url);
}
var SELECTED_NAV = null;
$(document).ready(function () {
    $("#main").jqxDockPanel({ width: '100%', height: '100%' });
    $('#navbar').jqxDockPanel({ width: '100%', height: '100%' });
    $('#content').jqxDockPanel({ width: '100%', height: '100%' });
    for (var i = 0; i < MAINMENUS.length; ++i) {
        $('#navs').append($('<li><a href="javascript:;"><img src="' + MAINMENUS[i].icon + '" alt="' + MAINMENUS[i].name + '" /><h2>' + MAINMENUS[i].name + '</h2></a></li>'));
    }
    $('#navs a').click(function () {
        if (SELECTED_NAV != $(this)) {
            $('#navtitle').html($(this).text() + '设置');
            if (SELECTED_NAV != null)
                SELECTED_NAV.removeClass('selected');
            SELECTED_NAV = $(this);
            SELECTED_NAV.addClass('selected');
            loadMenus($('li').index($(this).parent()));
        }
        return false;
    });
    $('#navs li:eq(0) a:eq(0)').click();
    $('#winpass').jqxWindow({
        height: 'auto',
        width: 'auto',
        autoOpen: false,
        resizable: false,
        draggable: false,
        isModal: true,
        modalOpacity: 0.3,
        cancelButton: $('#passcancel'),
        initContent: function () {
            $('#passok').focus();
        }
    });
    $('#passform').jqxValidator({
        rules: [
            { input: '#oldpass', message: '旧密码不能为空!', action: 'keyup, blur', rule: 'required' },
            { input: '#oldpass', message: '旧密码长度必须为6～32位!', action: 'keyup, blur', rule: 'length=6,32' },
            { input: '#newpass', message: '新密码不能为空!', action: 'keyup, blur', rule: 'required' },
            { input: '#newpass', message: '新密码长度必须为6～32位!', action: 'keyup, blur', rule: 'length=6,32' },
            { input: '#conpass', message: '确认密码不能为空!', action: 'keyup, blur', rule: 'required' },
            {
                input: '#conpass', message: '两次密码不一致!', action: 'keyup, focus', rule: function (input, commit) {
                    if (input.val() === $('#newpass').val()) {
                        return true;
                    }
                    return false;
                }
            }
        ]
    });
    $('#passok').on('click', function (e) {
        $('#passok').attr('disabled', true);
        $('#passcancel').attr('disabled', true);
        var form = $('#passform');
        if (form.jqxValidator('validate')) {
            $.ajax({
                type: "POST",
                url: form.attr('action'),
                data: form.serialize(),
                dataType: 'json',
                success: function (data) {
                    showResult(data, '密码修改成功', '旧密码错误或新密码修改失败', null);
                    $('#winpass').jqxWindow('close');
                    $('#passok').attr('disabled', false);
                    $('#passcancel').attr('disabled', false);
                }
            });
        }
        else {
            $('#passok').attr('disabled', false);
            $('#passcancel').attr('disabled', false);
        }
        return false;
    });
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
    $("#loader").jqxLoader({ width: 100, height: 60, imagePosition: 'top' });
    loadUrl(Welcome_Url);
});
var __window = {
    id: null,
    form: null,
    idName: null,
    validate: null,
    openUrl: null,
    opened: false,
    open: null,
    submit: false,
    add: null,
    mod: null,
    close: function () {
        $('#' + __window.id).jqxWindow('close');
    },
    end: function () {
        enableWindow(true);
        __window.submit = false;
    },
    reset: function () {
        __window.destroy();
        __window.idName = null;
        __window.validate = null;
        __window.openUrl = null;
        __window.opened = false;
        __window.open = null;
        __window.submit = false;
        __window.add = null;
        __window.mod = null;
        $('#ok').click(null);
    },
    destroy: function () {
        if (__window.id != null) {
            $('#' + __window.id).jqxWindow('destroy');
            __window.id = null;
        }
    }
};
//id:弹出框ID  form:formId  rules：要验证规则数组  idName:获取数据ID的控件ID　 validate:验证方法  openUrl：ajax地址 open:打开后处理方法   add,添加方法, mod:修改方法
function initWindow(id, form, rules, idName, validate, openUrl, open, add, mod) {
    __window.reset();
    __window.id = id;
    __window.form = form;
    __window.idName = idName;
    __window.validate = validate;
    __window.openUrl = openUrl;
    __window.open = open;
    __window.add = add;
    __window.mod = mod;
    $('#' + __window.id).jqxWindow({
        width: 'auto',
        height: 'auto',
        autoOpen: false,
        resizable: false,
        draggable: false,
        isModal: true,
        modalOpacity: 0.3,
        //okButton:$('#ok'),
        cancelButton: $('#cancel'),
        initContent: function () {
            $('#ok').focus();
        }
    });
    if (rules != null) {
        $('#' + __window.form).jqxValidator({
            rules: rules
        });
    }
    $('#' + __window.id).on('open', function (e) {
        if (!__window.opened) {
            __window.opened = true;
            if (__window.openUrl == null) {
                __window.open();
                __window.opened = false;
            }
            else {
                getAjax(__window.openUrl, function (data, args) {
                    __window.open(data, args);
                    __window.opened = false;
                }, null);
            }
        }
    });
    $('#ok').click(function (e) {
        if (!__window.submit) {
            __window.submit = true;
            enableWindow(false);
            var form = $('#' + __window.form);
            var aid = null;
            if (__window.idName != null)
                aid = $('#' + __window.idName).val();
            var isok = false;
            if (__window.validate != null)
                isok = __window.validate(form, aid);
            else
                isok = form.jqxValidator('validate');
            if (isok) {
                if (__window.idName == null) {
                    __window.add(form);
                }
                else {
                    if (aid == "0")
                        __window.add(form);
                    else
                        __window.mod(form);
                }
            }
            else {
                __window.end();
            }
        }
        return false;
    });
    return __window;
}
var __dataTable = {
    id: null,
    url: null,
    source: {
        datatype: "array",
        datafields: null,
        updaterow: null,
        pagesize: 10,
        localdata: []
    },
    dataAdapter: null,
    pageIndex: -1,
    pageType: -2147483648,
    tempData: [],
    renderRow: function (params) {
        var page = params.endindex / 10;
        if (__dataTable.pageIndex != page) {
            var data = $.ajax({
                type: "GET",
                url: getUrl(__dataTable.url + ((__dataTable.pageType != -2147483648) ? (__dataTable.pageType + '/') : '') + page),
                dataType: 'json',
                async: false
            }).responseJSON;
            __dataTable.source.totalrecords = data.data.total;
            __dataTable.tempData = data.data.data;
            __dataTable.pageIndex = page;
        }
        return __dataTable.tempData;
    },
    loadPage: function (page) {
        if (__dataTable.pageIndex != page) {
            getAjax(getUrl(__dataTable.url + page), function (data, args) {
                if (data.code == -200) {
                    if (data.data) {
                        __dataTable.tempData = data.data.data;
                        __dataTable.source.totalrecords = data.data.total;
                    }
                }
                __dataTable.pageIndex = args;
                $('#' + __dataTable.id).jqxGrid('updatebounddata');
            }, page);
        }
    },
    loadTypePage: function (type, page) {
        if (__dataTable.pageType != type || __dataTable.pageIndex != page) {
            getAjax(getUrl(__dataTable.url + type + '/' + page), function (data, args) {
                if (data.code == -200) {
                    if (data.data) {
                        __dataTable.tempData = data.data.data;
                        __dataTable.source.totalrecords = data.data.total;
                    }
                }
                __dataTable.pageType = args.type;
                __dataTable.pageIndex = args.page;
                $('#' + __dataTable.id).jqxGrid('updatebounddata');
            }, { type: type, page: page });
        }
    },
    loadCustomPage: function (page, url, params, func) {
        getAjax(url, function (data, args) {
            if (data.code == -200) {
                if (data.data) {
                    __dataTable.tempData = data.data.data;
                    __dataTable.source.totalrecords = data.data.total;
                }
            }
            __dataTable.pageIndex = args.page;
            args.func(args.params);
            $('#' + __dataTable.id).jqxGrid('updatebounddata');
        }, { page: page, params: params, func: func });
    },
    reloadPage: function () {
        var index = __dataTable.pageIndex;
        __dataTable.pageIndex = -1;
        __dataTable.tempData = [];
        __dataTable.source.totalrecords = 0;
        if (__dataTable.pageType != -2147483648)
            __dataTable.loadTypePage(__dataTable.pageType, index);
        else
            __dataTable.loadPage(index);
    },
    reset: function () {
        __dataTable.destroy();
        __dataTable.url = null;
        __dataTable.source = {
            datatype: "array",
            datafields: null,
            updaterow: null,
            pagesize: 10,
            localdata: []
        };
        __dataTable.dataAdapter = null;
        __dataTable.pageIndex = -1;
        __dataTable.pageType = -2147483648;
        __dataTable.tempData = [];
    },
    destroy: function () {
        if (__dataTable.id != null) {
            $('#' + __dataTable.id).jqxGrid('destroy');
            __dataTable.id = null;
        }
    },
    get: function (row, name) {
        try { return __dataTable.tempData[row = row % __dataTable.source.pagesize][name]; }
        catch (e) { }
        return "";
    }
};
//id:数据表id   url:获取数据ajax的url fields：要数据字段及类型 columns：数据对应绑定的控件类型和ID toolbar：自定义工具条 
function initDataTable(id, url, fields, columns, toolbar, update) {
    __dataTable.reset();
    __dataTable.id = id;
    __dataTable.url = url;
    __dataTable.source.datafields = fields;
    __dataTable.source.updaterow = update;
    __dataTable.source.localdata = [];
    __dataTable.dataAdapter = new $.jqx.dataAdapter(__dataTable.source);
    __dataTable.pageIndex = -1;
    __dataTable.pageType = -2147483648;
    __dataTable.tempData = [];
    $('#' + __dataTable.id).jqxGrid({
        width: 960,
        height: 350,
        source: __dataTable.dataAdapter,
        pageable: true,
        pagermode: 'simple',
        virtualmode: true,
        rendergridrows: __dataTable.renderRow,
        editable: (__dataTable.source.updaterow != null) ? true : false,
        editmode: 'selectedcell',
        selectionmode: 'singlerow',
        columnsresize: true,
        columns: columns,
        showtoolbar: (toolbar != null) ? true : false,
        rendertoolbar: toolbar,
        rowsheight: (arguments.length > 6) ? arguments[6] : 25
    });
    return __dataTable;
}