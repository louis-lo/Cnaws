jQuery.fn.scaling = function (width, height, errpic) {
    var container = window;
    if (arguments.length > 3) {
        container = arguments[3];
    }
    var unit = 'px';
    if (arguments.length > 4) {
        unit = arguments[4];
    }
    this.lazyload({
        width: width,
        height: height,
        placeholder: errpic,
        container: container,
        load: function (i, o) {
            var t = $(this);
            var src = t.attr('src');
            var img = new Image();
            img.src = src;
            var load = function () {
                if (img.width > 0 && img.height > 0) {
                    var sx = parseFloat(o.width) / parseFloat(img.width);
                    var sy = parseFloat(o.height) / parseFloat(img.height);
                    var s = Math.min(sx, sy);
                    var cx = img.width * s;
                    if (unit == 'px') {
                        cx = parseInt(Math.floor(cx));
                        if ((cx % 2) == 1) {
                            cx -= 1;
                        }
                    }
                    var cy = img.height * s;
                    if (unit == 'px') {
                        cy = parseInt(Math.floor(cy));
                        if ((cy % 2) == 1) {
                            cy -= 1;
                        }
                    }
                    var x = (o.width - cx) / 2;
                    var y = (o.height - cy) / 2;
                    t.width(cx + unit);
                    t.height(cy + unit);
                    t.css('padding-top', y + unit);
                    t.css('padding-right', x + unit);
                    t.css('padding-bottom', y + unit);
                    t.css('padding-left', x + unit);
                }
            }
            if (img.complete) {
                load();
                return;
            }
            $(img).load(function () {
                load();
            });
        }
    });
}