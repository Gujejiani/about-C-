

using System.Data;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Controllers {


    public class AuthController: ControllerBase {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config) {
            
            _dapper = new DataContextDapper(config);
            _config = config;
            
        }

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

                    string passwordPlusString = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

                    byte [] passwordHash = KeyDerivation.Pbkdf2(
                        password: user.Password,
                        salt: Encoding.ASCII.GetBytes(passwordPlusString),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 10000,
                        numBytesRequested: 256 / 8
                    );

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
                    passwordHashParameter.Value = passwordHashParameter;


                    sqlParameters.Add(passwordSaltParameter);
                    sqlParameters.Add(passwordHashParameter);

                    if(_dapper.executeSqlWithParameters(sqlAddAuth, sqlParameters)){
                        return Ok();
                    }
                    throw new Exception("Failed to add user");
                }
                  throw new Exception("User already exists");
              
            }

            throw new Exception("Passwords do not match");

            
        }



         [HttpPost("Login")]

        public IActionResult Login(UserLoginDto user){

            return Ok();

        }
    }
    
}