using System.Net;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2;
using System;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DynamoDbLambda
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
        public APIGatewayProxyResponse Get(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.Log("Get Request\n");
            
            var dynamoDbClient = new AmazonDynamoDBClient("test", "test", new AmazonDynamoDBConfig
            {                
                ServiceURL = $@"http://{Environment.GetEnvironmentVariable("LOCALSTACK_HOSTNAME")}:{Environment.GetEnvironmentVariable("EDGE_PORT")}"
            });

            var dynamoDbSampleTable = Table.LoadTable(dynamoDbClient, "sample-table");

            var documentValues = new Dictionary<string, DynamoDBEntry> {
                { "id", "123" },
                { "FirstName", "John" },
                { "LastName", "Smith" }
            };

            var result = dynamoDbSampleTable.UpdateItemAsync(new Document(documentValues)).GetAwaiter().GetResult();

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };

            return response;
        }
    }
}