//Targeting the subscription so that we can create the resource group.
//Resource group is the default and could be omited.
targetScope = 'resourceGroup'

param environment string
param location string = resourceGroup().location
param functionAppNameBase string = 'wif-blog-aaes-api-host-'
param staticWebsiteNameBase string = 'wif-blog-aaes-static-website-'
param storageAccountNameBase string = 'wifblogaaesstorageaccount'

var env = toLower(environment)

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: '${storageAccountNameBase}${env}'
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
  name: '${staticWebsiteNameBase}${env}'
  location: location
  sku: {
    name: 'Free'
  }
  properties: {
    
  }
}

resource functionAppServerFarm 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: 'app-plan-${env}'
  location: location
  sku: {
    name: 'Y1'  //consumption based
    tier: 'Dynamic'
  }
}

var storageAccountConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
resource functionApp 'Microsoft.Web/sites@2022-09-01' = {
  name: '${functionAppNameBase}${env}'
  location: location
  kind: 'linux,functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: functionAppServerFarm.id
    siteConfig: {
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
          name: 'Quotes[0].Id'
          value: '1'
        },{
          name: 'Quotes[0].Text'
          value: 'Software undergoes beta testing shortly before it\'s released. Beta is Latin for “still doesn\'t work”.'
        },{
          name: 'Quotes[0].Source'
          value: 'Anonymous'
        },{
          name: 'Quotes[1].Id'
          value: '2'
        },{
          name: 'Quotes[1].Text'
          value: 'Measuring programming progress by lines of code is like measuring aircraft building progress by weight.'
        },{
          name: 'Quotes[1].Source'
          value: 'Bill Gates'
        },{
          name: 'Quotes[2].Id'
          value: '3'
        },{
          name: 'Quotes[2].Text'
          value: 'If debugging is the process of removing software bugs, then programming must be the process of putting them in.'
        },{
          name: 'Quotes[2].Source'
          value: 'Edsger Dijkstra'
        }
      ]
    }
  }
}

output staticWebsiteName string = staticWebsite.name
