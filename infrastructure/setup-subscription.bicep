targetScope = 'subscription' //targeting the subscription so that we can create the resource group

@description('Type of environment.  Usually qa, uat, stg, prd, etc.')
param environment string

@description('The azure datacenter location (i.e. westus2).  Can be found with Azure CLI `az account list-locations -o table`.')
param location string = deployment().location

resource resourceGroup 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: 'my-resouce-group-${environment}'
  location: location
}

module resourceGroupModule 'setup-resource-group.bicep' = {
  scope: resourceGroup
  name: 'setupResourceGroup'
  params: {
    environment: environment
    location: location
  }
}

output staticWebsiteName string = resourceGroupModule.outputs.staticWebsiteName
