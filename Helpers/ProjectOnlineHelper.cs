using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace groveale 
{

    public class ProjectOnlineHelper
    {
        private readonly HttpClient _httpClient;
        private readonly string _projectOnlineUrl;
        private readonly string _accessToken;

        public ProjectOnlineHelper(string projectOnlineUrl, string accessToken, DateTime accessTokenExpiration)
        {
            _httpClient = new HttpClient();
            _projectOnlineUrl = projectOnlineUrl;
            _accessToken = accessToken;

            // Set the base address of the Project Online API
            _httpClient.BaseAddress = new Uri($"{_projectOnlineUrl}/_api/ProjectData/");
        }

        public async Task<string> GetProjects()
        {
            return await GetApiData("Projects");
        }

        public async Task<string> GetProjectsBaseline()
        {
            return await GetApiData("ProjectsBaseline");
        }

        public async Task<string> GetTasks()
        {
            return await GetApiData("Tasks");
        }

        public async Task<string> GetTaskBaseline()
        {
            return await GetApiData("TaskBaseline");
        }

        public async Task<string> GetAssignments()
        {
            return await GetApiData("Assignments");
        }

        public async Task<string> GetAssignmentBaseline()
        {
            return await GetApiData("AssignmentBaseline");
        }

        public async Task<string> GetResources()
        {
            return await GetApiData("Resources");
        }

        private async Task<string> GetApiData(string apiEndpoint)
        {
            // Obtain access token using refresh token
            string accessToken = await GetAccessToken();

            // Set authorization header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Set the date for the filter
            DateTime last24Hours = DateTime.Now.AddHours(-24);

            // Build the REST API URL with the filter
            // Projects does have a `ProjectLastPublishedDate` that we could use instead
            // $"Projects?$filter=ProjectLastPublishedDate gt datetime'{last24Hours.ToString("yyyy-MM-ddTHH:mm:ss")}'";
            string apiUrl = $"{apiEndpoint}?$filter=ProjectModifiedDate gt datetime'{last24Hours.ToString("yyyy-MM-ddTHH:mm:ss")}'";

            // Make the GET request
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                // Process the response (parse JSON or other actions)
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                // Handle error
                throw new InvalidOperationException($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }

        private async Task<string> GetAccessToken()
        {
            // This code has been move to the AuthenticationHelper class
            // TODO - check expiration I think we get just over an hour

            return this._accessToken;
        }
    }
        
}
