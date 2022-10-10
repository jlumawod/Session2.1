using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using System.Net;

namespace Session2_1_HW
{
    [TestClass]
    public class Pet
    {

        private static HttpClient httpClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string PetsEndpoint = "pet";

        private static string GetURL(string endpoint) => $"{BaseURL}{endpoint}";

        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<PetModel> cleanUpList = new List<PetModel>();

        


        [TestInitialize]
        public void TestInitialize()
        {
            httpClient = new HttpClient();
        }

        [TestMethod]
        public async Task UpdatePet()
        {
            
            #region Add new pet to the store
            // Create New Pet

            PetModel petData = new PetModel()
            {
                id = 2024,
                category = new Category()
                {
                    id = 0,
                    name = "Husky"
                },
                name = "TestPet",
                photoUrls = new string[]
                {
                    "zzzz",
                    "aaaa"
                },
                status = "available",
                tags = new Tag[]
                {
                    new Tag()
                    { 
                       id = 0,
                       name = "Kid Friendly"
                    }
                }                
            };

            // Serialize Content
            var request = JsonConvert.SerializeObject(petData);
            var postRequest = new StringContent(request, Encoding.UTF8, "application/json");

            // Send Post Request
            await httpClient.PostAsync(GetURL(PetsEndpoint), postRequest);
            #endregion


            #region Get name of created pet
            // Get Request
            var getResponse = await httpClient.GetAsync(GetURI($"{PetsEndpoint}/{petData.id}"));

            // Deserialize Content
            var listPetData = JsonConvert.DeserializeObject<PetModel>(getResponse.Content.ReadAsStringAsync().Result);

            // filter created data
            var createdPetName = listPetData.name;

            //Assert the new pet has been added
            Assert.AreEqual(petData.name, createdPetName, "Pet name not matching");
            #endregion

            #region Update Pet Data
            petData = new PetModel()
            {
                id = 2024,
                category = new Category()
                {
                    id = 0,
                    name = "Husky"
                },
                name = "TestPetUpdated",
                photoUrls = new string[]
                {
                    "zzzz",
                    "aaaa",
                    "bbbb"
                },
                status = "sold",
                tags = new Tag[]
                {
                    new Tag()
                    {
                       id = 0,
                       name = "Kid Friendly"
                    },
                    new Tag()
                    {
                       id = 0,
                       name = "Always Hungry"
                    }
                }
            };

            // Serialize Content
            request = JsonConvert.SerializeObject(petData);
            postRequest = new StringContent(request, Encoding.UTF8, "application/json");

            // Send Put Request
            var httpResponse = await httpClient.PutAsync(GetURL($"{PetsEndpoint}"), postRequest);

            // Get Status Code
            var statusCode = httpResponse.StatusCode;

            #endregion

            #region Get Updated Data
            // Get Request
            getResponse = await httpClient.GetAsync(GetURI($"{PetsEndpoint}/{petData.id}"));

            // Deserialize Content
            listPetData = JsonConvert.DeserializeObject<PetModel>(getResponse.Content.ReadAsStringAsync().Result);

            // filter created data
            var newCreatedPetName = listPetData.name;

            // Add data to cleanup list
            cleanUpList.Add(listPetData);

            #endregion


            #region assertion

            // Assertion
            Assert.AreEqual(HttpStatusCode.OK, statusCode, "Status code is not equal to 201");
            Assert.AreEqual(petData.name, newCreatedPetName, "Pet name not matching");

            #endregion
        }


        [TestCleanup]
        public async Task TestCleanUp()
        {
            foreach (var data in cleanUpList)
            {
                var httpResponse = await httpClient.DeleteAsync(GetURL($"{PetsEndpoint}/{data.id}"));
            }
        }
    }
}
