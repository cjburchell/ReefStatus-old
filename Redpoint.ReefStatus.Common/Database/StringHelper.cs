namespace MyCompany.Extensions
{
    using System;
    using System.Linq;

    /// <summary>
    /// String Extensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Reverses the specified string.
        /// </summary>
        /// <param name="str">The string to reverse.</param>
        /// <returns>
        /// the reversed string
        /// </returns>
        /// <exception cref="ArgumentNullException">if the string is null</exception>
        /// <example>
        /// var str = "HELLO";
        /// str.Reverse();
        /// </example>
        public static string Reverse(this string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            return string.IsNullOrEmpty(str) ? string.Empty : new string(str.ToCharArray().Reverse().ToArray());
        }
    }
}