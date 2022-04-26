using CASIO_HariKrishna.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace CASIO_HariKrishna
{
    public interface ITokenService
    {
        Task<Token> GetElibilityToken();
    }

    public class TokenService : ITokenService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TokenService> _logger;
        public TokenService(IHttpClientFactory httpClientFactory, ILogger<TokenService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<Token> GetElibilityToken()
        {
            Token token = new Token();
            try
            {
                string baseAddress = @"https://cisoc.com/oauth2/token";

                string grant_type = "client_credentials";
                string client_id = "cisoc-clientId";
                string client_secret = "cisoc-password";

                var form = new Dictionary<string, string>
                {
                    {"grant_type", grant_type},
                    {"client_id", client_id},
                    {"client_secret", client_secret},
                };
                var httpCLient = _httpClientFactory.CreateClient();
                HttpResponseMessage tokenResponse = await httpCLient.PostAsync(baseAddress, new FormUrlEncodedContent(form));
                if (tokenResponse.IsSuccessStatusCode)
                {
                    var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
                    token = JsonConvert.DeserializeObject<Token>(jsonContent);
                }

            }
            catch (System.Exception ex)
            {
                _logger.LogInformation("TokenService | GetElibilityToken | Error while fetching the token from end point  : " + ex.Message);
                throw;
            }
            return token;
        }
    }
}
