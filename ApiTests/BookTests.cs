using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace ApiTests
{
    [TestFixture]
    public class BookTests : IDisposable
    {
        private RestClient client;
        private string token;
        private object _token;

        [SetUp]
        public void Setup()
        {
            client = new RestClient(GlobalConstants.BaseUrl);
            token = GlobalConstants.AuthenticateUser("john.doe@example.com", "password123");

            Assert.That(token, Is.Not.Null.Or.Empty, "Authentication token should not be null or empty");
        }

        [Test]
        public void Test_GetAllBooks()
        {
         
                var request = new RestRequest("book", Method.Get);
                request.AddHeader("Authorization", $"Bearer {token}");

                var response = client.Execute(request);

                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.Not.Null.Or.Empty, "Response content should not be empty");

                var content = JArray.Parse(response.Content);
                Assert.That(content.Count, Is.GreaterThan(0), "The JSON array should contain at least one book");

                foreach (var book in content)
                {
                    Assert.That(book["title"], Is.Not.Null.Or.Empty, "Each book's title should not be null or empty");
                    Assert.That(book["author"], Is.Not.Null.Or.Empty, "Each book's author should not be null or empty");
                    Assert.That(book["description"], Is.Not.Null.Or.Empty, "Each book's description should not be null or empty");
                    Assert.That(book["price"], Is.Not.Null.Or.Empty, "Each book's price should not be null or empty");
                    Assert.That(book["pages"], Is.Not.Null.Or.Empty, "Each book's pages should not be null or empty");
                    Assert.That(book["category"], Is.Not.Null.Or.Empty, "Each book's category should not be null or empty");
                }
            }



        [Test]
        public void Test_GetBookByTitle()
        {
         
                var client = new RestClient(GlobalConstants.BaseUrl);
                var request = new RestRequest("book?title=The%20Great%20Gatsby", Method.Get);
                request.AddHeader("Authorization", $"Bearer {_token}");

                var response = client.Execute(request);

             
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected HTTP status code 200 OK.");
                Assert.IsNotEmpty(response.Content, "Response content should not be empty.");

                var content = JArray.Parse(response.Content);
                var book = content.FirstOrDefault(b => b["title"]?.ToString() == "The Great Gatsby");
                Assert.IsNotNull(book, "Book with the title 'The Great Gatsby' should be returned in the response.");

               
                Assert.AreEqual("F. Scott Fitzgerald", book["author"]?.ToString(), "The author should be 'F. Scott Fitzgerald'.");
            }


        [Test]
        public void Test_AddBook()
        {
           
               
                var categoriesRequest = new RestRequest("category", Method.Get);
                categoriesRequest.AddHeader("Authorization", $"Bearer {token}");

                var categoriesResponse = client.Execute(categoriesRequest);
                Assert.That(categoriesResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                var categories = JArray.Parse(categoriesResponse.Content);
                var firstCategoryId = categories[0]["_id"].ToString();

                
                var request = new RestRequest("book", Method.Post);
                request.AddHeader("Authorization", $"Bearer {token}");
                request.AddJsonBody(new
                {
                    title = "New Book",
                    author = "New Author",
                    description = "New Description",
                    price = 29.99,
                    pages = 350,
                    category = firstCategoryId
                });

                var response = client.Execute(request);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.Not.Null.Or.Empty, "Response content should not be empty");

                var content = JObject.Parse(response.Content);
                var bookId = content["_id"].ToString();

                var bookRequest = new RestRequest($"book/{bookId}", Method.Get);
                bookRequest.AddHeader("Authorization", $"Bearer {token}");

                var bookResponse = client.Execute(bookRequest);
                Assert.That(bookResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(bookResponse.Content, Is.Not.Null.Or.Empty, "Response content should not be empty");

                var bookContent = JObject.Parse(bookResponse.Content);
                Assert.That(bookContent["title"].ToString(), Is.EqualTo("New Book"));
                Assert.That(bookContent["author"].ToString(), Is.EqualTo("New Author"));
                Assert.That(bookContent["description"].ToString(), Is.EqualTo("New Description"));
                Assert.That(bookContent["price"].ToString(), Is.EqualTo("29.99"));
                Assert.That(bookContent["pages"].ToString(), Is.EqualTo("350"));
                Assert.That(bookContent["category"]["_id"].ToString(), Is.EqualTo(firstCategoryId));
            }


        [Test]
        public void Test_UpdateBook()
        {
            
                
                var getAllBooksRequest = new RestRequest("book", Method.Get);
                getAllBooksRequest.AddHeader("Authorization", $"Bearer {token}");

                var getAllBooksResponse = client.Execute(getAllBooksRequest);
                Assert.That(getAllBooksResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(getAllBooksResponse.Content, Is.Not.Null.Or.Empty, "Response content should not be empty");

                var books = JArray.Parse(getAllBooksResponse.Content);
                var book = books.FirstOrDefault(b => b["title"].ToString() == "The Catcher in the Rye");

                Assert.That(book, Is.Not.Null, "The book with the title 'The Catcher in the Rye' should exist in the response");

                var bookId = book["_id"].ToString();

                
                var request = new RestRequest($"book/{bookId}", Method.Put);
                request.AddHeader("Authorization", $"Bearer {token}");
                request.AddJsonBody(new
                {
                    title = "Updated Book Title",
                    author = "Updated Author"
                });

                var response = client.Execute(request);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.Not.Null.Or.Empty, "Response content should not be empty");

                var content = JObject.Parse(response.Content);
                Assert.That(content["title"].ToString(), Is.EqualTo("Updated Book Title"));
                Assert.That(content["author"].ToString(), Is.EqualTo("Updated Author"));
            }


        [Test]
        public void Test_DeleteBook()
        {
          
               
                var getAllBooksRequest = new RestRequest("book", Method.Get);
                getAllBooksRequest.AddHeader("Authorization", $"Bearer {token}");

                var getAllBooksResponse = client.Execute(getAllBooksRequest);
                Assert.That(getAllBooksResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(getAllBooksResponse.Content, Is.Not.Null.Or.Empty, "Response content should not be empty");

                var books = JArray.Parse(getAllBooksResponse.Content);
                var book = books.FirstOrDefault(b => b["title"].ToString() == "To Kill a Mockingbird");

                Assert.That(book, Is.Not.Null, "The book with the title 'To Kill a Mockingbird' should exist in the response");

                var bookId = book["_id"].ToString();

             
                var request = new RestRequest($"book/{bookId}", Method.Delete);
                request.AddHeader("Authorization", $"Bearer {token}");

                var response = client.Execute(request);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

               
                var deletedBookRequest = new RestRequest($"book/{bookId}", Method.Get);
                deletedBookRequest.AddHeader("Authorization", $"Bearer {token}");

                var deletedBookResponse = client.Execute(deletedBookRequest);
                Assert.That(deletedBookResponse.Content, Is.EqualTo("null"), "Response content should be 'null'");
            }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}
