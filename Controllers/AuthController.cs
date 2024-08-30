

using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers {


    [Authorize]
     [ApiController]
    [Route("[controller]")]
    public class AuthController: ControllerBase {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config) {
            
            _dapper = new DataContextDapper(config);
            _config = config;
            
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto user) {
            
            if(user.Password == user.PasswordConfirm){
                string sqlCheckUserExist = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email= '" +
                user.Email + "'";

                IEnumerable<string> existingUser = _dapper.LoadData<string>(sqlCheckUserExist);

                if(existingUser.Count() == 0){

                    byte[] passwordSalt = new byte[128 / 8];

                    using(RandomNumberGenerator rng  = RandomNumberGenerator.Create()){
                        rng.GetNonZeroBytes(passwordSalt);
                    }


                    byte [] passwordHash = GetPasswordHash(user.Password, passwordSalt);

                    string sqlAddAuth = @"
                    INSERT INTO TutorialAppSchema.Auth(
                        [Email],
                        [PasswordHash],
                        [PasswordSalt]
                    ) VALUES ('" + user.Email + "', @passwordHash, @passwordSalt)";
                    
                   List<SqlParameter> sqlParameters = new List<SqlParameter>();

                   SqlParameter passwordSaltParameter = new SqlParameter("@passwordSalt", SqlDbType.VarBinary);
                    passwordSaltParameter.Value = passwordSalt;

                   SqlParameter passwordHashParameter = new SqlParameter("@passwordHash", SqlDbType.VarBinary);
                    passwordHashParameter.Value = passwordHash;


                    sqlParameters.Add(passwordSaltParameter);
                    sqlParameters.Add(passwordHashParameter);

                    if(_dapper.executeSqlWithParameters(sqlAddAuth, sqlParameters)){

                             string sqlAddUser = @"
                                    INSERT INTO TutorialAppSchema.Users(
                                            [FirstName],
                                            [LastName],
                                            [Email],
                                            [Gender],
                                            [Active]
                                            ) VALUES (" +
                                        "'" + user.FirstName +
                                        "', '" + user.LastName +
                                        "', '" + user.Email +
                                        "', '" + user.Gender +
                                        "', 1)";



                        if(_dapper.executeSql(sqlAddUser)){
                            return Ok();
                        }


                        throw new Exception("Failed to add user");
                    }
                    throw new Exception("Failed to add user");
                }
                  throw new Exception("User already exists");
              
            }

            throw new Exception("Passwords do not match");

            
        }


        [AllowAnonymous]
         [HttpPost("Login")]

        public IActionResult Login(UserLoginDto user){

            string sqlForHashAndSalt = "SELECT [PasswordHash], [PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email = '" + user.Email + "'";


            UserForLoginConfirmationDto userForConfirmation = _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);


            byte[] passwordHash  = GetPasswordHash(user.Password, userForConfirmation.passwordSalt);


            // if(passwordHash.SequenceEqual(userForConfirmation.passwordHash)){
            //     return Ok();
            // }

            for (int i = 0; i < passwordHash.Length; i++)
            {
                if(passwordHash[i] != userForConfirmation.passwordHash[i]){
                   return StatusCode(401, "Invalid Password");
                }

            }

            int userId = _dapper.LoadDataSingle<int>("SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '" + user.Email + "'");

            return Ok( new Dictionary<string, string> {
                {
                    "token", CreateToken(userId)
                }
            } );

        }

 

        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            string userIdSql = @"
                SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = '" +
                User.FindFirst("userId")?.Value + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return CreateToken(userId);
        }

        private byte[] GetPasswordHash(string password, byte[] passwordSalt){
            
                 string passwordPlusString = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

                    byte [] passwordHash = KeyDerivation.Pbkdf2(
                        password: password,
                        salt: Encoding.ASCII.GetBytes(passwordPlusString),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 10000,
                        numBytesRequested: 256 / 8
                    );

                    return passwordHash;

        }


          private string CreateToken(int userId)
        {
              Claim[] claims = new Claim[] {
                new Claim("userId", userId.ToString())
            };
            
            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        tokenKeyString != null ? tokenKeyString : ""
                    )
                );

            SigningCredentials credentials = new SigningCredentials(
                    tokenKey, 
                    SecurityAlgorithms.HmacSha512Signature
                );

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(claims),
                    SigningCredentials = credentials,
                    Expires = DateTime.Now.AddDays(1)
                };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);

        }


    }
    
}