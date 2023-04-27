using MongoDB.Bson;
using MongoDB.Driver;
using BlogAPI.Models;
using AuthorAPI.Models;
using MongoDB.Driver.Linq;

namespace BlogAPI.Data
{
    public class MongoDbDatabase : IDatabaseAdapter
    {
        private readonly IConfiguration _configuration;
        public MongoDbDatabase(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private IMongoCollection<BsonDocument> GetCollection(
            string databaseName, string collectionName)
        {
            var client = new MongoClient();
            var database = client.GetDatabase(databaseName);
            var collection = database.GetCollection<BsonDocument>(collectionName);
            return collection;
        }

        private Blog ConvertBsonToBlog(BsonDocument document)
        {
            if (document == null) return null;

            return new Blog
            {
                Id = document["id"].AsString,
                AuthorId = document["author_id"].AsInt32,
                PublishedOn = document["published_on"].AsBsonDateTime.ToUniversalTime(),
                Text = document["text"].AsString,
                CreatedOn = document["created_on"].AsBsonDateTime.ToUniversalTime()
            };
        }
        private Author ConvertBsonToAuthor(BsonDocument document)
        {
            if (document == null) return null;

            return new Author
            {
                Id = document["id"].AsInt32,
                FirstName = document["first_name"].AsString,
                LastName = document["last_name"].AsString
            };
        }

        public async Task<List<Blog>> GetAllBlogs()
        {
            var collection = GetCollection(_configuration["databaseName"], _configuration["collectionBlogsName"]);
            var documents = collection.Find(_ => true).ToListAsync();

            var blogList = new List<Blog>();

            if (documents == null) return blogList;

            foreach (var document in await documents)
            {
                blogList.Add(ConvertBsonToBlog(document));
            }

            return blogList;
        }

        public async Task<Blog> GetBlogById(string id)
        {
            var collection = GetCollection(_configuration["databaseName"], _configuration["collectionBlogsName"]);
            var blogCursor = await collection.FindAsync(
                Builders<BsonDocument>.Filter.Eq("id", id));
            var document = blogCursor.FirstOrDefault();
            var blog = ConvertBsonToBlog(document);

            if (blog == null)
            {
                return new Blog();
            }

            return blog;
        }

        public async Task<List<Author>> GetAllAuthors()
        {
            var collection = GetCollection(_configuration["databaseName"], _configuration["collectionAuthorsName"]);
            var documents = collection.Find(_ => true).ToListAsync();

            var authorList = new List<Author>();

            if (documents == null) return authorList;

            foreach (var document in await documents)
            {
                authorList.Add(ConvertBsonToAuthor(document));
            }

            return authorList;
        }


        public async Task<TransactionResult> AddBlog(Blog blog)
        {
            var collection = GetCollection(_configuration["databaseName"], _configuration["collectionBlogsName"]);
            var collectionAuthors = GetCollection(_configuration["databaseName"], _configuration["collectionAuthorsName"]);
            var blogCursor = await collectionAuthors.FindAsync(
                Builders<BsonDocument>.Filter.Eq("id", blog.AuthorId));
            var documentAuthor = blogCursor.FirstOrDefault();
            if (documentAuthor == null)
            {
                return TransactionResult.BadRequest;
            }

            var document = new BsonDocument
            {
                {"id", Guid.NewGuid().ToString("N") },
                {"author_id", blog.AuthorId },
                {"published_on", blog.PublishedOn },
                {"text", blog.Text },
                {"created_on", blog.CreatedOn }
            };

            try
            {
                await collection.InsertOneAsync(document);
                if (document["_id"].IsObjectId)
                {
                    return TransactionResult.Success;
                }

                return TransactionResult.BadRequest;
            }
            catch
            {
                return TransactionResult.ServerError;
            }

        }
        public async Task<TransactionResult> AddAuthor(Author author)
        {
            var collection = GetCollection(_configuration["databaseName"], _configuration["collectionAuthorsName"]);
            var builder = Builders<BsonDocument>.Sort;
            var sort = builder.Descending("id");
            var CursorToResults = collection.Find<BsonDocument>(new BsonDocument()).Sort(sort);
            var RecordwithMax_id_Value = await CursorToResults.FirstOrDefaultAsync();
            var newAuthorId = 0;
            if (RecordwithMax_id_Value != null)
            {
                newAuthorId = RecordwithMax_id_Value["id"].AsInt32 + 1;
            }

            var document = new BsonDocument
            {
                {"id", newAuthorId },
                {"first_name", author.FirstName },
                {"last_name", author.LastName }
            };

            try
            {
                await collection.InsertOneAsync(document);
                if (document["_id"].IsObjectId)
                {
                    return TransactionResult.Success;
                }

                return TransactionResult.BadRequest;
            }
            catch
            {
                return TransactionResult.ServerError;
            }

        }

    }
}
