using System;
using System.Collections.Generic;

namespace Cnaws.Web.Templates.Parser
{
    /// <summary>
    /// 变量域
    /// </summary>
    public class VariableScope
    {
        private TemplateContext context;
        private VariableScope parent;

        private Dictionary<string, object> dic;

        /// <summary>
        /// VariableScope
        /// </summary>
        public VariableScope()
            : this(null)
        {
        }

        /// <summary>
        /// VariableScope
        /// </summary>
        public VariableScope(VariableScope parent)
        {
            this.context = null;
            this.parent = parent;
            this.dic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public int Count
        {
            get { return dic.Count; }
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        /// <param name="all">是否清空父数据</param>
        public void Clear(bool all)
        {
            this.dic.Clear();
            if (all)
            {
                if (this.parent != null)
                {
                    this.parent.Clear(all);
                }
            }
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        public void Clear()
        {
            Clear(false);
        }

        internal void SetContext(TemplateContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 父对象
        /// </summary>
        public VariableScope Parent
        {
            get { return this.parent; }
        }

        /// <summary>
        /// 获取索引值
        /// </summary>
        /// <param name="name">索引名称</param>
        /// <returns></returns>
        public object this[string name]
        {
            get
            {
                object val;
                if (dic.TryGetValue(name, out val))
                    return val;
                if (parent != null)
                    return this.parent[name];
                if (context == null)
                    throw new KeyNotFoundException(string.Concat("Variable \"", name, "\" not found"));
                throw new VariableNotFoundException(context.CurrentPath, context.RuntimeTag);
            }
            set
            {
                dic[name] = value;
            }
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        internal bool SetValue(string key, object value)
        {
            if (this.dic.ContainsKey(key))
            {
                this[key] = value;
                return true;
            }
            if (this.parent != null)
            {
                return this.parent.SetValue(key, value);
            }
            return false;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Push(string key, object value)
        {
            this.dic.Add(key, value);
        }

        /// <summary>
        /// 是否包含指定键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            if (this.dic.ContainsKey(key))
            {
                return true;
            }
            if (parent != null)
            {
                return this.parent.ContainsKey(key);
            }

            return false;
        }

        /// <summary>
        /// 移除指定对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return this.dic.Remove(key);
        }

    }
}
