using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace groveale 
{

    public class ProjectOnlineHelper
    {
        private readonly HttpClient _httpClient;
        private readonly string _projectOnlineUrl;
        private string _accessToken;
        public DateTime _accessTokenExpiration { get; private set; }
        private readonly bool _fullPull;
        private readonly AuthenticationHelper _authHelper;
        private ILogger _log;

        private const string RETRY_AFTER = "Retry-After";


        // dictionary of composite keys for each table
        // Key details obtained from key defined in the API metadata for each object _api/projectdata/$metadata
        public readonly Dictionary<string, string[]> _compositeKeys = new Dictionary<string, string[]>
        {
            { "Projects", new string[] { "ProjectId" } },
            { "ProjectBaselines", new string[] { "ProjectId", "BaselineNumber" } },
            { "Tasks", new string[] { "ProjectId", "TaskId" } },
            { "TaskBaselines", new string[] { "ProjectId", "TaskId", "BaselineNumber" } },
            { "Assignments", new string[] { "ProjectId", "AssignmentId" } },
            { "AssignmentBaselines", new string[] { "ProjectId", "AssignmentId", "BaselineNumber" } },
            { "Resources", new string[] { "ResourceId" } }
        };

        public ProjectOnlineHelper(string projectOnlineUrl, string accessToken, DateTime accessTokenExpiration, ILogger log, bool fullPull = false, AuthenticationHelper authHelper = null)
        {
            _httpClient = new HttpClient();
            _projectOnlineUrl = projectOnlineUrl;
            _accessToken = accessToken;
            _accessTokenExpiration = accessTokenExpiration;
            _fullPull = fullPull;
            _authHelper = authHelper;
            _log = log;

            // Set the base address of the Project Online API
            _httpClient.BaseAddress = new Uri($"{_projectOnlineUrl}/_api/ProjectData/");
        }

        public async Task<Dictionary<string, List<JObject>>> GetProjectData()
        {
            // Dictionary of lists of objects
            Dictionary<string, List<JObject>> projectData = new Dictionary<string, List<JObject>>();
            
            projectData.Add("Projects", await GetProjects());
            projectData.Add("ProjectBaselines", await GetProjectsBaseline());
            projectData.Add("Tasks", await GetTasks());
            projectData.Add("TaskBaselines", await GetTaskBaseline());
            projectData.Add("Assignments", await GetAssignments());
            projectData.Add("AssignmentBaselines", await GetAssignmentBaseline());
            projectData.Add("Resources", await GetResources());
            
            return projectData;
        }

        public async Task<List<JObject>> GetProjects()
        {
            return await GetApiData("Projects", "ProjectModifiedDate");
        }

        public async Task<List<JObject>> GetProjectsBaseline()
        {
            return await GetApiData("ProjectBaselines", "ProjectBaselineModifiedDate");
        }

        public async Task<List<JObject>> GetTasks()
        {
            return await GetApiData("Tasks", "TaskModifiedDate");
        }

        public async Task<List<JObject>> GetTaskBaseline()
        {
            return await GetApiData("TaskBaselines", "TaskBaselineModifiedDate");
        }

        public async Task<List<JObject>> GetAssignments()
        {
            return await GetApiData("Assignments", "AssignmentModifiedDate");
        }

        public async Task<List<JObject>> GetAssignmentBaseline()
        {
            return await GetApiData("AssignmentBaselines", "AssignmentBaselineModifiedDate");
        }

        public async Task<List<JObject>> GetResources()
        {
            return await GetApiData("Resources", "ResourceModifiedDate");
        }

        private async Task<List<JObject>> GetApiData(string apiEndpoint, string filterField)
        {
            
            // Set the accept header to JSON
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Set the date for the filter
            DateTime last24Hours = DateTime.Now.AddHours(-24);

            // Build the REST API URL with the filter
            // Projects does have a `ProjectLastPublishedDate` that we could use instead
            // $"Projects?$filter=ProjectLastPublishedDate gt datetime'{last24Hours.ToString("yyyy-MM-ddTHH:mm:ss")}'";

            string apiUrl = $"{apiEndpoint}?$filter={filterField} gt datetime'{last24Hours.ToString("yyyy-MM-ddTHH:mm:ss")}'";

            // Set the page size to 300
            int pageSize = 300;

            if (_fullPull)
            {
                apiUrl = $"{apiEndpoint}?$top={pageSize}";
            }

            List<JObject> allObjects = new List<JObject>();

            string doUrl = apiUrl;

            if (!_fullPull)
            {
                doUrl = apiUrl + $"&$top={pageSize}";
            }

            // retry counter
            int retries = 0;

            do
            {

                // Obtain access token using refresh token
                string accessToken = await GetAccessToken();

                // Set authorization header
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Make the GET request
                HttpResponseMessage response = await _httpClient.GetAsync(doUrl);

                if (response.IsSuccessStatusCode)
                {
                    // reset retry counter
                    retries = 0;

                    // Process the response (parse JSON and convert to list of objects)
                    // need to handle pagination
                    // check if there is a next link in the response
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(content);
                    var value = jsonObject["value"].ToString();

                    var result = JsonConvert.DeserializeObject<List<JObject>>(value);

                    allObjects.AddRange(result);

                     // Check if there are more pages
                    if (result.Count == pageSize)
                    {
                        // More items to fetch, update URL to get the next page
                        int skipCount = allObjects.Count;
                        doUrl = apiUrl + $"?$top={pageSize}&$skip={skipCount}";
                    }
                    else
                    {
                        // All items fetched, exit the loop
                        break;
                    }
                }
                else
                {
                    
                    // Handel 429 - Too many requests
                    if (ShouldRetry(response.StatusCode, retries))
                    {
                        _log.LogInformation($"Received: {response.StatusCode} - calculating wait time");
                        var waitTime = CalculateWaitTime(response);
                        _log.LogInformation($"Waiting for {waitTime.TotalSeconds} seconds before retrying");
                        await Task.Delay(waitTime);

                        // Retry the request
                        retries++;
                        continue;
                    }
                    else
                    {
                        // Log error
                        _log.LogError($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        throw new InvalidOperationException($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }

                }

            } while (true);

            return allObjects;
            
        }


        private async Task<string> GetAccessToken()
        {
            if (DateTime.Now > _accessTokenExpiration)
            {
                // Get a new access token
                this._accessToken = await _authHelper.GetAccessToken();
                _accessTokenExpiration = _accessTokenExpiration.AddMinutes(50);
            }
            

            return this._accessToken;
        }


        internal static bool ShouldRetry(HttpStatusCode statusCode, int retries)
        {
            return retries < 10 &&
                (statusCode == HttpStatusCode.ServiceUnavailable ||
                statusCode == HttpStatusCode.GatewayTimeout ||
                statusCode == HttpStatusCode.TooManyRequests ||
                statusCode == (HttpStatusCode)429); 
        }

        private static TimeSpan CalculateWaitTime(HttpResponseMessage response)
        {
            double delayInSeconds = 10;

            if (response != null && response.Headers.TryGetValues(RETRY_AFTER, out IEnumerable<string> values))
            {
                // Can we use the provided retry-after header?
                string retryAfter = values.First();
                if (int.TryParse(retryAfter, out int delaySeconds))
                {
                    delayInSeconds = delaySeconds;
                }
            }

            return TimeSpan.FromSeconds(delayInSeconds);
        }

    }
        
}
