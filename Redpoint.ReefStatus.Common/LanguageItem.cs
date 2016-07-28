// <copyright file="LanguageItem.cs" company="RedPoint Games">
// Copyright (c) RedPoint Games. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common
{
    /// <summary>
    /// Language Item
    /// </summary>
    public class LanguageItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageItem"/> class.
        /// </summary>
        public LanguageItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageItem"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public LanguageItem(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; set; }
    }
}
