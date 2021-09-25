using Cosmos.GraphQL.Service.configurations;
using Cosmos.GraphQL.Service.Controllers;
using Cosmos.GraphQL.Service.Resolvers;
using Cosmos.GraphQL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.GraphQL.Service.Tests
{
    [TestClass, TestCategory(TestCategory.Cosmos)]
    public class SchemaTests
    {
        [TestMethod]
        public Task TestAddSchemaAsync()
        {
            TestHelper.LoadConfig();
            CosmosClientProvider clientProvider = new CosmosClientProvider(TestHelper.DataGatewayConfig);
            IMetadataStoreProvider metadataStoreProvider = new CachedMetadataStoreProvider(new DocumentMetadataStoreProvider(clientProvider));
            CosmosQueryEngine queryEngine = new CosmosQueryEngine(clientProvider, metadataStoreProvider);
            CosmosMutationEngine mutationEngine = new CosmosMutationEngine(clientProvider, metadataStoreProvider);
            GraphQLService graphQLService = new GraphQLService(queryEngine, mutationEngine, metadataStoreProvider, TestHelper.DataGatewayConfig);

            var graphql_schema = TestHelper.GraphQLTestSchema;
            var request = new HttpRequestMessage();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(graphql_schema));
            request.Method = HttpMethod.Post;
            request.Content = new StringContent(graphql_schema);
            var httpContext = new DefaultHttpContext()
            {
                Request = { Body = stream, ContentLength = stream.Length }
            };

            var controller = new GraphQLController(null, queryEngine, mutationEngine, graphQLService)
            {
                //Request = request;
            };


            // Add scehma
            controller.ControllerContext.HttpContext = httpContext;
            controller.Schema();
            return Task.CompletedTask;
        }
    }
}
