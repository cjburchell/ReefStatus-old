namespace RedPoint.ReefStatus.Common.UI.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    [TemplatePart(Name = "PART_Number", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_Down", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_Up", Type = typeof(RepeatButton))]
    public class SpinBox : Control
    {
        // Using a DependencyProperty as the backing store for Value.  
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(decimal), typeof(SpinBox), new FrameworkPropertyMetadata((decimal)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        // Using a DependencyProperty as the backing store for MinValue.  
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(decimal), typeof(SpinBox), new FrameworkPropertyMetadata(decimal.MinValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        // Using a DependencyProperty as the backing store for MaxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(decimal), typeof(SpinBox), new FrameworkPropertyMetadata(decimal.MinValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// The value of the text box
        /// </summary>
        public decimal Value
        {
            get
            {
                return (decimal)GetValue(ValueProperty);
            }

            set
            {
                SetValue(ValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the min value of the text box
        /// </summary>
        public decimal MinValue
        {
            get
            {
                return (decimal)GetValue(MinValueProperty);
            }

            set
            {
                SetValue(MinValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the max value of the text box
        /// </summary>
        public decimal MaxValue
        {
            get
            {
                return (decimal)GetValue(MaxValueProperty);
            }

            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        private TextBox textBox;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.textBox = GetTemplateChild("PART_Number") as TextBox;
            if (this.textBox != null)
            {
                this.textBox.PreviewTextInput += this.spinboxTextBox_PreviewTextInput;
            }

            RepeatButton downButton = GetTemplateChild("PART_Down") as RepeatButton;
            if (downButton != null)
            {
                downButton.Click += this.spinboxDownButton_Click;
            }

            RepeatButton upButton = GetTemplateChild("PART_Up") as RepeatButton;
            if (upButton != null)
            {
                upButton.Click += this.spinboxUpButton_Click;
            }
        }

        /// <summary>
        /// captures any input the user attempts to place in the textblock and rejects any
        /// non numerical values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void spinboxTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (this.textBox != null)
            {
                e.Handled = true;
                string newText = this.textBox.Text + e.Text;
                decimal value;
                if (!decimal.TryParse(newText, out value))
                {
                    return;
                }

                if (value < this.MinValue || value > this.MaxValue)
                {
                    return;
                }

                this.Value = value;
            }
        }

        /// <summary>
        /// called when the user clicks on the spin up button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void spinboxUpButton_Click(object sender, RoutedEventArgs e)
        {
            this.Value = this.Value >= this.MaxValue ? this.Value : this.Value + 1;
        }

        /// <summary>
        /// called when the user clicks on the spin down button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void spinboxDownButton_Click(object sender, RoutedEventArgs e)
        {
            this.Value = this.Value <= this.MinValue ? this.Value : this.Value - 1;
        }
    }
}
