// <copyright file="BaseInfo.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Input;
    using System.Xml.Serialization;

    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.Mvvm;

    using RedPoint.ReefStatus.Common.Commands;
    using RedPoint.ReefStatus.Common.Settings;
    using RedPoint.ReefStatus.Common.UI.ViewModel;
    using RedPoint.ReefStatus.Common.ViewModel;

    /// <summary>
    /// The range of the graph
    /// </summary>
    public enum GraphRange
    {
        /// <summary>
        /// Show all the graph
        /// </summary>
        [Description("strAll")]
        All = 0, 

        /// <summary>
        /// Show data points from the last year
        /// </summary>
        [Description("strYear")]
        Year = 4, 

        /// <summary>
        /// Show data points from the last month
        /// </summary>
        [Description("strMonth")]
        Month = 3, 

        /// <summary>
        /// Show data points from the last day
        /// </summary>
        [Description("strDay")]
        Day = 1, 

        /// <summary>
        /// Show data points from the last week
        /// </summary>
        [Description("strWeek")]
        Week = 2
    }

    /// <summary>
    /// Protocol item
    /// </summary>
    public abstract class BaseInfo : BindableBase
    {
        #region Fields

        /// <summary>
        /// The graph.
        /// </summary>
        protected GraphViewModel graph;

        /// <summary>
        /// Colour Value
        /// </summary>
        private int colourValue;

        /// <summary>
        /// The data points.
        /// </summary>
        private DataPointsViewModel dataPoints;

        /// <summary>
        /// Default Name Command
        /// </summary>
        private ICommand defaultNameCommand;

        /// <summary>
        /// Default Units
        /// </summary>
        private string defaultUnits;

        /// <summary>
        /// Default Units Command
        /// </summary>
        private ICommand defaultUnitsCommand;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Properties"/> is display.
        /// </summary>
        /// <value><c>true</c> if display; otherwise, <c>false</c>.</value>
        private bool display;

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        private string displayName;

        /// <summary>
        /// Id of the object
        /// </summary>
        private string id;

        /// <summary>
        /// The max colour value.
        /// </summary>
        private int maxColourValue;

        /// <summary>
        /// The min colour value.
        /// </summary>
        private int minColourValue;

        /// <summary>
        /// Gets or sets the range.
        /// </summary>
        /// <value>The range.</value>
        private GraphRange range;

        /// <summary>
        /// The report.
        /// </summary>
        private ReportViewModel report;

        /// <summary>
        /// The save to database.
        /// </summary>
        private bool saveToDatabase;

        /// <summary>
        /// Gets or sets the units.
        /// </summary>
        /// <value>The units.</value>
        private string units;

        /// <summary>
        /// the current value of the item
        /// </summary>
        private object value;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseInfo"/> class.
        /// </summary>
        protected BaseInfo(string type)
        {
            this.Range = GraphRange.Week;
            this.Colour = Color.Red;
            this.MaxColour = Color.Purple;
            this.MinColour = Color.Brown;
            this.Display = false;
            this.SaveToDatabase = true;

            var resource = Application.Current.TryFindResource(type);
            if (resource is string)
            {
                this.Type = resource as string;
            }
            else
            {
                this.Type = type;
            }
            
        }

        public string Type { get; set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        [XmlIgnore]
        public Color Colour
        {
            get
            {
                return Color.FromArgb(this.ColourValue);
            }

            set
            {
                this.ColourValue = value.ToArgb();
            }
        }

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        public int ColourValue
        {
            get
            {
                return this.colourValue;
            }

            set
            {
                if (this.colourValue != value)
                {
                    this.colourValue = value;
                    this.OnPropertyChanged(() => this.MediaColour);
                    this.OnPropertyChanged(() => this.ColourValue);
                    this.OnPropertyChanged(() => this.Colour);
                }
            }
        }

        /// <summary>
        /// Gets the commands.
        /// </summary>
        /// <value>The commands.</value>
        [XmlIgnore]
        public CommandThread Commands
        {
            get
            {
                return this.Controller.Commands;
            }
        }

        /// <summary>
        /// Gets the controller.
        /// </summary>
        /// <value>The controller.</value>
        [XmlIgnore]
        public Controller Controller
        {
            get
            {
                return ReefStatusSettings.Instance.GetController(this);
            }
        }

        /// <summary>
        /// Gets the converted value.
        /// </summary>
        /// <value>
        /// The converted value.
        /// </value>
        public abstract object ConvertedValue { get; }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        [XmlIgnore]
        public DataPointsViewModel DataPoints
        {
            get
            {
                return this.dataPoints ?? (this.dataPoints = new DataPointsViewModel(this));
            }
        }

        /// <summary>
        /// Gets or sets the default name command.
        /// </summary>
        /// <value>The default name command.</value>
        [XmlIgnore]
        public ICommand DefaultNameCommand
        {
            get
            {
                return this.defaultNameCommand ?? (this.defaultNameCommand = new DelegateCommand(this.SetDefaultName));
            }

            set
            {
                this.defaultNameCommand = value;
            }
        }

        /// <summary>
        /// Gets or sets the units.
        /// </summary>
        /// <value>The units.</value>
        public string DefaultUnits
        {
            get
            {
                return this.defaultUnits;
            }

            set
            {
                if (this.defaultUnits != value)
                {
                    this.defaultUnits = value;
                    if (string.IsNullOrEmpty(this.Units))
                    {
                        this.Units = value;
                    }

                    this.OnPropertyChanged(() => this.DefaultUnits);
                }
            }
        }

        /// <summary>
        /// Gets or sets the default units command.
        /// </summary>
        /// <value>The default units command.</value>
        [XmlIgnore]
        public ICommand DefaultUnitsCommand
        {
            get
            {
                return this.defaultUnitsCommand
                       ?? (this.defaultUnitsCommand = new DelegateCommand(this.SetDefaultUnits));
            }

            set
            {
                this.defaultUnitsCommand = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BaseInfo"/> is display.
        /// </summary>
        /// <value><c>true</c> if display; otherwise, <c>false</c>.</value>
        public bool Display
        {
            get
            {
                return this.display;
            }

            set
            {
                if (value != this.display)
                {
                    this.display = value;
                    this.OnPropertyChanged(() => this.Display);
                }
            }
        }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get
            {
                return this.displayName;
            }

            set
            {
                if (this.displayName != value)
                {
                    this.displayName = value;
                    this.OnPropertyChanged(() => this.DisplayName);
                    this.OnPropertyChanged(() => this.DisplayNameValue);
                }
            }
        }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        [XmlIgnore]
        public virtual string DisplayNameValue
        {
            get
            {
                return this.DisplayName;
            }

            set
            {
                this.DisplayName = value;
            }
        }

        /// <summary>
        /// Gets the double value.
        /// </summary>
        /// <value>The double value.</value>
        public abstract double DoubleValue { get; }

        /// <summary>
        /// Gets the graph.
        /// </summary>
        [XmlIgnore]
        public virtual GraphViewModel Graph
        {
            get
            {
                return this.graph ?? (this.graph = new GraphViewModel(this));
            }
        }

        /// <summary>
        /// Gets the graph id.
        /// </summary>
        public virtual string GraphId
        {
            get
            {
                return this.Id;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has data points.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has data points; otherwise, <c>false</c>.
        /// </value>
        public bool HasDataPoints
        {
            get
            {
                return this.dataPoints != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has graph.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has graph; otherwise, <c>false</c>.
        /// </value>
        public bool HasGraph
        {
            get
            {
                return this.graph != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has report.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has report; otherwise, <c>false</c>.
        /// </value>
        public bool HasReport
        {
            get
            {
                return this.report != null;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Id
        {
            get
            {
                return this.id;
            }

            set
            {
                if (this.id != value)
                {
                    this.id = value;
                    if (string.IsNullOrEmpty(this.DisplayName))
                    {
                        this.DisplayName = value;
                    }

                    this.OnPropertyChanged(() => this.Id);
                    this.OnPropertyChanged(() => this.GraphId);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is constant.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is constant; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsConstant
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        [XmlIgnore]
        public Color MaxColour
        {
            get
            {
                return Color.FromArgb(this.MaxColourValue);
            }

            set
            {
                this.MaxColourValue = value.ToArgb();
            }
        }

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        public int MaxColourValue
        {
            get
            {
                return this.maxColourValue;
            }

            set
            {
                if (this.maxColourValue != value)
                {
                    this.maxColourValue = value;
                    this.OnPropertyChanged(() => this.MaxMediaColour);
                    this.OnPropertyChanged(() => this.MaxColourValue);
                    this.OnPropertyChanged(() => this.MaxColour);
                }
            }
        }

        /// <summary>
        /// Gets or sets the max media colour.
        /// </summary>
        [XmlIgnore]
        public System.Windows.Media.Color MaxMediaColour
        {
            get
            {
                return System.Windows.Media.Color.FromArgb(
                    this.MaxColour.A, 
                    this.MaxColour.R, 
                    this.MaxColour.G, 
                    this.MaxColour.B);
            }

            set
            {
                this.MaxColour = Color.FromArgb(value.A, value.R, value.G, value.B);
            }
        }

        /// <summary>
        /// Gets or sets the media colour.
        /// </summary>
        [XmlIgnore]
        public System.Windows.Media.Color MediaColour
        {
            get
            {
                return System.Windows.Media.Color.FromArgb(this.Colour.A, this.Colour.R, this.Colour.G, this.Colour.B);
            }

            set
            {
                this.Colour = Color.FromArgb(value.A, value.R, value.G, value.B);
            }
        }

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        [XmlIgnore]
        public Color MinColour
        {
            get
            {
                return Color.FromArgb(this.MinColourValue);
            }

            set
            {
                this.MinColourValue = value.ToArgb();
            }
        }

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        public int MinColourValue
        {
            get
            {
                return this.minColourValue;
            }

            set
            {
                if (this.minColourValue != value)
                {
                    this.minColourValue = value;
                    this.OnPropertyChanged(() => this.MinMediaColour);
                    this.OnPropertyChanged(() => this.MinColourValue);
                    this.OnPropertyChanged(() => this.MinColour);
                }
            }
        }

        /// <summary>
        /// Gets or sets the min media colour.
        /// </summary>
        [XmlIgnore]
        public System.Windows.Media.Color MinMediaColour
        {
            get
            {
                return System.Windows.Media.Color.FromArgb(
                    this.MinColour.A, 
                    this.MinColour.R, 
                    this.MinColour.G, 
                    this.MinColour.B);
            }

            set
            {
                this.MinColour = Color.FromArgb(value.A, value.R, value.G, value.B);
            }
        }

        /// <summary>
        /// Gets the mode.
        /// </summary>
        /// <value>The mode.</value>
        public virtual string Mode
        {
            get
            {
                return this.Units;
            }
        }

        /// <summary>
        /// Gets the old double value.
        /// </summary>
        public abstract double? OldDoubleValue { get; }

        /// <summary>
        /// Gets or sets the old value.
        /// </summary>
        /// <value>
        /// The old value.
        /// </value>
        public object OldValue { get; set; }

        /// <summary>
        /// Gets or sets the range.
        /// </summary>
        /// <value>The range.</value>
        public GraphRange Range
        {
            get
            {
                return this.range;
            }

            set
            {
                if (this.range != value)
                {
                    this.range = value;
                    this.OnPropertyChanged(() => this.Range);

                    if (this.HasGraph)
                    {
                        this.Graph.Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the report.
        /// </summary>
        [XmlIgnore]
        public virtual ReportViewModel Report
        {
            get
            {
                return this.report ?? (this.report = new ReportViewModel(this));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [save to database].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [save to database]; otherwise, <c>false</c>.
        /// </value>
        public bool SaveToDatabase
        {
            get
            {
                return this.saveToDatabase;
            }

            set
            {
                if (this.saveToDatabase != value)
                {
                    this.saveToDatabase = value;
                    this.OnPropertyChanged(() => this.SaveToDatabase);
                }
            }
        }

        /// <summary>
        /// Gets or sets the units.
        /// </summary>
        /// <value>The units.</value>
        public string Units
        {
            get
            {
                return this.units;
            }

            set
            {
                if (value != this.units)
                {
                    this.units = value;
                    this.OnPropertyChanged(() => this.Units);
                    this.OnPropertyChanged(() => this.Mode);
                    this.OnPropertyChanged(() => this.ValueWithUnits);
                }
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (this.value == value)
                {
                    return;
                }

                this.value = value;
                this.OnPropertyChanged(() => this.Value);
                this.OnPropertyChanged(() => this.DoubleValue);
                this.OnPropertyChanged(() => this.ValueWithUnits);
                this.OnPropertyChanged("IsActive");
                this.OnPropertyChanged("LightValue");
                this.OnPropertyChanged("IsLightOn");
                this.OnPropertyChanged(() => this.ConvertedValue);
            }
        }

        /// <summary>
        /// Gets the value with units.
        /// </summary>
        /// <value>The value with units.</value>
        public virtual string ValueWithUnits
        {
            get
            {
                return this.ConvertedValue + this.Units;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return this.DisplayName;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Defaults the name.
        /// </summary>
        private void SetDefaultName()
        {
            this.DisplayNameValue = this.Id;
        }

        /// <summary>
        /// Defaults the units.
        /// </summary>
        private void SetDefaultUnits()
        {
            this.Units = this.DefaultUnits;
        }

        #endregion
    }
}