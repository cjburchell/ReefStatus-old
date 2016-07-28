// <copyright file="ReefStatusException.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
//   This software is provided 'as-is', without any express or implied 
//   warranty.  In no event will the author be held liable for any damages 
//   arising from the use of this software. 
// 
//   Permission is granted to anyone to use this software for any purpose,
//   excluding commercial applications, and to alter it and redistribute it
//   freely, subject to the following restrictions:
// 
//   1. The origin of this software must not be misrepresented; you must not
//      claim that you wrote the original software. If you use this software
//      in a product, an acknowledgment in the product documentation would be
//      appreciated but is not required.
//   2. Altered source versions must be plainly marked as such, and must not be
//      misrepresented as being the original software.
//   3. This notice may not be removed or altered from any source distribution.
//   4. The author permission is required to use this software in commercial 
//      applications 
// </copyright>

namespace RedPoint.ReefStatus.Common
{
    using System;

    /// <summary>
    /// Base Exception for Reef status application 
    /// </summary>
    [Serializable]
    public class ReefStatusException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReefStatusException"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        public ReefStatusException(int code, string message)
            : base(message)
        {
            this.Code = code;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReefStatusException"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public ReefStatusException(int code, string message, System.Exception inner)
            : base(message, inner)
        {
            this.Code = code;
        }

        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>The code.</value>
        public int Code { get; private set; }
    }
}
