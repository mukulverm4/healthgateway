﻿// -------------------------------------------------------------------------
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
namespace HealthGateway.ExternalClient.Controllers
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using HealthGateway.Common.AccessManagement.Authorization;
    using HealthGateway.Common.Models;
    using HealthGateway.ExternalClient.Models;
    using HealthGateway.ExternalClient.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Web API to handle user profile interactions.
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/api/[controller]")]
    [ApiController]
    public class UserProfileController
    {
        private readonly IUserProfileService userProfileService;

        private readonly IHttpContextAccessor httpContextAccessor;

        private readonly IAuthorizationService authorizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileController"/> class.
        /// </summary>
        /// <param name="userProfileService">The injected user profile service.</param>
        /// <param name="httpContextAccessor">The injected http context accessor provider.</param>
        /// <param name="authorizationService">The injected authorization service.</param>
        public UserProfileController(
            IUserProfileService userProfileService,
            IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService)
        {
            this.userProfileService = userProfileService;
            this.httpContextAccessor = httpContextAccessor;
            this.authorizationService = authorizationService;
        }

        /// <summary>
        /// Gets a user profile json.
        /// </summary>
        /// <returns>The user profile model wrapped in a request result.</returns>
        /// <param name="hdid">The user hdid.</param>
        /// <response code="200">Returns the user profile json.</response>
        /// <response code="401">the client must authenticate itself to get the requested response.</response>
        /// <response code="403">The client does not have access rights to the content; that is, it is unauthorized, so the server is refusing to give the requested resource. Unlike 401, the client's identity is known to the server.</response>
        [HttpGet]
        [Route("{hdid}")]
        public async Task<IActionResult> GetUserProfile(string hdid)
        {
            Contract.Requires(hdid != null);
            ClaimsPrincipal user = this.httpContextAccessor.HttpContext.User;
            var isAuthorized = await this.authorizationService
                .AuthorizeAsync(user, hdid, PolicyNameConstants.UserIsPatient)
                .ConfigureAwait(true);
            if (!isAuthorized.Succeeded)
            {
                return new ForbidResult();
            }

            string rowAuthTime = user.FindFirst(c => c.Type == "auth_time").Value;

            // Auth time at comes in the JWT as seconds after 1970-01-01
            DateTime jwtAuthTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(int.Parse(rowAuthTime, CultureInfo.CurrentCulture));

            //RequestResult<UserProfileModel> result = this.userProfileService.GetUserProfile(hdid, jwtAuthTime);
            return new JsonResult(null);
        }
    }
}
