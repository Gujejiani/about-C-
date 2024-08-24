using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controller;

    [ApiController]
    [Route("[controller]")]
    public class UserEFController : ControllerBase
    {
         DataContextEF _entityFramework;
         IMapper _mapper;

         IUserRepository _userRepository;
       public UserEFController(IConfiguration config, IUserRepository userRepository) {

            _userRepository = userRepository;

            _entityFramework = new DataContextEF(config);
            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<UserAddDto, User>()));

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
            if(_userRepository.SaveChanges()){
                return Ok();
            }

            throw new Exception("Failed to update user");

          
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(UserAddDto user) {
            User userDb = _mapper.Map<User>(user);


                // userDb.FirstName = user.FirstName;
                // userDb.LastName = user.LastName;
                // userDb.Email = user.Email;
                // userDb.Active = user.Active;
                //   userDb.Gender = user.Gender;
            _userRepository.AddEntity<User>(userDb);
            if(_userRepository.SaveChanges()){
                return Ok();
            }

            
             throw new Exception("Failed to create user");
        }


        [HttpDelete("DeleteUser/{userId}")] 

        public IActionResult DeleteUser(int userId) {
            User? userDb = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault<User>();

                
            if(userDb !=null){
                
               _userRepository.RemoveEntity(userDb);
            }
            if(_userRepository.SaveChanges()){
                return Ok();
            }

            throw new Exception("Failed to delete user");
        }

    }

    
