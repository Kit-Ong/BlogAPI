using System.Text.Json.Serialization;

namespace BlogAPI.Models
{
    public class Blog
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("author_id")]
        public int AuthorId { get; set; }

        [JsonPropertyName("published_on")]
        public DateTime PublishedOn { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("created_on")]
        public DateTime CreatedOn { get; set; }
    }
}
