jQuery.fn.scrolling = function (page, pages) {
    var scroll = new IScroll('#' + this.attr('id'), {
        scrollbars: true,
        mouseWheel: true,
        interactiveScrollbars: true,
        shrinkScrollbars: 'scale',
        fadeScrollbars: true,
        probeType: 3,
        click: true
    });
    scroll.target = this;
    scroll.pageIndex = page;
    scroll.pageCount = pages;
    scroll.locked = false;
    scroll.acDown = null;
    scroll.acUp = null;
    scroll.abDown = false;
    scroll.abUp = false;
    if (arguments.length > 3) {
        scroll.acDown = arguments[3];
        scroll.abDown = scroll.acDown != null;
    }
    if (arguments.length > 2) {
        scroll.acUp = arguments[2];
        scroll.abUp = scroll.acUp != null;
    }
    scroll.abUp = (scroll.pageIndex < scroll.pageCount);
    scroll.elDown = null;
    scroll.elUp = null;
    if (scroll.abDown) {
        this.children().first().prepend($('<div id="pullDown" style="display:none"><span class="pullDownIcon"></span><span class="pullDownLabel">下拉刷新...</span></div>'));
        scroll.elDown = document.querySelector('#pullDown');
    }
    if (scroll.abUp) {
        this.children().first().append($('<div id="pullUp" style="display:none"><span class="pullUpIcon"></span><span class="pullUpLabel">上拉加载更多...</span></div>'));
        scroll.elUp = document.querySelector('#pullUp');
    }
    scroll.acScroll = null;
    if (arguments.length > 4) {
        scroll.acScroll = arguments[4];
    }
    scroll.beginDown = 0;
    scroll.beginUp = 0;
    scroll.hasNext = function () {
        return this.pageIndex < this.pageCount;
    };
    scroll.nextPage = function () {
        return this.pageIndex + 1;
    };
    scroll.isLocked = function () {
        return this.locked;
    };
    scroll.lock = function () {
        this.locked = true;
    };
    scroll.unlock = function () {
        this.locked = false;
    };
    scroll.update = function (page, pages) {
        this.pageIndex = page;
        this.pageCount = pages;
        if (this.abDown) {
            $(this.elDown).hide();
        }
        if (this.abUp) {
            $(this.elUp).hide();
        }
        this.refresh();
    };
    scroll.on('scroll', function () {
        this.target.trigger('scroll');
        if (!this.locked) {
            if (this.abDown) {
                if (this.beginDown == 0 && this.y > 0) {
                    this.beginDown = 1;
                    $(this.elDown).show();
                    this.refresh();
                    this.scrollTo(this.x, this.y - $(this.elDown).height());
                }
                else if (this.beginDown == 1 && this.y > ($(this.elDown).height() * 2)) {
                    this.beginDown = 2;
                    this.elDown.className = 'flip';
                    this.elDown.querySelector('.pullDownLabel').innerHTML = '松手开始更新...';
                }
            }
            if (this.abUp) {
                if (this.beginUp == 0 && this.y < this.maxScrollY) {
                    this.beginUp = 1;
                    $(this.elUp).show();
                    this.refresh();
                    this.scrollTo(this.x, this.y - $(this.elUp).height());
                }
                else if (this.beginUp == 1 && this.y < (this.maxScrollY - ($(this.elUp).height() * 2))) {
                    this.beginUp = 2;
                    this.elUp.className = 'flip';
                    this.elUp.querySelector('.pullUpLabel').innerHTML = '松手开始更新...';
                }
            }
        }
        if (scroll.acScroll != null) {
            scroll.acScroll();
        }
    });
    scroll.on('scrollEnd', function () {
        if (!this.locked) {
            if (this.beginDown == 2) {
                this.beginDown = 3;
                this.elDown.className = 'loading';
                this.elDown.querySelector('.pullDownLabel').innerHTML = '加载中...';
                this.acDown();
            }
            else {
                this.beginDown = 0;
                $(this.elDown).hide();
                this.refresh();
            }
            if (this.beginUp == 2) {
                if (this.pageIndex < this.pageCount) {
                    this.beginUp = 3;
                    this.elUp.className = 'loading';
                    this.elUp.querySelector('.pullUpLabel').innerHTML = '加载中...';
                    this.acUp();
                }
                else {
                    this.beginUp = 0;
                    this.elUp.className = '';
                    this.elUp.querySelector('.pullUpLabel').innerHTML = '已到最后一页';
                }
            }
            else {
                this.beginUp = 0;
                $(this.elUp).hide();
                this.refresh();
            }
        }
    });
    scroll.on('refresh', function () {
        if (this.beginDown == 3) {
            this.beginDown = 0;
            this.elDown.className = '';
            this.elDown.querySelector('.pullDownLabel').innerHTML = '下拉刷新...';
        }
        if (this.beginUp == 3) {
            this.beginUp = 0;
            this.elUp.className = '';
            this.elUp.querySelector('.pullUpLabel').innerHTML = '上拉加载更多...';
        }
    });
    document.addEventListener('touchmove', function (e) { e.preventDefault(); }, false);
    return scroll;
}