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
namespace HealthGateway.Common.AccessManagement.Administration.Models
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Class that represents the user model in the Authorization Server's user accounts.
    /// </summary>
    public class UserRepresentation
    {
        /// <summary>
        /// Gets or sets the user created timestamp.
        /// </summary>
        [JsonPropertyName("createdTimeStamp")]
        public DateTime? CreatedTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the user's email.
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        /// <summary>
        /// Gets the user's roles in the AuthServer's application realm.
        /// </summary>
        [JsonPropertyName("realmRoles")]
        public string[] RealmRoles { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the user's username.
        /// </summary>
        [JsonPropertyName("username")]
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the user's unique account identifier.
        /// </summary>
        [JsonPropertyName("id")]
        public Guid? UserId { get; set; }
    }
}