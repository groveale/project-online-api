#######################################
# Description: This login in a user and get an access token to post to a function
#
# Author:      Alex Grover (alexgrover@microsoft.com)
# 
# Usage:       .\CreateLists.ps1
#
# Notes:       This script requires the PnP PowerShell module.
#              https://docs.microsoft.com/en-us/powershell/sharepoint/sharepoint-pnp/sharepoint-pnp-cmdlets?view=sharepoint-ps
#
#          

##############################################
# Varibales
##############################################

$client_id = "afdbe70d-fbb8-47fb-9348-ad43fed0cbda"
$tenantId = "936f0468-e5bb-4846-b365-5ae3790deadf"
$redirect_uri = "http://localhost:7071/api/UpdateRefreshTokenFromAccessCode"
$scope = "profile openid email https://graph.microsoft.com/EnterpriseResource.Read https://graph.microsoft.com/Project.Read https://graph.microsoft.com/ProjectWebApp.FullControl https://graph.microsoft.com/ProjectWebAppReporting.Read https://graph.microsoft.com/User.Read offline_access"
$state = "12345"
$authorize_url = "https://login.microsoftonline.com/$tenantId/oauth2/v2.0/authorize"

# Step 1: Get the authorization code
$authorizeParams = @{
    client_id     = $client_id
    scope         = $scope
    redirect_uri  = $redirect_uri
    response_type = "code"
    state         = $state
}


## Append the parameters to the URL
$authorize_url_with_params = $authorize_url + "?client_id=" + $client_id + "&scope=" + $scope + "&redirect_uri=" + $redirect_uri + "&response_type=code&state=" + $state


## Open the browser to the login page
Start-Process $authorize_url_with_params

## The token will be returned to the function which will then update the refresh token in the keyVault. The browser will indicate the the token has been sucessfully updated