// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgramableLogic.cs" company="Redpoint">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    using RedPoint.ReefStatus.Common.ProfiLux.Protocol;

    /// <summary>
    ///     The programmable logic.
    /// </summary>
    public class ProgramableLogic
    {
        public ProgramableLogic(int index)
        {
            this.Index = index;
            this.DisplayName = "Programable Logic " + (index + 1);
        }

        public ProgramableLogic()
        {
        }

        /// <summary>
        ///     Gets or sets the function.
        /// </summary>
        public LogicFunction Function { get; set; }

        public string DisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        ///     Gets or sets the input 2 item.
        /// </summary>
        public PortMode Input2 { get; set; }

        /// <summary>
        ///     Gets or sets the input 1 item.
        /// </summary>
        public PortMode Input1 { get; set; }
    }
}