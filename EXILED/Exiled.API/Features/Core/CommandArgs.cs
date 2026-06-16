// -----------------------------------------------------------------------
// <copyright file="CommandArgs.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace Exiled.API.Features.Core
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A helper class to easily access command arguments.
    /// </summary>
    public class CommandArgs
    {
        private readonly string[] args;
        private readonly CultureInfo cultureInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandArgs"/> class.
        /// </summary>
        /// <param name="args">Parsed command arguments.</param>
        /// <param name="cultureInfo">Current culture info. Used for float conversions.</param>
        internal CommandArgs(string[] args, CultureInfo cultureInfo)
        {
            this.args = args;
            this.cultureInfo = cultureInfo;
        }

        /// <summary>
        /// Parses an instance of <see cref="CommandArgs"/> from an array of arguments.
        /// </summary>
        /// <param name="arguments">Array of arguments.</param>
        /// <param name="useRegex">Whether the regex should be used.</param>
        /// <param name="cultureInfo"><see cref="CultureInfo"/> to be used with float-pointing number conversions.</param>
        /// <returns>An instance of <see cref="CommandArgs"/>.</returns>
        /// <remarks>
        /// Usage of <paramref name="useRegex"/> allows to separate arguments not by space char, but also with quotes.
        /// For example, command with structure
        /// <code>commandName one "two three" four</code>
        /// will be separated into
        /// <code>{ "one", "two three", "four" }</code>
        /// </remarks>
        public static CommandArgs Parse(ArraySegment<string> arguments, bool useRegex = false, CultureInfo cultureInfo = null)
        {
            if (!useRegex)
                return new(arguments.Array, cultureInfo ?? CultureInfo.CurrentCulture);

            string[] result = Regex.Matches(string.Join(" ", arguments), @"""([^""]*)""|(\S+)")
                .Select(m => m.Groups[1].Success ? m.Groups[1].Value : m.Value)
                .ToArray();

            return new(result, cultureInfo ?? CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Gets a value as an <see cref="int"/> from specified position.
        /// </summary>
        /// <param name="index">Index of an argument.</param>
        /// <returns>Returned value or <c>null</c>.</returns>
        public int GetInt(uint index) => GetValue<int>(index);

        /// <summary>
        /// Gets a value as a <see cref="float"/> from specified position.
        /// </summary>
        /// <param name="index">Index of an argument.</param>
        /// <returns>Returned value or <c>null</c>.</returns>
        public float GetFloat(uint index) => GetValue<float>(index);

        /// <summary>
        /// Gets a value as a <see cref="Player"/> from specified position.
        /// </summary>
        /// <param name="index">Index of an argument.</param>
        /// <returns>Returned value or <c>null</c>.</returns>
        public Player GetPlayer(uint index) => Player.Get(GetString(index));

        /// <summary>
        /// Gets a value as a <see cref="string"/> from specified position.
        /// </summary>
        /// <param name="index">Index of an argument.</param>
        /// <returns>Returned value or <c>null</c>.</returns>
        public string GetString(uint index)
        {
            if (index >= args.Length)
                return null;

            return args[index];
        }

        /// <summary>
        /// Tries to get a value as an <see cref="int"/> from specified position.
        /// </summary>
        /// <param name="index">Index of an argument.</param>
        /// <param name="result">Returned value. May be <c>null</c>.</param>
        /// <returns><c>true</c> if succeeded. Otherwise, <c>false</c>.</returns>
        public bool TryGetInt(uint index, out int result) => TryGetValue(index, out result);

        /// <summary>
        /// Tries to get a value as an <see cref="float"/> from specified position.
        /// </summary>
        /// <param name="index">Index of an argument.</param>
        /// <param name="result">Returned value. May be <c>null</c>.</param>
        /// <returns><c>true</c> if succeeded. Otherwise, <c>false</c>.</returns>
        public bool GetFloat(uint index, out float result) => TryGetValue(index, out result);

        /// <summary>
        /// Tries to get a value as an <see cref="Player"/> from specified position.
        /// </summary>
        /// <param name="index">Index of an argument.</param>
        /// <param name="result">Returned value. May be <c>null</c>.</param>
        /// <returns><c>true</c> if succeeded. Otherwise, <c>false</c>.</returns>
        public bool TryGetPlayer(uint index, out Player result) => (result = GetPlayer(index)) != null;

        /// <summary>
        /// Tries to get a value as an <see cref="string"/> from specified position.
        /// </summary>
        /// <param name="index">Index of an argument.</param>
        /// <param name="result">Returned value. May be <c>null</c>.</param>
        /// <returns><c>true</c> if succeeded. Otherwise, <c>false</c>.</returns>
        public bool TryGetString(uint index, out string result) => (result = GetString(index)) != null;

        /// <summary>
        /// Gets a value as a generic type.
        /// </summary>
        /// <param name="index">Index of a value.</param>
        /// <typeparam name="T">Type of object to be casted to.</typeparam>
        /// <returns>Casted value or <c>null</c>.</returns>
        public T GetValue<T>(uint index)
        {
            if (index >= args.Length)
                return default(T);

            try
            {
                return (T)Convert.ChangeType(args[index], typeof(T), cultureInfo);
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Gets a value as a generic type.
        /// </summary>
        /// <param name="index">Index of a value.</param>
        /// <param name="result">Casted value.</param>
        /// <typeparam name="T">Type of object to be casted to.</typeparam>
        /// <returns><c>true</c> if value of specific type was successfully retrieved. Otherwise, <c>false</c>.</returns>
        public bool TryGetValue<T>(uint index, out T result)
        {
            result = default(T);

            if (index >= args.Length)
                return false;

            object value;

            try
            {
                value = Convert.ChangeType(args[index], typeof(T), cultureInfo);
            }
            catch (InvalidCastException)
            {
                return false;
            }

            if (value == null)
                return false;

            result = (T)value;
            return true;
        }
    }
}