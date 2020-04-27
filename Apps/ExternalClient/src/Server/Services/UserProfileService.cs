// -------------------------------------------------------------------------
//  Copyright © 2019 Province of British Columbia
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// -------------------------------------------------------------------------
namespace HealthGateway.ExternalClient.Services
{
    using System;
    using HealthGateway.Common.Constants;
    using HealthGateway.Common.Models;
    using HealthGateway.ExternalClient.Models;
    using Microsoft.Extensions.Logging;

    /// <inheritdoc />
    public class UserProfileService : IUserProfileService
    {
        private readonly ILogger logger;
        private readonly IConfigurationService configurationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileService"/> class.
        /// </summary>
        /// <param name="logger">Injected Logger Provider.</param>
        /// <param name="configuration">The configuration service.</param>
        public UserProfileService(ILogger<UserProfileService> logger, IConfigurationService configuration)
        {
            this.logger = logger;
            this.configurationService = configuration;
        }

        /// <inheritdoc />
        public RequestResult<UserProfileModel> GetUserProfile(string hdid, DateTime? lastLogin = null)
        {
            UserProfileModel userProfile = new UserProfileModel();
            return new RequestResult<UserProfileModel>()
            {
                ResultStatus = ResultType.Success,
                ResultMessage = "",
                ResourcePayload = userProfile,
            };
        }
    }
}
