﻿using System;

namespace Cnaws.Html
{
    public enum TokenKind
    {
        ///// <summary>
        ///// 无
        ///// </summary>
        //None,
        /// <summary>
        /// 文本
        /// </summary>
        Text,
        /// <summary>
        /// 文本数据
        /// </summary>
        TextData,
        /// <summary>
        /// 标签
        /// </summary>
        Element,
        /// <summary>
        /// 标签开始标记
        /// </summary>
        ElementStart,
        /// <summary>
        /// 标签结束标记
        /// </summary>
        ElementEnd,
        /// <summary>
        /// 字符串
        /// </summary>
        String,
        /// <summary>
        /// 数字
        /// </summary>
        Number,
        /// <summary>
        /// 左中括号
        /// </summary>
        LeftBracket,
        /// <summary>
        /// 右中括号
        /// </summary>
        RightBracket,
        /// <summary>
        /// 左圆括号
        /// </summary>
        LeftParentheses,
        /// <summary>
        /// 右员括号
        /// </summary>
        RightParentheses,
        /// <summary>
        /// 新行（换行符）
        /// </summary>
        NewLine,
        /// <summary>
        /// 点
        /// </summary>
        Dot,
        /// <summary>
        /// 字符串开始
        /// </summary>
        StringStart,
        /// <summary>
        /// 字符串结束
        /// </summary>
        StringEnd,
        /// <summary>
        /// 空格
        /// </summary>
        Space,
        /// <summary>
        /// 标点
        /// </summary>
        Punctuation,
        /// <summary>
        /// 运算符
        /// </summary>
        Operator,
        /// <summary>
        /// 逗号
        /// </summary>
        Comma,
        /// <summary>
        /// 结束
        /// </summary>
        EOF
    }
}
