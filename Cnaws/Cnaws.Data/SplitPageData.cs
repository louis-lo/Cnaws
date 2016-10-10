using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Cnaws.Data
{
    [Serializable]
    public sealed class PageData<T>
    {
        public int Total;
        public IList<T> Data;
    }

    [Serializable]
    public abstract class SplitPage
    {
        public abstract long PageIndex { get; }
        public abstract int PageSize { get; }
        public abstract long TotalCount { get; }
        public abstract long PagesCount { get; }
        public abstract long BeginPage { get; }
        public abstract long EndPage { get; }
        public abstract string UrlFormatter { get; set; }

        public string Url(long page)
        {
            return string.Format(UrlFormatter, page);
        }
    }

    [Serializable]
    public sealed class SplitPageData<T> : SplitPage, IEnumerable<T>
    {
#if(DEBUG)
        [Description("页码")]
#endif
        private long index;
#if (DEBUG)
        [Description("每页条目数")]
#endif
        private int size;
#if (DEBUG)
        [Description("数据")]
#endif
        private IList<T> data;
#if (DEBUG)
        [Description("总条目数")]
#endif
        private long total;
#if (DEBUG)
        [Description("总页数")]
#endif
        private long pages;
#if (DEBUG)
        [Description("显示起始页")]
#endif
        private long begin;
#if (DEBUG)
        [Description("显示结束页")]
#endif
        private long end;
#if (DEBUG)
        [Description("页链接规则")]
#endif
        private string url;

        public SplitPageData(int index, int size, IList<T> data, int total, int show = 8)
            : this((long)index, size, data, (long)total, show)
        {
        }
        public SplitPageData(long index, int size, IList<T> data, long total, int show = 8)
        {
            this.index = index;
            this.size = size;
            this.data = data;
            this.total = total;

            pages = (long)Math.Ceiling((double)this.total / (double)this.size);
            int page = (show - 1) / 2;
            begin = index - page;
            end = index + page;
            url = string.Empty;

            if (begin < 1)
            {
                end += (1 - begin);
                begin = 1;
            }
            if (end > pages)
            {
                begin -= (end - pages);
                end = pages;
            }
            if (begin < 1)
                begin = 1;
        }

        public override long PageIndex
        {
            get { return index; }
        }
        public override int PageSize
        {
            get { return size; }
        }
        public IList<T> Data
        {
            get { return data; }
        }
        public override long TotalCount
        {
            get { return total; }
        }
        public override long PagesCount
        {
            get { return pages; }
        }
        public override long BeginPage
        {
            get { return begin; }
        }
        public override long EndPage
        {
            get { return end; }
        }
        public override string UrlFormatter
        {
            get
            {
                return url;
            }
            set
            {
                url = value;
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return data.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }
    }
}
