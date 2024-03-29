﻿using System;
using System.Windows;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Automation.Peers;
using System.Windows.Automation;
using System.Globalization;

namespace RedPoint.ReefStatus.Common.UI.Controls
{
    /// <summary>
    /// A Numeric Up down Control
    /// </summary>
    public class NumericUpDown : Control
    {
        static NumericUpDown()
        {
            InitializeCommands();

            // Listen to MouseLeftButtonDown event to determine if NumericUpDown should move focus to itself
            EventManager.RegisterClassHandler(typeof(NumericUpDown),
                                              Mouse.MouseDownEvent, new MouseButtonEventHandler(NumericUpDown.OnMouseLeftButtonDown), true);

            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }

        public NumericUpDown()
        {
            UpdateValueString();
        }

        #region Properties
        #region Value
        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value", typeof(decimal), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(DefaultValue,
                                              new PropertyChangedCallback(OnValueChanged),
                                              CoerceValue
                    )
                );

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            NumericUpDown control = (NumericUpDown)obj;

            decimal oldValue = (decimal)args.OldValue;
            decimal newValue = (decimal)args.NewValue;

            #region Fire Automation events
            NumericUpDownAutomationPeer peer = UIElementAutomationPeer.FromElement(control) as NumericUpDownAutomationPeer;
            if (peer != null)
            {
                peer.RaiseValueChangedEvent(oldValue, newValue);
            }
            #endregion

            RoutedPropertyChangedEventArgs<decimal> e = new RoutedPropertyChangedEventArgs<decimal>(
                oldValue, newValue, ValueChangedEvent);

            control.OnValueChanged(e);

            control.UpdateValueString();
        }

        /// <summary>
        /// Raises the ValueChanged event.
        /// </summary>
        /// <param name="args">Arguments associated with the ValueChanged event.</param>
        protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<decimal> args)
        {
            RaiseEvent(args);
        }

        private static object CoerceValue(DependencyObject element, object value)
        {
            decimal newValue = (decimal)value;
            NumericUpDown control = (NumericUpDown)element;

            newValue = Math.Max(control.Minimum, Math.Min(control.Maximum, newValue));
            newValue = Decimal.Round(newValue, control.DecimalPlaces);

            return newValue;
        }
        #endregion
        #region Minimum
        public decimal Minimum
        {
            get { return (decimal)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum", typeof(decimal), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(DefaultMinValue,
                                              new PropertyChangedCallback(OnMinimumChanged), CoerceMinimum
                    )
                );

        private static void OnMinimumChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            element.CoerceValue(MaximumProperty);
            element.CoerceValue(ValueProperty);
        }
        private static object CoerceMinimum(DependencyObject element, object value)
        {
            decimal minimum = (decimal)value;
            NumericUpDown control = (NumericUpDown)element;
            return Decimal.Round(minimum, control.DecimalPlaces);
        }
        #endregion
        #region Maximum
        public decimal Maximum
        {
            get { return (decimal)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum", typeof(decimal), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(DefaultMaxValue,
                                              new PropertyChangedCallback(OnMaximumChanged),
                                              CoerceMaximum
                    )
                );

        private static void OnMaximumChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            element.CoerceValue(ValueProperty);
        }

        private static object CoerceMaximum(DependencyObject element, object value)
        {
            NumericUpDown control = (NumericUpDown)element;
            decimal newMaximum = (decimal)value;
            return Decimal.Round(Math.Max(newMaximum, control.Minimum), control.DecimalPlaces);
        }
        #endregion
        #region Change
        public decimal Change
        {
            get { return (decimal)GetValue(ChangeProperty); }
            set { SetValue(ChangeProperty, value); }
        }

        public static readonly DependencyProperty ChangeProperty =
            DependencyProperty.Register(
                "Change", typeof(decimal), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(DefaultChange, new PropertyChangedCallback(OnChangeChanged), CoerceChange),
                ValidateChange
                );

        private static bool ValidateChange(object value)
        {
            decimal change = (decimal)value;
            return change > 0;
        }

        private static void OnChangeChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {

        }

        private static object CoerceChange(DependencyObject element, object value)
        {
            decimal newChange = (decimal)value;
            NumericUpDown control = (NumericUpDown)element;

            decimal coercedNewChange = Decimal.Round(newChange, control.DecimalPlaces);

            //If Change is .1 and DecimalPlaces is changed from 1 to 0, we want Change to go to 1, not 0.
            //Put another way, Change should always be rounded to DecimalPlaces, but never smaller than the 
            //previous Change
            if (coercedNewChange < newChange)
            {
                coercedNewChange = SmallestForDecimalPlaces(control.DecimalPlaces);
            }

            return coercedNewChange;
        }

        private static decimal SmallestForDecimalPlaces(int decimalPlaces)
        {
            RequireArgumentRange(decimalPlaces >= 0, "decimalPlaces");

            decimal d = 1;

            for (int i = 0; i < decimalPlaces; i++)
            {
                d /= 10;
            }

            return d;
        }

        public static void RequireArgumentRange(bool truth, string parameterName)
        {
            RequireNotNullOrEmpty(parameterName, "parameterName");

            if (!truth)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void RequireNotNullOrEmpty(string stringParameter, string parameterName)
        {
            if (stringParameter == null)
            {
                throw new ArgumentNullException(parameterName);
            }
            if (stringParameter.Length == 0)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        #endregion
        #region DecimalPlaces
        public int DecimalPlaces
        {
            get { return (int)GetValue(DecimalPlacesProperty); }
            set { SetValue(DecimalPlacesProperty, value); }
        }

        public static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.Register(
                "DecimalPlaces", typeof(int), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(DefaultDecimalPlaces,
                                              new PropertyChangedCallback(OnDecimalPlacesChanged)
                    ), ValidateDecimalPlaces
                );

        private static void OnDecimalPlacesChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            NumericUpDown control = (NumericUpDown)element;
            control.CoerceValue(ChangeProperty);
            control.CoerceValue(MinimumProperty);
            control.CoerceValue(MaximumProperty);
            control.CoerceValue(ValueProperty);
            control.UpdateValueString();
        }

        private static bool ValidateDecimalPlaces(object value)
        {
            int decimalPlaces = (int)value;
            return decimalPlaces >= 0;
        }

        #endregion

        #region ValueString
        public string ValueString
        {
            get
            {
                return (string)GetValue(ValueStringProperty);
            }
        }

        private static readonly DependencyPropertyKey ValueStringPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("ValueString", typeof(string), typeof(NumericUpDown), new PropertyMetadata());

        public static readonly DependencyProperty ValueStringProperty = ValueStringPropertyKey.DependencyProperty;

        private void UpdateValueString()
        {
            numberFormatInfo.NumberDecimalDigits = this.DecimalPlaces;
            string newValueString = this.Value.ToString("f", numberFormatInfo);
            this.SetValue(ValueStringPropertyKey, newValueString);
        }

        private NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
        #endregion

        #endregion

        #region Events
        /// <summary>
        /// Identifies the ValueChanged routed event.
        /// </summary>
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
            "ValueChanged", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<decimal>), typeof(NumericUpDown));

        /// <summary>
        /// Occurs when the Value property changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<decimal> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }
        #endregion

        #region Commands

        public static RoutedCommand IncreaseCommand
        {
            get
            {
                return increaseCommand;
            }
        }
        public static RoutedCommand DecreaseCommand
        {
            get
            {
                return decreaseCommand;
            }
        }

        private static void InitializeCommands()
        {
            increaseCommand = new RoutedCommand("IncreaseCommand", typeof(NumericUpDown));
            CommandManager.RegisterClassCommandBinding(typeof(NumericUpDown), new CommandBinding(increaseCommand, OnIncreaseCommand));
            CommandManager.RegisterClassInputBinding(typeof(NumericUpDown), new InputBinding(increaseCommand, new KeyGesture(Key.Up)));

            decreaseCommand = new RoutedCommand("DecreaseCommand", typeof(NumericUpDown));
            CommandManager.RegisterClassCommandBinding(typeof(NumericUpDown), new CommandBinding(decreaseCommand, OnDecreaseCommand));
            CommandManager.RegisterClassInputBinding(typeof(NumericUpDown), new InputBinding(decreaseCommand, new KeyGesture(Key.Down)));
        }

        private static void OnIncreaseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            NumericUpDown control = sender as NumericUpDown;
            if (control != null)
            {
                control.OnIncrease();
            }
        }
        private static void OnDecreaseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            NumericUpDown control = sender as NumericUpDown;
            if (control != null)
            {
                control.OnDecrease();
            }
        }

        protected virtual void OnIncrease()
        {
            this.Value += Change;
        }
        protected virtual void OnDecrease()
        {
            this.Value -= Change;
        }

        private static RoutedCommand increaseCommand;
        private static RoutedCommand decreaseCommand;
        #endregion

        #region Automation
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new NumericUpDownAutomationPeer(this);
        }
        #endregion

        /// <summary>
        /// This is a class handler for MouseLeftButtonDown event.
        /// The purpose of this handle is to move input focus to NumericUpDown when user pressed
        /// mouse left button on any part of slider that is not focusable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NumericUpDown control = (NumericUpDown)sender;

            // When someone click on a part in the NumericUpDown and it's not focusable
            // NumericUpDown needs to take the focus in order to process keyboard correctly
            if (!control.IsKeyboardFocusWithin)
            {
                e.Handled = control.Focus() || e.Handled;
            }
        }

        private const decimal DefaultMinValue = 0,
                              DefaultValue = DefaultMinValue,
                              DefaultMaxValue = 100,
                              DefaultChange = 1;
        private const int DefaultDecimalPlaces = 0;
    }

    public class NumericUpDownAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
    {
        public NumericUpDownAutomationPeer(global::RedPoint.ReefStatus.Common.UI.Controls.NumericUpDown control)
            : base(control)
        {
        }

        protected override string GetClassNameCore()
        {
            return "NumericUpDown";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Spinner;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.RangeValue)
            {
                return this;
            }
            return base.GetPattern(patternInterface);
        }

        internal void RaiseValueChangedEvent(decimal oldValue, decimal newValue)
        {
            RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty,
                                           (double)oldValue, (double)newValue);
        }

        #region IRangeValueProvider Members

        bool IRangeValueProvider.IsReadOnly
        {
            get
            {
                return !IsEnabled();
            }
        }

        double IRangeValueProvider.LargeChange
        {
            get { return (double)MyOwner.Change; }
        }

        double IRangeValueProvider.Maximum
        {
            get { return (double)MyOwner.Maximum; }
        }

        double IRangeValueProvider.Minimum
        {
            get { return (double)MyOwner.Minimum; }
        }

        void IRangeValueProvider.SetValue(double value)
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            decimal val = (decimal)value;
            if (val < MyOwner.Minimum || val > MyOwner.Maximum)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            MyOwner.Value = val;
        }

        double IRangeValueProvider.SmallChange
        {
            get { return (double)MyOwner.Change; }
        }

        double IRangeValueProvider.Value
        {
            get { return (double)MyOwner.Value; }
        }

        #endregion

        private global::RedPoint.ReefStatus.Common.UI.Controls.NumericUpDown MyOwner
        {
            get
            {
                return (global::RedPoint.ReefStatus.Common.UI.Controls.NumericUpDown)Owner;
            }
        }
    }
}