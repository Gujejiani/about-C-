

using DotnetAPI.Models;

using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

    [ApiController]
    [Route("[controller]")]
   public class UserSalaryController: ControllerBase
    {
        DataContextDapper _dapper;

        public UserSalaryController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("getAllSalaries")]
        public UserSalary[]  AllSalaries()
        {
            string sql = @" SELECT 
                             
                               [Salary],
                               [AvgSalary]
                             
                               FROM TutorialAppSchema.UserSalary
                 ";

            UserSalary[] response = _dapper.LoadData<UserSalary>(sql).ToArray();

            return response;

        }

        [HttpGet("getSingleSalary/{userId}")]

        public UserSalary GetSingleSalary(int userId)
        {

            UserSalary response = _dapper.LoadDataSingle<UserSalary>($"SELECT * FROM TutorialAppSchema.UserSalary WHERE UserId = {userId}");

            return response;
        }

        [HttpPost("AddSalary")] 
        public IActionResult AddSalary(UserSalary userSalary)
        {
            string sql = @"
               INSERT INTO TutorialAppSchema.UserSalary(
                    [Salary],
                    [AvgSalary]
                    ) VALUES (" +
                "'" + userSalary.Salary +
                "', '" + userSalary.AvgSalary +
                "')";

            Console.WriteLine(sql);

            if (_dapper.executeSql(sql))
            {
                      return Ok();
            }
            throw new Exception("Failed to add salary");
        }

        [HttpPut("EditSalary")]
        public IActionResult EditSalary(UserSalary userSalary)
        {
            string sql = @"
                UPDATE TutorialAppSchema.UserSalary 
                SET [Salary]='" + userSalary.Salary +
                "', [AvgSalary]='" + userSalary.AvgSalary +
                "' WHERE UserId = " + userSalary.UserId;

            Console.WriteLine(sql);
            if (_dapper.executeSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to update salary");
        }
    }