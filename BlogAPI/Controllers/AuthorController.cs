using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using BlogAPI.Data;
using BlogAPI.Models;
using AuthorAPI.Models;

namespace AuthorAPI.Controllers
{
    [Route("api/v1/author")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private IDatabaseAdapter _database;

        public AuthorController(IDatabaseAdapter database)
        {
            _database = database;
        }

        [HttpGet]
        [Authorize]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "No authors found")]
        public async Task<IActionResult> AuthorList()
        {
            var authorList = await _database.GetAllAuthors();
            if (authorList.Count == 0)
            {
                return NoContent();
            }

            return Ok(authorList);
        }

        /// <summary>
        /// Adds a new author
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/author/add
        ///     {
        ///         "first_name": "John",
        ///         "last_name": "Smith"
        ///     }
        /// </remarks>
        /// <param name="author">The author to add.</param>
        /// <response code="400">There is a problem with the blog data</response>
        /// <response code="500">The blog is valid but it cannot be added at this time</response>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("add")]
        public async Task<IActionResult> AddAuthor(Author author)
        {
            var transactionResult = await _database.AddAuthor(author);
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
