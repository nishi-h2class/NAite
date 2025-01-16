using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.DataTransferObjects.Responses;
using NAiteWebApi.Libs;
using NAiteWebApi.Repository.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace NAiteWebApi.Controllers
{
    /// <summary>
    /// 認証用API
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IRepositoryWrapper _repository;
        private IMapper _mapper;

        public AuthController(IRepositoryWrapper repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Login認証用API
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        /// <remarks>
        /// sample request:
        /// 
        ///		{
        ///			"loginId": "admin",
        ///			"loginPassword": "password"
        ///		}
        ///		
        /// </remarks>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody] AuthParams param)
        {
            Logs.Logger.Debug($"ログイン認証:{JsonSerializer.Serialize(param)}");

            if (param is null)
            {
                Logs.Logger.Error("ログイン情報が指定されていません");
                return BadRequest("ログイン情報がありません");
            }

            if (!ModelState.IsValid)
            {
                Logs.Logger.Error("ModelState is not valid");
                return BadRequest("ログイン情報にエラーがあります");
            }

            if (param.LoginId == NAiteSettings.GetSystemAdminLoginId())
            {
                if (param.Password != NAiteSettings.GetSystemAdminLoginPassword())
                {
                    Logs.Logger.Error($"SystemAdmin認証エラー：Password:{param.Password}");
                    return BadRequest("パスワードが違います");
                }
            }

            var user = _repository.Auth.Auth(param);

            if (user != null)
            {
                if (param.LoginId != "admin")
                {
                    var result = LdapLib.Auth(NAiteSettings.GetAdServerAddress(), Convert.ToInt32(NAiteSettings.GetAdServerPort()), param.LoginId, param.Password);

                    if (result == false)
                    {
                        Logs.Logger.Error($"LDAP認証に失敗しました：LoginId:{param.LoginId}:Password:{param.Password}");
                        return Unauthorized("認証に失敗しました");
                    }
                }

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(NAiteSettings.GetTokenSecretKey()));
                var issuer = NAiteSettings.GetTokenIssuer();
                var audience = NAiteSettings.GetTokenAudience();
                var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Role, user.Authority)
                };

                var tokenOptions = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.Now.AddHours(24),
                    signingCredentials: signingCredentials
                    );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                var authUser = _mapper.Map<AuthRes>(user);
                authUser.Token = tokenString;

                ApiContext.CurrentUser = user;


                Logs.Logger.Info($"ログイン成功: LoginID:{user.Id}, Name:{user.LastName}{user.FirstName}, Authority:{user.Authority}");

                return Ok(authUser);
            }

            Logs.Logger.Error($"指定されたユーザは登録されていません：LoginId:{param.LoginId}");
            return Unauthorized("指定されたユーザは登録されていません");
        }
    }
}
