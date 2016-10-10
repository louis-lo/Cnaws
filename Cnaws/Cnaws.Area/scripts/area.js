var Cnaws = typeof (Cnaws) != "undefined" ? Cnaws : {
    getUrl: function (url) {
        return url + Url_Ext;
    },
    getAjax: function (url, func, args) {
        $.ajax({
            type: "GET",
            url: url,
            dataType: 'json',
            __func: func,
            __args: args,
            async: false,
            success: function (data) {
                this.__func(data, this.__args);
            }
        });
    }
};
Cnaws.Area = {
    Types: ['', 'provinces', 'cities', 'counties'],
    Init: function (el, url) {
        var value = 0;
        if (arguments.length > 2)
            value = arguments[2];

        if (value > 0) {
            Cnaws.getAjax(Cnaws.getUrl(url + 'parent/' + value), function (data, args) {
                if (data.code == -200) {
                    if (data.data.length > 0) {
                        Cnaws.Area.onChange(1, args.el, args.url, 0, true, data.data[1].id, data.data);
                    }
                }
            }, { el: el, url: url });
        }
        else {
            Cnaws.Area.onChange(1, el, url, value, true, value, null);
        }
    },
    onChange: function (type, el, url, id, must, def, parent) {
        for (var i = type; i <= 3; ++i) {
            $('#' + el + '_' + Cnaws.Area.Types[i]).remove();
        }
        if (id > 0 || must) {
            Cnaws.getAjax(Cnaws.getUrl(url + Cnaws.Area.Types[type] + '/' + id), function (data, args) {
                if (data.code == -200) {
                    if (data.data.length > 0) {
                        var html = '<select id="' + args.el + '_' + Cnaws.Area.Types[args.type] + '" name="' + args.el + '_' + Cnaws.Area.Types[args.type] + '"';
                        if (args.type < 3)
                            html += ' onchange="Cnaws.Area.onChange(' + (args.type + 1) + ',\'' + args.el + '\',\'' + args.url + '\',this.options[this.options.selectedIndex].value,false,0)"';
                        html += '><option value="0">请选择</option>';
                        for (var i = 0; i < data.data.length; ++i) {
                            html += '<option value="' + data.data[i].id + '"' + (data.data[i].id == args.def ? ' selected="selected"' : '') + '>' + data.data[i].name + '</option>';
                        }
                        html += '</select>';
                        $('#' + args.el).append($(html));
                        if (args.parent != null && args.type < 3) {
                            if (args.type < (args.parent.length - 1)) {
                                Cnaws.Area.onChange(args.type + 1, args.el, args.url, args.parent[args.type].id, true, args.parent[args.type + 1].id, args.parent);
                            }
                            else {
                                Cnaws.Area.onChange(args.type + 1, args.el, args.url, args.def, true, 0, null);
                            }
                        }
                    }
                }
            }, { type: type, el: el, url: url, id: id, def: def, parent: parent });
        }
    },
};
Cnaws.AreaShort = {
    Types: ['', 'provinces', 'cities', 'counties'],
    Init: function (el, url) {
        var value = 0;
        if (arguments.length > 2)
            value = arguments[2];

        if (value > 0) {
            Cnaws.getAjax(Cnaws.getUrl(url + 'parent/' + value), function (data, args) {
                if (data.code == -200) {
                    if (data.data.length > 0) {
                        Cnaws.AreaShort.onChange(1, args.el, args.url, 0, true, data.data[1].id, data.data);
                    }
                }
            }, { el: el, url: url });
        }
        else {
            Cnaws.AreaShort.onChange(1, el, url, value, true, value, null);
        }
    },
    onChange: function (type, el, url, id, must, def, parent) {
        for (var i = type; i <= 3; ++i) {
            $('#' + el + '_' + Cnaws.AreaShort.Types[i]).remove();
        }
        if (id > 0 || must) {
            Cnaws.getAjax(Cnaws.getUrl(url + Cnaws.AreaShort.Types[type] + '/' + id), function (data, args) {
                if (data.code == -200) {
                    if (data.data.length > 0) {
                        var html = '<select id="' + args.el + '_' + Cnaws.AreaShort.Types[args.type] + '" name="' + args.el + '_' + Cnaws.AreaShort.Types[args.type] + '"';
                        if (args.type < 3)
                            html += ' onchange="Cnaws.AreaShort.onChange(' + (args.type + 1) + ',\'' + args.el + '\',\'' + args.url + '\',this.options[this.options.selectedIndex].value,false,0)"';
                        html += '><option value="0">请选择</option>';
                        for (var i = 0; i < data.data.length; ++i) {
                            html += '<option value="' + data.data[i].id + '"' + (data.data[i].id == args.def ? ' selected="selected"' : '') + '>' + data.data[i].sname + '</option>';
                        }
                        html += '</select>';
                        $('#' + args.el).append($(html));
                        if (args.parent != null && args.type < 3) {
                            if (args.type < (args.parent.length - 1)) {
                                Cnaws.AreaShort.onChange(args.type + 1, args.el, args.url, args.parent[args.type].id, true, args.parent[args.type + 1].id, args.parent);
                            }
                            else {
                                Cnaws.AreaShort.onChange(args.type + 1, args.el, args.url, args.def, true, 0, null);
                            }
                        }
                    }
                }
            }, { type: type, el: el, url: url, id: id, def: def, parent: parent });
        }
    }
};