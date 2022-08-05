using System;

namespace MEI.SPDocuments
{
    /// <summary>
    ///     Extension methods on the Enum type.
    /// </summary>
    internal static class EnumerationExtensions
    {
        /// <summary>
        ///     Determines whether the <paramref name="type" /> has the specified <paramref name="value" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The Enum type.</param>
        /// <param name="value">The enum member.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="type" /> has the specified <paramref name="value" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool Has<T>(this Enum type, T value)
        {
            try
            {
                return (Convert.ToInt32(type) & Convert.ToInt32(value)) == Convert.ToInt32(value);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Determines if the <paramref name="value" /> is only the provided <paramref name="type" />.
        /// </summary>
        /// <typeparam name="T">The Enum type.</typeparam>
        /// <param name="type">The Enum.</param>
        /// <param name="value">The enum member.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="value" /> is only the provided <paramref name="type" />; otherwise, <c>false</c>
        ///     .
        /// </returns>
        /// <remarks>Used only on Enums which are bit flags.</remarks>
        public static bool Is<T>(this Enum type, T value)
        {
            try
            {
                return Convert.ToInt32(type) == Convert.ToInt32(value);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Adds the specified <paramref name="value" /> to then <paramref name="type" />.
        /// </summary>
        /// <typeparam name="T">The Enum type</typeparam>
        /// <param name="type">The enum.</param>
        /// <param name="value">The enum member.</param>
        /// <returns>The enum after the add.</returns>
        /// <remarks>Used only on Enums which are bit flags.</remarks>
        public static T Add<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)(Convert.ToInt32(type) | Convert.ToInt32(value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format(Resources.Default.Could_not_append_value_from_enumerated_type___0, typeof(T).Name), ex);
            }
        }

        /// <summary>
        ///     Removes the specified <paramref name="value" /> from the <paramref name="type" />.
        /// </summary>
        /// <typeparam name="T">The Enum type.</typeparam>
        /// <param name="type">The Enum.</param>
        /// <param name="value">The enum member.</param>
        /// <returns>The enum after the remove.</returns>
        /// <remarks>Used only on Enums which are bit flags.</remarks>
        public static T Remove<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)(Convert.ToInt32(type) & ~Convert.ToInt32(value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format(Resources.Default.Could_not_remove_value_from_enumerated_type___0, typeof(T).Name), ex);
            }
        }
    }
}
