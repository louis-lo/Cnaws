using System;
using System.Collections.Generic;
using Cnaws.Templates;

namespace Cnaws.Web
{
    internal enum SegmentType
    {
        Unknown,
        Resource,
        Themes,
        Uploads,
        Favicon,
        Robots,
        Sitemap,
        Default,
        Management,
        Admin,
        Caches,
        Runtime,
        Install,
        CrossDomain
    }

    internal sealed unsafe class UrlParse
    {
        private string _url;
        private SegmentType _segmentType;
        private ExtensionType _extensionType;
        private List<string> _segments;
        private string _extension;
        private bool _isDirectory;

        private int _pos;
        private bool _ext;
        private char* _begin;
        private char* _end;
        private char* _start;
        private char* _current;

        public unsafe UrlParse(string url)
        {
            _url = string.IsNullOrEmpty(url) ? "/" : url;
            _segmentType = SegmentType.Unknown;
            _extensionType = ExtensionType.Unknown;
            _segments = new List<string>();
            _extension = null;
            _isDirectory = false;
            Parse();
            InitHome();
        }
        private unsafe void Parse()
        {
            fixed (char* p = _url)
            {
                _pos = 0;
                _ext = false;
                _begin = p;
                _end = _begin + _url.Length;
                _start = _begin;
                for (_current = _begin; _current != _end; ++_current)
                    ReadChar();
                ReadEnd();
                _pos = 0;
                _ext = false;
                _begin = null;
                _end = null;
                _start = null;
                _current = null;
            }
        }
        private unsafe void ReadChar()
        {
            switch (*_current)
            {
                case '/':
                    if (_pos > 0)
                        ReadSegment();
                    _start = _current + 1;
                    _ext = false;
                    ++_pos;
                    break;
                case '.':
                    ReadExtension();
                    _start = _current;
                    _ext = true;
                    break;
            }
        }
        private unsafe void ReadSegment()
        {
            if (_start < _current)
            {
                if (_ext)
                {
                    char* start = _start + 1;
                    string ext = new string(start, 0, (int)(_current - start));
                    try { _extensionType = (ExtensionType)Enum.Parse(TType<ExtensionType>.Type, ext, true); }
                    catch (Exception) { _extensionType = ExtensionType.Unknown; }
                    _extension = new string(_start, 0, (int)(_current - _start));
                }
                else
                {
                    int len = (int)(_current - _start);
                    string segment = new string(_start, 0, len);
                    if (_pos == 1)
                    {
                        bool error = false;
                        try
                        {
                            _segmentType = (SegmentType)Enum.Parse(TType<SegmentType>.Type, segment, true);
                            if (_segmentType == SegmentType.Admin)
                                _segmentType = SegmentType.Unknown;
                        }
                        catch (Exception) { error = true; }
                        if (error || _segmentType == SegmentType.Management)
                        {
                            string management = Settings.Instance.Management;
                            if (management.Length == len && segment.Equals(management, StringComparison.OrdinalIgnoreCase))
                                _segmentType = SegmentType.Admin;
                            else
                                _segmentType = SegmentType.Unknown;
                        }
                    }
                    _segments.Add(segment);
                }
            }
        }
        private unsafe void ReadExtension()
        {
            ReadSegment();
            if (_extension != null)
            {
                _segments[_segments.Count - 1] = string.Concat(_segments[_segments.Count - 1], _extension);
                _extensionType = ExtensionType.Unknown;
            }
        }
        private unsafe void ReadEnd()
        {
            if (_start == _end)
                _isDirectory = true;
            else
                ReadSegment();
        }

        public bool IsDirectory
        {
            get { return _isDirectory; }
        }
        public string Extension
        {
            get { return _extension; }
        }
        public List<string> Segments
        {
            get { return _segments; }
        }
        public SegmentType SegmentType
        {
            get { return _segmentType; }
        }
        public ExtensionType ExtensionType
        {
            get { return _extensionType; }
        }
        public string Url
        {
            get { return _url; }
        }

        public void AppendExtension()
        {
            if (_extension != null)
                _segments[_segments.Count - 1] = string.Concat(_segments[_segments.Count - 1], _extension);
        }
        public void FormatManagement()
        {
            _segments[0] = Utility.DefaultManagement;
        }

        private void InitHome()
        {
            if (_segmentType == Web.SegmentType.Default && _segments.Count == 1 && _extensionType == Web.ExtensionType.Aspx)
            {
                _url = "/";
                _segmentType = SegmentType.Unknown;
                _extensionType = ExtensionType.Unknown;
                _segments.Clear();
                _extension = null;
                _isDirectory = true;
            }
        }

        public static unsafe bool EqualsDomain(string x, string y)
        {
            if (y != null)
            {
                int ylen = y.Length;
                if (ylen > 0)
                {
                    int xlen = x.Length;
                    if (xlen >= ylen)
                    {
                        fixed (char* px = x)
                        {
                            char* ptx = px + xlen - 1;
                            fixed (char* py = y)
                            {
                                for (char* pty = py + ylen - 1; pty >= py; --pty, --ptx)
                                {
                                    if (*pty != *ptx)
                                    {
                                        if (char.ToUpper(*pty) != char.ToUpper(*ptx))
                                            return false;
                                    }
                                }
                                return (xlen == ylen) ? true : (*ptx == '.');
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
