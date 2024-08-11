using Microsoft.EntityFrameworkCore;

namespace UrlShortner_AWSLambdaWithPostgreSQLDb.Services
{
    public class UrlShorteningService
    {
        public const int NumberOfCharactersInShortLink = 7;
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private readonly Random _random = new Random();
        private readonly ApplicationDbContext _dbContext;

        public UrlShorteningService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GenerateUniqueCode()
        {
            var codeChars = new char[NumberOfCharactersInShortLink];
            while (true)
            {
                for (var i = 0; i < NumberOfCharactersInShortLink; i++)
                {
                    var randomIndex = _random.Next(Alphabet.Length - 1);
                    codeChars[i] = Alphabet[randomIndex];
                }
                var code = new string(codeChars);
                if (!await _dbContext.ShortenedUrl.AnyAsync(s => s.Code == code))
                { return code; }
            }
        }
    }
}
