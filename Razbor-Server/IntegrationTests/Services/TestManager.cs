using IntegrationTests.Extensions;
using IntegrationTests.Models;
using RequestBodies.Authentication;
using Responses.Responses.Authentication;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace IntegrationTests.Services
{
    public class TestManager : IAsyncDisposable
    {
        private readonly HttpClient _httpClient;

        private TestUser? defaultUser;

        private List<TestUser> testUsers;
         
        public TestManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
            testUsers = new List<TestUser>();
        }

        public async Task<TestUser> GetDefaultTestUser()
        {
             if  (defaultUser != null)
                return defaultUser;

            defaultUser = await CreateTestUser();
            return defaultUser;
        }

        public async Task<TestUser> CreateTestUser(RegisterRequest userRequest)
        {
            var response =
                await _httpClient.SendPostAsync("/register", userRequest);

            var responseContent = await response.GetObject<RegisterResponse>();

            var user = new TestUser
            {
                Username = userRequest.Username,
                Password = userRequest.Password,
                Token = responseContent.Token
            };

            testUsers.Add(user);
            return user;
        }

        public Task<TestUser> CreateTestUser(string username, string password)
            => CreateTestUser(GenerateRegisterRequest(username, password));

        public Task<TestUser> CreateTestUser() =>
            CreateTestUser(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString());

        public RegisterRequest GenerateRegisterRequest() =>
            GenerateRegisterRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        public RegisterRequest GenerateRegisterRequest(string username, string password) =>
            new RegisterRequest
            {
                Username = username,
                Password = password,
                Email = $"{Random.Shared.Next(0,10000)}@test.tester",
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                UserGender = Database.Enums.Gender.Other,
                Birthday = DateTime.Now,
                Height = (double)Random.Shared.Next(150,200) / 10,
                Weight = (double)Random.Shared.Next(0,1000) / 10,
                SelectedSong = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                PrimaryPicture = Guid.NewGuid().ToString()
            };

        public async ValueTask DisposeAsync()
        {
            foreach (var user in testUsers)
            {
                var response = await _httpClient.SendDeleteAsync("/delete", user.Token);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
