/*******************************************************************************
* KindEditor - WYSIWYG HTML Editor for Internet
* Copyright (C) 2006-2011 kindsoft.net
*
* @author Roddy <luolonghao@gmail.com>
* @site http://www.kindsoft.net/
* @licence http://www.kindsoft.net/license.php
*******************************************************************************/

// Baidu Maps: http://dev.baidu.com/wiki/map/index.php?title=%E9%A6%96%E9%A1%B5

KindEditor.plugin('coordinates', function (K) {
    var self = this, name = 'coordinates', lang = self.lang('baidumap.');
    self.plugin.coordinatesDialog = function (options) {
        var mapWidth = K.undef(options.mapWidth, 558);
        var mapHeight = K.undef(options.mapHeight, 360);
        var mapLng = K.undef(options.mapLng, 121.473704);
        var mapLat = K.undef(options.mapLat, 31.230393);
        var clickFn = options.clickFn;
        var html = ['<div style="padding:10px 20px;">',
			'<div class="ke-header">',
			// left start
			'<div class="ke-left">',
			lang.address + ' <input id="kindeditor_plugin_map_address" name="address" class="ke-input-text" value="" style="width:200px;" /> ',
			'<span class="ke-button-common ke-button-outer">',
			'<input type="button" name="searchBtn" class="ke-button-common ke-button" value="' + lang.search + '" />',
			'</span>',
			'</div>',
			// right start
			'<div class="ke-right">',
			//'<input type="checkbox" id="keInsertDynamicMap" name="insertDynamicMap" value="1" /> <label for="keInsertDynamicMap">' + lang.insertDynamicMap + '</label>',
			'</div>',
			'<div class="ke-clearfix"></div>',
			'</div>',
			'<div class="ke-map" style="width:' + mapWidth + 'px;height:' + mapHeight + 'px;"></div>',
			'</div>'].join('');
        var dialog = self.createDialog({
            name: name,
            width: mapWidth + 42,
            title: self.lang(name),
            body: html,
            yesBtn: {
                name: self.lang('yes'),
                click: function (e) {
                    var map = win.map;
                    var centerObj = map.getCenter();
                    clickFn.call(self, centerObj.lng, centerObj.lat, map.getZoom());
                }
            },
            beforeRemove: function () {
                searchBtn.remove();
                if (doc) {
                    doc.write('');
                }
                iframe.remove();
            }
        });
        var div = dialog.div,
			addressBox = K('[name="address"]', div),
			searchBtn = K('[name="searchBtn"]', div),
			win, doc;
        var iframe = K('<iframe class="ke-textarea" frameborder="0" src="' + self.pluginsPath + 'coordinates/coordinates.html?' + mapLng + '|' + mapLat + '" style="width:' + mapWidth + 'px;height:' + mapHeight + 'px;"></iframe>');
        function ready() {
            win = iframe[0].contentWindow;
            doc = K.iframeDoc(iframe);
        }
        iframe.bind('load', function () {
            iframe.unbind('load');
            if (K.IE) {
                ready();
            } else {
                setTimeout(ready, 0);
            }
        });
        K('.ke-map', div).replaceWith(iframe);
        // search map
        searchBtn.click(function () {
            win.search(addressBox.val());
        });
    };
    self.plugin.coordinates = {
        edit: function () {
            self.plugin.coordinatesDialog({
                clickFn: function (lng, lat, zoom) {
                    var center = lng + ',' + lat;
                    var url = ['http://api.map.baidu.com/staticimage',
						'?center=' + encodeURIComponent(center),
						'&zoom=' + encodeURIComponent(zoom),
						'&width=' + 558,
						'&height=' + 360,
						'&markers=' + encodeURIComponent(center),
						'&markerStyles=' + encodeURIComponent('l,A')].join('');
                    self.exec('insertimage', url);
                    self.hideDialog().focus();
                }
            });
        }
    };
    self.clickToolbar(name, self.plugin.coordinates.edit);
});
