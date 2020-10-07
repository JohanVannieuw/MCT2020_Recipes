using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Recipes_DB.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Recipes.IntegrationTests
{
    [TestClass]
    public class CategoriesIntegrationTests
    {

        private HttpClient _client;

        public CategoriesIntegrationTests()
        {
            TestFixture fixture = new TestFixture();
            _client = fixture.Client;
        }

        [TestMethod]
        public async Task ClientGETcategories_returns_categories()
        {
            //ARRANGE: Het endpoint of route van de controller actie.
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/categories");
            request.Headers.Add("Accept", "application/json");
            var httpResponse = await _client.SendAsync(request);
            //Console.WriteLine($"Sending Request {request}");

            //ACT: Request en Deserialize naar een string -> naar JSON.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var result =
              JsonConvert.DeserializeObject<List<CategoryDTO>>(stringResponse);

            //ASSERT ( null – type - statuscode – inhoud) 
            Assert.IsNotNull(result); //null
            Assert.AreEqual(httpResponse.Content.Headers.ContentType.MediaType, "application/json"); //type response
            httpResponse.EnsureSuccessStatusCode();  // 200 – 299
            Assert.AreEqual(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.AreEqual(result.Count, 7); //inhoud
            Assert.IsTrue(stringResponse.Contains("Soup"));
            Assert.IsFalse(stringResponse.Contains("abcdefg"));
            }
    }

}
