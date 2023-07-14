//Resource group is the default and could be omited.
targetScope = 'resourceGroup'

param environment string
param location string = resourceGroup().location
param tenantId string = tenant().tenantId
param resourceNamePrefix string = 'wif-blog-aaes-'
param appPlanName string = '${toLower(resourceNamePrefix)}app-plan-${toLower(environment)}'
param functionAppName string = '${toLower(resourceNamePrefix)}api-host-${toLower(environment)}'
param staticWebsiteName string = '${toLower(resourceNamePrefix)}static-website-${toLower(environment)}'
param storageAccountName string = '${replace(toLower(resourceNamePrefix), '-', '')}storage${toLower(environment)}'
param dbServerName string = '${toLower(resourceNamePrefix)}db-srv-${toLower(environment)}'
param dbSku string
param dbInstanceName string = 'Quotes'
param dbAdminSID string
param dbAdminLogin string
@allowed(['Application','Group','User'])
param dbAdminPrincipalType string

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageAccountName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: 'Hot'
  }
}

resource staticWebsite 'Microsoft.Web/staticSites@2022-09-01' = {
  name: staticWebsiteName
  location: location
  sku: {
    name: 'Free'
  }
  properties: {
    
  }
}

resource dbServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: dbServerName
  location: location
  properties: {
    administrators: {
      administratorType: 'ActiveDirectory'
      azureADOnlyAuthentication: true
      login: dbAdminLogin
      principalType: dbAdminPrincipalType
      sid: dbAdminSID
      tenantId: tenantId
    }
  }
}

resource dbInstance 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  name: dbInstanceName
  location: location
  parent: dbServer
  sku:{
    name: dbSku
  }
}

resource functionAppServerFarm 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: appPlanName
  location: location
  sku: {
    name: 'Y1'  //consumption based
    tier: 'Dynamic'
  }
}

var storageAccountConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
resource functionApp 'Microsoft.Web/sites@2022-09-01' = {
  name: functionAppName
  location: location
  kind: 'linux,functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: functionAppServerFarm.id
    
    siteConfig: {
      cors: {
        allowedOrigins: ['https://${staticWebsite.properties.defaultHostname}']
      }
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: storageAccountConnectionString
        },{
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        },{
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        },{
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: storageAccountConnectionString
        },{
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
      ]
    }
  }
}

output staticWebsiteName string = staticWebsite.name
output functionAppName string = functionApp.name
output functionAppUrl string = 'https://${functionApp.properties.defaultHostName}'
output dbServerName string = dbServer.name
output dbInstanceName string = dbInstance.name
