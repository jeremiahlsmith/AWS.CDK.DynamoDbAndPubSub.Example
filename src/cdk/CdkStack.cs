using Amazon.CDK;
using Amazon.CDK.AWS;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.SNS;
using Amazon.CDK.AWS.SNS.Subscriptions;
using Amazon.CDK.AWS.APIGateway;

namespace cdk
{
    public class CdkStack : Stack
    {
        internal CdkStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // The code that defines your stack goes here


            //DynamoDB Table
            new Table(this, "sample-table", new TableProps
            {
                TableName = "sample-table",
                PartitionKey = new Attribute { Name = "id", Type = AttributeType.STRING }
            });

            //DynamoDb Lambda
            var dynamoDbLambda = new Function(this, "dynamoDbLambda", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Code = Code.FromAsset("src/DynamoDbLambda/bin/Debug/netcoreapp3.1/publish"),
                Handler = "DynamoDbLambda::DynamoDbLambda.Functions::Get"
            });

            new LambdaRestApi(this, "dynamoDbLambdaApiEndpoint", new LambdaRestApiProps
            {
                Handler = dynamoDbLambda
            });

            //Publisher Lambda
            var snsPublisher = new Function(this, "snsPublisher", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Code = Code.FromAsset("src/SnsPublisher/bin/Debug/netcoreapp3.1/publish"),
                Handler = "SnsPublisherLambda::SnsPublisherLambda.Functions::Get"
            });

            new LambdaRestApi(this, "snsPublisherApiEndpoint", new LambdaRestApiProps
            {
                Handler = snsPublisher
            });

            //Subscriber Lambda
            var snsSubscriber = new Function(this, "snsSubscriber", new FunctionProps
            {
                Runtime = Runtime.DOTNET_CORE_3_1,
                Code = Code.FromAsset("src/SnsSubscriber/bin/Debug/netcoreapp3.1/publish"),
                Handler = "SnsSubscriber::SnsSubscriber.Functions::Get"
            });

            //SNS
            var topic = new Amazon.CDK.AWS.SNS.Topic(this, "sample-topic", new TopicProps
            {
                TopicName = "sample-topic"
            });

            //Subscription
            topic.AddSubscription(new LambdaSubscription(snsSubscriber));
        }
    }
}