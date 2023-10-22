using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using RestSharp;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;

namespace RestSharpAPITests
{
    public class RestSharpAPI_Tests
    {
        private RestClient client;
        private object responce;
        private const string baseUrl = "https://contactbook.damiant4.repl.co/api";

        [SetUp]
        public void Setup()
        {
            this.client = new RestClient(baseUrl);
        }

        [Test]
        public void Test_Get_List_Contacts_Check_First_name()
        {
            // Arrange
            var request = new RestRequest("contacts", Method.Get);

            // Act
            var response = this.client.Execute(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var tasks = JsonSerializer.Deserialize<List<Contacts>>(response.Content);
            Assert.That(tasks[0].firstName, Is.EqualTo("Steve"));
            Assert.That(tasks[0].lastName, Is.EqualTo("Jobs"));

        }        


        [Test]
        public void Test_Serch_By_Keyword_Valid_Results_firstName()
        {
            // Arrange
            var request = new RestRequest("contacts/search/albert", Method.Get);

            // Act
            var response = this.client.Execute(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var tasks = JsonSerializer.Deserialize<List<Contacts>>(response.Content);
            Assert.That(tasks[0].firstName, Is.EqualTo("Albert"));
            Assert.That(tasks[0].lastName, Is.EqualTo("Einstein"));
        }

        [Test]
        public void Test_Serch_By_Keyword_Invalid_Results()
        {
            // Arrange
            var request = new RestRequest("contacts/search/missing", Method.Get);

            // Act
            var response = this.client.Execute(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.EqualTo("[]"));
        }


        [Test]
        public void Test_Try_To_Create_Contact_Missing_firstName()
        {
            // Arrange
            var request = new RestRequest("contacts", Method.Post);
            var reqBody = new
            {
                email = "some email",
                phone = "+41 44 634 49 09"
            };
            request.AddBody(reqBody);

            // Act
            var response = this.client.Execute(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Is.EqualTo("{\"errMsg\":\"First name cannot be empty!\"}"));
        }



        [Test]
        public void Test_CreateTask_Valid_Contact()
        {
            // Arrange
            var request = new RestRequest("/contacts", Method.Post);
            var reqBody = new
            {
                firstName = "Damian",
                lastName = "Tocev",
                email = "marie67@gmail.com",
                phone = "+1 800 200 300",
                comments = "Old friend",
            };
            request.AddBody(reqBody);

            // Act
            var response = this.client.Execute(request);
            //var taskObject1 = JsonSerializer.Deserialize<taskObject>(response.Content);
            var taskObject1 = JsonSerializer.Deserialize<taskObject>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(taskObject1.contact.id, Is.GreaterThan(0));
            Assert.That(taskObject1.contact.firstName, Is.EqualTo(reqBody.firstName));
            Assert.That(taskObject1.contact.lastName, Is.EqualTo(reqBody.lastName));
            Assert.That(taskObject1.contact.email, Is.EqualTo(reqBody.email));
            Assert.That(taskObject1.contact.phone, Is.EqualTo(reqBody.phone));
            Assert.That(taskObject1.contact.dateCreated, Is.Not.Empty);
            Assert.That(taskObject1.contact.comments, Is.EqualTo(reqBody.comments));
        }
    }
}