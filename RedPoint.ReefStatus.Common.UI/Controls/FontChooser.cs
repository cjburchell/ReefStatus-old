namespace RedPoint.ReefStatus.Common.UI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Threading;


    /// <summary>
    /// The font chooser.
    /// </summary>
    [TemplatePart(Name = "PART_fontFamily", Type = typeof(Selector))]
    [TemplatePart(Name = "PART_typeface", Type = typeof(Selector))]
    [TemplatePart(Name = "PART_size", Type = typeof(ComboBox))]
    [TemplatePart(Name = "PART_underline", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_baseline", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_strikethrough", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_overline", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_italic", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_bold", Type = typeof(ToggleButton))]
    public class FontChooser : UserControl
    {
        #region Constants and Fields

        /// <summary>
        /// The selected font family property.
        /// </summary>
        public static readonly DependencyProperty SelectedFontFamilyProperty = RegisterFontProperty(
            "SelectedFontFamily", TextBlock.FontFamilyProperty, SelectedFontFamilyChangedCallback);

        /// <summary>
        /// The selected font size property.
        /// </summary>
        public static readonly DependencyProperty SelectedFontSizeProperty = RegisterFontProperty(
            "SelectedFontSize", TextBlock.FontSizeProperty, SelectedFontSizeChangedCallback);

        /// <summary>
        /// The selected font stretch property.
        /// </summary>
        public static readonly DependencyProperty SelectedFontStretchProperty =
            RegisterFontProperty("SelectedFontStretch", TextBlock.FontStretchProperty, SelectedTypefaceChangedCallback);

        /// <summary>
        /// The selected font style property.
        /// </summary>
        public static readonly DependencyProperty SelectedFontStyleProperty = RegisterFontProperty(
            "SelectedFontStyle", TextBlock.FontStyleProperty, SelectedTypefaceChangedCallback);

        /// <summary>
        /// The selected font weight property.
        /// </summary>
        public static readonly DependencyProperty SelectedFontWeightProperty = RegisterFontProperty(
            "SelectedFontWeight", TextBlock.FontWeightProperty, SelectedTypefaceChangedCallback);

        /// <summary>
        /// The selected text decorations property.
        /// </summary>
        public static readonly DependencyProperty SelectedTextDecorationsProperty =
            RegisterFontProperty(
                "SelectedTextDecorations", TextBlock.TextDecorationsProperty, SelectedTextDecorationsChangedCallback);

        /// <summary>
        /// The commonly used font sizes.
        /// </summary>
        private static readonly double[] CommonlyUsedFontSizes = new[]
            {
                3.0, 4.0, 5.0, 6.0, 6.5, 7.0, 7.5, 8.0, 8.5, 9.0, 9.5, 10.0, 10.5, 11.0, 11.5, 12.0, 12.5, 13.0, 13.5, 
                14.0, 15.0, 16.0, 17.0, 18.0, 19.0, 20.0, 22.0, 24.0, 26.0, 28.0, 30.0, 32.0, 34.0, 36.0, 38.0, 40.0, 44.0, 48.0, 52.0, 56.0, 60.0, 64.0, 68.0, 72.0, 76.0, 80.0, 88.0, 96.0, 104.0, 112.0, 120.0, 128.0, 136.0, 
                144.0
            };

        /// <summary>
        /// The baseline check box.
        /// </summary>
        private ToggleButton baselineCheckBox;

        /// <summary>
        /// The font family list.
        /// </summary>
        private Selector fontFamilyList;

        /// <summary>
        /// The overline check box.
        /// </summary>
        private ToggleButton overlineCheckBox;

        /// <summary>
        /// The size list.
        /// </summary>
        private ComboBox sizeList;

        /// <summary>
        /// The strikethrough check box.
        /// </summary>
        private ToggleButton strikethroughCheckBox;

        /// <summary>
        /// The typeface list.
        /// </summary>
        private Selector typefaceList;

        /// <summary>
        /// The typeface list selection valid.
        /// </summary>
        private bool typefaceListSelectionValid; // indicates the current selection in the typeface list is valid

        /// <summary>
        /// The typeface list valid.
        /// </summary>
        private bool typefaceListValid; // indicates the list of typefaces is valid

        /// <summary>
        /// The underline check box.
        /// </summary>
        private ToggleButton underlineCheckBox;

        /// <summary>
        /// The update pending.
        /// </summary>
        private bool updatePending; // indicates a call to OnUpdate is scheduled

        private ToggleButton boldCheckBox;

        private ToggleButton italicCheckBox;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets SelectedFontFamily.
        /// </summary>
        public FontFamily SelectedFontFamily
        {
            get
            {
                return this.GetValue(SelectedFontFamilyProperty) as FontFamily;
            }

            set
            {
                this.SetValue(SelectedFontFamilyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets SelectedFontSize.
        /// </summary>
        public double SelectedFontSize
        {
            get
            {
                return (double)this.GetValue(SelectedFontSizeProperty);
            }

            set
            {
                this.SetValue(SelectedFontSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets SelectedFontStretch.
        /// </summary>
        public FontStretch SelectedFontStretch
        {
            get
            {
                return (FontStretch)this.GetValue(SelectedFontStretchProperty);
            }

            set
            {
                this.SetValue(SelectedFontStretchProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets SelectedFontStyle.
        /// </summary>
        public FontStyle SelectedFontStyle
        {
            get
            {
                return (FontStyle)this.GetValue(SelectedFontStyleProperty);
            }

            set
            {
                this.SetValue(SelectedFontStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets SelectedFontWeight.
        /// </summary>
        public FontWeight SelectedFontWeight
        {
            get
            {
                return (FontWeight)this.GetValue(SelectedFontWeightProperty);
            }

            set
            {
                this.SetValue(SelectedFontWeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets SelectedTextDecorations.
        /// </summary>
        public TextDecorationCollection SelectedTextDecorations
        {
            get
            {
                return this.GetValue(SelectedTextDecorationsProperty) as TextDecorationCollection;
            }

            set
            {
                this.SetValue(SelectedTextDecorationsProperty, value);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The on apply template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.fontFamilyList = this.GetTemplateChild("PART_fontFamily") as Selector;
            this.sizeList = this.GetTemplateChild("PART_size") as ComboBox;
            this.typefaceList = this.GetTemplateChild("PART_typeface") as Selector;

            this.underlineCheckBox = this.GetTemplateChild("PART_underline") as ToggleButton;
            this.baselineCheckBox = this.GetTemplateChild("PART_baseline") as ToggleButton;
            this.strikethroughCheckBox = this.GetTemplateChild("PART_strikethrough") as ToggleButton;
            this.overlineCheckBox = this.GetTemplateChild("PART_overline") as ToggleButton;

            this.boldCheckBox = this.GetTemplateChild("PART_bold") as ToggleButton;
            this.italicCheckBox = this.GetTemplateChild("PART_italic") as ToggleButton;

            // Hook up events for the font family list and associated text box.
            if (this.fontFamilyList != null)
            {
                this.fontFamilyList.SelectionChanged += this.FontFamilyListSelectionChanged;
            }

            // Hook up events for the typeface list.
            if (this.typefaceList != null)
            {
                this.typefaceList.SelectionChanged += this.TypefaceListSelectionChanged;
            }

            if (this.boldCheckBox != null)
            {
                this.boldCheckBox.Checked += this.TypefaceListSelectionChanged;
                this.boldCheckBox.Unchecked += this.TypefaceListSelectionChanged;
            }

            if (this.italicCheckBox != null)
            {
                this.italicCheckBox.Checked += this.TypefaceListSelectionChanged;
                this.italicCheckBox.Unchecked += this.TypefaceListSelectionChanged;
            }

            // Hook up events for the font size list and associated text box.
            if (this.sizeList != null)
            {
                this.sizeList.SelectionChanged += this.SizeListSelectionChanged;
                this.sizeList.LostFocus += this.SizeListLostFocus;

                // Initialize the list of font sizes and select the nearest size.
                foreach (double value in CommonlyUsedFontSizes)
                {
                    this.sizeList.Items.Add(new FontSizeListItem(value));
                }
            }

            // Hook up events for text decoration check boxes.
            RoutedEventHandler textDecorationEventHandler = this.TextDecorationCheckStateChanged;
            if (this.underlineCheckBox != null)
            {
                this.underlineCheckBox.Checked += textDecorationEventHandler;
                this.underlineCheckBox.Unchecked += textDecorationEventHandler;
            }

            if (this.baselineCheckBox != null)
            {
                this.baselineCheckBox.Checked += textDecorationEventHandler;
                this.baselineCheckBox.Unchecked += textDecorationEventHandler;
            }

            if (this.strikethroughCheckBox != null)
            {
                this.strikethroughCheckBox.Checked += textDecorationEventHandler;
                this.strikethroughCheckBox.Unchecked += textDecorationEventHandler;
            }

            if (this.overlineCheckBox != null)
            {
                this.overlineCheckBox.Checked += textDecorationEventHandler;
                this.overlineCheckBox.Unchecked += textDecorationEventHandler;
            }

            this.OnSelectedFontSizeChanged(this.SelectedFontSize);

            if (this.fontFamilyList != null)
            {
                ICollection<FontFamily> familyCollection = Fonts.SystemFontFamilies;
                if (familyCollection != null)
                {
                    var items = new FontFamilyListItem[familyCollection.Count];

                    int i = 0;

                    foreach (FontFamily family in familyCollection)
                    {
                        try
                        {
                            items[i++] = new FontFamilyListItem(family);
                        }
                        catch (Exception)
                        {
                            // in case the font is "invalid".
                            continue;
                        }
                    }

                    Array.Sort(items);

                    foreach (FontFamilyListItem item in items)
                    {
                        this.fontFamilyList.Items.Add(item);
                    }
                }
            }

            this.OnSelectedFontFamilyChanged(this.SelectedFontFamily);

            // Schedule background updates.
            this.ScheduleUpdate();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The register font property.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="targetProperty">
        /// The target property.
        /// </param>
        /// <param name="changeCallback">
        /// The change callback.
        /// </param>
        /// <returns>
        /// </returns>
        private static DependencyProperty RegisterFontProperty(
            string propertyName, DependencyProperty targetProperty, PropertyChangedCallback changeCallback)
        {
            return DependencyProperty.Register(
                propertyName,
                targetProperty.PropertyType,
                typeof(FontChooser),
                new FrameworkPropertyMetadata(targetProperty.DefaultMetadata.DefaultValue, changeCallback));
        }

        /// <summary>
        /// The selected font family changed callback.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void SelectedFontFamilyChangedCallback(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((FontChooser)obj).OnSelectedFontFamilyChanged(e.NewValue as FontFamily);
        }

        /// <summary>
        /// The selected font size changed callback.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void SelectedFontSizeChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((FontChooser)obj).OnSelectedFontSizeChanged((double)e.NewValue);
        }

        /// <summary>
        /// The selected text decorations changed callback.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void SelectedTextDecorationsChangedCallback(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var chooser = (FontChooser)obj;
            chooser.OnTextDecorationsChanged();
        }

        /// <summary>
        /// The selected typeface changed callback.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void SelectedTypefaceChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((FontChooser)obj).InvalidateTypefaceListSelection();
        }

        /// <summary>
        /// The initialize typeface list.
        /// </summary>
        private void InitializeTypefaceList()
        {
            FontFamily family = this.SelectedFontFamily;
            if (family != null)
            {
                if (this.typefaceList != null)
                {
                    ICollection<Typeface> faceCollection = family.GetTypefaces();

                    var items = new TypefaceListItem[faceCollection.Count];

                    int i = 0;

                    foreach (Typeface face in faceCollection)
                    {
                        items[i++] = new TypefaceListItem(face);
                    }

                    Array.Sort(items);

                    foreach (TypefaceListItem item in items)
                    {
                        this.typefaceList.Items.Add(item);
                    }
                }
            }
        }


        /// <summary>
        /// The initialize typeface list selection.
        /// </summary>
        private void InitializeTypefaceListSelection()
        {
            // If the typeface list is not valid, do nothing for now.
            // We'll be called again after the list is initialized.
            if (this.typefaceListValid)
            {
                var typeface = new Typeface(
                    this.SelectedFontFamily, this.SelectedFontStyle, this.SelectedFontWeight, this.SelectedFontStretch);

                // Select the typeface in the list.
                this.SelectTypefaceListItem(typeface);
            }
        }

        /// <summary>
        /// The invalidate typeface list.
        /// </summary>
        private void InvalidateTypefaceList()
        {
            if (this.typefaceListValid)
            {
                if (this.typefaceList != null)
                {
                    this.typefaceList.Items.Clear();
                }

                this.typefaceListValid = false;

                this.ScheduleUpdate();
            }
        }

        // Schedule background selection of the current typeface list item.
        /// <summary>
        /// The invalidate typeface list selection.
        /// </summary>
        private void InvalidateTypefaceListSelection()
        {
            if (this.typefaceListSelectionValid)
            {
                this.typefaceListSelectionValid = false;
                this.ScheduleUpdate();
            }
        }

        // Handle changes to the SelectedFontFamily property
        /// <summary>
        /// The on selected font family changed.
        /// </summary>
        /// <param name="family">
        /// The family.
        /// </param>
        private void OnSelectedFontFamilyChanged(FontFamily family)
        {
            // Select the family in the list; this will return null if the family is not in the list.
            SelectFontFamilyListItem(family);

            // The typeface list is no longer valid; update it in the background to improve responsiveness.
            this.InvalidateTypefaceList();
        }

        // Handle changes to the SelectedFontSize property
        /// <summary>
        /// The on selected font size changed.
        /// </summary>
        /// <param name="sizeInPixels">
        /// The size in pixels.
        /// </param>
        private void OnSelectedFontSizeChanged(double sizeInPixels)
        {
            if (this.sizeList != null)
            {
                // Select the list item, if the size is in the list.
                double sizeInPoints = FontSizeListItem.PixelsToPoints(sizeInPixels);
                if (!this.SelectListItem(this.sizeList, sizeInPoints))
                {
                    this.sizeList.Text = sizeInPoints.ToString();
                }
            }
        }

        // Handle changes to any of the text decoration properties.
        /// <summary>
        /// The on text decorations changed.
        /// </summary>
        private void OnTextDecorationsChanged()
        {
            bool underline = false;
            bool baseline = false;
            bool strikethrough = false;
            bool overline = false;

            TextDecorationCollection textDecorations = this.SelectedTextDecorations;
            if (textDecorations != null)
            {
                foreach (TextDecoration td in textDecorations)
                {
                    switch (td.Location)
                    {
                        case TextDecorationLocation.Underline:
                            underline = true;
                            break;
                        case TextDecorationLocation.Baseline:
                            baseline = true;
                            break;
                        case TextDecorationLocation.Strikethrough:
                            strikethrough = true;
                            break;
                        case TextDecorationLocation.OverLine:
                            overline = true;
                            break;
                    }
                }
            }

            if (this.underlineCheckBox != null)
            {
                this.underlineCheckBox.IsChecked = underline;
            }

            if (this.baselineCheckBox != null)
            {
                this.baselineCheckBox.IsChecked = baseline;
            }

            if (this.strikethroughCheckBox != null)
            {
                this.strikethroughCheckBox.IsChecked = strikethrough;
            }

            if (this.overlineCheckBox != null)
            {
                this.overlineCheckBox.IsChecked = overline;
            }
        }

        // Schedule background initialization of the typeface list.

        // Dispatcher callback that performs background initialization.
        /// <summary>
        /// The on update.
        /// </summary>
        private void OnUpdate()
        {
            this.updatePending = false;

            if (!this.typefaceListValid)
            {
                // Initialize the typeface list.
                this.InitializeTypefaceList();
                this.typefaceListValid = true;

                // Select the current typeface in the list.
                this.InitializeTypefaceListSelection();
                this.typefaceListSelectionValid = true;

                // Defer any other initialization until later.
                this.ScheduleUpdate();
            }
            else if (!this.typefaceListSelectionValid)
            {
                // Select the current typeface in the list.
                this.InitializeTypefaceListSelection();
                this.typefaceListSelectionValid = true;

                // Defer any other initialization until later.
                this.ScheduleUpdate();
            }
        }

        /// <summary>
        /// The schedule update.
        /// </summary>
        private void ScheduleUpdate()
        {
            if (!this.updatePending)
            {
                this.updatePending = true;
                this.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(this.OnUpdate));
            }
        }

        // Update font family list based on selection.
        // Return list item if there's an exact match, or null if not.
        /// <summary>
        /// The select font family list item.
        /// </summary>
        /// <param name="displayName">
        /// The display name.
        /// </param>
        /// <returns>
        /// </returns>
        private FontFamilyListItem SelectFontFamilyListItem(string displayName)
        {
            if (this.fontFamilyList != null)
            {
                var listItem = this.fontFamilyList.SelectedItem as FontFamilyListItem;
                if (listItem != null &&
                    string.Compare(listItem.ToString(), displayName, true, CultureInfo.CurrentCulture) == 0)
                {
                    // Already selected
                    return listItem;
                }

                if (this.SelectListItem(this.fontFamilyList, displayName))
                {
                    // Exact match found
                    return this.fontFamilyList.SelectedItem as FontFamilyListItem;
                }
            }

            // Not in the list
            return null;
        }

        // Update font family list based on selection.
        // Return list item if there's an exact match, or null if not.
        /// <summary>
        /// The select font family list item.
        /// </summary>
        /// <param name="family">
        /// The family.
        /// </param>
        private void SelectFontFamilyListItem(FontFamily family)
        {
            if (this.fontFamilyList != null)
            {
                var listItem = this.fontFamilyList.SelectedItem as FontFamilyListItem;

                if (listItem != null && listItem.FontFamily.Equals(family))
                {
                    // Already selected
                    return;
                }

                this.SelectListItem(this.fontFamilyList, FontFamilyListItem.GetDisplayName(family));
            }
        }

        // Update typeface list based on selection.
        // Return list item if there's an exact match, or null if not.

        // Update list based on selection.
        // Return true if there's an exact match, or false if not.
        /// <summary>
        /// The select list item.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The select list item.
        /// </returns>
        private bool SelectListItem(Selector list, object value)
        {
            if (list == null)
            {
                return false;
            }

            ItemCollection itemList = list.Items;

            // Perform a binary search for the item.
            int first = 0;
            int limit = itemList.Count;

            while (first < limit)
            {
                int i = first + (limit - first) / 2;
                var item = (IComparable)itemList[i];
                int comparison = item.CompareTo(value);
                if (comparison < 0)
                {
                    // Value must be after i
                    first = i + 1;
                }
                else if (comparison > 0)
                {
                    // Value must be before i
                    limit = i;
                }
                else
                {
                    // Exact match; select the item.
                    list.SelectedIndex = i;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The select typeface list item.
        /// </summary>
        /// <param name="typeface">
        /// The typeface.
        /// </param>
        /// <returns>
        /// </returns>
        private void SelectTypefaceListItem(Typeface typeface)
        {
            if (this.typefaceList != null)
            {
                var listItem = this.typefaceList.SelectedItem as TypefaceListItem;
                if (listItem != null && listItem.Typeface.Equals(typeface))
                {
                    // Already selected
                    return;
                }

                this.SelectListItem(this.typefaceList, new TypefaceListItem(typeface));
            }

            if (this.boldCheckBox != null)
            {
                this.boldCheckBox.IsChecked = typeface.Weight == FontWeights.Bold;
            }

            if (this.italicCheckBox != null)
            {
               this.italicCheckBox.IsChecked = typeface.Style == FontStyles.Italic;
            }
        }


        /// <summary>
        /// Fonts the family list selection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void FontFamilyListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.fontFamilyList != null)
            {
                var item = this.fontFamilyList.SelectedItem as FontFamilyListItem;
                if (item != null)
                {
                    this.SelectedFontFamily = item.FontFamily;
                }
            }
        }


        /// <summary>
        /// Sizes the list selection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void SizeListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.sizeList != null)
            {
                var item = this.sizeList.SelectedItem as FontSizeListItem;
                if (item != null)
                {
                    this.SelectedFontSize = item.SizeInPixels;
                }
            }
        }

        /// <summary>
        /// Sizes the list lost focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SizeListLostFocus(object sender, RoutedEventArgs e)
        {
            if (this.sizeList != null)
            {
                double item;
                if (double.TryParse(this.sizeList.Text, out item))
                {
                    this.SelectedFontSize = FontSizeListItem.PointsToPixels(item);
                }
            }
        }


        /// <summary>
        /// Texts the decoration check state changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void TextDecorationCheckStateChanged(object sender, RoutedEventArgs e)
        {
            var textDecorations = new TextDecorationCollection();

            if (this.underlineCheckBox.IsChecked.HasValue && this.underlineCheckBox.IsChecked.Value)
            {
                textDecorations.Add(TextDecorations.Underline[0]);
            }

            if (this.baselineCheckBox.IsChecked.HasValue && this.baselineCheckBox.IsChecked.Value)
            {
                textDecorations.Add(TextDecorations.Baseline[0]);
            }

            if (this.strikethroughCheckBox.IsChecked.HasValue && this.strikethroughCheckBox.IsChecked.Value)
            {
                textDecorations.Add(TextDecorations.Strikethrough[0]);
            }

            if (this.overlineCheckBox.IsChecked.HasValue && this.overlineCheckBox.IsChecked.Value)
            {
                textDecorations.Add(TextDecorations.OverLine[0]);
            }

            textDecorations.Freeze();
            this.SelectedTextDecorations = textDecorations;
        }

        /// <summary>
        /// Typefaces the list selection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArg">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void TypefaceListSelectionChanged(object sender, RoutedEventArgs routedEventArgs)
        {
            if (this.typefaceList != null)
            {
                var item = this.typefaceList.SelectedItem as TypefaceListItem;
                if (item != null)
                {
                    this.SelectedFontWeight = item.FontWeight;
                    this.SelectedFontStyle = item.FontStyle;
                    this.SelectedFontStretch = item.FontStretch;
                }
            }

            if (this.boldCheckBox != null && this.boldCheckBox.IsChecked.HasValue)
            {
                this.SelectedFontWeight = this.boldCheckBox.IsChecked.Value ? FontWeights.Bold : FontWeights.Normal;
            }

            if (this.italicCheckBox != null && this.italicCheckBox.IsChecked.HasValue)
            {
                this.SelectedFontStyle = this.italicCheckBox.IsChecked.Value ? FontStyles.Italic : FontStyles.Normal;
            }
        }

        #endregion
    }
}
