using Cnaws.Web.Templates.Parser.Node;
using System;
using System.Collections.Generic;

namespace Cnaws.Web.Templates.Common
{
    /// <summary>
    /// 计算器
    /// </summary>
    internal static class Calculator
    {
        private static readonly object[] _values;

        static Calculator()
        {
            _values = new object[19];
            _values[0] = null;
            _values[1] = null;
            _values[2] = null;
            _values[3] = false;
            _values[4] = '\0';
            _values[5] = (sbyte)0;
            _values[6] = (byte)0;
            _values[7] = (short)0;
            _values[8] = (ushort)0;
            _values[9] = 0;
            _values[10] = 0U;
            _values[11] = 0L;
            _values[12] = 0UL;
            _values[13] = 0F;
            _values[14] = 0D;
            _values[15] = 0M;
            _values[16] = new DateTime(1970, 1, 1);
            _values[17] = null;
            _values[18] = null;
        }

        #region Weights Array
        private static readonly TypeCode[] numberWeights = new TypeCode[] {
            TypeCode.Int16,
            TypeCode.Int32,
            TypeCode.Int64,
            TypeCode.Single,
            TypeCode.Double,
            TypeCode.Decimal
        };
        private static readonly TypeCode[] uintWeights = new TypeCode[] {
            TypeCode.UInt16,
            TypeCode.UInt32,
            TypeCode.UInt64
        };
        #endregion

        #region LetterType
        /// <summary>
        /// 字符类型
        /// </summary>
        public enum LetterType
        {
            /// <summary>
            /// 无
            /// </summary>
            None = 0,
            /// <summary>
            /// 操作符
            /// </summary>
            Operator = 1,
            /// <summary>
            /// 左圆括号
            /// </summary>
            LeftParentheses = 2,
            /// <summary>
            /// 右中括号
            /// </summary>
            RightParentheses = 3,
            /// <summary>
            /// 数字
            /// </summary>
            Number = 4,
            /// <summary>
            /// 其它
            /// </summary>
            Other = 5
        }
        #endregion

