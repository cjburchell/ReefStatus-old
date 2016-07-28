
namespace RedPoint.ReefStatus.Common.UI.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// The button expander.
    /// </summary>
    [TemplatePart(Name = "PART_Toggle", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_Container", Type = typeof(DependencyObject))]
    public class ButtonExpander : UserControl
    {
        #region Constants and Fields

        /// <summary>
        /// The header property.
        /// </summary>
        public static readonly DependencyProperty ButtonStyleProperty =
            DependencyProperty.RegisterAttached(
                "ButtonStyle", 
                typeof(Style), 
                typeof(ButtonExpander), 
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// The corner radius property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius", 
            typeof(CornerRadius),
            typeof(ButtonExpander),
            new FrameworkPropertyMetadata(new CornerRadius(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// The expander template property.
        /// </summary>
        public static readonly DependencyProperty ExpanderTemplateProperty =
            DependencyProperty.RegisterAttached(
                "ExpanderTemplate", 
                typeof(ControlTemplate), 
                typeof(ButtonExpander), 
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// The header property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached(
            "Header", 
            typeof(object), 
            typeof(ButtonExpander), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// The popup placement property.
        /// </summary>
        public static readonly DependencyProperty PopupPlacementProperty =
            DependencyProperty.RegisterAttached(
                "PopupPlacement", 
                typeof(PlacementMode), 
                typeof(ButtonExpander), 
                new FrameworkPropertyMetadata(PlacementMode.Bottom, FrameworkPropertyMetadataOptions.AffectsRender) { Inherits = true });

        /// <summary>
        /// The container.
        /// </summary>
        private DependencyObject container;

        /// <summary>
        /// The toggle button.
        /// </summary>
        private ToggleButton toggleButton;

        /// <summary>
        /// The top level window.
        /// </summary>
        private Window topLevelWindow;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the button style.
        /// </summary>
        /// <value>
        /// The button style.
        /// </value>
        public Style ButtonStyle
        {
            get
            {
                return this.GetValue(ButtonStyleProperty) as Style;
            }

            set
            {
                this.SetValue(ButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the corner radius.
        /// </summary>
        /// <value>
        /// The corner radius.
        /// </value>
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)this.GetValue(CornerRadiusProperty);
            }

            set
            {
                this.SetValue(CornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the expander template.
        /// </summary>
        /// <value>The expander template.</value>
        public ControlTemplate ExpanderTemplate
        {
            get
            {
                return (ControlTemplate)this.GetValue(ExpanderTemplateProperty);
            }

            set
            {
                this.SetValue(ExpanderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public object Header
        {
            get
            {
                return this.GetValue(HeaderProperty);
            }

            set
            {
                this.SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets PopupPlacement.
        /// </summary>
        public PlacementMode PopupPlacement
        {
            get
            {
                return (PlacementMode)this.GetValue(PopupPlacementProperty);
            }

            set
            {
                this.SetValue(PopupPlacementProperty, value);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether [is visual child] [the specified root].
        /// </summary>
        /// <param name="root">
        /// The root element.
        /// </param>
        /// <param name="childToFind">
        /// The child to find.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is visual child] [the specified root]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsVisualChild(DependencyObject root, DependencyObject childToFind)
        {
            if (childToFind != null && root != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(root, i);
                    if (child == childToFind)
                    {
                        return true;
                    }

                    if (IsVisualChild(child, childToFind))
                    {
                        return true;
                    }

                    if (child is ButtonExpander)
                    {
                        var expander = (ButtonExpander)child;
                        if (IsVisualChild(expander.container, childToFind))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.toggleButton = this.GetTemplateChild("PART_Toggle") as ToggleButton;
            this.container = this.GetTemplateChild("PART_Container");
            this.topLevelWindow = this.GetTopLevelWindow();
            if (this.topLevelWindow != null)
            {
                this.topLevelWindow.PreviewMouseDown += this.TopLevelWindowPreviewMouseDown;
                this.topLevelWindow.Deactivated += this.TopLevelWindowDeactivated;
            }

            this.Loaded += this.ButtonExpanderLoaded;
            this.Unloaded += this.ButtonExpanderUnloaded;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the top level control.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        /// <returns>
        /// Top level Control
        /// </returns>
        private static DependencyObject GetTopLevelControl(DependencyObject control)
        {
            DependencyObject tmp = control;
            DependencyObject parent = null;
            while ((tmp = VisualTreeHelper.GetParent(tmp)) != null)
            {
                parent = tmp;
            }

            return parent;
        }

        /// <summary>
        /// Buttons the expander loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void ButtonExpanderLoaded(object sender, RoutedEventArgs e)
        {
            if (this.topLevelWindow != null)
            {
                this.topLevelWindow.PreviewMouseDown -= this.TopLevelWindowPreviewMouseDown;
                this.topLevelWindow.Deactivated -= this.TopLevelWindowDeactivated;
                this.topLevelWindow = null;
            }

            this.topLevelWindow = this.GetTopLevelWindow();
            if (this.topLevelWindow != null)
            {
                this.topLevelWindow.PreviewMouseDown += this.TopLevelWindowPreviewMouseDown;
                this.topLevelWindow.Deactivated += this.TopLevelWindowDeactivated;
            }
        }

        /// <summary>
        /// Buttons the expander unloaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void ButtonExpanderUnloaded(object sender, RoutedEventArgs e)
        {
            if (this.topLevelWindow != null)
            {
                this.topLevelWindow.PreviewMouseDown -= this.TopLevelWindowPreviewMouseDown;
                this.topLevelWindow.Deactivated -= this.TopLevelWindowDeactivated;
                this.topLevelWindow = null;
            }
        }

        /// <summary>
        /// Gets the top level window.
        /// </summary>
        /// <returns>
        /// The Top most window object
        /// </returns>
        private Window GetTopLevelWindow()
        {
            DependencyObject parent = GetTopLevelControl(this);
            while (!(parent is Window))
            {
                parent = LogicalTreeHelper.GetParent(parent);
                parent = GetTopLevelControl(parent);
            }

            return parent as Window;
        }

        /// <summary>
        /// Tops the level window deactivated.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private void TopLevelWindowDeactivated(object sender, EventArgs e)
        {
            if (this.toggleButton != null)
            {
                this.toggleButton.IsChecked = false;
            }
        }

        /// <summary>
        /// Handles the PreviewMouseDown event of the topLevelWindow control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.
        /// </param>
        private void TopLevelWindowPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.toggleButton != null && this.toggleButton.IsChecked.HasValue && this.toggleButton.IsChecked.Value)
            {
                if (!IsVisualChild(this, e.OriginalSource as DependencyObject))
                {
                    if (!IsVisualChild(this.container, e.OriginalSource as DependencyObject))
                    {
                        this.toggleButton.IsChecked = false;
                    }
                }
            }
        }

        #endregion
    }
}