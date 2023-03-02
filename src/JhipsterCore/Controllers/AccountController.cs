using AutoMapper;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using JhipsterCore.Domain.Entities;
using JhipsterCore.Dto;
using JhipsterCore.Web.Extensions;
using JhipsterCore.Web.Filters;
using JhipsterCore.Web.Rest.Problems;
using JhipsterCore.Configuration;
using JhipsterCore.Crosscutting.Constants;
using JhipsterCore.Crosscutting.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JhipsterCore.Controllers;

[Route("api")]
[ApiController]
public class AccountController : ControllerBase
{
    [Authorize]
    [HttpGet("account")]
    public ActionResult<UserDto> GetAccount()
    {
        UserDto user = null;
        if (User != null)
        {
            user = new UserDto
            {
                Login = User.Claims.FirstOrDefault(claim => claim.Type.Equals("preferred_username"))?.Value,
                Activated = true,
                Email = User.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.Email))?.Value,
                FirstName = User.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.GivenName))?.Value,
                LastName = User.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.Surname))?.Value,
                LangKey = User.Claims.FirstOrDefault(claim => claim.Type.Equals("langKey"))?.Value ?? Constants.DefaultLangKey,
                ImageUrl = User.Claims.FirstOrDefault(claim => claim.Type.Equals("picture"))?.Value,
                Roles = User.Claims.Where(claim => claim.Type.Equals("role"))
                    .Select(claim => claim.Value).ToHashSet()
            };
        }

        if (user == null) return Unauthorized("User could not be found");

        return Ok(user);
    }
}