        #region common function
        private static bool IsOperator(string value)
        {
            switch (value)
            {
                case "||":
                case "|":
                case "&":
                case "&&":
                case ">":
                case ">=":
                case "<":
                case "<=":
                case "==":
                case "!!":
                case "!=":
                case "+":
                case "-":
                case "*":
                case "/":
                case "(":
                case ")":
                case "%":
                    return true;
                default:
                    return false;
            }
        }
        private static int GetPriority(string c)
        {
            switch (c)
            {
                case "||":
                case "|":
                case "&":
                case "&&":
                    return 5;
                case ">":
                case ">=":
                case "<":
                case "<=":
                case "==":
                case "!!":
                case "!=":
                    return 6;
                case "+":
                case "-":
                    return 7;
                case "%":
                case "*":
                    return 8;
                case "/":
                    return 9;
                default:
                    return 9;
            }
        }
        private static bool IsNumber(TypeCode code)
        {
            switch (code)
            {
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Decimal:
                    return true;
                default:
                    return false;
            }
        }
        private static bool IsUNumber(TypeCode code)
        {
            switch (code)
            {
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }
        private static TypeCode UNumberMap(TypeCode code, object value)
        {
            switch (code)
            {
                case TypeCode.UInt16: return ((ushort)value) > short.MaxValue ? TypeCode.Int32 : TypeCode.Int16;
                case TypeCode.UInt32: return ((uint)value) > int.MaxValue ? TypeCode.Int64 : TypeCode.Int32;
                default: return ((ulong)value) > long.MaxValue ? TypeCode.Decimal : TypeCode.Int64;
            }
        }
        #endregion

        #region ProcessExpression
        /// <summary>
        /// 处理表达式
        /// </summary>
        /// <param name="value">表达式</param>
        /// <returns></returns>
        public static Stack<object> ProcessExpression(string value)
        {
            value = value.Replace("  ", string.Empty);
            List<object> result = new List<object>();
            int j = 0;
            int i;

            for (i = 0; i < value.Length; ++i)
            {
                switch (value[i])
                {
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '(':
                    case ')':
                    case '%':
                        if (j < i)
                        {
                            result.Add(double.Parse(value.Substring(j, i - j)));
                            j = i;
                        }
                        result.Add(value[i].ToString());
                        ++j;
                        break;
                }
            }
            if (j < i)
            {
                result.Add(double.Parse(value.Substring(j, i - j)));
            }
            return ProcessExpression(result.ToArray());
        }
        /// <summary>
        /// 处理表达式
        /// </summary>
        /// <param name="value">表达式</param>
        /// <returns></returns>
        public static Stack<object> ProcessExpression(object[] value)
        {
            Stack<object> post = new Stack<object>();
            Stack<string> stack = new Stack<string>();

            for (int i = 0; i < value.Length; ++i)
            {
                TypeCode code;
                if (value[i] != null)
                {
                    code = Convert.GetTypeCode(value[i]);
                }
                else
                {
                    code = TypeCode.String;
                    value[i] = string.Empty;
                }
                switch (code)
                {
                    default:
                        post.Push(value[i]);
                        break;
                    case TypeCode.String:
                        switch (value[i].ToString())
                        {
                            case "(":
                                stack.Push("(");
                                break;
                            case ")":
                                while (stack.Count > 0)
                                {
                                    if (stack.Peek() == "(")
                                    {
                                        stack.Pop();
                                        break;
                                    }
                                    else
                                        post.Push(stack.Pop());
                                }
                                break;
                            case "+":
                            case "-":
                            case "*":
                            case "%":
                            case "/":
                            case "||":
                            case "|":
                            case "&&":
                            case "&":
                            case ">":
                            case ">=":
                            case "<":
                            case "<=":
                            case "==":
                            case "!!":
                            case "!=":
                                if (stack.Count == 0)
                                {
                                    stack.Push(value[i].ToString());
                                }
                                else
                                {
                                    string eX = stack.Peek();
                                    string eY = value[i].ToString();
                                    if (GetPriority(eY) >= GetPriority(eX))
                                    {
                                        stack.Push(eY);
                                    }
                                    else
                                    {
                                        while (stack.Count > 0)
                                        {
                                            if (GetPriority(eX) > GetPriority(eY) && stack.Peek() != "(")// && stack.Peek() != '('
                                            {
                                                post.Push(stack.Pop());
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        stack.Push(eY);
                                    }
                                }
                                break;
                            default:
                                post.Push(value[i]);
                                break;
                        }
                        break;

                }
            }

            while (stack.Count > 0)
            {
                post.Push(stack.Pop());
            }

            return post;
        }

        #endregion

        #region Calculate
        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static object Calculate(object x, object y, string value)
        {
            TypeCode tX = Convert.GetTypeCode(x);
            TypeCode tY = Convert.GetTypeCode(y);

            if (tX == TypeCode.DBNull && tY == TypeCode.DBNull)
            {
                tX = TypeCode.Int32;
                tY = TypeCode.Int32;
                x = 0;
                y = 0;
            }
            else
            {
                if (tX == TypeCode.DBNull)
                {
                    tX = tY;
                    x = _values[(int)tX];
                }
                else if (tY == TypeCode.DBNull)
                {
                    tY = tX;
                    y = _values[(int)tY];
                }
            }

            if (IsNumber(tX) && IsNumber(tY))
            {
                TypeCode t;
                if (tX == tY)
                {
                    t = tX;
                }
                else
                {
                    int i, j;
                    if (IsUNumber(tX) && IsUNumber(tY))
                    {
                        i = Array.IndexOf<TypeCode>(uintWeights, tX);
                        j = Array.IndexOf<TypeCode>(uintWeights, tY);
                    }
                    else
                    {
                        if (IsUNumber(tX))
                        {
                            tX = UNumberMap(tX, x);
                        }

                        if (IsUNumber(tY))
                        {
                            tY = UNumberMap(tY, y);
                        }

                        i = Array.IndexOf<TypeCode>(numberWeights, tX);
                        j = Array.IndexOf<TypeCode>(numberWeights, tY);
                    }
                    if (i > j)
                    {
                        t = tX;
                    }
                    else
                    {
                        t = tY;
                    }
                }
                switch (t)
                {
                    case TypeCode.Double:
                        return Calculate(Convert.ToDouble(x), Convert.ToDouble(y), value);
                    case TypeCode.Int16:
                        return Calculate(Convert.ToInt16(x), Convert.ToInt16(y), value);
                    case TypeCode.Int32:
                        return Calculate(Convert.ToInt32(x), Convert.ToInt32(y), value);
                    case TypeCode.Int64:
                        return Calculate(Convert.ToInt64(x), Convert.ToInt64(y), value);
                    case TypeCode.UInt16:
                        return Calculate(Convert.ToUInt16(x), Convert.ToUInt16(y), value);
                    case TypeCode.UInt32:
                        return Calculate(Convert.ToUInt32(x), Convert.ToUInt32(y), value);
                    case TypeCode.UInt64:
                        return Calculate(Convert.ToUInt64(x), Convert.ToUInt64(y), value);
                    case TypeCode.Single:
                        return Calculate(Convert.ToSingle(x), Convert.ToSingle(y), value);
                    case TypeCode.Decimal:
                        return Calculate(Convert.ToDecimal(x), Convert.ToDecimal(y), value);
                    default:
                        return null;
                }
            }

            if (tX == TypeCode.Boolean && tY == TypeCode.Boolean)
                return Calculate((bool)x, (bool)y, value);
            if (tX == TypeCode.String && tY == TypeCode.String)
                return Calculate((string)x, (string)y, value);
            if (tX == TypeCode.DateTime && tY == TypeCode.DateTime)
                return Calculate((DateTime)x, (DateTime)y, value);
            switch (value)
            {
                case "==":
                case "!!":
                    return Equals(x, y, tX, tY);
                case "!=":
                    return !Equals(x, y, tX, tY);
                default:
                    throw new TemplateException(string.Concat("Operator \"", value, "\" can not be applied operand \"Object\" and \"Object\""));
            }
        }
        private static Boolean Equals(object x, object y, TypeCode tX, TypeCode tY)
        {
            if (x == null || x == null)
            {
                return x == null && y == null;
            }
            if (tX == tY)
            {
                return x == y;
            }
            return false;
        }
        /// <summary>
        /// 计算后缀表达式
        /// </summary>
        /// <param name="value">后缀表达式</param>
        /// <returns></returns>
        public static object Calculate(Stack<object> value)
        {
            Stack<object> post = new Stack<object>();
            while (value.Count > 0)
            {
                post.Push(value.Pop());
            }
            Stack<object> stack = new Stack<object>();

            while (post.Count > 0)
            {
                object obj = post.Pop();
                if (obj != null && Convert.GetTypeCode(obj) == TypeCode.String && IsOperator((string)obj))
                {
                    object y = stack.Pop();
                    object x = stack.Pop();
                    stack.Push(Calculate(x, y, (string)obj));
                }
                else
                {
                    stack.Push(obj);
                }
            }

            return stack.Pop();
        }
        /// <summary>
        /// 计算表达式
        /// </summary>
        /// <param name="value">表达式</param>
        /// <returns></returns>
        public static object Calculate(object[] value)
        {
            Stack<object> stack = ProcessExpression(value);

            return Calculate(stack);
        }
        /// <summary>
        /// 计算表达式
        /// </summary>
        /// <param name="value">表达式</param>
        /// <returns></returns>
        public static object Calculate(string value)
        {
            Stack<object> stack = ProcessExpression(value);

            return Calculate(stack);
        }

        #endregion

        #region  Calculate

        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static object Calculate(bool x, bool y, string value)
        {
            switch (value)
            {
                case "==":
                case "!!":
                    return x == y;
                case "!=":
                    return x != y;
                case "||":
                    return x || y;
                case "&&":
                    return x && y;
                default:
                    throw new TemplateException(string.Concat("Operator \"", value, "\" can not be applied operand \"Boolean\" and \"Boolean\""));
            }
        }
        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static object Calculate(string x, string y, string value)
        {
            switch (value)
            {
                case "==":
                case "!!":
                    return x.Equals(y, StringComparison.OrdinalIgnoreCase);
                case "!=":
                    return !x.Equals(y, StringComparison.OrdinalIgnoreCase);
                case "+":
                    return string.Concat(x, y);
                default:
                    throw new TemplateException(string.Concat("Operator \"", value, "\" can not be applied operand \"String\" and \"String\""));
            }
        }
        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static object Calculate(DateTime x, DateTime y, string value)
        {
            switch (value)
            {
                case "==":
                case "!!":
                    return x == y;
                case "!=":
                    return x != y;
                case ">":
                    return x > y;
                case ">=":
                    return x >= y;
                case "<":
                    return x < y;
                case "<=":
                    return x <= y;
                default:
                    throw new TemplateException(string.Concat("Operator \"", value, "\" can not be applied operand \"DateTime\" and \"DateTime\""));
            }
        }
        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static object Calculate(double x, double y, string value)
        {
            switch (value)
            {
                case "+":
                    return x + y;
                case "-":
                    return x - y;
                case "*":
                    return x * y;
                case "/":
                    return x / y;
                case "%":
                    return x % y;
                case ">=":
                    return x >= y;
                case "<=":
                    return x <= y;
                case "<":
                    return x < y;
                case ">":
                    return x > y;
                case "==":
                case "!!":
                    return x == y;
                case "!=":
                    return x != y;
                default:
                    throw new TemplateException(string.Concat("Operator \"", value, "\" can not be applied operand \"Double\" and \"Double\""));
            }
        }
        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static object Calculate(float x, float y, string value)
        {
            switch (value)
            {
                case "+":
                    return x + y;
                case "-":
                    return x - y;
                case "*":
                    return x * y;
                case "/":
                    return x / y;
                case "%":
                    return x % y;
                case ">=":
                    return x >= y;
                case "<=":
                    return x <= y;
                case "<":
                    return x < y;
                case ">":
                    return x > y;
                case "==":
                case "!!":
                    return x == y;
                case "!=":
                    return x != y;
                default:
                    throw new TemplateException(string.Concat("Operator \"", value, "\" can not be applied operand \"Single\" and \"Single\""));
            }
        }
        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static object Calculate(decimal x, decimal y, string value)
        {
            switch (value)
            {
                case "+":
                    return x + y;
                case "-":
                    return x - y;
                case "*":
                    return x * y;
                case "/":
                    return x / y;
                case "%":
                    return x % y;
                case ">=":
                    return x >= y;
                case "<=":
                    return x <= y;
                case "<":
                    return x < y;
                case ">":
                    return x > y;
                case "==":
                case "!!":
                    return x == y;
                case "!=":
                    return x != y;
                default:
                    throw new TemplateException(string.Concat("Operator \"", value, "\" can not be applied operand \"Decimal\" and \"Decimal\""));
            }
        }
        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static object Calculate(int x, int y, string value)
        {
            switch (value)
            {
                case "+":
                    return x + y;
                case "-":
                    return x - y;
                case "*":
                    return x * y;
                case "/":
                    return x / y;
                case "%":
                    return x % y;
                case ">=":
                    return x >= y;
                case "<=":
                    return x <= y;
                case "<":
                    return x < y;
                case ">":
                    return x > y;
                case "==":
                case "!!":
                    return x == y;
                case "!=":
                    return x != y;
                default:
                    throw new TemplateException(string.Concat("Operator \"", value, "\" can not be applied operand \"Int32\" and \"Int32\""));
            }
        }
        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static object Calculate(long x, long y, string value)
        {
            switch (value)
            {
                case "+":
                    return x + y;
                case "-":
                    return x - y;
                case "*":
                    return x * y;
                case "/":
                    return x / y;
                case "%":
                    return x % y;
                case ">=":
                    return x >= y;
                case "<=":
                    return x <= y;
                case "<":
                    return x < y;
                case ">":
                    return x > y;
                case "==":
                case "!!":
                    return x == y;
                case "!=":
                    return x != y;
                default:
                    throw new TemplateException(string.Concat("Operator \"", value, "\" can not be applied operand \"Int64\" and \"Int64\""));
            }
        }
        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static object Calculate(short x, short y, string value)
        {
            switch (value)
            {
                case "+":
                    return x + y;
                case "-":
                    return x - y;
                case "*":
                    return x * y;
                case "/":
                    return x / y;
                case "%":
                    return x % y;
                case ">=":
                    return x >= y;
                case "<=":
                    return x <= y;
                case "<":
                    return x < y;
                case ">":
                    return x > y;
                case "==":
                case "!!":
                    return x == y;
                case "!=":
                    return x != y;
                default:
                    throw new TemplateException(string.Concat("Operator \"", value, "\" can not be applied operand \"Int16\" and \"Int16\""));
            }
        }
        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static object Calculate(uint x, uint y, string value)
        {
            switch (value)
            {
                case "+":
                    return x + y;
                case "-":
                    return x - y;
                case "*":
                    return x * y;
                case "/":
                    return x / y;
                case "%":
                    return x % y;
                case ">=":
                    return x >= y;
                case "<=":
                    return x <= y;
                case "<":
                    return x < y;
                case ">":
                    return x > y;
                case "==":
                case "!!":
                    return x == y;
                case "!=":
                    return x != y;
                default:
                    throw new TemplateException(string.Concat("Operator \"", value, "\" can not be applied operand \"UInt32\" and \"UInt32\""));
            }
        }
        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static object Calculate(ulong x, ulong y, string value)
        {
            switch (value)
            {
                case "+":
                    return x + y;
                case "-":
                    return x - y;
                case "*":
                    return x * y;
                case "/":
                    return x / y;
                case "%":
                    return x % y;
                case ">=":
                    return x >= y;
                case "<=":
                    return x <= y;
                case "<":
                    return x < y;
                case ">":
                    return x > y;
                case "==":
                case "!!":
                    return x == y;
                case "!=":
                    return x != y;
                default:
                    throw new TemplateException(string.Concat("Operator \"", value, "\" can not be applied operand \"UInt64\" and \"UInt64\""));
            }
        }
        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="x">值一</param>
        /// <param name="y">值二</param>
        /// <param name="value">操作符</param>
        /// <returns></returns>
        public static object Calculate(ushort x, ushort y, string value)
        {
            switch (value)
            {
                case "+":
                    return x + y;
                case "-":
                    return x - y;
                case "*":
                    return x * y;
                case "/":
                    return x / y;
                case "%":
                    return x % y;
                case ">=":
                    return x >= y;
                case "<=":
                    return x <= y;
                case "<":
                    return x < y;
                case ">":
                    return x > y;
                case "==":
                case "!!":
                    return x == y;
                case "!=":
                    return x != y;
                default:
                    throw new TemplateException(string.Concat("Operator \"", value, "\" can not be applied operand \"UInt16\" and \"UInt16\""));
            }
        }

        #endregion
    }
}
