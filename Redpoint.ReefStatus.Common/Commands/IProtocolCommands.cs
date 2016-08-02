// <copyright file="IProtocolCommands.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Commands
{
    using RedPoint.ReefStatus.Common.ProfiLux;

    /// <summary>
    /// The protocal commands
    /// </summary>
    public interface IProtocolCommands
    {
        /// <summary>
        /// Gets the sensors.
        /// </summary>
        /// <param name="updateInfo">if set to <c>true</c> [update info].</param>
        void UpdateProbes(bool updateInfo);

        /// <summary>
        /// Gets the sensors.
        /// </summary>
        /// <param name="updateInfo">if set to <c>true</c> [update info].</param>
        void UpdateLevelSensors(bool updateInfo);

        /// <summary>
        /// Gets the devices.
        /// </summary>
        /// <param name="updateInfo">if set to <c>true</c> [update info].</param>
        void UpdateSPorts(bool updateInfo);

        /// <summary>
        /// Gets the devices.
        /// </summary>
        /// <param name="updateInfo">if set to <c>true</c> [update info].</param>
        void UpdateLPorts(bool updateInfo);

        /// <summary>
        /// Gets the info.
        /// </summary>
        /// <param name="fullUpdate">if set to <c>true</c> [full update].</param>
        void UpdateInfo(bool fullUpdate);

        /// <summary>
        /// Gets the display string.
        /// </summary>
        /// <returns>The display String</returns>
        string GetDisplayString();

        /// <summary>
        /// Gets the view string.
        /// </summary>
        /// <returns>The View string</returns>
        string GetViewString();

        /// <summary>
        /// Updates all.
        /// </summary>
        void UpdateAll();

        /// <summary>
        /// Keys the command.
        /// </summary>
        /// <param name="facePlateKey">The face plate key.</param>
        void KeyCommand(FaceplateKey facePlateKey);

        void LogParameters();
    }
}
