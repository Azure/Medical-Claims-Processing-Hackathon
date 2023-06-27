# Challenge 2: Get Loaded

Contoso Insurance exported a portion of their customer, adjudicator, claims, and provider data from their legacy system and stored it in Azure Blob Storage. They want to load this data into Cosmos DB so that we can begin to build the new claims management application.

In this challenge, you must load the pregenerated sample claims data using an Azure Synapse pipeline.

## Challenge

Your team must:

1. Find an efficient and repeatable way to load data from the storage account into Cosmos DB. For this exercise, you only need to load the data once, but you want to be able to repeat the process in the future with new data.
2. Verify that the data was loaded into Cosmos DB.

### Hints

- Pre-generated data can be found in a public accessible Azure Blob Storage account (https://solliancepublicdata.blob.core.windows.net/medical-claims).
- Each folder contains JSON files that should map to the Azure Cosmos DB containers as follows:
  - `adjudicators.json` -> `Adjudicator`
  - `claims-small.json` (where `type` == `ClaimDetail`) -> `Claim`
  - `claims-small.json` (where `type` == `ClaimHeader`) -> `Claim`
  - `claimprocedure.json` -> `ClaimProcedure`
  - `members.json` -> `Member`
  - `coverage.json` -> `Member`
  - `providers.json` -> `Provider`
  - `payers.json` -> `Payer`

### Success Criteria

To complete this challenge successfully, you must:

- Load all JSON data into the appropriate Cosmos DB containers.
- Verify that the data was loaded into Cosmos DB.

### Resources

- Integrate with Azure Synapse Analytics pipelines(https://learn.microsoft.com/azure/synapse-analytics/get-started-pipelines)

## Explore Further

Add some context to the challenge and technologies used.
