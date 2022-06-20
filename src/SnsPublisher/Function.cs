using System.Net;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SnsPublisherLambda
{
    public class Functions
    {
        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public Functions()
        {
        }


        /// <summary>
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The API Gateway response.</returns>
        public async Task<APIGatewayProxyResponse> Get(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.Log("Get Request\n");

            var client = new AmazonSimpleNotificationServiceClient("test", "test", new AmazonSimpleNotificationServiceConfig
            {
                ServiceURL = $@"http://{Environment.GetEnvironmentVariable("LOCALSTACK_HOSTNAME")}:{Environment.GetEnvironmentVariable("EDGE_PORT")}"
            });

            var publishRequest = new PublishRequest("arn:aws:sns:us-east-1:000000000000:sample-topic", "test");

            try
            {
                var publishResponse = await client.PublishAsync(publishRequest);
                context.Logger.Log($"Publish Response HTTP Status Code: {publishResponse.HttpStatusCode}");
            }
            catch (Exception ex)
            {
                context.Logger.Log(ex.Message + " " + ex.StackTrace);
            }

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };

            return response;
        }
    }
}