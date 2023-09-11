# Azure Cosmos DB & Azure OpenAI Service Reference Architecture: Medical Claims Management Hackathon

# **NOTICE: REPO MOVED**
## This repo has moved to its new location here: <https://github.com/Azure/Medical-Claims-Processing-Hackathon>

Contoso Insurance manages medical insurance claims through a custom-built application. The application is built on a legacy platform that is difficult to maintain and is not scalable. They want to modernize their application and move it to the cloud. They have decided to use Azure Cosmos DB as the database for their new application. They have also decided to use Azure OpenAI Service to help with the claims management process. They have asked you to help them build a proof of concept (POC) for their new application.

Currently, Contoso has a very tedious process for managing and processing claims. This process involves an extensive amount of human interaction and, therefore, clerical errors are imminent. The manual nature of the process, including the relatively high amount of issues that arise from data introduced by human entry alone, adds unnecessary costs to the company. These costs are realized in the inputting and the processing of the claim. Contoso is seeking help as they search for a better way to process claims.

Contoso would like to build a pilot atop of a version of their current claims submission solution to experiment with various tools in Azure for improving and optimizing operations around claims submission and processing. Ideally, Contoso would like to be able to accomplish the following with the proposed solution:

- Reduce the amount of time it takes to process a claim
- Implement business rules to automatically approve or deny claims
- Run their application at a global scale, enabling them to ingest high volumes of medical claims and apply automated processing logic
- Add a layer of intelligence to their application to help with the claims management process by providing an adjudicator with AI generated guidance on what action to take

Their long-term goal after accomplishing the above, which lays the ground work for automated decisions, is to harness AI to automatically review, approve, deny or forward the claim to a manager, replacing the hardcoded rule logic used by the solution.

## Prerequisites

- Azure Subscription
- Subscription with access to the Azure OpenAI Service. Start here to [Request Access to Azure OpenAI Service](https://customervoice.microsoft.com/Pages/ResponsePage.aspx?id=v4j5cvGGr0GRqy180BHbR7en2Ais5pxKtso_Pz4b1_xUOFA5Qk1UWDRBMjg0WFhPMkIzTzhKQ1dWNyQlQCN0PWcu)

### Prerequisites for running/debugging locally

- Backend (Function App, Console Apps, etc.)
  - Visual Studio Code or Visual Studio 2022
  - .NET 7 SDK
- Frontend (React web app)
  - Visual Studio Code
  - Ensure you have the latest version of NPM and node.js:
    - Install NVM from https://github.com/coreybutler/nvm-windows
    - Run nvm install latest
    - Run nvm list (to see the versions of NPM/node.js available)
    - Run nvm use latest (to use the latest available version)

To start the React web app:

1. Navigate to the `ui/medical-claims-ui` folder
2. Run npm install to restore the packages
3. Run npm run dev
4. Open localhost:3000 in a web browser

### Clone this repo

Clone this repository:

```pwsh
git clone https://github.com/...
```

### Deploy to Azure the core services

1. Open the PowerShell command line and navigate to the directory where you cloned the repo.
2. Navigate into the `starter-artifacts\deploy\powershell` folder.
3. Run the following PowerShell script to provision the infrastructure and deploy the base set of required Azure services. Provide the name of a NEW resource group that will be created. This will provision the resource group, blob storage accounts, Event Hub, and a Synapse Workspace.

```pwsh
./Starter-Deploy.ps1  -resourceGroup <resource-group-name> -subscription <subscription-id>
```

## Run the solution locally using Visual Studio

You can run the website and the REST API provided by the Azure Function App that supports it locally. You need to first update your local configuration and then you can run the solution in the debugger using Visual Studio.

#### Configure local settings

- In the `CoreClaims.FunctionApp` project, copy the `local.settings.template.json` file and name it `local.settings.json`. This file should like similar to this (make sure you replace the `{{suffix}}` and other placeholders with your deployed resource names):

    ```json
    {
      "IsEncrypted": false,
      "Values": {
        "AzureWebJobsStorage__accountName": "UseDevelopmentStorage=true",
        "CoreClaimsCosmosDB__accountEndpoint": "https://<...>.documents.azure.com:443/",
        "CoreClaimsEventHub__fullyQualifiedNamespace": "eh-coreclaims-{{suffix}}.servicebus.windows.net",
        "BusinessRuleOptions__AutoApproveThreshold": "200",
        "BusinessRuleOptions__RequireManagerApproval": "500",
        "BusinessRuleOptions__DemoAdjudicatorId": "df166300-5a78-3502-a46a-832842197811",
        "BusinessRuleOptions__DemoManagerAdjudicatorId": "a735bf55-83e9-331a-899d-a82a60b9f60c"
      },
      "Host": {
        "LocalHttpPort": 7061,
        "CORS": "*"
      },
      "RulesEngine": {
        "OpenAIEndpoint": "{{openAiEndpoint}}",
        "OpenAIKey": "{{openAiKey}}",
        "OpenAICompletionsDeployment": "{{openAiDeployment}}"
      }
    }
    ```

- In the `CoreClaims.Publisher` project, copy `settings.template.json` to a new file named `settings.json` and make sure it looks similar to this:

    ```json
    {
      "GeneratorOptions": {
        "RunMode": "OneTime",
        "BatchSize": 10,
        "Verbose": true,
        "SleepTime": 1000
      },
      "CoreClaimsCosmosDB": {
        "accountEndpoint": "AccountEndpoint=https://db-coreclaims-{{suffix}}.documents.azure.com:443/;AccountKey={{cosmosKey}};"
      },
      "CoreClaimsEventHub": {
        "fullyQualifiedNamespace": "Endpoint=sb://eh-coreclaims-{{suffix}}.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey={{eventHubKey}}"
      }
    }
    ```

### Running in debug

To run locally and debug using Visual Studio, open the solution file to load the projects and prepare for debugging.

Before you can start debugging the Function App, make sure the newly created `local.settings.json` file is copied to the output directory. To do this, right-click on the file in the Solution Explorer and select `Properties`. In the properties window, set the `Copy to Output Directory` property to `Copy always`..

You are now ready to start debugging the solution locally. To do this, press `F5` or select `Debug > Start Debugging` from the menu.

**NOTE**: With Visual Studio, you can also use alternate ways to manage the secrets and configuration. For example, you can use the `Manage User Secrets` option from the context menu of the `CoreClaims.FunctionApp` project to open the `secrets.json` file and add the configuration values there.

## Teardown

When you have finished with the hackathon, simply delete the resource group that was created.
