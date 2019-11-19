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
namespace HealthGateway.WebClient.Controllers
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using HealthGateway.Common.Authorization;
    using HealthGateway.Database.Models;
    using HealthGateway.Database.Wrapper;
    using HealthGateway.WebClient.Models;
    using HealthGateway.WebClient.Services;
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
        /// <param name="authorizationService">The injected authorization service.</param>
        /// <param name="httpContextAccessor">The injected http context accessor provider.</param>
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
        /// Posts a user profile json to be inserted into the database.
        /// </summary>
        /// <returns>The http status.</returns>
        /// <param name="hdid">The resource hdid.</param>
        /// <param name="userProfile">The user profile model.</param>
        /// <response code="200">The user profile record was saved.</response>
        /// <response code="400">The user profile was already inserted.</response>
        /// <response code="401">The client must authenticate itself to get the requested response.</response>
        /// <response code="403">The client does not have access rights to the content; that is, it is unauthorized, so the server is refusing to give the requested resource. Unlike 401, the client's identity is known to the server.</response>
        [HttpPost]
        [Route("{hdid}")]
        [Authorize(Policy = "PatientOnly")]
        public async Task<IActionResult> CreateUserProfile(string hdid, [FromBody] UserProfile userProfile)
        {
            Contract.Requires(hdid != null);
            Contract.Requires(userProfile != null);
            ClaimsPrincipal user = this.httpContextAccessor.HttpContext.User;
            string referer = this.httpContextAccessor.HttpContext.Request
                .GetTypedHeaders()
                .Referer
                .GetLeftPart(UriPartial.Authority);
            var isAuthorized = await this.authorizationService
                .AuthorizeAsync(user, userProfile.HdId, PolicyNameConstants.UserIsPatient)
                .ConfigureAwait(true);

            if (!isAuthorized.Succeeded)
            {
                return new ForbidResult();
            }

            if (!hdid.Equals(userProfile.HdId, StringComparison.CurrentCultureIgnoreCase))
            {
                return new BadRequestResult();
            }

            userProfile.CreatedBy = hdid;
            userProfile.UpdatedBy = hdid;
            DBResult<UserProfile> result = this.userProfileService.CreateUserProfile(userProfile, new Uri(referer));
            if (result.Status != Database.Constant.DBStatusCode.Created)
            {
                return new ConflictResult();
            }

            return new OkResult();
        }

        /// <summary>
        /// Gets a user profile json.
        /// </summary>
        /// <returns>The user profile model.</returns>
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

            DBResult<UserProfile> result = this.userProfileService.GetUserProfile(hdid);
            return new JsonResult(result.Payload);
        }
    }
}
