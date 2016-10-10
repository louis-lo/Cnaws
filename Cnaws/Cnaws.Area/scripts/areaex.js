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
            success: function (data) {
                this.__func(data, this.__args);
            }
        });
    }
};
Cnaws.Area = {
    Types: ['', 'provinces', 'cities', 'counties'],
    Scrolls: [null, null, null, null],
    Init: function (el, url) {
        var value = 0;
        var hide = true;
        if (arguments.length > 2)
            value = arguments[2];
        if (arguments.length > 3)
            hide = arguments[3];

        var t = $('#' + el);
        t.empty();
        t.append($('<input type="text" id="' + el + '_value" el-data="' + el + '" url-data="' + url + '" readonly="readonly" />'));
        t.append($('<input id="' + el + '_provinces" type="hidden" name="' + el + '_provinces" />'));
        t.append($('<input id="' + el + '_cities" type="hidden" name="' + el + '_cities" />'));
        t.append($('<input id="' + el + '_counties" type="hidden" name="' + el + '_counties" />'));
        t.append($('<div id="' + el + '_wrapper"' + (hide ? ' style="display:none"' : '') + '><div class="header"><a href="javascript:void(0)" onclick="$(this).parent().parent().hide()">完成</a></div><div id="' + el + '_select"></div><div id="' + el + '_provinces_wrapper" class="wrapper"><div id="' + el + '_provinces_scroller" class="scroller"><ul class="content"><li value-data="0">&nbsp;</li><li value-data="0">&nbsp;</li><li value-data="0">&nbsp;</li><li value-data="0">&nbsp;</li></ul></div></div><div id="' + el + '_cities_wrapper" class="wrapper"><div id="' + el + '_cities_scroller" class="scroller"><ul class="content"><li value-data="0">&nbsp;</li><li value-data="0">&nbsp;</li><li value-data="0">&nbsp;</li><li value-data="0">&nbsp;</li></ul></div></div><div id="' + el + '_counties_wrapper" class="wrapper"><div id="' + el + '_counties_scroller" class="scroller"><ul class="content"><li value-data="0">&nbsp;</li><li value-data="0">&nbsp;</li><li value-data="0">&nbsp;</li><li value-data="0">&nbsp;</li></ul></div></div></div>'));

        $('#' + el + '_value').click(function () {
            if ($('#' + $(this).attr('el-data') + '_wrapper').is(":hidden")) {
                Cnaws.Area.Init($(this).attr('el-data'), $(this).attr('url-data'), $('#' + el + '_counties').val(), false);
            }
        });

        for (var i = 1; i <= 3; ++i) {
            Cnaws.Area.Scrolls[i] = new IScroll('#' + el + '_' + Cnaws.Area.Types[i] + '_wrapper', {
                //snap: 'li',
                scrollbars: false,
                mouseWheel: true,
                shrinkScrollbars: 'scale',
                probeType: 3
            });
            Cnaws.Area.Scrolls[i].on('scroll', function () {
                $(this).trigger('scroll');
            });
            Cnaws.Area.Scrolls[i].on('scrollEnd', function () {
                var wrap = $(this.wrapper);
                var ul = wrap.find('ul:eq(0)');
                var type = parseInt(ul.attr('type-data'));
                var el = ul.attr('el-data');
                var top = Math.abs(this.y);
                var height = ul.find('li:eq(0)').get(0).offsetHeight;
                var index = parseInt(Math.round(parseFloat(top) / parseFloat(height))) + 2;
                var li = ul.find('li:eq(' + index + ')');
                var y = height * index;
                if (top != y) {
                    this.scrollTo(this.x, -y + height * 2);
                }
                var value = li.attr('value-data');
                $('#' + el + '_' + ul.attr('key-data')).val(value);
                $('#' + el + '_' + ul.attr('key-data')).attr('name-data', li.text());
                Cnaws.Area.val(el);
                if (type < 3) {
                    Cnaws.Area.onChange(type + 1, el, ul.attr('url-data'), value, false, 0);
                }
            });
        }

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
    val: function (el) {
        $('#' + el + '_value').val($('#' + el + '_provinces').attr('name-data') + ' ' + $('#' + el + '_cities').attr('name-data') + ' ' + $('#' + el + '_counties').attr('name-data'));
    },
    onChange: function (type, el, url, id, must, def, parent) {
        if (id > 0 || must) {
            Cnaws.getAjax(Cnaws.getUrl(url + Cnaws.Area.Types[type] + '/' + id), function (data, args) {
                if (data.code == -200) {
                    if (data.data.length > 0) {
                        var def = -1;
                        var defn = '';
                        var first = 0;
                        var firstn = '';
                        var scroll = Cnaws.Area.Scrolls[args.type];
                        var ul = $(scroll.wrapper).find('ul:eq(0)');
                        ul.attr('key-data', Cnaws.Area.Types[args.type]);
                        ul.attr('type-data', args.type);
                        ul.attr('el-data', args.el);
                        ul.attr('url-data', args.url);
                        var html = '<li value-data="0">&nbsp;</li><li value-data="0">&nbsp;</li>';
                        for (var i = 0; i < data.data.length; ++i) {
                            if (i == 0) {
                                first = data.data[i].id;
                                firstn = data.data[i].name;
                            }
                            if (data.data[i].id == args.def) {
                                def = i;
                                defn = data.data[i].name;
                            }
                            html += '<li value-data="' + data.data[i].id + '">' + data.data[i].name + '</li>';
                        }
                        html += '<li value-data="0">&nbsp;</li><li value-data="0">&nbsp;</li>';
                        ul.html(html);
                        scroll.refresh();

                        var hidden = $('#' + args.el + '_' + Cnaws.Area.Types[args.type]);
                        if (def > -1) {
                            var li = $(scroll.wrapper).find('li:eq(0)').get(0);
                            scroll.scrollTo(scroll.x, -(li.offsetHeight * def));
                            hidden.val(args.def);
                            hidden.attr('name-data', defn);
                        }
                        else {
                            scroll.scrollTo(scroll.x, 0);
                            hidden.val(first);
                            hidden.attr('name-data', firstn);
                        }
                        Cnaws.Area.val(args.el);
                        if (args.type < 3) {
                            if (args.parent != null) {
                                if (args.type < (args.parent.length - 1)) {
                                    Cnaws.Area.onChange(args.type + 1, args.el, args.url, args.parent[args.type].id, true, args.parent[args.type + 1].id, args.parent);
                                }
                                else {
                                    Cnaws.Area.onChange(args.type + 1, args.el, args.url, args.def, true, 0, null);
                                }
                            }
                            else {
                                Cnaws.Area.onChange(args.type + 1, args.el, args.url, first, false, 0);
                            }
                        }
                    }
                }
            }, { type: type, el: el, url: url, id: id, def: def, parent: parent });
        }
    }
};