# Challenge 3: Rules Are Made to be Broken

Contoso Insurance has a set of rules that they use to determine whether a claim should be approved or denied. They want to use these rules to help adjudicators make decisions on claims. They also want to be able to update the rules as needed.

In this challenge, you familiarize yourself with core of the application by seeing the end-to-end process in action. Claims data arrives from an upstream ingestion process and is processed by the claim publisher. Contoso has provided you with a mock claims publisher console application that sends claims to Event Hubs to simulate this upstream process. These ingested claims trigger an Azure Function through an Event Hub trigger, which writes the data into Azure Cosmos DB. The Cosmos DB change feed triggers another function, which uses the rules engine to determine whether the claim should automatically be approved or denied. From this point, the remaining steps are more or less manual, with adjudicators reviewing claims assigned to them that did not get automatically approved or denied.

## Challenge

Your team must:

1. Run the claim publisher and examine how it drives the change feed.
2. As an adjudicator, acknowledge a claim using the UI and then adjudicate that claim.
3. Understand the rules that are in place.
4. Review the claim history.

### Hints

- Search through the solution for the `TODO: Challenge 3` comments and follow the instructions provided.
- Review the `BusinessRulesOptions` and `CoreBusinessRule` classes in the `CoreClaims.Infrastructure` project to see how the business rules function.
- Observe data flowing through the Event Hub and Change Feed-triggered Azure Functions to see the process flow.
- Look at which Adjudicators are assigned in the business rules and see how that controls claim assignments for demo purposes.
- Since the app uses role-based access control (RBAC), if you want to run the Function App locally, you have to assign yourself to the "Cosmos DB Built-in Data Contributor" role via the Azure Cloud Shell or Azure CLI with the following:

    ```bash
    az cosmosdb sql role assignment create --account-name YOUR_COSMOS_DB_ACCOUNT_NAME --resource-group YOUR_RESOURCE_GROUP_NAME --scope "/" --principal-id YOUR_AZURE_AD_PRINCIPAL_ID --role-definition-id 00000000-0000-0000-0000-000000000002
    ```

- Event Hubs is also using RBAC. There is an Event Hubs-triggered function in the Function App that fires when adding claims through the `CoreClaims.Publisher` console app. You need to add yourself to the "Azure Event Hubs Data Owner" role via the Azure Cloud Shell or Azure CLI with the following:

    ```bash
    az role assignment create --assignee "YOUR_EMAIL_ADDRESS" --role "Azure Event Hubs Data Owner" --scope "/subscriptions/YOUR_AZURE_SUBSCRIPTION_ID/resourceGroups/YOUR_RESOURCE_GROUP_NAME/providers/Microsoft.EventHub/namespaces/YOUR_EVENT_HUBS_NAMESPACE"
    ```

### Success Criteria

To complete this challenge successfully, you must:

- Generate a small batch of claims through the `CoreClaims.Publisher` console application.
- Verify that the claims were processed by the Azure Functions and written to Cosmos DB. You can do this by debugging the Function App locally and setting breakpoints in the Event Hubs and Change Feed-triggered functions to walk through the process. Use the Azure Cosmos DB Data Explorer to verify that the claims were written to the database.
- Acknowledge an assigned claim as an adjudicator in the UI.
- Deny a claim.
- Propose a claim for approval without applying discounts.
- Propose a claim for approval with discounts applied that are less than the threshold.
- Propose a claim for as an adjudicator manager.
- Approve or deny a claim as an adjudicator manager.
