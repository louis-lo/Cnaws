using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Cnaws.Data.Linq
{
    public static class QueryExtensions
    {
        //
        // 摘要:
        //     对序列应用累加器函数。
        //
        // 参数:
        //   source:
        //     要聚合的序列。
        //
        //   func:
        //     要应用于每个元素的累加器函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     累加器的最终值。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 func 为 null。
        //
        //   T:System.InvalidOperationException:
        //     source 中不包含任何元素。
        public static T Aggregate<T>(this Query<T> query, Expression<Func<T, T, T>> func)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     对序列应用累加器函数。将指定的种子值用作累加器初始值。
        //
        // 参数:
        //   source:
        //     要聚合的序列。
        //
        //   seed:
        //     累加器的初始值。
        //
        //   func:
        //     要对每个元素调用的累加器函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   A:
        //     累加器值的类型。
        //
        // 返回结果:
        //     累加器的最终值。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 func 为 null。
        public static A Aggregate<T, A>(this Query<T> query, A seed, Expression<Func<A, T, A>> func)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     对序列应用累加器函数。将指定的种子值用作累加器的初始值，并使用指定的函数选择结果值。
        //
        // 参数:
        //   source:
        //     要聚合的序列。
        //
        //   seed:
        //     累加器的初始值。
        //
        //   func:
        //     要对每个元素调用的累加器函数。
        //
        //   selector:
        //     将累加器的最终值转换为结果值的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   A:
        //     累加器值的类型。
        //
        //   R:
        //     结果值的类型。
        //
        // 返回结果:
        //     已转换的累加器最终值。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 func 或 selector 为 null。
        public static R Aggregate<T, A, R>(this Query<T> query, A seed, Expression<Func<A, T, A>> func, Expression<Func<A, R>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     确定序列中的所有元素是否都满足条件。
        //
        // 参数:
        //   source:
        //     要测试其元素是否满足条件的序列。
        //
        //   predicate:
        //     用于测试每个元素是否满足条件的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     如果源序列中的每个元素都通过指定谓词中的测试，或者序列为空，则为 true；否则为 false。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 predicate 为 null。
        public static bool All<T>(this Query<T> query, Expression<Func<T, bool>> predicate)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     确定序列是否包含任何元素。
        //
        // 参数:
        //   source:
        //     要检查是否为空的序列。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     如果源序列包含任何元素，则为 true；否则为 false。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static bool Any<T>(this Query<T> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     确定序列中的任何元素是否都满足条件。
        //
        // 参数:
        //   source:
        //     要测试其元素是否满足条件的序列。
        //
        //   predicate:
        //     用于测试每个元素是否满足条件的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     如果源序列中的任何元素都通过指定谓词中的测试，则为 true；否则为 false。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 predicate 为 null。
        public static bool Any<T>(this Query<T> query, Expression<Func<T, bool>> predicate)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     将 System.Collections.IEnumerable 转换为 System.Linq.IQueryable。
        //
        // 参数:
        //   source:
        //     要转换的序列。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable，表示输入序列。
        //
        // 异常:
        //   T:System.ArgumentException:
        //     source 未为某些 T 实现 System.Collections.Generic.IEnumerable`1。
        //
        //   T:System.ArgumentNullException:
        //     source 为 null。
        //public static IQueryable AsQueryable(this IEnumerable source)
        //{
        //    throw new NotSupportedException();
        //}
        ////
        //// 摘要:
        ////     将泛型 System.Collections.Generic.IEnumerable`1 转换为泛型 System.Linq.IQueryable`1。
        ////
        //// 参数:
        ////   source:
        ////     要转换的序列。
        ////
        //// 类型参数:
        ////   TElement:
        ////     source 中的元素的类型。
        ////
        //// 返回结果:
        ////     一个 System.Linq.IQueryable`1，表示输入序列。
        ////
        //// 异常:
        ////   T:System.ArgumentNullException:
        ////     source 为 null。
        //public static IQueryable<TElement> AsQueryable<TElement>(this IEnumerable<TElement> source)
        //{
        //    throw new NotSupportedException();
        //}
        //
        // 摘要:
        //     计算可以为 null 的 System.Double 值序列的平均值。
        //
        // 参数:
        //   source:
        //     要计算平均值的可以为 null 的 System.Double 值序列。
        //
        // 返回结果:
        //     值序列的平均值；如果 Source 序列为空或仅包含 null 值，则为 null。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static double? Average(this Query<double?> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算 System.Double 值序列的平均值。
        //
        // 参数:
        //   source:
        //     要计算平均值的 System.Double 值序列。
        //
        // 返回结果:
        //     值序列的平均值。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        //
        //   T:System.InvalidOperationException:
        //     source 中不包含任何元素。
        public static double Average(this Query<double> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算可以为 null 的 System.Single 值序列的平均值。
        //
        // 参数:
        //   source:
        //     要计算平均值的可以为 null 的 System.Single 值序列。
        //
        // 返回结果:
        //     值序列的平均值；如果 Source 序列为空或仅包含 null 值，则为 null。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static float? Average(this Query<float?> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算 System.Single 值序列的平均值。
        //
        // 参数:
        //   source:
        //     要计算平均值的 System.Single 值序列。
        //
        // 返回结果:
        //     值序列的平均值。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        //
        //   T:System.InvalidOperationException:
        //     source 中不包含任何元素。
        public static float Average(this Query<float> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算可以为 null 的 System.Int64 值序列的平均值。
        //
        // 参数:
        //   source:
        //     要计算平均值的可以为 null 的 System.Int64 值序列。
        //
        // 返回结果:
        //     值序列的平均值；如果 Source 序列为空或仅包含 null 值，则为 null。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static double? Average(this Query<long?> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算 System.Int64 值序列的平均值。
        //
        // 参数:
        //   source:
        //     要计算平均值的 System.Int64 值序列。
        //
        // 返回结果:
        //     值序列的平均值。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        //
        //   T:System.InvalidOperationException:
        //     source 中不包含任何元素。
        public static double Average(this Query<long> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算可以为 null 的 System.Int32 值序列的平均值。
        //
        // 参数:
        //   source:
        //     要计算平均值的、可以为 null 的 System.Int32 值序列。
        //
        // 返回结果:
        //     值序列的平均值；如果 Source 序列为空或仅包含 null 值，则为 null。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static double? Average(this Query<int?> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算可以为 null 的 System.Decimal 值序列的平均值。
        //
        // 参数:
        //   source:
        //     要计算平均值的可以为 null 的 System.Decimal 值序列。
        //
        // 返回结果:
        //     值序列的平均值；如果 Source 序列为空或仅包含 null 值，则为 null。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static decimal? Average(this Query<decimal?> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算 System.Decimal 值序列的平均值。
        //
        // 参数:
        //   source:
        //     要计算平均值的 System.Decimal 值序列。
        //
        // 返回结果:
        //     值序列的平均值。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        //
        //   T:System.InvalidOperationException:
        //     source 中不包含任何元素。
        public static decimal Average(this Query<decimal> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算 System.Int32 值序列的平均值。
        //
        // 参数:
        //   source:
        //     要计算平均值的 System.Int32 值序列。
        //
        // 返回结果:
        //     值序列的平均值。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        //
        //   T:System.InvalidOperationException:
        //     source 中不包含任何元素。
        public static double Average(this Query<int> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算可以为 null 的 System.Int32 值序列的平均值，该序列是通过对输入序列中的每个元素调用投影函数而获得的。
        //
        // 参数:
        //   source:
        //     要计算其平均值的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     值序列的平均值；如果 source 序列为空或仅包含 null 值，则为 null。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        public static double? Average<T>(this Query<T> query, Expression<Func<T, int?>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算可以为 null 的 System.Int64 值序列的平均值，该序列是通过对输入序列中的每个元素调用投影函数而获得的。
        //
        // 参数:
        //   source:
        //     要计算其平均值的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     值序列的平均值；如果 source 序列为空或仅包含 null 值，则为 null。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        public static double? Average<T>(this Query<T> query, Expression<Func<T, long?>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算 System.Int64 值序列的平均值，该序列是通过对输入序列中的每个元素调用投影函数而获得的。
        //
        // 参数:
        //   source:
        //     要计算其平均值的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     值序列的平均值。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        //
        //   T:System.InvalidOperationException:
        //     source 中不包含任何元素。
        public static double Average<T>(this Query<T> query, Expression<Func<T, long>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算可以为 null 的 System.Single 值序列的平均值，该序列是通过对输入序列中的每个元素调用投影函数而获得的。
        //
        // 参数:
        //   source:
        //     要计算其平均值的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     值序列的平均值；如果 source 序列为空或仅包含 null 值，则为 null。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        public static float? Average<T>(this Query<T> query, Expression<Func<T, float?>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算 System.Single 值序列的平均值，该序列是通过对输入序列中的每个元素调用投影函数而获得的。
        //
        // 参数:
        //   source:
        //     要计算其平均值的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     值序列的平均值。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        //
        //   T:System.InvalidOperationException:
        //     source 中不包含任何元素。
        public static float Average<T>(this Query<T> query, Expression<Func<T, float>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算 System.Decimal 值序列的平均值，该序列是通过对输入序列中的每个元素调用投影函数而获得的。
        //
        // 参数:
        //   source:
        //     用于计算平均值的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     值序列的平均值。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        //
        //   T:System.InvalidOperationException:
        //     source 中不包含任何元素。
        public static decimal Average<T>(this Query<T> query, Expression<Func<T, decimal>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算可以为 null 的 System.Decimal 值序列的平均值，该序列是通过对输入序列中的每个元素调用投影函数而获得的。
        //
        // 参数:
        //   source:
        //     要计算其平均值的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     值序列的平均值；如果 source 序列为空或仅包含 null 值，则为 null。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        public static decimal? Average<T>(this Query<T> query, Expression<Func<T, decimal?>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算可以为 null 的 System.Double 值序列的平均值，该序列是通过对输入序列中的每个元素调用投影函数而获得的。
        //
        // 参数:
        //   source:
        //     要计算其平均值的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     值序列的平均值；如果 source 序列为空或仅包含 null 值，则为 null。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        public static double? Average<T>(this Query<T> query, Expression<Func<T, double?>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算 System.Double 值序列的平均值，该序列是通过对输入序列中的每个元素调用投影函数而获得的。
        //
        // 参数:
        //   source:
        //     要计算其平均值的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     值序列的平均值。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        //
        //   T:System.InvalidOperationException:
        //     source 中不包含任何元素。
        public static double Average<T>(this Query<T> query, Expression<Func<T, double>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算 System.Int32 值序列的平均值，该序列是通过对输入序列中的每个元素调用投影函数而获得的。
        //
        // 参数:
        //   source:
        //     要计算其平均值的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     值序列的平均值。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        //
        //   T:System.InvalidOperationException:
        //     source 中不包含任何元素。
        public static double Average<T>(this Query<T> query, Expression<Func<T, int>> selector)
        {
            throw new NotSupportedException();
        }
        ////
        //// 摘要:
        ////     将 System.Linq.IQueryable 的元素转换为指定的类型。
        ////
        //// 参数:
        ////   source:
        ////     包含要转换的元素的 System.Linq.IQueryable。
        ////
        //// 类型参数:
        ////   R:
        ////     source 中的元素要转换成的类型。
        ////
        //// 返回结果:
        ////     一个 System.Linq.IQueryable`1，包含被转换为指定类型的源序列中的每个元素。
        ////
        //// 异常:
        ////   T:System.ArgumentNullException:
        ////     source 为 null。
        ////
        ////   T:System.InvalidCastException:
        ////     序列中的元素不能强制转换为 R 类型。
        //public static IQueryable<R> Cast<R>(this IQueryable source)
        //{
        //    throw new NotSupportedException();
        //}
        //
        // 摘要:
        //     连接两个序列。
        //
        // 参数:
        //   source1:
        //     要连接的第一个序列。
        //
        //   source2:
        //     要与第一个序列连接的序列。
        //
        // 类型参数:
        //   T:
        //     输入序列中的元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，包含两个输入序列的连接元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source1 或 source2 为 null。
        public static Query<T> Concat<T>(this Query<T> query1, IEnumerable<T> source2)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     通过使用默认的相等比较器确定序列是否包含指定的元素。
        //
        // 参数:
        //   source:
        //     要在其中定位 item 的 System.Linq.IQueryable`1。
        //
        //   item:
        //     要在序列中定位的对象。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     如果输入序列包含具有指定值的元素，则为 true；否则为 false。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static bool Contains<T>(this Query<T> query, T item)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     通过使用指定的 System.Collections.Generic.IEqualityComparer`1 确定序列是否包含指定的元素。
        //
        // 参数:
        //   source:
        //     要在其中定位 item 的 System.Linq.IQueryable`1。
        //
        //   item:
        //     要在序列中定位的对象。
        //
        //   comparer:
        //     用于比较值的 System.Collections.Generic.IEqualityComparer`1。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     如果输入序列包含具有指定值的元素，则为 true；否则为 false。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static bool Contains<T>(this Query<T> query, T item, IEqualityComparer<T> comparer)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     返回序列中的元素数量。
        //
        // 参数:
        //   source:
        //     包含要计数的元素的 System.Linq.IQueryable`1。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     输入序列中的元素数量。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        //
        //   T:System.OverflowException:
        //     source 中的元素数量大于 System.Int32.MaxValue。
        public static long Count<T>(this Query<T> query)
        {
            return query.ExecuteCount();
        }
        //
        // 摘要:
        //     返回指定序列中满足条件的元素数量。
        //
        // 参数:
        //   source:
        //     包含要进行计数的元素的 System.Linq.IQueryable`1。
        //
        //   predicate:
        //     用于测试每个元素是否满足条件的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     序列中满足谓词函数的条件的元素数量。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 predicate 为 null。
        //
        //   T:System.OverflowException:
        //     source 中的元素数量大于 System.Int32.MaxValue。
        public static long Count<T>(this Query<T> query, Expression<Func<T, bool>> predicate)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     返回指定序列的元素；如果序列为空，则返回单一实例集合中的类型参数的默认值。
        //
        // 参数:
        //   source:
        //     用于在序列为空的情况下返回默认值的 System.Linq.IQueryable`1。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     用于在 source 为空的情况下包含 default(T) 的 System.Linq.IQueryable`1；否则为 source。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static Query<T> DefaultIfEmpty<T>(this Query<T> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     返回指定序列中的元素；如果序列为空，则返回单一实例集合中的指定值。
        //
        // 参数:
        //   source:
        //     用于在序列为空的情况下返回指定值的 System.Linq.IQueryable`1。
        //
        //   defaultValue:
        //     序列为空时要返回的值。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     在 source 为空的情况下包含 defaultValue 的 System.Linq.IQueryable`1；否则为 source。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static Query<T> DefaultIfEmpty<T>(this Query<T> query, T defaultValue)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     通过使用默认的相等比较器对值进行比较返回序列中的非重复元素。
        //
        // 参数:
        //   source:
        //     要从中删除重复项的 System.Linq.IQueryable`1。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     包含 source 中的非重复元素的 System.Linq.IQueryable`1。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static Query<T> Distinct<T>(this Query<T> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     通过使用指定的 System.Collections.Generic.IEqualityComparer`1 对值进行比较返回序列中的非重复元素。
        //
        // 参数:
        //   source:
        //     要从中删除重复项的 System.Linq.IQueryable`1。
        //
        //   comparer:
        //     用于比较值的 System.Collections.Generic.IEqualityComparer`1。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     包含 source 中的非重复元素的 System.Linq.IQueryable`1。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 comparer 为 null。
        public static Query<T> Distinct<T>(this Query<T> query, IEqualityComparer<T> comparer)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     返回序列中指定索引处的元素。
        //
        // 参数:
        //   source:
        //     要从中返回元素的 System.Linq.IQueryable`1。
        //
        //   index:
        //     要检索的从零开始的元素索引。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     source 中指定位置处的元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        //
        //   T:System.ArgumentOutOfRangeException:
        //     index 小于零。
        public static T ElementAt<T>(this Query<T> query, int index)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     返回序列中指定索引处的元素；如果索引超出范围，则返回默认值。
        //
        // 参数:
        //   source:
        //     要从中返回元素的 System.Linq.IQueryable`1。
        //
        //   index:
        //     要检索的从零开始的元素索引。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     如果 index 超出 source 的界限，则返回 default(T)；否则返回 source 中指定位置处的元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static T ElementAtOrDefault<T>(this Query<T> query, int index)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     通过使用默认的相等比较器对值进行比较生成两个序列的差集。
        //
        // 参数:
        //   source1:
        //     一个 System.Linq.IQueryable`1，将返回其不在 source2 中的元素。
        //
        //   source2:
        //     一个 System.Collections.Generic.IEnumerable`1，其也出现在第一个序列中的元素将不会出现在返回的序列中。
        //
        // 类型参数:
        //   T:
        //     输入序列中的元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，包含两个序列的差集。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source1 或 source2 为 null。
        public static Query<T> Except<T>(this Query<T> query1, IEnumerable<T> source2)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     通过使用指定的 System.Collections.Generic.IEqualityComparer`1 对值进行比较产生两个序列的差集。
        //
        // 参数:
        //   source1:
        //     一个 System.Linq.IQueryable`1，将返回其不在 source2 中的元素。
        //
        //   source2:
        //     一个 System.Collections.Generic.IEnumerable`1，其也出现在第一个序列中的元素将不会出现在返回的序列中。
        //
        //   comparer:
        //     用于比较值的 System.Collections.Generic.IEqualityComparer`1。
        //
        // 类型参数:
        //   T:
        //     输入序列中的元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，包含两个序列的差集。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source1 或 source2 为 null。
        public static Query<T> Except<T>(this Query<T> query1, IEnumerable<T> source2, IEqualityComparer<T> comparer)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     返回序列中的第一个元素。
        //
        // 参数:
        //   source:
        //     要返回其第一个元素的 System.Linq.IQueryable`1。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     source 中的第一个元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        //
        //   T:System.InvalidOperationException:
        //     源序列为空。
        public static T First<T>(this Query<T> query)
        {
            return query.ExecuteSingleRow();
        }
        //
        // 摘要:
        //     返回序列中满足指定条件的第一个元素。
        //
        // 参数:
        //   source:
        //     要从中返回元素的 System.Linq.IQueryable`1。
        //
        //   predicate:
        //     用于测试每个元素是否满足条件的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     通过 predicate 中测试的 source 中的第一个元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 predicate 为 null。
        //
        //   T:System.InvalidOperationException:
        //     没有元素满足 predicate 中的条件。- 或 -源序列为空。
        public static T First<T>(this Query<T> query, Expression<Func<T, bool>> predicate)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     返回序列中的第一个元素；如果序列中不包含任何元素，则返回默认值。
        //
        // 参数:
        //   source:
        //     要返回其第一个元素的 System.Linq.IQueryable`1。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     如果 source 为空，则返回 default(T)；否则返回 source 中的第一个元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static T FirstOrDefault<T>(this Query<T> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     返回序列中满足指定条件的第一个元素，如果未找到这样的元素，则返回默认值。
        //
        // 参数:
        //   source:
        //     要从中返回元素的 System.Linq.IQueryable`1。
        //
        //   predicate:
        //     用于测试每个元素是否满足条件的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     如果 source 为空或没有元素通过 predicate 指定的测试，则返回 default(T)，否则返回 source 中通过 predicate
        //     指定的测试的第一个元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 predicate 为 null。
        public static T FirstOrDefault<T>(this Query<T> query, Expression<Func<T, bool>> predicate)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     根据指定的键选择器函数对序列中的元素进行分组。
        //
        // 参数:
        //   source:
        //     要对其元素进行分组的 System.Linq.IQueryable`1。
        //
        //   keySelector:
        //     用于提取每个元素的键的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   TKey:
        //     由 keySelector 表示的函数返回的键类型。
        //
        // 返回结果:
        //     在 C# 中为 IQueryable<IGrouping<TKey, T>>，或者在 Visual Basic 中为 IQueryable(Of
        //     IGrouping(Of TKey, T))，其中每个 System.Linq.IGrouping`2 对象都包含一个对象序列和一个键。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 keySelector 为 null。
        public static Query<T> GroupBy<T, K>(this Query<T> query, Expression<Func<T, K>> keySelector)
        {
            return query.GroupByImpl(keySelector);
        }
        //
        // 摘要:
        //     根据指定的键选择器函数对序列中的元素进行分组，并使用指定的比较器对键进行比较。
        //
        // 参数:
        //   source:
        //     要对其元素进行分组的 System.Linq.IQueryable`1。
        //
        //   keySelector:
        //     用于提取每个元素的键的函数。
        //
        //   comparer:
        //     一个用于对键进行比较的 System.Collections.Generic.IEqualityComparer`1。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   TKey:
        //     由 keySelector 表示的函数返回的键类型。
        //
        // 返回结果:
        //     在 C# 中为 IQueryable<IGrouping<TKey, T>>，或者在 Visual Basic 中为 IQueryable(Of
        //     IGrouping(Of TKey, T))，其中每个 System.Linq.IGrouping`2 都包含一个对象序列和一个键。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 keySelector 或 comparer 为 null。
        public static Query<T> GroupBy<T, TKey>(this Query<T> query, Expression<Func<T, TKey>> keySelector, IEqualityComparer<TKey> comparer)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     根据指定的键选择器函数对序列中的元素进行分组，并且从每个组及其键中创建结果值。
        //
        // 参数:
        //   source:
        //     要对其元素进行分组的 System.Linq.IQueryable`1。
        //
        //   keySelector:
        //     用于提取每个元素的键的函数。
        //
        //   resultSelector:
        //     用于从每个组中创建结果值的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   TKey:
        //     由 keySelector 表示的函数返回的键类型。
        //
        //   R:
        //     resultSelector 返回的结果值的类型。
        //
        // 返回结果:
        //     一个 T:System.Linq.IQueryable`1，它具有 R 的类型参数，并且其中每个元素都表示对一个组及其键的投影。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 keySelector 或 resultSelector 为 null。
        public static Query<R> GroupBy<T, TKey, R>(this Query<T> query, Expression<Func<T, TKey>> keySelector, Expression<Func<TKey, IEnumerable<T>, R>> resultSelector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     根据指定的键选择器函数对序列中的元素进行分组，并且通过使用指定的函数对每个组中的元素进行投影。
        //
        // 参数:
        //   source:
        //     要对其元素进行分组的 System.Linq.IQueryable`1。
        //
        //   keySelector:
        //     用于提取每个元素的键的函数。
        //
        //   elementSelector:
        //     用于将每个源元素映射到 System.Linq.IGrouping`2 中的元素的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   TKey:
        //     由 keySelector 表示的函数返回的键类型。
        //
        //   TElement:
        //     每个 System.Linq.IGrouping`2 中的元素的类型。
        //
        // 返回结果:
        //     在 C# 中为 IQueryable<IGrouping<TKey, TElement>>，或在 Visual Basic 中为 IQueryable(Of
        //     IGrouping(Of TKey, TElement))，其中每个 System.Linq.IGrouping`2 都包含一个 TElement 类型的对象序列和一个键。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 keySelector 或 elementSelector 为 null。
        public static Query<T> GroupBy<T, K, E>(this Query<T> query, Expression<Func<T, K>> keySelector, Expression<Func<T, E>> elementSelector)
        {
            return query.GroupByImpl(keySelector, elementSelector);
        }
        //
        // 摘要:
        //     根据指定的键选择器函数对序列中的元素进行分组，并且从每个组及其键中创建结果值。通过使用指定的比较器对键进行比较。
        //
        // 参数:
        //   source:
        //     要对其元素进行分组的 System.Linq.IQueryable`1。
        //
        //   keySelector:
        //     用于提取每个元素的键的函数。
        //
        //   resultSelector:
        //     用于从每个组中创建结果值的函数。
        //
        //   comparer:
        //     一个用于对键进行比较的 System.Collections.Generic.IEqualityComparer`1。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   TKey:
        //     由 keySelector 表示的函数返回的键类型。
        //
        //   R:
        //     resultSelector 返回的结果值的类型。
        //
        // 返回结果:
        //     一个 T:System.Linq.IQueryable`1，它具有 R 的类型参数，并且其中每个元素都表示对一个组及其键的投影。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 keySelector 或 resultSelector 或 comparer 为 null。
        public static Query<R> GroupBy<T, TKey, R>(this Query<T> query, Expression<Func<T, TKey>> keySelector, Expression<Func<TKey, IEnumerable<T>, R>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     对序列中的元素进行分组并且通过使用指定的函数对每组中的元素进行投影。通过使用指定的比较器对键值进行比较。
        //
        // 参数:
        //   source:
        //     要对其元素进行分组的 System.Linq.IQueryable`1。
        //
        //   keySelector:
        //     用于提取每个元素的键的函数。
        //
        //   elementSelector:
        //     用于将每个源元素映射到 System.Linq.IGrouping`2 中的元素的函数。
        //
        //   comparer:
        //     一个用于对键进行比较的 System.Collections.Generic.IEqualityComparer`1。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   TKey:
        //     由 keySelector 表示的函数返回的键类型。
        //
        //   TElement:
        //     每个 System.Linq.IGrouping`2 中的元素的类型。
        //
        // 返回结果:
        //     在 C# 中为 IQueryable<IGrouping<TKey, TElement>>，或在 Visual Basic 中为 IQueryable(Of
        //     IGrouping(Of TKey, TElement))，其中每个 System.Linq.IGrouping`2 都包含一个 TElement 类型的对象序列和一个键。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 keySelector 或 elementSelector 或 comparer 为 null。
        public static Query<T> GroupBy<T, TKey, TElement>(this Query<T> query, Expression<Func<T, TKey>> keySelector, Expression<Func<T, TElement>> elementSelector, IEqualityComparer<TKey> comparer)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     根据指定的键选择器函数对序列中的元素进行分组，并且从每个组及其键中创建结果值。通过使用指定的函数对每个组的元素进行投影。
        //
        // 参数:
        //   source:
        //     要对其元素进行分组的 System.Linq.IQueryable`1。
        //
        //   keySelector:
        //     用于提取每个元素的键的函数。
        //
        //   elementSelector:
        //     用于将每个源元素映射到 System.Linq.IGrouping`2 中的元素的函数。
        //
        //   resultSelector:
        //     用于从每个组中创建结果值的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   TKey:
        //     由 keySelector 表示的函数返回的键类型。
        //
        //   TElement:
        //     每个 System.Linq.IGrouping`2 中的元素的类型。
        //
        //   R:
        //     resultSelector 返回的结果值的类型。
        //
        // 返回结果:
        //     一个 T:System.Linq.IQueryable`1，它具有 R 的类型参数，并且其中每个元素都表示对一个组及其键的投影。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 keySelector 或 elementSelector 或 resultSelector 为 null。
        public static Query<R> GroupBy<T, TKey, TElement, R>(this Query<T> query, Expression<Func<T, TKey>> keySelector, Expression<Func<T, TElement>> elementSelector, Expression<Func<TKey, IEnumerable<TElement>, R>> resultSelector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     根据指定的键选择器函数对序列中的元素进行分组，并且从每个组及其键中创建结果值。通过使用指定的比较器对键进行比较，并且通过使用指定的函数对每个组的元素进行投影。
        //
        // 参数:
        //   source:
        //     要对其元素进行分组的 System.Linq.IQueryable`1。
        //
        //   keySelector:
        //     用于提取每个元素的键的函数。
        //
        //   elementSelector:
        //     用于将每个源元素映射到 System.Linq.IGrouping`2 中的元素的函数。
        //
        //   resultSelector:
        //     用于从每个组中创建结果值的函数。
        //
        //   comparer:
        //     一个用于对键进行比较的 System.Collections.Generic.IEqualityComparer`1。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   TKey:
        //     由 keySelector 表示的函数返回的键类型。
        //
        //   TElement:
        //     每个 System.Linq.IGrouping`2 中的元素的类型。
        //
        //   R:
        //     resultSelector 返回的结果值的类型。
        //
        // 返回结果:
        //     一个 T:System.Linq.IQueryable`1，它具有 R 的类型参数，并且其中每个元素都表示对一个组及其键的投影。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 keySelector 或 elementSelector 或 resultSelector 或 comparer 为 null。
        public static Query<R> GroupBy<T, TKey, TElement, R>(this Query<T> query, Expression<Func<T, TKey>> keySelector, Expression<Func<T, TElement>> elementSelector, Expression<Func<TKey, IEnumerable<TElement>, R>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     基于键相等对两个序列的元素进行关联并对结果进行分组。使用默认的相等比较器对键进行比较。
        //
        // 参数:
        //   outer:
        //     要联接的第一个序列。
        //
        //   inner:
        //     要与第一个序列联接的序列。
        //
        //   outerKeySelector:
        //     用于从第一个序列的每个元素提取联接键的函数。
        //
        //   innerKeySelector:
        //     用于从第二个序列的每个元素提取联接键的函数。
        //
        //   resultSelector:
        //     用于从第一个序列的元素和第二个序列的匹配元素集合中创建结果元素的函数。
        //
        // 类型参数:
        //   TOuter:
        //     第一个序列中的元素的类型。
        //
        //   TInner:
        //     第二个序列中的元素的类型。
        //
        //   TKey:
        //     键选择器函数返回的键的类型。
        //
        //   R:
        //     结果元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，包含通过对两个序列执行已分组的联接而获得的 R 类型的元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     outer 或 inner 或 outerKeySelector 或 innerKeySelector 或 resultSelector 为 null。
        public static Query<R> GroupJoin<TOuter, TInner, TKey, R>(this IQueryable<TOuter> outer, IEnumerable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, IEnumerable<TInner>, R>> resultSelector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     基于键相等对两个序列的元素进行关联并对结果进行分组。使用指定的 System.Collections.Generic.IEqualityComparer`1
        //     对键进行比较。
        //
        // 参数:
        //   outer:
        //     要联接的第一个序列。
        //
        //   inner:
        //     要与第一个序列联接的序列。
        //
        //   outerKeySelector:
        //     用于从第一个序列的每个元素提取联接键的函数。
        //
        //   innerKeySelector:
        //     用于从第二个序列的每个元素提取联接键的函数。
        //
        //   resultSelector:
        //     用于从第一个序列的元素和第二个序列的匹配元素集合中创建结果元素的函数。
        //
        //   comparer:
        //     用于对键进行哈希处理和比较的比较器。
        //
        // 类型参数:
        //   TOuter:
        //     第一个序列中的元素的类型。
        //
        //   TInner:
        //     第二个序列中的元素的类型。
        //
        //   TKey:
        //     键选择器函数返回的键的类型。
        //
        //   R:
        //     结果元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，包含通过对两个序列执行已分组的联接而获得的 R 类型的元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     outer 或 inner 或 outerKeySelector 或 innerKeySelector 或 resultSelector 为 null。
        public static Query<R> GroupJoin<TOuter, TInner, TKey, R>(this IQueryable<TOuter> outer, IEnumerable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, IEnumerable<TInner>, R>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     通过使用默认的相等比较器对值进行比较生成两个序列的交集。
        //
        // 参数:
        //   source1:
        //     一个序列，将返回其也出现在 source2 中的非重复元素。
        //
        //   source2:
        //     一个序列，将返回其也在第一个序列中出现的非重复元素。
        //
        // 类型参数:
        //   T:
        //     输入序列中的元素的类型。
        //
        // 返回结果:
        //     一个包含两个序列的交集的序列。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source1 或 source2 为 null。
        public static Query<T> Intersect<T>(this Query<T> query1, IEnumerable<T> source2)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     通过使用指定的 System.Collections.Generic.IEqualityComparer`1 对值进行比较以生成两个序列的交集。
        //
        // 参数:
        //   source1:
        //     一个 System.Linq.IQueryable`1，将返回其也出现在 source2 中的非重复元素。
        //
        //   source2:
        //     一个 System.Collections.Generic.IEnumerable`1，将返回其也出现在第一个序列中的非重复元素。
        //
        //   comparer:
        //     用于比较值的 System.Collections.Generic.IEqualityComparer`1。
        //
        // 类型参数:
        //   T:
        //     输入序列中的元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，它包含两个序列的交集。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source1 或 source2 为 null。
        public static Query<T> Intersect<T>(this Query<T> query1, IEnumerable<T> source2, IEqualityComparer<T> comparer)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     基于匹配键对两个序列的元素进行关联。使用默认的相等比较器对键进行比较。
        //
        // 参数:
        //   outer:
        //     要联接的第一个序列。
        //
        //   inner:
        //     要与第一个序列联接的序列。
        //
        //   outerKeySelector:
        //     用于从第一个序列的每个元素提取联接键的函数。
        //
        //   innerKeySelector:
        //     用于从第二个序列的每个元素提取联接键的函数。
        //
        //   resultSelector:
        //     用于从两个匹配元素创建结果元素的函数。
        //
        // 类型参数:
        //   TOuter:
        //     第一个序列中的元素的类型。
        //
        //   TInner:
        //     第二个序列中的元素的类型。
        //
        //   TKey:
        //     键选择器函数返回的键的类型。
        //
        //   R:
        //     结果元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，具有通过对两个序列执行内部联接而获得的 R 类型的元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     outer 或 inner 或 outerKeySelector 或 innerKeySelector 或 resultSelector 为 null。
        public static Query<R> Join<A, B, K, R>(this Query<A> outer, IEnumerable<B> inner, Expression<Func<A, K>> outerKey, Expression<Func<B, K>> innerKey, Expression<Func<A, B, R>> result)
        {
            return outer.JoinImpl(inner, outerKey, innerKey, result);
        }
        //
        // 摘要:
        //     基于匹配键对两个序列的元素进行关联。使用指定的 System.Collections.Generic.IEqualityComparer`1 对键进行比较。
        //
        // 参数:
        //   outer:
        //     要联接的第一个序列。
        //
        //   inner:
        //     要与第一个序列联接的序列。
        //
        //   outerKeySelector:
        //     用于从第一个序列的每个元素提取联接键的函数。
        //
        //   innerKeySelector:
        //     用于从第二个序列的每个元素提取联接键的函数。
        //
        //   resultSelector:
        //     用于从两个匹配元素创建结果元素的函数。
        //
        //   comparer:
        //     一个 System.Collections.Generic.IEqualityComparer`1，用于对键进行哈希处理和比较。
        //
        // 类型参数:
        //   TOuter:
        //     第一个序列中的元素的类型。
        //
        //   TInner:
        //     第二个序列中的元素的类型。
        //
        //   TKey:
        //     键选择器函数返回的键的类型。
        //
        //   R:
        //     结果元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，具有通过对两个序列执行内部联接而获得的 R 类型的元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     outer 或 inner 或 outerKeySelector 或 innerKeySelector 或 resultSelector 为 null。
        public static Query<R> Join<TOuter, TInner, TKey, R>(this Query<TOuter> outer, IEnumerable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, R>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     返回序列中的最后一个元素。
        //
        // 参数:
        //   source:
        //     要返回其最后一个元素的 System.Linq.IQueryable`1。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     位于 source 中最后位置处的值。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        //
        //   T:System.InvalidOperationException:
        //     源序列为空。
        public static T Last<T>(this Query<T> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     返回序列中满足指定条件的最后一个元素。
        //
        // 参数:
        //   source:
        //     要从中返回元素的 System.Linq.IQueryable`1。
        //
        //   predicate:
        //     用于测试每个元素是否满足条件的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     source 中的最后一个元素，它通过了由 predicate 指定的测试。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 predicate 为 null。
        //
        //   T:System.InvalidOperationException:
        //     没有元素满足 predicate 中的条件。- 或 -源序列为空。
        public static T Last<T>(this Query<T> query, Expression<Func<T, bool>> predicate)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     返回序列中的最后一个元素，如果序列中不包含任何元素，则返回默认值。
        //
        // 参数:
        //   source:
        //     要返回其最后一个元素的 System.Linq.IQueryable`1。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     如果 source 为空，则返回 default(T)；否则返回 source 中的最后一个元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static T LastOrDefault<T>(this Query<T> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     返回序列中满足条件的最后一个元素；如果未找到这样的元素，则返回默认值。
        //
        // 参数:
        //   source:
        //     要从中返回元素的 System.Linq.IQueryable`1。
        //
        //   predicate:
        //     用于测试每个元素是否满足条件的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     如果 source 为空或没有元素通过谓词函数中的测试，则返回 default(T)；否则，返回通过谓词函数中测试的 source 的最后一个元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 predicate 为 null。
        public static T LastOrDefault<T>(this Query<T> query, Expression<Func<T, bool>> predicate)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     返回一个 System.Int64，表示序列中的元素的总数量。
        //
        // 参数:
        //   source:
        //     包含要进行计数的元素的 System.Linq.IQueryable`1。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     source 中的元素的数量。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        //
        //   T:System.OverflowException:
        //     元素的数量超过 System.Int64.MaxValue。
        public static long LongCount<T>(this Query<T> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     返回一个 System.Int64，它表示序列中满足条件的元素数量。
        //
        // 参数:
        //   source:
        //     包含要进行计数的元素的 System.Linq.IQueryable`1。
        //
        //   predicate:
        //     用于测试每个元素是否满足条件的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     source 中满足谓词函数的条件的元素数量。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 predicate 为 null。
        //
        //   T:System.OverflowException:
        //     匹配元素的数量超过 System.Int64.MaxValue。
        public static long LongCount<T>(this Query<T> query, Expression<Func<T, bool>> predicate)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     返回泛型 System.Linq.IQueryable`1 中的最大值。
        //
        // 参数:
        //   source:
        //     要确定最大值的值序列。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     序列中的最大值。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static T Max<T>(this Query<T> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     对泛型 System.Linq.IQueryable`1 的每个元素调用投影函数，并返回最大结果值。
        //
        // 参数:
        //   source:
        //     要确定最大值的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   R:
        //     由 selector 表示的函数返回的值类型。
        //
        // 返回结果:
        //     序列中的最大值。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        public static R Max<T, R>(this Query<T> query, Expression<Func<T, R>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     返回泛型 System.Linq.IQueryable`1 中的最小值。
        //
        // 参数:
        //   source:
        //     要确定最小值的值序列。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     序列中的最小值。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static T Min<T>(this Query<T> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     对泛型 System.Linq.IQueryable`1 的每个元素调用投影函数，并返回最小结果值。
        //
        // 参数:
        //   source:
        //     要确定最小值的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   R:
        //     由 selector 表示的函数返回的值类型。
        //
        // 返回结果:
        //     序列中的最小值。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        public static R Min<T, R>(this Query<T> query, Expression<Func<T, R>> selector)
        {
            throw new NotSupportedException();
        }
        ////
        //// 摘要:
        ////     根据指定类型筛选 System.Linq.IQueryable 的元素。
        ////
        //// 参数:
        ////   source:
        ////     要对其元素进行筛选的 System.Linq.IQueryable。
        ////
        //// 类型参数:
        ////   R:
        ////     筛选序列元素所根据的类型。
        ////
        //// 返回结果:
        ////     一个集合，包含 source 中的类型为 R 的元素。
        ////
        //// 异常:
        ////   T:System.ArgumentNullException:
        ////     source 为 null。
        //public static IQueryable<R> OfType<R>(this IQueryable source)
        //{
        //    throw new NotSupportedException();
        //}
        //
        // 摘要:
        //     根据键按升序对序列的元素排序。
        //
        // 参数:
        //   source:
        //     一个要排序的值序列。
        //
        //   keySelector:
        //     用于从元素中提取键的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   TKey:
        //     由 keySelector 表示的函数返回的键类型。
        //
        // 返回结果:
        //     一个 System.Linq.IOrderedQueryable`1，根据键对其元素排序。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 keySelector 为 null。
        public static Query<T> OrderBy<T, K>(this Query<T> query, Expression<Func<T, K>> selector)
        {
            return query.OrderByImpl(selector, DataSortType.Asc);
        }
        //
        // 摘要:
        //     使用指定的比较器按升序对序列的元素排序。
        //
        // 参数:
        //   source:
        //     一个要排序的值序列。
        //
        //   keySelector:
        //     用于从元素中提取键的函数。
        //
        //   comparer:
        //     一个用于比较键的 System.Collections.Generic.IComparer`1。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   TKey:
        //     由 keySelector 表示的函数返回的键类型。
        //
        // 返回结果:
        //     一个 System.Linq.IOrderedQueryable`1，根据键对其元素排序。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 keySelector 或 comparer 为 null。
        public static Query<T> OrderBy<T, K>(this Query<T> query, Expression<Func<T, K>> selector, IComparer<K> comparer)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     根据键按降序对序列的元素排序。
        //
        // 参数:
        //   source:
        //     一个要排序的值序列。
        //
        //   keySelector:
        //     用于从元素中提取键的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   TKey:
        //     由 keySelector 表示的函数返回的键类型。
        //
        // 返回结果:
        //     一个 System.Linq.IOrderedQueryable`1，将根据键按降序对其元素进行排序。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 keySelector 为 null。
        public static Query<T> OrderByDescending<T, K>(this Query<T> query, Expression<Func<T, K>> selector)
        {
            return query.OrderByImpl(selector, DataSortType.Desc);
        }
        //
        // 摘要:
        //     使用指定的比较器按降序对序列的元素排序。
        //
        // 参数:
        //   source:
        //     一个要排序的值序列。
        //
        //   keySelector:
        //     用于从元素中提取键的函数。
        //
        //   comparer:
        //     一个用于比较键的 System.Collections.Generic.IComparer`1。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   TKey:
        //     由 keySelector 表示的函数返回的键类型。
        //
        // 返回结果:
        //     一个 System.Linq.IOrderedQueryable`1，将根据键按降序对其元素进行排序。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 keySelector 或 comparer 为 null。
        public static Query<T> OrderByDescending<T, K>(this Query<T> query, Expression<Func<T, K>> selector, IComparer<K> comparer)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     反转序列中元素的顺序。
        //
        // 参数:
        //   source:
        //     要反转的值序列。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，其元素以相反顺序对应于输入序列的元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static Query<T> Reverse<T>(this Query<T> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     通过合并元素的索引将序列的每个元素投影到新表中。
        //
        // 参数:
        //   source:
        //     一个要投影的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   R:
        //     由 selector 表示的函数返回的值类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，其元素为对 source 的每个元素调用投影函数的结果。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        public static Query<R> Select<T, R>(this Query<T> query, Expression<Func<T, int, R>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     将序列中的每个元素投影到新表中。
        //
        // 参数:
        //   source:
        //     一个要投影的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   R:
        //     由 selector 表示的函数返回的值类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，其元素为对 source 的每个元素调用投影函数的结果。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        public static Query<R> Select<T, R>(this Query<T> query, Expression<Func<T, R>> selector)
        {
            return query.SelectImpl(selector);
        }
        //
        // 摘要:
        //     将序列的每个元素投影到一个 System.Collections.Generic.IEnumerable`1，并将结果序列组合为一个序列。每个源元素的索引用于该元素的投影表。
        //
        // 参数:
        //   source:
        //     一个要投影的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数；此函数的第二个参数表示源元素的索引。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   R:
        //     由 selector 表示的函数返回的序列中的元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，其元素是对输入序列的每个元素调用一对多投影函数的结果。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        public static Query<R> SelectMany<T, R>(this Query<T> query, Expression<Func<T, int, IEnumerable<R>>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     将序列的每个元素投影到一个 System.Collections.Generic.IEnumerable`1，并将结果序列组合为一个序列。
        //
        // 参数:
        //   source:
        //     一个要投影的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   R:
        //     由 selector 表示的函数返回的序列中的元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，其元素是对输入序列的每个元素调用一对多投影函数的结果。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        public static Query<R> SelectMany<T, R>(this Query<T> query, Expression<Func<T, IEnumerable<R>>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     将序列的每个元素投影到一个 System.Collections.Generic.IEnumerable`1，并对其中的每个元素调用结果选择器函数。每个中间序列的结果值都组合为一个一维序列，并将其返回。
        //
        // 参数:
        //   source:
        //     一个要投影的值序列。
        //
        //   collectionSelector:
        //     一个应用于输入序列的每个元素的投影函数。
        //
        //   resultSelector:
        //     一个应用于每个中间序列的每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   TCollection:
        //     由 collectionSelector 表示的函数收集的中间元素类型。
        //
        //   R:
        //     结果序列的元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，其元素是通过对 source 的每个元素调用一对多投影函数 collectionSelector，然后将这些序列元素的每一个及其对应的
        //     source 元素映射到结果元素中的结果。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 collectionSelector 或 resultSelector 为 null。
        public static Query<R> SelectMany<T, TCollection, R>(this Query<T> query, Expression<Func<T, IEnumerable<TCollection>>> collectionSelector, Expression<Func<T, TCollection, R>> resultSelector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     将序列中的每个元素投影到一个 System.Collections.Generic.IEnumerable`1，它合并了生成它的源元素的索引。对每个中间序列的每个元素调用结果选择器函数，并且将结果值合并为一个一维序列，并将其返回。
        //
        // 参数:
        //   source:
        //     一个要投影的值序列。
        //
        //   collectionSelector:
        //     要应用于输入序列的每个元素的投影函数；此函数的第二个参数表示源元素的索引。
        //
        //   resultSelector:
        //     一个应用于每个中间序列的每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   TCollection:
        //     由 collectionSelector 表示的函数收集的中间元素类型。
        //
        //   R:
        //     结果序列的元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，其元素是通过对 source 的每个元素调用一对多投影函数 collectionSelector，然后将这些序列元素的每一个及其对应的
        //     source 元素映射到结果元素中的结果。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 collectionSelector 或 resultSelector 为 null。
        public static Query<R> SelectMany<T, TCollection, R>(this Query<T> query, Expression<Func<T, int, IEnumerable<TCollection>>> collectionSelector, Expression<Func<T, TCollection, R>> resultSelector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     通过使用默认的相等比较器比较元素以确定两个序列是否相等。
        //
        // 参数:
        //   source1:
        //     一个 System.Linq.IQueryable`1，其元素用于与 source2 中的元素进行比较。
        //
        //   source2:
        //     一个 System.Collections.Generic.IEnumerable`1，其元素用于与第一个序列中的元素进行比较。
        //
        // 类型参数:
        //   T:
        //     输入序列中的元素的类型。
        //
        // 返回结果:
        //     如果两个源序列的长度相等并且它们的对应元素也相等，则为 true；否则为 false。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source1 或 source2 为 null。
        public static bool SequenceEqual<T>(this Query<T> query1, IEnumerable<T> source2)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     通过使用指定的 System.Collections.Generic.IEqualityComparer`1 比较元素以确定两个序列是否相等。
        //
        // 参数:
        //   source1:
        //     一个 System.Linq.IQueryable`1，其元素用于与 source2 中的元素进行比较。
        //
        //   source2:
        //     一个 System.Collections.Generic.IEnumerable`1，其元素用于与第一个序列中的元素进行比较。
        //
        //   comparer:
        //     一个用于比较元素的 System.Collections.Generic.IEqualityComparer`1。
        //
        // 类型参数:
        //   T:
        //     输入序列中的元素的类型。
        //
        // 返回结果:
        //     如果两个源序列的长度相等并且它们的对应元素也相等，则为 true；否则为 false。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source1 或 source2 为 null。
        public static bool SequenceEqual<T>(this Query<T> query1, IEnumerable<T> source2, IEqualityComparer<T> comparer)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     返回序列的唯一元素；如果该序列并非恰好包含一个元素，则会引发异常。
        //
        // 参数:
        //   source:
        //     要返回其单个元素的 System.Linq.IQueryable`1。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     输入序列的单个元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        //
        //   T:System.InvalidOperationException:
        //     source 具有多个元素。
        public static T Single<T>(this Query<T> query)
        {
            if (query == null)
                throw new ArgumentNullException("query");
            IList<T> list = query.ExecuteReader(2);
            if (list.Count == 1)
                return list[0];
            if (list.Count > 0)
                throw new InvalidOperationException();
            return default(T);
        }
        //
        // 摘要:
        //     返回序列中满足指定条件的唯一元素；如果有多个这样的元素存在，则会引发异常。
        //
        // 参数:
        //   source:
        //     要从中返回单个元素的 System.Linq.IQueryable`1。
        //
        //   predicate:
        //     用于测试元素是否满足条件的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     满足 predicate 中条件的输入序列中的单个元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 predicate 为 null。
        //
        //   T:System.InvalidOperationException:
        //     没有元素满足 predicate 中的条件。- 或 -多个元素满足 predicate 中的条件。- 或 -源序列为空。
        public static T Single<T>(this Query<T> query, Expression<Func<T, bool>> predicate)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     返回序列中的唯一元素；如果该序列为空，则返回默认值；如果该序列包含多个元素，此方法将引发异常。
        //
        // 参数:
        //   source:
        //     要返回其单个元素的 System.Linq.IQueryable`1。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     返回输入序列的单个元素；如果序列不包含任何元素，则返回 default(T)。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        //
        //   T:System.InvalidOperationException:
        //     source 具有多个元素。
        public static T SingleOrDefault<T>(this Query<T> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     返回序列中满足指定条件的唯一元素；如果这类元素不存在，则返回默认值；如果有多个元素满足该条件，此方法将引发异常。
        //
        // 参数:
        //   source:
        //     要从中返回单个元素的 System.Linq.IQueryable`1。
        //
        //   predicate:
        //     用于测试元素是否满足条件的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     返回满足 predicate 中条件的输入序列的单个元素；如果未找到这样的元素，则返回 default(T)。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 predicate 为 null。
        //
        //   T:System.InvalidOperationException:
        //     多个元素满足 predicate 中的条件。
        public static T SingleOrDefault<T>(this Query<T> query, Expression<Func<T, bool>> predicate)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     跳过序列中指定数量的元素，然后返回剩余的元素。
        //
        // 参数:
        //   source:
        //     要从中返回元素的 System.Linq.IQueryable`1。
        //
        //   count:
        //     返回剩余元素前要跳过的元素数量。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，包含输入序列中指定索引后出现的元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static Query<T> Skip<T>(this Query<T> query, int count)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     只要满足指定的条件，就跳过序列中的元素，然后返回剩余元素。将在谓词函数的逻辑中使用元素的索引。
        //
        // 参数:
        //   source:
        //     要从中返回元素的 System.Linq.IQueryable`1。
        //
        //   predicate:
        //     用于测试每个元素是否满足条件的函数；此函数的第二个参数表示源元素的索引。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，包含从未通过 predicate 指定测试的线性系列中的第一个元素开始的 source 中的元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 predicate 为 null。
        public static Query<T> SkipWhile<T>(this Query<T> query, Expression<Func<T, int, bool>> predicate)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     只要满足指定的条件，就跳过序列中的元素，然后返回剩余元素。
        //
        // 参数:
        //   source:
        //     要从中返回元素的 System.Linq.IQueryable`1。
        //
        //   predicate:
        //     用于测试每个元素是否满足条件的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，包含从未通过 predicate 指定测试的线性系列中的第一个元素开始的 source 中的元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 predicate 为 null。
        public static Query<T> SkipWhile<T>(this Query<T> query, Expression<Func<T, bool>> predicate)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算可以为 null 的 System.Single 值序列之和。
        //
        // 参数:
        //   source:
        //     要计算和的可以为 null 的 System.Single 值序列。
        //
        // 返回结果:
        //     序列值之和。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static float? Sum(this Query<float?> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算 System.Double 值序列之和。
        //
        // 参数:
        //   source:
        //     一个要计算和的 System.Double 值序列。
        //
        // 返回结果:
        //     序列值之和。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static double Sum(this Query<double> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算 System.Decimal 值序列之和。
        //
        // 参数:
        //   source:
        //     一个要计算和的 System.Decimal 值序列。
        //
        // 返回结果:
        //     序列值之和。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        //
        //   T:System.OverflowException:
        //     和大于 System.Decimal.MaxValue。
        public static decimal Sum(this Query<decimal> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算可以为 null 的 System.Decimal 值序列之和。
        //
        // 参数:
        //   source:
        //     要计算和的可以为 null 的 System.Decimal 值序列。
        //
        // 返回结果:
        //     序列值之和。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        //
        //   T:System.OverflowException:
        //     和大于 System.Decimal.MaxValue。
        public static decimal? Sum(this Query<decimal?> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算可以为 null 的 System.Double 值序列之和。
        //
        // 参数:
        //   source:
        //     要计算和的可以为 null 的 System.Double 值序列。
        //
        // 返回结果:
        //     序列值之和。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static double? Sum(this Query<double?> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算 System.Int64 值序列之和。
        //
        // 参数:
        //   source:
        //     一个要计算和的 System.Int64 值序列。
        //
        // 返回结果:
        //     序列值之和。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        //
        //   T:System.OverflowException:
        //     和大于 System.Int64.MaxValue。
        public static long Sum(this Query<long> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算可以为 null 的 System.Int64 值序列之和。
        //
        // 参数:
        //   source:
        //     要计算和的可以为 null 的 System.Int64 值序列。
        //
        // 返回结果:
        //     序列值之和。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        //
        //   T:System.OverflowException:
        //     和大于 System.Int64.MaxValue。
        public static long? Sum(this Query<long?> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算 System.Single 值序列之和。
        //
        // 参数:
        //   source:
        //     一个要计算和的 System.Single 值序列。
        //
        // 返回结果:
        //     序列值之和。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static float Sum(this Query<float> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算可以为 null 的 System.Int32 值序列之和。
        //
        // 参数:
        //   source:
        //     要计算和的可以为 null 的 System.Int32 值序列。
        //
        // 返回结果:
        //     序列值之和。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        //
        //   T:System.OverflowException:
        //     和大于 System.Int32.MaxValue。
        public static int? Sum(this Query<int?> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算 System.Int32 值序列之和。
        //
        // 参数:
        //   source:
        //     一个要计算和的 System.Int32 值序列。
        //
        // 返回结果:
        //     序列值之和。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        //
        //   T:System.OverflowException:
        //     和大于 System.Int32.MaxValue。
        public static int Sum(this Query<int> query)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算可以为 null 的 System.Int64 值序列之和，而该序列是通过对输入序列中的每个元素调用投影函数而获得的。
        //
        // 参数:
        //   source:
        //     一个 T 类型的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     投影值之和。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        //
        //   T:System.OverflowException:
        //     和大于 System.Int64.MaxValue。
        public static long? Sum<T>(this Query<T> query, Expression<Func<T, long?>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算 System.Single 值序列之和，而该序列是通过对输入序列中的每个元素调用投影函数而获得的。
        //
        // 参数:
        //   source:
        //     一个 T 类型的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     投影值之和。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        public static float Sum<T>(this Query<T> query, Expression<Func<T, float>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算可以为 null 的 System.Single 值序列之和，而该序列是通过对输入序列中的每个元素调用投影函数而获得的。
        //
        // 参数:
        //   source:
        //     一个 T 类型的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     投影值之和。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        public static float? Sum<T>(this Query<T> query, Expression<Func<T, float?>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算 System.Int64 值序列之和，而该序列是通过对输入序列中的每个元素调用投影函数而获得的。
        //
        // 参数:
        //   source:
        //     一个 T 类型的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     投影值之和。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        //
        //   T:System.OverflowException:
        //     和大于 System.Int64.MaxValue。
        public static long Sum<T>(this Query<T> query, Expression<Func<T, long>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算可以为 null 的 System.Int32 值序列之和，而该序列是通过对输入序列中的每个元素调用投影函数而获得的。
        //
        // 参数:
        //   source:
        //     一个 T 类型的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     投影值之和。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        //
        //   T:System.OverflowException:
        //     和大于 System.Int32.MaxValue。
        public static int? Sum<T>(this Query<T> query, Expression<Func<T, int?>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算 System.Double 值序列之和，而该序列是通过对输入序列中的每个元素调用投影函数而获得的。
        //
        // 参数:
        //   source:
        //     一个 T 类型的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     投影值之和。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        public static double Sum<T>(this Query<T> query, Expression<Func<T, double>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算可以为 null 的 System.Double 值序列之和，而该序列是通过对输入序列中的每个元素调用投影函数而获得的。
        //
        // 参数:
        //   source:
        //     一个 T 类型的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     投影值之和。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        public static double? Sum<T>(this Query<T> query, Expression<Func<T, double?>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算 System.Decimal 值序列之和，而该序列是通过对输入序列中的每个元素调用投影函数而获得的。
        //
        // 参数:
        //   source:
        //     一个 T 类型的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     投影值之和。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        //
        //   T:System.OverflowException:
        //     和大于 System.Decimal.MaxValue。
        public static decimal Sum<T>(this Query<T> query, Expression<Func<T, decimal>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算可以为 null 的 System.Decimal 值序列之和，而该序列是通过对输入序列中的每个元素调用投影函数而获得的。
        //
        // 参数:
        //   source:
        //     一个 T 类型的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     投影值之和。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        //
        //   T:System.OverflowException:
        //     和大于 System.Decimal.MaxValue。
        public static decimal? Sum<T>(this Query<T> query, Expression<Func<T, decimal?>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     计算 System.Int32 值序列之和，而该序列是通过对输入序列中的每个元素调用投影函数而获得的。
        //
        // 参数:
        //   source:
        //     一个 T 类型的值序列。
        //
        //   selector:
        //     要应用于每个元素的投影函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     投影值之和。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 selector 为 null。
        //
        //   T:System.OverflowException:
        //     和大于 System.Int32.MaxValue。
        public static int Sum<T>(this Query<T> query, Expression<Func<T, int>> selector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     从序列的开头返回指定数量的连续元素。
        //
        // 参数:
        //   source:
        //     要从其返回元素的序列。
        //
        //   count:
        //     要返回的元素数量。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，包含从 source 开始处的指定数量的元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 为 null。
        public static IList<T> Take<T>(this Query<T> query, int count)
        {
            if (query == null)
                throw new ArgumentNullException("query");
            return query.ExecuteReader(count);
        }
        //
        // 摘要:
        //     只要满足指定的条件，就会返回序列的元素。将在谓词函数的逻辑中使用元素的索引。
        //
        // 参数:
        //   source:
        //     要从其返回元素的序列。
        //
        //   predicate:
        //     用于测试每个元素是否满足条件的函数；此函数的第二个参数表示源序列中元素的索引。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，包含不再通过由 predicate 指定测试的元素之前出现的输入序列中的元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 predicate 为 null。
        public static Query<T> TakeWhile<T>(this Query<T> query, Expression<Func<T, int, bool>> predicate)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     只要满足指定的条件，就会返回序列的元素。
        //
        // 参数:
        //   source:
        //     要从其返回元素的序列。
        //
        //   predicate:
        //     用于测试每个元素是否满足条件的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，包含不再通过由 predicate 指定测试的元素之前出现的输入序列中的元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 predicate 为 null。
        public static Query<T> TakeWhile<T>(this Query<T> query, Expression<Func<T, bool>> predicate)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     根据某个键按升序对序列中的元素执行后续排序。
        //
        // 参数:
        //   source:
        //     一个 System.Linq.IOrderedQueryable`1，包含要排序的元素。
        //
        //   keySelector:
        //     用于从每个元素中提取键的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   TKey:
        //     由 keySelector 表示的函数返回的键类型。
        //
        // 返回结果:
        //     一个 System.Linq.IOrderedQueryable`1，根据键对其元素排序。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 keySelector 为 null。
        public static Query<T> ThenBy<T, TKey>(this Query<T> source, Expression<Func<T, TKey>> keySelector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     使用指定的比较器按升序对序列中的元素执行后续排序。
        //
        // 参数:
        //   source:
        //     一个 System.Linq.IOrderedQueryable`1，包含要排序的元素。
        //
        //   keySelector:
        //     用于从每个元素中提取键的函数。
        //
        //   comparer:
        //     一个用于比较键的 System.Collections.Generic.IComparer`1。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   TKey:
        //     由 keySelector 表示的函数返回的键类型。
        //
        // 返回结果:
        //     一个 System.Linq.IOrderedQueryable`1，根据键对其元素排序。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 keySelector 或 comparer 为 null。
        public static Query<T> ThenBy<T, TKey>(this Query<T> source, Expression<Func<T, TKey>> keySelector, IComparer<TKey> comparer)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     根据某个键按降序对序列中的元素执行后续排序。
        //
        // 参数:
        //   source:
        //     一个 System.Linq.IOrderedQueryable`1，包含要排序的元素。
        //
        //   keySelector:
        //     用于从每个元素中提取键的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   TKey:
        //     由 keySelector 表示的函数返回的键类型。
        //
        // 返回结果:
        //     一个 System.Linq.IOrderedQueryable`1，将根据键按降序对其元素进行排序。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 keySelector 为 null。
        public static Query<T> ThenByDescending<T, TKey>(this Query<T> source, Expression<Func<T, TKey>> keySelector)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     使用指定的比较器按降序对序列中的元素执行后续排序。
        //
        // 参数:
        //   source:
        //     一个 System.Linq.IOrderedQueryable`1，包含要排序的元素。
        //
        //   keySelector:
        //     用于从每个元素中提取键的函数。
        //
        //   comparer:
        //     一个用于比较键的 System.Collections.Generic.IComparer`1。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        //   TKey:
        //     由 keySelector 函数返回的键的类型。
        //
        // 返回结果:
        //     一个集合，其中的元素是根据某个键按降序进行排序的。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 keySelector 或 comparer 为 null。
        public static Query<T> ThenByDescending<T, TKey>(this Query<T> source, Expression<Func<T, TKey>> keySelector, IComparer<TKey> comparer)
        {
            throw new NotSupportedException();
        }
        public static IList<T> ToList<T>(this Query<T> query)
        {
            if (query == null)
                throw new ArgumentNullException("query");
            return query.ExecuteReader(0);
        }
        public static IList<T> ToPage<T>(this Query<T> query, long index, int size, out long count)
        {
            if (query == null)
                throw new ArgumentNullException("query");
            return query.ExecutePage(index, size, out count);
        }
        //
        // 摘要:
        //     通过使用默认的相等比较器生成两个序列的并集。
        //
        // 参数:
        //   source1:
        //     非重复元素组成合并运算的第一组的一个序列。
        //
        //   source2:
        //     非重复元素组成合并运算的第二组的一个序列。
        //
        // 类型参数:
        //   T:
        //     输入序列中的元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，包含两个输入序列中的元素（重复元素除外）。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source1 或 source2 为 null。
        public static Query<T> Union<T>(this Query<T> query1, IEnumerable<T> source2)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     通过使用指定的 System.Collections.Generic.IEqualityComparer`1 生成两个序列的并集。
        //
        // 参数:
        //   source1:
        //     非重复元素组成合并运算的第一组的一个序列。
        //
        //   source2:
        //     非重复元素组成合并运算的第二组的一个序列。
        //
        //   comparer:
        //     用于比较值的 System.Collections.Generic.IEqualityComparer`1。
        //
        // 类型参数:
        //   T:
        //     输入序列中的元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，包含两个输入序列中的元素（重复元素除外）。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source1 或 source2 为 null。
        public static Query<T> Union<T>(this Query<T> query1, IEnumerable<T> source2, IEqualityComparer<T> comparer)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     基于谓词筛选值序列。将在谓词函数的逻辑中使用每个元素的索引。
        //
        // 参数:
        //   source:
        //     要筛选的 System.Linq.IQueryable`1。
        //
        //   predicate:
        //     用于测试每个元素是否满足条件的函数；此函数的第二个参数表示源序列中元素的索引。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，包含满足由 predicate 指定的条件的输入序列中的元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 predicate 为 null。
        public static Query<T> Where<T>(this Query<T> query, Expression<Func<T, int, bool>> predicate)
        {
            throw new NotSupportedException();
        }
        //
        // 摘要:
        //     基于谓词筛选值序列。
        //
        // 参数:
        //   source:
        //     要筛选的 System.Linq.IQueryable`1。
        //
        //   predicate:
        //     用于测试每个元素是否满足条件的函数。
        //
        // 类型参数:
        //   T:
        //     source 中的元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，包含满足由 predicate 指定的条件的输入序列中的元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source 或 predicate 为 null。
        public static Query<T> Where<T>(this Query<T> query, Expression<Func<T, bool>> predicate)
        {
            return query.WhereImpl(predicate);
        }
        //
        // 摘要:
        //     通过使用指定的谓词函数合并两个序列。
        //
        // 参数:
        //   source1:
        //     要合并的第一个序列。
        //
        //   source2:
        //     要合并的第二个序列。
        //
        //   resultSelector:
        //     用于指定如何合并这两个序列的元素的函数。
        //
        // 类型参数:
        //   TFirst:
        //     第一个输入序列中的元素的类型。
        //
        //   TSecond:
        //     第二个输入序列中的元素的类型。
        //
        //   R:
        //     结果序列的元素的类型。
        //
        // 返回结果:
        //     一个 System.Linq.IQueryable`1，包含两个输入序列的已合并元素。
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     source1 或 source2  为 null。
        public static Query<R> Zip<TFirst, TSecond, R>(this Query<TFirst> source1, IEnumerable<TSecond> source2, Expression<Func<TFirst, TSecond, R>> resultSelector)
        {
            throw new NotSupportedException();
        }
    }
}
