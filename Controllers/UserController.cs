using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
         DataContextDapper _dapper;
       public UserController(IConfiguration config ) {

            _dapper = new DataContextDapper(config);

            Console.WriteLine("Connection string " + config.GetConnectionString("DefaultConnection"));

        }
    
        [HttpGet("TestConnection")]
        public string TestConnection()
        {
            return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()" as string).ToString();
        }
        // Fix the conflicting route by giving it a unique path
        [HttpGet("getUsers")]
        
        public User[] getUsers()

        {
            string sql = @"
            SELECT [UserId],
                    [FirstName],
                    [LastName],
                    [Email],
                    [Gender],
                    [Active] FROM TutorialAppSchema.Users
            ";
            User[] response = _dapper.LoadData<User>(sql).ToArray();
            

            return response;
        }

        [HttpGet("getSingleUser/{userId}")]
        
        public User getSingleUser(int userId)

        {
            
            User response = _dapper.LoadDataSingle<User>($"SELECT * FROM TutorialAppSchema.Users WHERE UserId = {userId}");
            

            return response;
        }
        [HttpPut("EditUser")]
        public IActionResult EditUser(User user) {
            string sql = @"
                UPDATE TutorialAppSchema.Users 
                SET [FirstName]='" + user.FirstName +
                "', [LastName]='" + user.LastName +
                "', [Email]='" + user.Email +
                "', [Gender]='" + user.Gender +
                "', [Active]= '" + user.Active +
                 "' WHERE UserId = " + user.UserId;

            Console.WriteLine(sql);
            if(_dapper.executeSql(sql)){
                return Ok();
            }
            
             throw new Exception("Failed to update user");
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(UserAddDto user) {
            string sql = @"
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
                "', '" + user.Active +
                "')";

                Console.WriteLine(sql);

            if(_dapper.executeSql(sql)){
                return Ok();
            }
            
             throw new Exception("Failed to create user");
        }


    }

    
