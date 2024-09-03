

using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace  DotnetAPI.Controllers;


    [Authorize]
    [ApiController]
    [Route("[controller]")]

    public class PostController: ControllerBase {


        private readonly DataContextDapper _dapper;

        public PostController(IConfiguration config) {

            _dapper = new DataContextDapper(config);

        }

    [HttpGet("Posts")]
    public IEnumerable<Post> GetPosts(){

        string sql = @"
        SELECT [postId],
            [userId],
            [postTitle],
            [postContent],
            [postCreated],
            [postUpdated] FROM TutorialAppSchema.Posts";


            return _dapper.LoadData<Post>(sql).ToArray();

    }

    [HttpGet("PostByUser/{userId}")]
    public Post getPostByUser(int userId){

          string id = this.User.FindFirst("userId").Value;

        string sql = $"SELECT * FROM TutorialAppSchema.Posts WHERE userId = {id}";

        Post response = _dapper.LoadDataSingle<Post>(sql);

        return response;

    }

   [HttpGet("MyPosts")]
public IEnumerable<Post> GetMyPosts()
{
    string userId = this.User.FindFirst("userId").Value;

    // Use a parameterized query to avoid SQL injection
    string sql = $"SELECT * FROM TutorialAppSchema.Posts WHERE userId = {userId}";

    // Load multiple posts instead of a single post
    IEnumerable<Post> response = _dapper.LoadData<Post>(sql);

    return response;
}



    [HttpPost("Post")]

    public IActionResult AddPost(PostToAddDto postToAddDto){

       string sql = @"
    INSERT INTO TutorialAppSchema.Posts (
        [userId],
        [postTitle],
        [postContent],
        [postCreated],
        [postUpdated]
    ) VALUES (
        '" + this.User.FindFirst("userId").Value + @"', 
        '" + postToAddDto.PostTitle + @"', 
        '" + postToAddDto.PostContent + @"', 
        GetDate(), 
        GetDate()
    )";


        if(_dapper.executeSql(sql)){
            return Ok();
        }

        throw new Exception("Failed to add post");


    }

     [HttpPut("Post")]

    public IActionResult EditPost(PostToEditDto postToEditDto){

        string sql = @"
      UPDATE TutorialAppSchema.Posts 
      SET postContent = '" +  postToEditDto.PostTitle + 
      "', postTitle ='" + postToEditDto.PostTitle + 
      @"', postUpdated = GETDATE(),
       WHERE postId = " + postToEditDto.PostId.ToString()  + 
       "And userId = " + this.User.FindFirst("userId").Value;
       ;
         //+ this.User.FindFirst("userId").Value 
       

        if(_dapper.executeSql(sql)){
            return Ok();
        }

        throw new Exception("Failed to edit post");


    }

    [HttpDelete("Post/{postId}")]
    public IActionResult DeletePost(int postId){

        string sql = @"
        DELETE FROM TutorialAppSchema.Posts WHERE postId = " + postId.ToString() + 
        "And userId = " + this.User.FindFirst("userId").Value;

        if(_dapper.executeSql(sql)){
            return Ok();
        }

            throw new Exception("Failed to delete post");
        }

    }