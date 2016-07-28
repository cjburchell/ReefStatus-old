namespace RedPoint.ReefStatus.Common.UI.Controls
{
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    [TemplatePart(Name = "PART_resizeGrip", Type = typeof(Thumb))]
    public class CustomWindow : Window
    {
        /// <summary>
        /// HasMaximize Property
        /// </summary>
        public static readonly DependencyProperty HasMaximizeProperty =
            DependencyProperty.Register("HasMaximize", typeof(bool), typeof(CustomWindow), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// HasMinimize Property
        /// </summary>
        public static readonly DependencyProperty HasMinimizeProperty =
            DependencyProperty.Register("HasMinimize", typeof(bool), typeof(CustomWindow), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// HasClose Property
        /// </summary>
        public static readonly DependencyProperty HasCloseProperty =
            DependencyProperty.Register("HasClose", typeof(bool), typeof(CustomWindow), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// IsMovable Property
        /// </summary>
        public static readonly DependencyProperty IsMovableProperty =
            DependencyProperty.Register("IsMovable", typeof(bool), typeof(CustomWindow), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IsSizableProperty =
            DependencyProperty.Register("IsSizable", typeof(bool), typeof(CustomWindow), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));
        
         public static readonly DependencyProperty HeaderHeightProperty =
            DependencyProperty.Register("HeaderHeight", typeof(double), typeof(CustomWindow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));


         public static readonly DependencyProperty TitleContentProperty =
    DependencyProperty.Register("TitleContent", typeof(object), typeof(CustomWindow), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));


         public object TitleContent
         {
             get { return this.GetValue(TitleContentProperty); }
             set { SetValue(TitleContentProperty, value); }
         }

        public bool IsSizable
        {
            get { return (bool)GetValue(IsSizableProperty); }
            set { SetValue(IsSizableProperty, value); }
        }

        public double HeaderHeight
        {
            get { return (double)GetValue(HeaderHeightProperty); }
            set { SetValue(HeaderHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is movable.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is movable; otherwise, <c>false</c>.
        /// </value>
        public bool IsMovable
        {
            get { return (bool)GetValue(IsMovableProperty); }
            set { SetValue(IsMovableProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has maximize.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has maximize; otherwise, <c>false</c>.
        /// </value>
        public bool HasMaximize
        {
            get { return (bool)GetValue(HasMaximizeProperty); }
            set { SetValue(HasMaximizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has minimize.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has minimize; otherwise, <c>false</c>.
        /// </value>
        public bool HasMinimize
        {
            get { return (bool)GetValue(HasMinimizeProperty); }
            set { SetValue(HasMinimizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has close.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has close; otherwise, <c>false</c>.
        /// </value>
        public bool HasClose
        {
            get { return (bool)GetValue(HasCloseProperty); }
            set { SetValue(HasCloseProperty, value); }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            {
                Thumb resizeButton = GetTemplateChild("PART_resizeGripBottomRight") as Thumb;
                if (resizeButton != null)
                {
                    resizeButton.DragDelta += this.ResizeGripDragDelta;
                    resizeButton.MouseDown += this.ResizeGripMouseDown;
                    resizeButton.MouseUp += this.ResizeGripMouseUp;
                }
            }
            {
                Thumb resizeButton = GetTemplateChild("PART_resizeGripBottom") as Thumb;
                if (resizeButton != null)
                {
                    resizeButton.DragDelta += this.ResizeGripDragDeltaHeight;
                    resizeButton.MouseDown += this.ResizeGripMouseDown;
                    resizeButton.MouseUp += this.ResizeGripMouseUp;
                }
            }
            {
                Thumb resizeButton = GetTemplateChild("PART_resizeGripRight") as Thumb;
                if (resizeButton != null)
                {
                    resizeButton.DragDelta += this.ResizeGripDragDeltaWidth;
                    resizeButton.MouseDown += this.ResizeGripMouseDown;
                    resizeButton.MouseUp += this.ResizeGripMouseUp;
                }
            }
            {
                Thumb resizeButton = GetTemplateChild("PART_resizeGripTop") as Thumb;
                if (resizeButton != null)
                {
                    resizeButton.DragDelta += this.ResizeGripDragDeltaTop;
                    resizeButton.MouseDown += this.ResizeGripMouseDown;
                    resizeButton.MouseUp += this.ResizeGripMouseUp;
                }
            }
            {
                Thumb resizeButton = GetTemplateChild("PART_resizeGripLeft") as Thumb;
                if (resizeButton != null)
                {
                    resizeButton.DragDelta += this.ResizeGripDragDeltaLeft;
                    resizeButton.MouseDown += this.ResizeGripMouseDown;
                    resizeButton.MouseUp += this.ResizeGripMouseUp;
                }
            }
            {
                Thumb resizeButton = GetTemplateChild("PART_resizeGripTopLeft") as Thumb;
                if (resizeButton != null)
                {
                    resizeButton.DragDelta += this.ResizeGripDragDeltaTopLeft;
                    resizeButton.MouseDown += this.ResizeGripMouseDown;
                    resizeButton.MouseUp += this.ResizeGripMouseUp;
                }
            }
            {
                Thumb resizeButton = GetTemplateChild("PART_resizeGripTopRight") as Thumb;
                if (resizeButton != null)
                {
                    resizeButton.DragDelta += this.ResizeGripDragDeltaTopRight;
                    resizeButton.MouseDown += this.ResizeGripMouseDown;
                    resizeButton.MouseUp += this.ResizeGripMouseUp;
                }
            }
            {
                Thumb resizeButton = GetTemplateChild("PART_resizeGripBottomLeft") as Thumb;
                if (resizeButton != null)
                {
                    resizeButton.DragDelta += this.ResizeGripDragDeltaBottomLeft;
                    resizeButton.MouseDown += this.ResizeGripMouseDown;
                    resizeButton.MouseUp += this.ResizeGripMouseUp;
                }
            }
        }

        /// <summary>
        /// Resizes the grip drag delta bottom left.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void ResizeGripDragDeltaBottomLeft(object sender, DragDeltaEventArgs e)
        {
            if (this.FlowDirection == FlowDirection.LeftToRight)
            {
                this.TrySetSize(Top, Left + e.HorizontalChange, ActualWidth - e.HorizontalChange, ActualHeight + e.VerticalChange);
            }
            else
            {
                this.TrySetSize(Top, Left, ActualWidth - e.HorizontalChange, ActualHeight + e.VerticalChange);
            }
        }

        /// <summary>
        /// Resizes the grip drag delta top right.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void ResizeGripDragDeltaTopRight(object sender, DragDeltaEventArgs e)
        {
            if (this.FlowDirection == FlowDirection.LeftToRight)
            {
                this.TrySetSize(
                    Top + e.VerticalChange, Left, ActualWidth + e.HorizontalChange, ActualHeight - e.VerticalChange);
            }
            else
            {
                this.TrySetSize(
                    Top + e.VerticalChange,
                    Left - e.HorizontalChange,
                    ActualWidth + e.HorizontalChange,
                    ActualHeight - e.VerticalChange);
            }
        }

        /// <summary>
        /// Resizes the grip drag delta top left.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void ResizeGripDragDeltaTopLeft(object sender, DragDeltaEventArgs e)
        {
            if (this.FlowDirection == FlowDirection.LeftToRight)
            {
                this.TrySetSize(
                    Top + e.VerticalChange,
                    Left + e.HorizontalChange,
                    ActualWidth - e.HorizontalChange,
                    ActualHeight - e.VerticalChange);
            }
            else
            {
                this.TrySetSize(
                    Top + e.VerticalChange, Left, ActualWidth - e.HorizontalChange, ActualHeight - e.VerticalChange);
            }
        }

        /// <summary>
        /// Resizes the grip drag delta left.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void ResizeGripDragDeltaLeft(object sender, DragDeltaEventArgs e)
        {
            if (this.FlowDirection == FlowDirection.RightToLeft)
            {
                this.TrySetSize(Top, Left, ActualWidth - e.HorizontalChange, ActualHeight);
            }
            else
            {
                this.TrySetSize(Top, Left + e.HorizontalChange, ActualWidth - e.HorizontalChange, ActualHeight);
            }
        }

        /// <summary>
        /// Resizes the grip drag delta top.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void ResizeGripDragDeltaTop(object sender, DragDeltaEventArgs e)
        {
            this.TrySetSize(Top + e.VerticalChange, Left, ActualWidth, ActualHeight - e.VerticalChange);
        }

        /// <summary>
        /// Resizes the grip mouse down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void ResizeGripMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed &&
                  e.RightButton == MouseButtonState.Released &&
                  e.MiddleButton == MouseButtonState.Released)
            {
                Mouse.Capture((Thumb)sender);
            }
        }

        /// <summary>
        /// Resizes the grip mouse up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void ResizeGripMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.Captured == sender)
            {
                Mouse.Capture(null);
            }
        }

        /// <summary>
        /// Resizes the grip drag delta.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void ResizeGripDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.FlowDirection == FlowDirection.LeftToRight)
            {
                this.TrySetSize(Top, Left, ActualWidth + e.HorizontalChange, ActualHeight + e.VerticalChange);
            }
            else
            {
                this.TrySetSize(Top, Left - e.HorizontalChange, ActualWidth + e.HorizontalChange, ActualHeight + e.VerticalChange);
            }
        }

        /// <summary>
        /// Resizes the width of the grip drag delta.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void ResizeGripDragDeltaWidth(object sender, DragDeltaEventArgs e)
        {
            if (this.FlowDirection == FlowDirection.LeftToRight)
            {
                this.TrySetSize(Top, Left, ActualWidth + e.HorizontalChange, ActualHeight);
            }
            else
            {
                this.TrySetSize(Top, Left - e.HorizontalChange, ActualWidth + e.HorizontalChange, ActualHeight);
            }
        }

        /// <summary>
        /// Resizes the height of the grip drag delta.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void ResizeGripDragDeltaHeight(object sender, DragDeltaEventArgs e)
        {
            this.TrySetSize(Top, Left, ActualWidth, ActualHeight + e.VerticalChange);
        }

        /// <summary>
        /// Tries to set the size
        /// </summary>
        /// <param name="top">The new top.</param>
        /// <param name="left">The new left.</param>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        private void TrySetSize(double top, double left, double width, double height)
        {
            if (width < MinWidth || height < MinHeight || width >= MaxWidth || height >= MaxHeight)
            {
                return;
            }

            SizeToContent = SizeToContent.Manual;
            Top = top;
            Left = left;
            Height = height;
            Width = width;
        }
    }
}
