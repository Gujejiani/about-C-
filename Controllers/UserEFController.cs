using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

    [ApiController]
    [Route("[controller]")]
    public class UserEFController : ControllerBase
    {
         DataContextEF _entityFramework;
       public UserEFController(IConfiguration config ) {

            _entityFramework = new DataContextEF(config);

            Console.WriteLine("Connection string " + config.GetConnectionString("DefaultConnection"));

        }
    
      
        // Fix the conflicting route by giving it a unique path
        [HttpGet("getUsers")]
        
        public IEnumerable<User> getUsers()

        {
            
            IEnumerable<User> response = _entityFramework.Users.ToList<User>();
            

            return response;
        }

        [HttpGet("getSingleUser/{userId}")]
        
        public User getSingleUser(int userId)

        {
            
            User? user = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault<User>();

            if(user != null){
               return user;
            }

            throw new Exception("User not found");
        }
        [HttpPut("EditUser")]
        public IActionResult EditUser(User user) {
             User? userDb = _entityFramework.Users.Where(u => u.UserId == user.UserId).FirstOrDefault<User>();

                
            if(userDb !=null){
                userDb.FirstName = user.FirstName;
                userDb.LastName = user.LastName;
                userDb.Email = user.Email;
                userDb.Active = user.Active;
                  userDb.Gender = user.Gender;
            }
            if(_entityFramework.SaveChanges() > 0){
                return Ok();
            }

            throw new Exception("Failed to update user");

          
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(UserAddDto user) {
            User userDb = new User();


                userDb.FirstName = user.FirstName;
                userDb.LastName = user.LastName;
                userDb.Email = user.Email;
                userDb.Active = user.Active;
                  userDb.Gender = user.Gender;

            _entityFramework.Add(userDb);
            if(_entityFramework.SaveChanges() > 0){
                return Ok();
            }

            
             throw new Exception("Failed to create user");
        }


        [HttpDelete("DeleteUser/{userId}")] 

        public IActionResult DeleteUser(int userId) {
            User? userDb = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault<User>();

                
            if(userDb !=null){
                
                _entityFramework.Users.Remove(userDb);
            }
            if(_entityFramework.SaveChanges() > 0){
                return Ok();
            }

            throw new Exception("Failed to delete user");
        }

    }

    
