@description('Synapse workspace name')
param synapseServiceName string

@description('Storage account with "claimsfs" container')
param storageAccountName string

@description('Resource location')
param location string = resourceGroup().location

var storageContainerName = 'claimsfs'

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: storageAccountName
}

resource blobServices 'Microsoft.Storage/storageAccounts/blobServices@2022-09-01' existing = {
  parent: storageAccount
  name: 'default'
}

resource container 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-09-01' = {
  parent: blobServices
  name: storageContainerName
  properties: {
    publicAccess: 'None'
  }
}

resource synapse 'Microsoft.Synapse/workspaces@2021-06-01' = {
  location: location
  name: synapseServiceName
  properties: {
    defaultDataLakeStorage: {
      resourceId: blobServices.id
      accountUrl: storageAccount.properties.primaryEndpoints.dfs
      filesystem: storageContainerName
    }
    publicNetworkAccess: 'Enabled'
    trustedServiceBypassEnabled: true
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource synapsefirewall 'Microsoft.Synapse/workspaces/firewallRules@2021-06-01' = {
  name: 'allow-all'
  parent: synapse
  properties: {
    endIpAddress: '255.255.255.255'
    startIpAddress: '0.0.0.0'
  }
}

// Grant Permissions to Identity for Storage
@description('This is the built-in "Storage Blob Data Contributor" role. See https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#storage-blob-data-contributor')
resource contributorRoleDefinition 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  scope: subscription()
  name: 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
}

resource storage 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: storageAccountName
}

resource roleAssignmentStorage 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: storage
  name: guid(storage.id, 'SynapseOwner')
  properties: {
    roleDefinitionId: contributorRoleDefinition.id
    principalId: synapse.identity.principalId
    principalType: 'ServicePrincipal'
  }
}
