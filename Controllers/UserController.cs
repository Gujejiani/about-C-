using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
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
        [HttpGet("getUsers/{testValue}")]
        
        public string[] getUsers(string testValue)

        {
            string [] response = new string[]{
                "Hello",
                "World",
                testValue
            };
            

            return response;
        }
    }

    
}
