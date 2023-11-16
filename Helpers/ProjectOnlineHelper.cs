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
        private readonly DateTime _accessTokenExpiration;
        private readonly bool _debug;

        public ProjectOnlineHelper(string projectOnlineUrl, string accessToken, DateTime accessTokenExpiration, bool debug = false)
        {
            _httpClient = new HttpClient();
            _projectOnlineUrl = projectOnlineUrl;
            _accessToken = accessToken;
            _accessTokenExpiration = accessTokenExpiration;
            _debug = debug;

            // Set the base address of the Project Online API
            _httpClient.BaseAddress = new Uri($"{_projectOnlineUrl}/_api/ProjectData/");
        }

        public async Task<object> GetProjects()
        {
            return await GetApiData("Projects");
        }

        public async Task<object> GetProjectsBaseline()
        {
            return await GetApiData("ProjectsBaseline");
        }

        public async Task<object> GetTasks()
        {
            return await GetApiData("Tasks");
        }

        public async Task<object> GetTaskBaseline()
        {
            return await GetApiData("TaskBaseline");
        }

        public async Task<object> GetAssignments()
        {
            return await GetApiData("Assignments");
        }

        public async Task<object> GetAssignmentBaseline()
        {
            return await GetApiData("AssignmentBaseline");
        }

        public async Task<object> GetResources()
        {
            return await GetApiData("Resources");
        }

        private async Task<object> GetApiData(string apiEndpoint)
        {
            // Obtain access token using refresh token
            string accessToken = await GetAccessToken();

            // Set authorization header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Set the accept header to JSON
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Set the date for the filter
            DateTime last24Hours = DateTime.Now.AddHours(-24);
            if (_debug)
            {
                last24Hours = DateTime.Now.AddMonths(-24);
            }

            // Build the REST API URL with the filter
            // Projects does have a `ProjectLastPublishedDate` that we could use instead
            // $"Projects?$filter=ProjectLastPublishedDate gt datetime'{last24Hours.ToString("yyyy-MM-ddTHH:mm:ss")}'";
            string apiUrl = $"{apiEndpoint}?$filter=ProjectModifiedDate gt datetime'{last24Hours.ToString("yyyy-MM-ddTHH:mm:ss")}'";

            // Make the GET request
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                // Process the response (parse JSON or other actions)
                return await response.Content.ReadAsAsync<object>();
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
