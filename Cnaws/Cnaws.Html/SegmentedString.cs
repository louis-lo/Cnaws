using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cnaws.Html
{
    internal unsafe sealed class SegmentedString
    {
        private enum FastPathFlags
        {
            NoFastPath = 0,
            Use8BitAdvanceAndUpdateLineNumbers = 1 << 0,
            Use8BitAdvance = 1 << 1,
        }
        public enum AdvancePastResult
        {
            DidNotMatch,
            DidMatch,
            NotEnoughCharacters
        }

        private char m_pushedChar1;
        private char m_pushedChar2;
        private SegmentedSubstring m_currentString;
        private char m_currentChar;
        private int m_numberOfCharactersConsumedPriorToCurrentString;
        private int m_numberOfCharactersConsumedPriorToCurrentLine;
        private int m_currentLine;
        private List<SegmentedSubstring> m_substrings;
        private bool m_closed;
        private bool m_empty;
        private FastPathFlags m_fastPathFlags;
        private Action m_advanceFunc;
        private Action m_advanceAndUpdateLineNumberFunc;

        public SegmentedString(PtrString str)
        {
            m_pushedChar1 = '\0';
            m_pushedChar2 = '\0';
            m_currentString = new SegmentedSubstring(str);
            m_currentChar = '\0';
            m_numberOfCharactersConsumedPriorToCurrentString = 0;
            m_numberOfCharactersConsumedPriorToCurrentLine = 0;
            m_currentLine = 0;
            m_substrings = new List<SegmentedSubstring>();
            m_closed = false;
            m_empty = str.Length == 0;
            m_fastPathFlags = FastPathFlags.NoFastPath;
            m_advanceFunc = null;
            m_advanceAndUpdateLineNumberFunc = null;
        }
        public SegmentedString(SegmentedString other)
        {
            m_pushedChar1 = other.m_pushedChar1;
            m_pushedChar2 = other.m_pushedChar2;
            m_currentString = new SegmentedSubstring(other.m_currentString);
            if (m_pushedChar2 != 0)
                m_currentChar = m_pushedChar2;
            else if (m_pushedChar1 != 0)
                m_currentChar = m_pushedChar1;
            else
                m_currentChar = m_currentString.m_length != 0 ? m_currentString.getCurrentChar() : '\0';
            m_numberOfCharactersConsumedPriorToCurrentString = other.m_numberOfCharactersConsumedPriorToCurrentString;
            m_numberOfCharactersConsumedPriorToCurrentLine = other.m_numberOfCharactersConsumedPriorToCurrentLine;
            m_currentLine = other.m_currentLine;
            m_substrings = new List<SegmentedSubstring>(other.m_substrings);
            m_closed = other.m_closed;
            m_empty = other.m_empty;
            m_fastPathFlags = other.m_fastPathFlags;
            m_advanceFunc = other.m_advanceFunc;
            m_advanceAndUpdateLineNumberFunc = other.m_advanceAndUpdateLineNumberFunc;
        }

        public void clear()
        {

        }
        public void close()
        {

        }
        public void append(SegmentedString)
        {

        }
        public void pushBack(SegmentedString)
        {

        }
        public void setExcludeLineNumbers()
        {

        }
        public void push(char c)
        {
            if (m_pushedChar1 == 0)
            {
                m_pushedChar1 = c;
                m_currentChar = m_pushedChar1 != 0 ? m_pushedChar1 : m_currentString.getCurrentChar();
                updateSlowCaseFunctionPointers();
            }
            else {
                Debug.Assert(m_pushedChar2 == 0);
                m_pushedChar2 = c;
            }
        }
        public bool isEmpty()
        {
            return m_empty;
        }
        public int length()
        {

        }
        public bool isClosed()
        {
            return m_closed;
        }
    }
}
