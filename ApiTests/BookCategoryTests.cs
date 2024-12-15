using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace ApiTests
{
    [TestFixture]
    public class BookCategoryTests : IDisposable
    {
        private RestClient client;
        private string token;

        [SetUp]
        public void Setup()
        {
            client = new RestClient(GlobalConstants.BaseUrl);
            token = GlobalConstants.AuthenticateUser("john.doe@example.com", "password123");

            Assert.That(token, Is.Not.Null.Or.Empty, "Authentication token should not be null or empty");
        }

        [Test]
        public void Test_BookCategoryLifecycle()
        {
            
                // Step 1: Create a new book category
                var createRequest = new RestRequest("category", Method.Post);
                createRequest.AddHeader("Authorization", $"Bearer {token}");
                createRequest.AddJsonBody(new { title = "Fictional Literature" });

                var createResponse = client.Execute(createRequest);
                Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected status code is 200 OK");

                var createContent = JObject.Parse(createResponse.Content);
                var categoryId = createContent["_id"].ToString();
                Assert.That(categoryId, Is.Not.Null.Or.Empty, "Category ID should not be null or empty");
                Assert.That(createContent["title"].ToString(), Is.EqualTo("Fictional Literature"), "Title should match the input value");

                // Step 2: Retrieve all book categories
                var getAllRequest = new RestRequest("category", Method.Get);
                getAllRequest.AddHeader("Authorization", $"Bearer {token}");

                var getAllResponse = client.Execute(getAllRequest);
                Assert.That(getAllResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected status code is 200 OK");
                Assert.That(getAllResponse.Content, Is.Not.Null.Or.Empty, "Response content should not be empty");

                var allCategories = JArray.Parse(getAllResponse.Content);
                Assert.That(allCategories.Count, Is.GreaterThan(0), "The response should be a JSON array with at least one category");

                var newCategory = allCategories.FirstOrDefault(cat => cat["_id"].ToString() == categoryId);
                Assert.That(newCategory, Is.Not.Null, "The new category should be present in the list");

                // Step 3: Update the category title
                var updateRequest = new RestRequest($"category/{categoryId}", Method.Put);
                updateRequest.AddHeader("Authorization", $"Bearer {token}");
                updateRequest.AddJsonBody(new { title = "Updated Fictional Literature" });

                var updateResponse = client.Execute(updateRequest);
                Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected status code is 200 OK");

                // Step 4: Verify the updated category
                var getUpdatedRequest = new RestRequest($"category/{categoryId}", Method.Get);
                getUpdatedRequest.AddHeader("Authorization", $"Bearer {token}");

                var getUpdatedResponse = client.Execute(getUpdatedRequest);
                Assert.That(getUpdatedResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected status code is 200 OK");
                Assert.That(getUpdatedResponse.Content, Is.Not.Null.Or.Empty, "Response content should not be empty");

                var updatedCategory = JObject.Parse(getUpdatedResponse.Content);
                Assert.That(updatedCategory["title"].ToString(), Is.EqualTo("Updated Fictional Literature"), "The title should be updated");

                // Step 5: Delete the category
                var deleteRequest = new RestRequest($"category/{categoryId}", Method.Delete);
                deleteRequest.AddHeader("Authorization", $"Bearer {token}");

                var deleteResponse = client.Execute(deleteRequest);
                Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected status code is 200 OK");

                // Step 6: Verify the deleted category cannot be found
                System.Threading.Thread.Sleep(500); // Add a delay to ensure deletion is processed
                var getDeletedRequest = new RestRequest($"category/{categoryId}", Method.Get);
                getDeletedRequest.AddHeader("Authorization", $"Bearer {token}");

                var getDeletedResponse = client.Execute(getDeletedRequest);
                Assert.That(getDeletedResponse.Content, Is.EqualTo("null"), "The response content should be null");
            }

        



        public void Dispose()
        {
            client?.Dispose();
        }
    }
}
