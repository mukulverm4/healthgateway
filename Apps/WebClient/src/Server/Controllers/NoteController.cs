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
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using HealthGateway.Common.AccessManagement.Authorization.Policy;
    using HealthGateway.Common.Filters;
    using HealthGateway.Common.Models;
    using HealthGateway.WebClient.Models;
    using HealthGateway.WebClient.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Web API to handle patient notes interactions.
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(AvailabilityFilter))]
    public class NoteController
    {
        private readonly INoteService noteService;

        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteController"/> class.
        /// </summary>
        /// <param name="noteService">The injected patient notes service.</param>
        /// <param name="httpContextAccessor">The injected http context accessor provider.</param>
        public NoteController(
            INoteService noteService,
            IHttpContextAccessor httpContextAccessor)
        {
            this.noteService = noteService;
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Posts a patient note json to be inserted into the database.
        /// </summary>
        /// <returns>The http status.</returns>
        /// <param name="note">The patient note request model.</param>
        /// <response code="200">The note record was saved.</response>
        /// <response code="401">The client must authenticate itself to get the requested response.</response>
        /// <response code="403">The client does not have access rights to the content; that is, it is unauthorized, so the server is refusing to give the requested resource. Unlike 401, the client's identity is known to the server.</response>
        [HttpPost]
        [Authorize(Policy = UserPolicy.UserOnly)]
        public IActionResult CreateNote([FromBody] UserNote note)
        {
            ClaimsPrincipal user = this.httpContextAccessor.HttpContext.User;
            string userHdid = user.FindFirst("hdid").Value;
            note.HdId = userHdid;
            note.CreatedBy = userHdid;
            note.UpdatedBy = userHdid;
            RequestResult<UserNote> result = this.noteService.CreateNote(note);
            return new JsonResult(result);
        }

        /// <summary>
        /// Puts a patient note json to be updated in the database.
        /// </summary>
        /// <returns>The updated Note wrapped in a RequestResult.</returns>
        /// <param name="note">The patient note.</param>
        /// <response code="200">The note was saved.</response>
        /// <response code="401">The client must authenticate itself to get the requested response.</response>
        /// <response code="403">The client does not have access rights to the content; that is, it is unauthorized, so the server is refusing to give the requested resource. Unlike 401, the client's identity is known to the server.</response>
        [HttpPut]
        [Authorize(Policy = UserPolicy.UserOnly)]
        public IActionResult UpdateNote([FromBody] UserNote note)
        {
            ClaimsPrincipal user = this.httpContextAccessor.HttpContext.User;
            string userHdid = user.FindFirst("hdid").Value;
            note.UpdatedBy = userHdid;
            RequestResult<UserNote> result = this.noteService.UpdateNote(note);
            return new JsonResult(result);
        }

        /// <summary>
        /// Deletes a note from the database.
        /// </summary>
        /// <returns>The deleted Note wrapped in a RequestResult.</returns>
        /// <param name="note">The patient note.</param>
        /// <response code="200">The note was deleted.</response>
        /// <response code="401">The client must authenticate itself to get the requested response.</response>
        /// <response code="403">The client does not have access rights to the content; that is, it is unauthorized, so the server is refusing to give the requested resource. Unlike 401, the client's identity is known to the server.</response>
        [HttpDelete]
        [Authorize(Policy = UserPolicy.UserOnly)]
        public IActionResult DeleteNote([FromBody] UserNote note)
        {
            // Validate the hdid to be a patient.
            ClaimsPrincipal user = this.httpContextAccessor.HttpContext.User;
            string userHdid = user.FindFirst("hdid").Value;

            RequestResult<UserNote> result = this.noteService.DeleteNote(note);
            return new JsonResult(result);
        }

        /// <summary>
        /// Gets all notes for the specified user.
        /// </summary>
        /// <returns>The list of notes model wrapped in a request result.</returns>
        /// <response code="200">Returns the list of notes.</response>
        /// <response code="401">the client must authenticate itself to get the requested response.</response>
        /// <response code="403">The client does not have access rights to the content; that is, it is unauthorized, so the server is refusing to give the requested resource. Unlike 401, the client's identity is known to the server.</response>
        [HttpGet]
        [Authorize(Policy = UserPolicy.UserOnly)]
        public IActionResult GetAll()
        {
            ClaimsPrincipal user = this.httpContextAccessor.HttpContext.User;
            string userHdid = user.FindFirst("hdid").Value;
            RequestResult<IEnumerable<UserNote>> result = this.noteService.GetNotes(userHdid);
            return new JsonResult(result);
        }
    }
}
