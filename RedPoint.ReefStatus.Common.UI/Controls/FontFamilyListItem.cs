namespace RedPoint.ReefStatus.Common.UI.Controls
{
    using System;
    using System.Globalization;
    using System.Windows.Media;

    public class FontFamilyListItem : IComparable
    {
        public string DisplayName { get; private set;}

        public FontFamily FontFamily { get; private set; } 

        /// <summary>
        /// Initializes a new instance of the <see cref="FontFamilyListItem"/> class.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        public FontFamilyListItem(FontFamily fontFamily)
        {
            this.DisplayName = GetDisplayName(fontFamily);
            this.FontFamily = fontFamily;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.DisplayName;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj"/>. Zero This instance is equal to <paramref name="obj"/>. Greater than zero This instance is greater than <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="obj"/> is not the same type as this instance. </exception>
        int IComparable.CompareTo(object obj)
        {
            return string.Compare(this.DisplayName, obj.ToString(), true, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Determines whether [is symbol font] [the specified font family].
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <returns>
        ///     <c>true</c> if [is symbol font] [the specified font family]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsSymbolFont(FontFamily fontFamily)
        {
            foreach (var typeface in fontFamily.GetTypefaces())
            {
                GlyphTypeface face;
                if (typeface.TryGetGlyphTypeface(out face))
                {
                    return face.Symbol;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <param name="family">The family.</param>
        /// <returns></returns>
        public static string GetDisplayName(FontFamily family)
        {
            return NameDictionaryHelper.GetDisplayName(family.FamilyNames);
        }
    }
}
