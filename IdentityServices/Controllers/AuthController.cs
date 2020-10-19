using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IdentityServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace IdentityServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<User> signInMgr;
        private readonly IConfiguration configuration;
        private readonly IPasswordHasher<User> hasher;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly ILogger<AuthController> logger;

        public AuthController(SignInManager<User> signInMgr, IPasswordHasher<User> hasher, UserManager<User> userManager, RoleManager<Role> roleManager, ILogger<AuthController> logger, IConfiguration configuration)
        {
            this.signInMgr = signInMgr;
            this.configuration = configuration;
            this.hasher = hasher;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.logger = logger;
        }

        public RoleManager<Role> RoleManager { get; }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
       // [ValidateAntiForgeryToken]
        //CSRF: enkel nodig indien (statefull) via een browser , form ingelogd wordt
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
          // , [FromQuery(Name = "d")] string destination = "frontend")
        {
            var returnMessage = "";
            //LoginViewModel met (Required) UserName en Password aanbrengen. 
            if (!ModelState.IsValid)
                return BadRequest("Onvolledige gegevens");
            try
            {
                //geen persistence, geen lockout -> via false, false 
                var result = await
             signInMgr.PasswordSignInAsync(loginDTO.UserName, loginDTO.Password, false, false);
            //optioneel: cardnumber controle
                if (result.Succeeded)
                {
                   try
                    {                      
                        //password controle gebeurt ook in de JWTService
                        //extra checks zijn mogelijk . bvb op basis vd rol en een querystring item
                        var jwtsvc = new JWTServices<User>(configuration, logger, userManager, hasher);
                        var token = await jwtsvc.GenerateJwtToken(loginDTO);
                        return Ok(token);  // HET TOKEN returnen
                    }
                    catch (Exception exc)
                    {
                        logger.LogError($"Exception thrown when creating JWT: {exc}");
                    }
                }
                throw new Exception("User of paswoord niet gevonden.");
                //zo algemeen mogelijke boodschap. Vertel niet dat het pwd niet juist is.
            }
            catch (Exception exc)
            {
                returnMessage = $"Foutief of ongeldig request: {exc.Message}";
                ModelState.AddModelError("", returnMessage);
                Debug.WriteLine(exc.Message);
            }
            return BadRequest(returnMessage); //zo weinig mogelijk (hacker) info
        }

        [HttpGet("validate")]
        public async Task<IActionResult> Validate([FromQuery(Name = "email")] string email, [FromQuery(Name = "token")] string token)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var jwtsvc = new JWTServices<User>(configuration, logger, userManager, hasher);
            var userId = jwtsvc.ValidateToken(token);

            if (userId.ToString() !=  user.Id)
            {
                return BadRequest("Invalid token.");
            }

            return new OkObjectResult(userId);
        }



    }
}