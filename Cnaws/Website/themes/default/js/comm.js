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
function formatResult(data, msg, cb) {
    if (data.code == -200) return true;
    if (data.code == -401) return '权限不够';
    if (data.code == -404) return '未安装"' + data.data + '"模块';
    if (cb != null) {
        var r = cb(data.code);
        if (r != null) return r.message;
    }
    if (data.data != undefined && typeof (data.data) == 'string') return data.data;
    return msg;
}