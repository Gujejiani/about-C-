using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
       public UserController() {

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
