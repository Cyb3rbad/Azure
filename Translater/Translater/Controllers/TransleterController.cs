using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Translater.Controllers
{
    public class TRequest
    {
        public string Text { get; set; }
        public string language { get; set; }
    }

    public class Translation
    {
        public string Text { get; set; }
        public string Totext { get; set; }
    }

    public class TResponse
    {
        public TLanguage Tlang { get; set; }
        public Translation[] Translations { get; set; }
    }

    public class TLanguage
    {
        public string Language  { get; set; }
        public float Score { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class TransleterController : ControllerBase
    {
        private readonly string Endpoint =
            "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0";
        string Key = "17d62fbb989246d78f76954885a0c2ea";
        string Region = "eastus";

        [HttpPost]
        public async Task<IActionResult> TranslateText([FromBody] TRequest request)
        {
            if (string.IsNullOrEmpty(request.Text) || string.IsNullOrEmpty(request.language))
            {
                return BadRequest("Need text and language");
            }

            string endpoint = Endpoint + "&to=" + request.language;

            using (HttpClient client = new())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Key);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", Region);

                var content = new[] {
                    new { Text = request.Text }};

                var response = await client.PostAsJsonAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var translations = JsonConvert.DeserializeObject<TResponse[]>(responseString);
                    var translatedText = translations[0].Translations[0].Text;
                    return Ok(translatedText);
                }
                else
                {
                    return BadRequest("Error code: " + response.StatusCode);
                }
            }

            
        }
    }
}