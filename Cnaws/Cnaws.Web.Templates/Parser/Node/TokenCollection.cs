using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Cnaws.Web.Templates.Parser.Node
{
    public class TokenCollection : IList<Token>
    {
        private List<Token> list;

        public TokenCollection()
        {
            this.list = new List<Token>();
        }

        public TokenCollection(int capacity)
        {
            list = new List<Token>(capacity);
        }

        public TokenCollection(IEnumerable<Token> collection)
        {
            list = new List<Token>(collection);
        }

        public TokenCollection(IList<Token> collection, int start, int end)
        {
            list = new List<Token>(end + 1 - start);
            for (int i = start; i <= end && i < collection.Count; ++i)
            {
                this.Add(collection[i]);
            }
        }

        public Token First
        {
            get
            {
                return this.Count == 0 ? null : this[0];
            }
        }

        public Token Last
        {
            get
            {
                return this.Count == 0 ? null : this[this.Count - 1];
            }
        }

        public void Add(IList<Token> list, int start, int end)
        {
            for (int i = start; i <= end && i < list.Count; ++i)
            {
                this.Add(list[i]);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.Count; ++i)
            {
                sb.Append(this[i].ToString());
            }
            return sb.ToString();
        }

        #region IList<Token> 成员

        public int IndexOf(Token item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, Token item)
        {
            if (item.TokenKind != TokenKind.Space)
            {
                list.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        public Token this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                if (value.TokenKind != TokenKind.Space)
                {
                    list[index] = value;
                }
            }
        }

        #endregion

        #region ICollection<Token> 成员

        public void Add(Token item)
        {
            if (item.TokenKind != TokenKind.Space)
            {
                list.Add(item);
            }
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(Token item)
        {
            return list.Contains(item);
        }

        public void CopyTo(Token[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(Token item)
        {
            return list.Remove(item);
        }

        #endregion

        #region IEnumerable<Token> 成员

        public IEnumerator<Token> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
