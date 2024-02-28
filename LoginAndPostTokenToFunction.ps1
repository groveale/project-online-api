#######################################
# Description: This login in a user and get an access token to post to a function
#
# Author:      Alex Grover (alexgrover@microsoft.com)
# 
# Usage:       .\LoginAndPostTokenToFunction.ps1
#
#
#          

##############################################
# Varibales
##############################################

$client_id = "cd85557e-65a9-4854-b879-2671dfaee51a"
$tenantId = "75e67881-b174-484b-9d30-c581c7ebc177"
$redirect_uri = "https://syncprojectonlinespodataag.azurewebsites.net/api/UpdateRefreshTokenFromAccessCode"
$scope = "profile openid email https://graph.microsoft.com/EnterpriseResource.Read https://graph.microsoft.com/Project.Read https://graph.microsoft.com/ProjectWebAppReporting.Read https://graph.microsoft.com/User.Read offline_access"
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

##############################################
# Main
##############################################

## Open the browser to the login page
Start-Process $authorize_url_with_params

## The token will be returned to the function which will then update the refresh token in the keyVault. The browser will indicate the the token has been sucessfully updated