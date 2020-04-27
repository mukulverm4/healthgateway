//-------------------------------------------------------------------------
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
namespace HealthGateway.ExternalClient.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Configuration data to be used by the Health Gateway Externalclient.
    /// </summary>
    public class ExternalClientConfiguration
    {
        /// <summary>
        /// Gets or sets the logging level used by the Externalclient.
        /// </summary>
        public string LogLevel { get; set; }

        /// <summary>
        /// Gets or sets the Externalclient timeout values.
        /// </summary>
        public TimeOutsConfiguration Timeouts { get; set; }

        /// <summary>
        /// Gets or sets the Externalclient registration status.
        /// </summary>
        public string RegistrationStatus { get; set; }

        /// <summary>
        /// Gets or sets the ExternalURLs used by the Externalclient.
        /// </summary>
#pragma warning disable CA2227 //disable read-only Dictionary
        public Dictionary<string, System.Uri> ExternalURLs { get; set; } = new Dictionary<string, System.Uri>();

        /// <summary>
        /// Gets or sets the state for each of our modules.
        /// </summary>
#pragma warning disable CA2227 //disable read-only Dictionary
        public Dictionary<string, bool> Modules { get; set; } = new Dictionary<string, bool>();

        /// <summary>
        /// Gets or sets the number of hours until an account is removed after being closed.
        /// </summary>
        public int HoursForDeletion { get; set; }
    }
}
