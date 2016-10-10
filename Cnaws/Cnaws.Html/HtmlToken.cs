using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace Cnaws.Html
{
    internal sealed class DoctypeData
    {
        public bool hasPublicIdentifier;
        public bool hasSystemIdentifier;
        public List<char> publicIdentifier;
        public List<char> systemIdentifier;
        public bool forceQuirks;

        public DoctypeData()
        {
            hasPublicIdentifier = false;
            hasSystemIdentifier = false;
            publicIdentifier = new List<char>();
            systemIdentifier = new List<char>();
            forceQuirks = false;
        }
    }

    internal sealed class HtmlToken
    {
        internal enum Type
        {
            Uninitialized,
            DOCTYPE,
            StartTag,
            EndTag,
            Comment,
            Character,
            EndOfFile,
        }
        internal sealed class Attribute
        {
            public List<char> name;
            public List<char> value;

            // Used by HtmlSourceTracker.
            public uint startOffset;
            public uint endOffset;

            public Attribute()
            {
                name = new List<char>(32);
                value = new List<char>(32);
            }
        }

        private Type m_type;
        private List<char> m_data;
        private char m_data8BitCheck;
        private bool m_selfClosing;
        private List<Attribute> m_attributes;
        private Attribute m_currentAttribute;
        private DoctypeData m_doctypeData;

        public HtmlToken()
        {
            m_type = Type.Uninitialized;
            m_data = new List<char>(256);
            m_data8BitCheck = '\0';
            m_selfClosing = false;
            m_attributes = new List<Attribute>(10);
            m_currentAttribute = null;
            m_doctypeData = null;
        }
        public void clear()
        {
            m_type = Type.Uninitialized;
            m_data.Clear();
            m_data8BitCheck = '\0';
        }
        public Type type()
        {
            return m_type;
        }
        public void makeEndOfFile()
        {
            Debug.Assert(m_type == Type.Uninitialized);
            m_type = Type.EndOfFile;
        }
        public List<char> name()
        {
            Debug.Assert(m_type == Type.StartTag || m_type == Type.EndTag || m_type == Type.DOCTYPE);
            return m_data;
        }
        public void appendToName(char character)
        {
            Debug.Assert(m_type == Type.StartTag || m_type == Type.EndTag || m_type == Type.DOCTYPE);
            Debug.Assert(character != 0);
            m_data.Add(character);
            m_data8BitCheck |= character;
        }
        public void beginDOCTYPE()
        {
            Debug.Assert(m_type == Type.Uninitialized);
            m_type = Type.DOCTYPE;
            m_doctypeData = new DoctypeData();
        }
        public void beginDOCTYPE(char character)
        {
            Debug.Assert(character != 0);
            beginDOCTYPE();
            m_data.Add(character);
            m_data8BitCheck |= character;
        }
        public void setForceQuirks()
        {
            Debug.Assert(m_type == Type.DOCTYPE);
            m_doctypeData.forceQuirks = true;
        }
        public void setPublicIdentifierToEmptyString()
        {
            Debug.Assert(m_type == Type.DOCTYPE);
            m_doctypeData.hasPublicIdentifier = true;
            m_doctypeData.publicIdentifier.Clear();
        }
        public void setSystemIdentifierToEmptyString()
        {
            Debug.Assert(m_type == Type.DOCTYPE);
            m_doctypeData.hasSystemIdentifier = true;
            m_doctypeData.systemIdentifier.Clear();
        }
        public void appendToPublicIdentifier(char character)
        {
            Debug.Assert(character != 0);
            Debug.Assert(m_type == Type.DOCTYPE);
            Debug.Assert(m_doctypeData.hasPublicIdentifier);
            m_doctypeData.publicIdentifier.Add(character);
        }
        public void appendToSystemIdentifier(char character)
        {
            Debug.Assert(character != 0);
            Debug.Assert(m_type == Type.DOCTYPE);
            Debug.Assert(m_doctypeData.hasSystemIdentifier);
            m_doctypeData.systemIdentifier.Add(character);
        }
        public DoctypeData releaseDoctypeData()
        {
            return m_doctypeData;
        }
        public bool selfClosing()
        {
            Debug.Assert(m_type == Type.StartTag || m_type == Type.EndTag);
            return m_selfClosing;
        }
        public List<Attribute> attributes()
        {
            Debug.Assert(m_type == Type.StartTag || m_type == Type.EndTag);
            return m_attributes;
        }
        public void beginStartTag(char character)
        {
            Debug.Assert(character != 0);
            Debug.Assert(m_type == Type.Uninitialized);
            m_type = Type.StartTag;
            m_selfClosing = false;
            m_attributes.Clear();
            m_currentAttribute = null;
            m_data.Add(character);
            m_data8BitCheck = character;
        }
        public void beginEndTag(char character)
        {
            Debug.Assert(m_type == Type.Uninitialized);
            m_type = Type.EndTag;
            m_selfClosing = false;
            m_attributes.Clear();
            m_currentAttribute = null;
            m_data.Add(character);
        }
        public void beginEndTag(List<char> characters)
        {
            Debug.Assert(m_type == Type.Uninitialized);
            m_type = Type.EndTag;
            m_selfClosing = false;
            m_attributes.Clear();
            m_currentAttribute = null;
            m_data.AddRange(characters);
        }
        public void beginAttribute(uint offset)
        {
            Debug.Assert(m_type == Type.StartTag || m_type == Type.EndTag);
            Debug.Assert(offset != 0);
            m_attributes.Add(new Attribute());
            m_currentAttribute = m_attributes.Last();
            m_currentAttribute.startOffset = offset;

        }
        public void appendToAttributeName(char character)
        {
            Debug.Assert(character != 0);
            Debug.Assert(m_type == Type.StartTag || m_type == Type.EndTag);
            Debug.Assert(m_currentAttribute != null);
            m_currentAttribute.name.Add(character);
        }
        public void appendToAttributeValue(char character)
        {
            Debug.Assert(character != 0);
            Debug.Assert(m_type == Type.StartTag || m_type == Type.EndTag);
            Debug.Assert(m_currentAttribute != null);
            m_currentAttribute.value.Add(character);
        }
        public void endAttribute(uint offset)
        {
            Debug.Assert(offset != 0);
            Debug.Assert(m_currentAttribute != null);
            m_currentAttribute.endOffset = offset;
            m_currentAttribute = null;
        }
        public void setSelfClosing()
        {
            Debug.Assert(m_type == Type.StartTag || m_type == Type.EndTag);
            m_selfClosing = true;
        }
        public void eraseValueOfAttribute(uint i)
        {
            Debug.Assert(m_type == Type.StartTag || m_type == Type.EndTag);
            Debug.Assert(i < m_attributes.Count);
            m_attributes[(int)i].value.Clear();
        }
        public void appendToAttributeValue(uint i, string value)
        {
            Debug.Assert(!string.IsNullOrEmpty(value));
            Debug.Assert(m_type == Type.StartTag || m_type == Type.EndTag);
            m_attributes[(int)i].value.AddRange(value);
        }
        public List<char> characters()
        {
            Debug.Assert(m_type == Type.Character);
            return m_data;
        }
        public bool charactersIsAll8BitData()
        {
            Debug.Assert(m_type == Type.Character);
            return m_data8BitCheck <= 0xFF;
        }
        public void appendToCharacter(char character)
        {
            Debug.Assert(m_type == Type.Uninitialized || m_type == Type.Character);
            m_type = Type.Character;
            m_data.Add(character);
            m_data8BitCheck |= character;
        }
        public void appendToCharacter(List<char> characters)
        {
            Debug.Assert(m_type == Type.Uninitialized || m_type == Type.Character);
            m_type = Type.Character;
            m_data.AddRange(characters);
        }
        public List<char> comment()
        {
            Debug.Assert(m_type == Type.Comment);
            return m_data;
        }
        public bool commentIsAll8BitData()
        {
            Debug.Assert(m_type == Type.Comment);
            return m_data8BitCheck <= 0xFF;
        }
        public void beginComment()
        {
            Debug.Assert(m_type == Type.Uninitialized);
            m_type = Type.Comment;
        }
        public void appendToComment(char character)
        {
            Debug.Assert(character != 0);
            Debug.Assert(m_type == Type.Comment);
            m_data.Add(character);
            m_data8BitCheck |= character;
        }

        private static bool nameMatches(Attribute attribute, string name)
        {
            int size = name.Length;
            if (attribute.name.Count != size)
                return false;
            for (int i = 0; i < size; ++i)
            {
                if (attribute.name[i] != name[i])
                    return false;
            }
            return true;
        }
        public static Attribute findAttribute(List<Attribute> attributes, string name)
        {
            foreach (Attribute attribute in attributes)
            {
                if (nameMatches(attribute, name))
                    return attribute;
            }
            return null;
        }
    }
}
