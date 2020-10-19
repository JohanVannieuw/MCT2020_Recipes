using IdentityServices.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


public class JWTServices<TEntity> where TEntity : IdentityUser
{
    private readonly ILogger logger;
    private readonly UserManager<TEntity> userManager;
    private readonly IPasswordHasher<TEntity> hasher;
    private readonly IConfiguration configuration;

    public JWTServices(Microsoft.Extensions.Configuration.IConfiguration configuration, ILogger logger, UserManager<TEntity> userManager, IPasswordHasher<TEntity> hasher)
    {
        this.logger = logger;
        this.userManager = userManager;
        this.hasher = hasher;
        this.configuration = configuration;
    }

    //returnen van een anoniem object
    public async Task<object> GenerateJwtToken(LoginDTO identityDTO)
    {
        //0. Geen gebruik van de signin manager voor het token-> deze signin manager 
        //maakt een autorisatie cookie aan . De signin manager gebruiken (Cookie 
        // authenticatie) zou kunnen als backup voor een falend token.

        //=> Token aanmaken na loggedin user  met extra claims + verplichte JWT claims.

        //1. Gebruiker opzoeken in de database met async UserManager en hash vergelijking

        TEntity user ;
        
        try
        {
            user = await userManager.FindByNameAsync(identityDTO.UserName);
            var roles = await userManager.GetRolesAsync(user);

            if (user != null)
            {
                //identity control op de hash (=kan zonder signin)
                if (hasher.VerifyHashedPassword(user, user.PasswordHash, identityDTO.Password) == PasswordVerificationResult.Success)
                {

                    //generateToken code hier indien paswoord controle.
                    Console.Write("password verified: {0}", user.PasswordHash);
                }
                else {
                    return new { error = "Unknown user or password" };
                }

            }
            else
            {
                return new { error = "Unknown user or password" };
            }

            //2. claims (key/value) toevoegen
            //2.1 Customised of extra claims, komende van het Identity system (vb. rollen):
            var userClaims = await userManager.GetClaimsAsync(user);
            await userManager.RemoveClaimsAsync(user, userClaims);

            await userManager.AddClaimAsync(user, new Claim("myExtraKey", "myExtraValue"));
            //combined string van roles kan niet => ClaimTypes.Role
            foreach (var role in roles)
            {
                await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, role));
            }

            userClaims = await userManager.GetClaimsAsync(user);


            //2.2. Noodzakelijke claims komende vd JWD spec
            var claims = new List<Claim>
             {
                //JWT claims zijn ingebouwd in de JWT spec: "sub"scriber, JWT Id
                 new Claim(JwtRegisteredClaimNames.Sub, identityDTO.UserName),  //subscriber
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                 //extra claims (waardoor toch datastore info beschikbaar wordt)
               //new Claim(JwtRegisteredClaimNames.Birthdate, identityDTO.Birthdate.ToString())
             }.Union(userClaims);   //nog de extra userClaims toevoegen.

            //3. Sigin credentials met de symmetric key & encryptie methode
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); //key en protocol

            //4. aanmaken van het token
            var token = new JwtSecurityToken(
             issuer: configuration["Tokens:Issuer"],  //onze website
             audience: configuration["Tokens:Audience"],//gebruikers
             claims: claims,
             expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(configuration["Tokens: Expires"])),
             signingCredentials: creds //controleert token v
             );

            //5. user info returnen (vervaldatum als additionele info)
            return new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token), //token generator
                expiration = token.ValidTo
            };
        }
        catch (Exception exc)
        {
            logger.LogError($"Exception thrown when creating JWT: {exc}");
        }
        return new { error = "Failed to generate JWT token" }; //minimale info ->meer in de logger
    }


    public Guid ValidateToken(string token)
    {
        var principal = GetPrincipal(token);
        if (principal == null)
        {
            return Guid.Empty;
        }

        ClaimsIdentity identity;
        try
        {
            identity = (ClaimsIdentity)principal.Identity;
        }
        catch (NullReferenceException e)
        {
            return Guid.Empty;
        }
        var userIdClaim = identity.FindFirst("userId");
        var userId = new Guid(userIdClaim.Value);
        return userId;
    }

    private ClaimsPrincipal GetPrincipal(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            if (jwtToken == null)
            {
                return null;
            }

            var key = Encoding.UTF8.GetBytes(configuration["Tokens:Key"]);
            var parameters = new TokenValidationParameters()
            {
                RequireExpirationTime = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
            IdentityModelEventSource.ShowPII = true;
            SecurityToken securityToken;
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                  parameters, out securityToken);
            return principal;
        }
        catch (Exception e)
        {
            return null;
        }
    }
}