

using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controller;

    
        [ApiController]
        [Route("[controller]")]

        public class UserJobInfoController: ControllerBase {
              DataContextDapper _dapper;

             public  UserJobInfoController(IConfiguration config) {
                  _dapper = new DataContextDapper(config);
             }



            [HttpGet("getAllJobInfo")]
            public UserJobInfo[] AllJobInfo(){

                string sql = @" SELECT 
                               [UserId],
                               [JobTitle],
                               [Department]
                               FROM TutorialAppSchema.UserJobInfo
                 ";   

                UserJobInfo[] response = _dapper.LoadData<UserJobInfo>(sql).ToArray();  

                 return response; 

            }

            [HttpGet("getSingleJobInfo/{userId}")]
             public UserJobInfo GetSingleJobInfo(int userId){
                    
                     UserJobInfo response = _dapper.LoadDataSingle<UserJobInfo>($"SELECT * FROM TutorialAppSchema.UserJobInfo WHERE UserId = {userId}");
    
                     return response; 
             }  

            [HttpPost("AddJobInfo")]
            public IActionResult AddJobInfo(CreateUserJobInfo userJobInfo){
                string sql = @"
               INSERT INTO TutorialAppSchema.UserJobInfo(
                    [JobTitle],
                    [Department]
                    ) VALUES (" +
                "'" + userJobInfo.JobTitle +
                "', '" + userJobInfo.Department +
                "')";            

                Console.WriteLine(sql);   
               
                if(_dapper.executeSql(sql)){
                    return Ok();
                }
                
                throw new Exception("Failed to add job info");
            }  

            [HttpPut("EditJobInfo")]
            public IActionResult EditJobInfo(UserJobInfo userJobInfo) {
                string sql = @"
                    UPDATE TutorialAppSchema.UserJobInfo 
                    SET [JobTitle]='" + userJobInfo.JobTitle +
                    "', [Department]='" + userJobInfo.Department +
                    "' WHERE UserId = " + userJobInfo.UserId;

                Console.WriteLine(sql);
                if(_dapper.executeSql(sql)){
                    return Ok();
                }
                
                throw new Exception("Failed to update job info");
            }

        }

    