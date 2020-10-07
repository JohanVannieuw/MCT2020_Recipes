using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Recipes_DB;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Recipes.IntegrationTests
{
    public class TestFixture : IDisposable
    {

        public HttpClient Client { get; set; }  //using .Net.Http
                                                //using MvcTesting to create test host:
        public readonly WebApplicationFactory<Recipes_DB.Startup> Factory;
        private readonly TestServer _server; //using ingebouwde TestHost = optioneel

        public TestFixture()
        {
            Factory = new WebApplicationFactory<Startup>(); //staging of productiedatabase
            Client = Factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("http://localhost:8012")
            });
        }

        public void Dispose()
        {
            Client.Dispose();
            Factory.Dispose();
        }
    }

}
