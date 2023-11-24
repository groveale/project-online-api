using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace groveale 
{

    public class ProjectOnlineHelper
    {
        private readonly HttpClient _httpClient;
        private readonly string _projectOnlineUrl;
        private string _accessToken;
        private readonly DateTime _accessTokenExpiration;
        private readonly bool _fullPull;
        private readonly AuthenticationHelper _authHelper;

        public ProjectOnlineHelper(string projectOnlineUrl, string accessToken, DateTime accessTokenExpiration, bool fullPull = false, AuthenticationHelper authHelper = null)
        {
            _httpClient = new HttpClient();
            _projectOnlineUrl = projectOnlineUrl;
            _accessToken = accessToken;
            _accessTokenExpiration = accessTokenExpiration;
            _fullPull = fullPull;
            _authHelper = authHelper;

            // Set the base address of the Project Online API
            _httpClient.BaseAddress = new Uri($"{_projectOnlineUrl}/_api/ProjectData/");
        }

        public async Task<List<JObject>> GetProjects()
        {
            return await GetApiData("Projects");
        }

        public async Task<List<JObject>> GetProjectsBaseline()
        {
            return await GetApiData("ProjectsBaseline");
        }

        public async Task<List<JObject>> GetTasks()
        {
            return await GetApiData("Tasks");
        }

        public async Task<List<JObject>> GetTaskBaseline()
        {
            return await GetApiData("TaskBaseline");
        }

        public async Task<List<JObject>> GetAssignments()
        {
            return await GetApiData("Assignments");
        }

        public async Task<List<JObject>> GetAssignmentBaseline()
        {
            return await GetApiData("AssignmentBaseline");
        }

        public async Task<List<JObject>> GetResources()
        {
            return await GetApiData("Resources");
        }

        private async Task<List<JObject>> GetApiData(string apiEndpoint, string projectId = null)
        {
            // Obtain access token using refresh token
            string accessToken = await GetAccessToken();

            // Set authorization header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Set the accept header to JSON
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Set the date for the filter
            DateTime last24Hours = DateTime.Now.AddHours(-24);

            // Build the REST API URL with the filter
            // Projects does have a `ProjectLastPublishedDate` that we could use instead
            // $"Projects?$filter=ProjectLastPublishedDate gt datetime'{last24Hours.ToString("yyyy-MM-ddTHH:mm:ss")}'";


            string apiUrl = $"{apiEndpoint}?$filter=ProjectModifiedDate gt datetime'{last24Hours.ToString("yyyy-MM-ddTHH:mm:ss")}'";

            if (_fullPull)
            {
                apiUrl = $"{apiEndpoint}";
            }

            // If we are getting any data except Projects we need to filter by ProjectId
            if (!string.IsNullOrEmpty(projectId))
            {
                apiUrl = $"{apiEndpoint}?$filter=ProjectId eq guid'{projectId}'";
            }

            // Make the GET request
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                // Process the response (parse JSON and convert to list of objects)
                var content = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(content);
                var value = jsonObject["value"].ToString();
                var result = JsonConvert.DeserializeObject<List<JObject>>(value);
                return result;
            }
            else
            {
                // Handle error
                throw new InvalidOperationException($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }


        private async Task<string> GetAccessToken()
        {
            // TODO - check expiration I think we get just over an hour
            if (DateTime.Now > _accessTokenExpiration)
            {
                // Get a new access token
                this._accessToken = await _authHelper.GetAccessToken();
            }
            

            return this._accessToken;
        }
    }
        
}
