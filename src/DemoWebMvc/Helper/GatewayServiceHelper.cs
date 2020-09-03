using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DemoWebMvc.Helper
{
    public class GatewayServiceHelper : IServiceHelper
    {
        private readonly IConfiguration _configuration;

        public GatewayServiceHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetOrderAsync(string accessToken)
        {
            var Client = new RestClient(_configuration["ApiGatewayAddress"]);
            var request = new RestRequest("/orders", Method.GET);
            request.AddHeader("Authorization", "Bearer " + accessToken);

            var response = await Client.ExecuteAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return response.StatusCode + " " + response.Content;
            }
            return response.Content;
        }

        public async Task<string> GetProductAsync(string accessToken)
        {
            var Client = new RestClient(_configuration["ApiGatewayAddress"]);
            var request = new RestRequest("/projects", Method.GET);
            request.AddHeader("Authorization", "Bearer " + accessToken);

            var response = await Client.ExecuteAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return response.StatusCode + " " + response.Content;
            }
            return response.Content;
        }

        public void GetServices()
        {
            throw new NotImplementedException();
        }
    }
}
