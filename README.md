# Azure Cosmos DB & Azure OpenAI Service Reference Architecture: Medical Claims Management Hackathon

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
- Subscription access to Azure OpenAI service. Start here to [Request Access to Azure OpenAI Service](https://aka.ms/oaiapply)

- Backend (Web API, Worker Service, Console Apps, etc.)
  - Visual Studio 2022 17.6 or later (required for passthrough Visual Studio authentication for the Docker container)
  - .NET 7 SDK
  - Docker Desktop (with WSL for Windows machines)
  - Azure CLI ([v2.49.0 or greater](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli))
  - [Helm 3.11.1 or greater](https://helm.sh/docs/intro/install/)
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

## Run the solution locally using Visual Studio

You can run the website and the REST API provided by the Azure Function App that supports it locally. You need to first update your local configuration and then you can run the solution in the debugger using Visual Studio.

#### Configure local settings

> **Note:** Only complete these steps if you did not deploy the solution to Azure using the deployment guide. The deployment scripts create the `appsettings.Development.json` files for you. If these files do not exist for some reason, you can follow these steps to create them.

- In the `CoreClaims.WebAPI` project, copy the `appsettings.Development.template.json` file and name it `appsettings.Development.json`. This file should like similar to this (make sure you replace the `{{suffix}}` and other placeholders with your deployed resource names):

    ```json
    {
      "AllowedHosts": "*",
      "CoreClaimsCosmosDB:accountEndpoint": "https://<...>.documents.azure.com:443/",
      "CoreClaimsEventHub:fullyQualifiedNamespace": "eh-coreclaims-{{suffix}}.servicebus.windows.net",
      "BusinessRuleOptions": {
        "AutoApproveThreshold": 200,
        "RequireManagerApproval": 500,
        "DemoMode": true,
        "DemoAdjudicatorId": "df166300-5a78-3502-a46a-832842197811",
        "DemoManagerAdjudicatorId": "a735bf55-83e9-331a-899d-a82a60b9f60c"
      },
      "RulesEngine": {
        "OpenAIEndpoint": "{{openAiEndpoint}}",
        "OpenAIKey": "{{openAiKey}}",
        "OpenAICompletionsDeployment": "{{openAiDeployment}}"
      }
    }
    ```

- In the `CoreClaims.WorkerService` project, copy the `appsettings.Development.template.json` file and name it `appsettings.Development.json`. This file should like similar to this (make sure you replace the `{{suffix}}` and other placeholders with your deployed resource names):

    ```json
    {
      "AllowedHosts": "*",
      "CoreClaimsCosmosDB:accountEndpoint": "https://<...>.documents.azure.com:443/",
      "CoreClaimsEventHub:fullyQualifiedNamespace": "eh-coreclaims-{{suffix}}.servicebus.windows.net",
      "BusinessRuleOptions": {
        "AutoApproveThreshold": 200,
        "RequireManagerApproval": 500,
        "DemoMode": true,
        "DemoAdjudicatorId": "df166300-5a78-3502-a46a-832842197811",
        "DemoManagerAdjudicatorId": "a735bf55-83e9-331a-899d-a82a60b9f60c"
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

Before you can start debugging the `CoreClaims.WebAPI` and `CoreClaims.WorkerService` projects, make sure the newly created `appsettings.Development.json` files are copied to the output directory in each project. To do this, right-click on the file in the Solution Explorer and select `Properties`. In the properties window, set the `Copy to Output Directory` property to `Copy always`..

You are now ready to start debugging the solution locally. To do this, you first need to set up multiple startup projects to run when you debug. Right-click the solution in Solution Explorer, then select **Configure Startup Projects...**. Set `CoreClaims.WorkerService` and `CoreClaims.WebAPI` to **Start** under Action. All others should be set to None. When you're ready to debug, press `F5` or select `Debug > Start Debugging` from the menu.

**NOTE**: With Visual Studio, you can also use alternate ways to manage the secrets and configuration. For example, you can use the `Manage User Secrets` option from the context menu of the `CoreClaims.WebAPI` and `CoreClaims.WorkerService` projects to open the `secrets.json` file and add the configuration values there.

## Teardown

When you have finished with the hackathon, simply delete the resource group that was created.
