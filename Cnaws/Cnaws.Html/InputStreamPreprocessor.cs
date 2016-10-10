using System;

namespace Cnaws.Html
{
    internal sealed class InputStreamPreprocessor<T>
    {
        private T m_tokenizer;
        private char m_nextInputCharacter;
        private bool m_skipNextNewLine;

        public InputStreamPreprocessor(T tokenizer)
        {
            m_tokenizer = tokenizer;
            reset();
        }
        public char nextInputCharacter()
        {
            return m_nextInputCharacter;
        }
        public bool peek(SegmentedString source, bool skipNullCharacters = false)
        {
            if (source.isEmpty())
                return false;

            m_nextInputCharacter = source.currentChar();

            // Every branch in this function is expensive, so we have a
            // fast-reject branch for characters that don't require special
            // handling. Please run the parser benchmark whenever you touch
            // this function. It's very hot.
            const char specialCharacterMask = (char)('\n' | '\r' | '\0');
            if (m_nextInputCharacter & ~specialCharacterMask)
            {
                m_skipNextNewLine = false;
                return true;
            }
            return processNextInputCharacter(source, skipNullCharacters);
        }
        public void reset(bool skipNextNewLine = false)
        {

        }
    }
}
