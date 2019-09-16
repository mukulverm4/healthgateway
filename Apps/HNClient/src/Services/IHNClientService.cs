﻿//-------------------------------------------------------------------------
// Copyright © 2019 Province of British Columbia
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-------------------------------------------------------------------------

namespace HealthGateway.HNClient.Services
{
    using System;
    using HealthGateway.HNClient.Models;

    /// <summary>
    /// Interface for the HNClient Service proxy.
    /// </summary>
    public interface IHNClientService
    {
        /// <summary>
        /// Gets the current time from HNClient.
        /// </summary>
        /// <returns>A time message.</returns>
        TimeMessage GetTime();

        /// <summary>
        /// Sends a message to HNClient.
        /// </summary>
        /// <param name="msg">The string message.</param>
        /// <returns>A HNClient message.</returns>
        Message SendMessage(string msg);
    }
}
