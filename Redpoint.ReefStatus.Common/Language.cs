// <copyright file="Language.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common
{
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Markup;

    /// <summary>
    /// Language Settings
    /// </summary>
    public static class Language
    {
        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>the string in the given language</returns>
        public static string GetResource(string key)
        {
            if (Application.Current == null)
            {
                return key;
            }

            try
            {
                return Application.Current.TryFindResource(key) as string;
            }
            catch (XamlParseException)
            {
                return key;
            }
            
        }

        /// <summary>
        /// Gets the name of the frendy.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>the resource string</returns>
        public static string GetFrendyName(object value)
        {
            if (value != null)
            {
                FieldInfo fi = value.GetType().GetField(value.ToString());
                if (fi != null)
                {
                    DescriptionAttribute[] attributes =
                        (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (attributes.Length > 0)
                    {
                        try
                        {
                            if (Application.Current != null)
                            {
                                return Application.Current.TryFindResource(attributes[0].Description) as string;
                            }
                        }
                        catch (XamlParseException)
                        {
                        }
                    }
                }

                return value.ToString();
            }

            return string.Empty;
        }
    }
}
