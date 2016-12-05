﻿/*******************************************************************************
* 命名空间: SF.Core.QueryExtensions.Extensions
*
* 功 能： N/A
* 类 名： PredicateTranslation
*
* Ver 变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2016/11/19 14:45:03 疯狂蚂蚁 初版
*
* Copyright (c) 2016 SF 版权所有
* Description: SF快速开发平台
* Website：http://www.mayisite.com
*********************************************************************************/
using System;
using System.Linq.Expressions;

namespace SF.Core.QueryExtensions.Extensions
{
    /// <summary>
    /// Represents a translation of a given predicate.
    /// </summary>
    /// <typeparam name="T">The type of the predicate's parameter.</typeparam>
    /// <remarks>Use <see cref="PredicateTranslator" /> to start a translation.</remarks>
    public class PredicateTranslation<T>
    {
        readonly Expression<Func<T, bool>> predicate;

        /// <summary>
        /// Creates a new predicate translation.
        /// </summary>
        /// <param name="predicate">The predicate to translate.</param>
        public PredicateTranslation(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            this.predicate = predicate;
        }

        /// <summary>
        /// Translates a given predicate for a given subtype.
        /// </summary>
        /// <typeparam name="U">The type of the translated predicate's parameter.</typeparam>
        /// <returns>A translated predicate expression.</returns>
        public Expression<Func<U, bool>> To<U>()
            where U : T
        {
            var t = predicate.Parameters[0];
            var u = Expression.Parameter(typeof(U), t.Name);

            var binder = new ParameterBinder(t, u);

            return Expression.Lambda<Func<U, bool>>(
                binder.Visit(predicate.Body), u);
        }

        /// <summary>
        /// Translates a given predicate for a given related type.
        /// </summary>
        /// <typeparam name="U">The type of the translated predicate's parameter.</typeparam>
        /// <param name="path">The path from the desired type to the given type.</param>
        /// <returns>A translated predicate expression.</returns>
        public Expression<Func<U, bool>> To<U>(Expression<Func<U, T>> path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var t = predicate.Parameters[0];
            var u = path.Parameters[0];

            var binder = new ParameterBinder(t, path.Body);

            return Expression.Lambda<Func<U, bool>>(
                binder.Visit(predicate.Body), u);
        }

        /// <summary>
        /// Translates a given predicate for a given related type.
        /// </summary>
        /// <typeparam name="U">The type of the translated predicate's parameter.</typeparam>
        /// <param name="translation">The translation from the desired type to the given type,
        /// using the initially given predicate to be injected into a new predicate.</param>
        /// <returns>A translated predicate expression.</returns>
        public Expression<Func<U, bool>> To<U>(Expression<Func<U, Func<T, bool>, bool>> translation)
        {
            if (translation == null)
                throw new ArgumentNullException(nameof(translation));

            var u = translation.Parameters[0];
            var p = translation.Parameters[1];

            var binder = new ParameterBinder(p, predicate);

            return Expression.Lambda<Func<U, bool>>(
                binder.Visit(translation.Body), u);
        }
    }
}