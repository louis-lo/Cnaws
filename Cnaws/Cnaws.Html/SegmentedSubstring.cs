using System;
using System.Diagnostics;
using System.Text;

namespace Cnaws.Html
{
    internal unsafe sealed class SegmentedSubstring
    {
        internal int m_length;
        private bool m_doNotExcludeLineNumbers;
        private PtrString m_string;
        private char* m_data;

        public SegmentedSubstring(PtrString str)
        {
            m_length = str.Length;
            m_doNotExcludeLineNumbers = true;
            m_string = str;
            m_data = str.Pointer;
        }
        public SegmentedSubstring(SegmentedSubstring other)
        {
            m_length = other.m_length;
            m_doNotExcludeLineNumbers = other.m_doNotExcludeLineNumbers;
            m_string = other.m_string;
            m_data = other.m_data;
        }

        public void clear()
        {
            m_length = 0;
            m_string = null;
            m_data = null;
        }
        public bool excludeLineNumbers()
        {
            return !m_doNotExcludeLineNumbers;
        }
        public bool doNotExcludeLineNumbers()
        {
            return m_doNotExcludeLineNumbers;
        }
        public void setExcludeLineNumbers()
        {
            m_doNotExcludeLineNumbers = false;
        }
        public int numberOfCharactersConsumed()
        {
            return m_string.Length - m_length;
        }
        public void appendTo(StringBuilder builder)
        {
            int offset = m_string.Length - m_length;
            if (offset == 0)
            {
                if (m_length != 0)
                    builder.Append(m_string);
            }
            else
                builder.Append(m_string.Substring(offset, m_length));
        }
        public string currentSubString(uint length)
        {
            int offset = m_string.Length - m_length;
            return m_string.Substring(offset, (int)length);
        }
        public char getCurrentChar()
        {
            Debug.Assert(m_length != 0);
            return *m_data;
        }
        public char incrementAndGetCurrentChar()
        {
            Debug.Assert(m_length != 0);
            return *++m_data;
        }
    }
}
