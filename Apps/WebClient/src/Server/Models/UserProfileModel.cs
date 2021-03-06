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
namespace HealthGateway.WebClient.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Model that provides a user representation of an user profile database model.
    /// </summary>
    public class UserProfileModel
    {
        /// <summary>
        /// Gets or sets the user hdid.
        /// </summary>
        public string HdId { get; set; } = null!;

        /// <summary>
        /// Gets or sets a value indicating whether the user accepted the terms of service.
        /// </summary>
        public bool AcceptedTermsOfService { get; set; }

        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the user SMS number.
        /// </summary>
        public string? SMSNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user needs to be notified about new terms of service.
        /// </summary>
        public bool HasTermsOfServiceUpdated { get; set; }

        /// <summary>
        /// Gets or sets the value indicating the date when the user last logon.
        /// </summary>
        public DateTime? LastLoginDateTime { get; set; }

        /// <summary>
        /// Gets or sets the Closed datetime of the account.
        /// After an account has been closed for n amount of days the row is physically deleted.
        /// </summary>
        public DateTime? ClosedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the user preference.
        /// </summary>
        public Dictionary<string, string> Preferences { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Constructs a UserProfile model from a UserProfile database model.
        /// </summary>
        /// <param name="model">The user profile database model.</param>
        /// <returns>The user profile model.</returns>
        public static UserProfileModel CreateFromDbModel(Database.Models.UserProfile model)
        {
            if (model == null)
            {
                return null!;
            }

            return new UserProfileModel()
            {
                HdId = model.HdId,
                AcceptedTermsOfService = model.AcceptedTermsOfService,
                Email = model.Email,
                SMSNumber = model.SMSNumber,
                LastLoginDateTime = model.LastLoginDateTime,
                ClosedDateTime = model.ClosedDateTime,
            };
        }
    }
}
