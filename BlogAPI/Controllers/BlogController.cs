using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using BlogAPI.Data;
using BlogAPI.Models;

namespace BlogAPI.Controllers
{
    [Route("api/v1/blog")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private IDatabaseAdapter _database;

        public BlogController(IDatabaseAdapter database)
        {
            _database = database;
        }

        [HttpGet]
        [Authorize]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "No blogs found")]
        public async Task<IActionResult> BlogList()
        {
            var blogList = await _database.GetAllBlogs();
            if (blogList.Count == 0)
            {
                return NoContent();
            }

            return Ok(blogList);
        }

        [HttpGet]
        [Authorize]
        [Route("{id}")]
        public async Task<IActionResult> GetBlogById(string id)
        {
            var blog = await _database.GetBlogById(id);
            if (blog.Id != id)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            return Ok(blog);
        }

        /// <summary>
        /// Adds a new blog
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/blog/add
        ///     {
        ///         "author_id": "1",
        ///         "published_on": "2022-07-08T03:49:45Z",
        ///         "text": "KBXA JOH J46 DMDUP J46 KNZY",
        ///         "created_on": "2022-07-08T00:26:45Z"
        ///     }
        /// </remarks>
        /// <param name="blog">The blog to add.</param>
        /// <response code="400">There is a problem with the blog data</response>
        /// <response code="500">The blog is valid but it cannot be added at this time</response>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("add")]
        public async Task<IActionResult> AddBlog(Blog blog)
        {
            var transactionResult = await _database.AddBlog(blog);
            switch (transactionResult)
            {
                case TransactionResult.Success:
                    return Ok();
                case TransactionResult.BadRequest:
                    return StatusCode(StatusCodes.Status400BadRequest);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
