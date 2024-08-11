namespace UrlShortner_AWSLambdaWithPostgreSQLDb.Models
{
    public class ShortenUrlRequest
    {
        public string Url { get; set; } = string.Empty;
    }
}
