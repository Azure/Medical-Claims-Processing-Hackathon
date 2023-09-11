# Challenge 1: Get Up, Set Up, Don't Let Up

Contoso Insurance is drowning in a backlog of submitted medical insurance claims. They have decided to modernize their claims management application and move it to the cloud. They have asked you to help them build a proof of concept (POC) for their new application to automate the claims management process and add intelligence to assist the adjudicators in making decisions.

For this challenge, you will deploy the services into the landing zone in preparation for the launch of the POC.

## Challenge

Your team must:

1. Deploy the Azure services needed to support the claims management interface
2. Clone the repo with the starter artifacts
3. Set up your development environment

### Hints

- Contoso Insurance has provided you a script to deploy the foundation of your Azure environment. See the instructions in the README.md of the repo.
- You will need to deploy the following Azure services within a new Resource Group:
  - Azure Blob Storage (ADLS Gen2)
  - Azure Blob Storage account for hosting a static web app
  - Azure Cosmos DB NoSQL API
  - Azure OpenAI
  - Azure Event Hubs Standard
  - Azure Functions Consumption Plan
  - Azure Application Insights
  - Azure Synapse Analytics Workspace
- You will load data in a later challenge, but you can get a head start by deploying the Azure Synapse Analytics workspace now.

### Success Criteria

To complete this challenge successfully, you must:

- Clone the repo with the starter artifacts and deployment scripts
- Deploy the Azure services needed to support the claims management interface
- Deploy Azure OpenAI with the following deployments:
  - `completions-003` with the `text-davinci-003` model
- Deploy an Azure Cosmos DB account with the following configurations:
  - API: NoSQL
  - Consistency: Session
  - Geo-Redundancy: Disabled
  - Multi-region writes: Disabled
  - Analytical store: Disabled
  - Autoscale: Enabled
  - Provision throughput: 1000 RU/s
  - Create a database named `CoreClaimsApp`
  - Create new containers named:
    - `Adjudicator` with partition key `/adjudicatorId`
    - `Claim` with partition key `/claimId` and autoscale throughput with a maximum of 2000 RU/s
    - `ClaimProcedure` with partition key `/code`
    - `Member` with partition key `/memberId` and autoscale throughput with a maximum of 1000 RU/s
    - `Payer` with partition key `/payerId`
    - `Provider` with partition key `/providerId`
- Validate that the services are deployed and running

### Resources

- [Azure Cosmos DB](https://learn.microsoft.com/azure/cosmos-db/)
- [Host a static website on Blob Storage](https://learn.microsoft.com/azure/storage/blobs/storage-blob-static-website-host)
- [Azure Synapse Analytics](https://learn.microsoft.com/azure/synapse-analytics/)
- [Azure OpenAI](https://learn.microsoft.com/azure/cognitive-services/openai/overview)
- [Azure Functions](https://learn.microsoft.com/azure/azure-functions/functions-overview)
