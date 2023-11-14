# Challenge 3: Rules Are Made to be Broken

Contoso Insurance has a set of rules that they use to determine whether a claim should be approved or denied. They want to use these rules to help adjudicators make decisions on claims. They also want to be able to update the rules as needed.

In this challenge, you familiarize yourself with core of the application by seeing the end-to-end process in action. Claims data arrives from an upstream ingestion process and is processed by the claim publisher. Contoso has provided you with a mock claims publisher console application that sends claims to Event Hubs to simulate this upstream process. These ingested claims are processed downstream by an Event Hub processor running within the Worker Service, which writes the data into Azure Cosmos DB. The Cosmos DB change feed triggers a Cosmos DB change feed processor also running in the Worker Service, which uses the rules engine to determine whether the claim should automatically be approved or denied. From this point, the remaining steps are more or less manual, with adjudicators reviewing claims assigned to them that did not get automatically approved or denied.

## Challenge

Your team must:

1. Update the project code to complete functionality.
2. Successfully run the solution locally and observe the data flowing through the change feed.
3. Run the claim publisher and examine how it drives Event Hub function trigger the change feed triggers.
4. As an adjudicator, acknowledge a claim using the UI and then adjudicate that claim.
5. Understand the rules that are in place.
6. Review the claim history.

### Hints

- Search through the solution for the `TODO: Challenge 3` comments and follow the instructions provided.
- Review the `BusinessRulesOptions` and `CoreBusinessRule` classes in the `CoreClaims.Infrastructure` project to see how the business rules function.
- Observe data flowing through the Event Hub and Change Feed processors in the Worker Service to see the process flow.
- Look at which Adjudicators are assigned in the business rules and see how that controls claim assignments for demo purposes.
- Since the app uses role-based access control (RBAC), if you want to run the solution locally, you have to assign yourself to the "Cosmos DB Built-in Data Contributor" role via the Azure Cloud Shell or Azure CLI with the following:

    ```bash
    az cosmosdb sql role assignment create --account-name YOUR_COSMOS_DB_ACCOUNT_NAME --resource-group YOUR_RESOURCE_GROUP_NAME --scope "/" --principal-id YOUR_AZURE_AD_PRINCIPAL_ID --role-definition-id 00000000-0000-0000-0000-000000000002
    ```

- Event Hubs is also using RBAC. There is an Event Hubs-triggered event processor in the Worker Service that fires when adding claims through the `CoreClaims.Publisher` console app. You need to add yourself to the "Azure Event Hubs Data Owner" role via the Azure Cloud Shell or Azure CLI with the following:

    ```bash
    az role assignment create --assignee "YOUR_EMAIL_ADDRESS" --role "Azure Event Hubs Data Owner" --scope "/subscriptions/YOUR_AZURE_SUBSCRIPTION_ID/resourceGroups/YOUR_RESOURCE_GROUP_NAME/providers/Microsoft.EventHub/namespaces/YOUR_EVENT_HUBS_NAMESPACE"
    ```

    > Make sure you're signed in to Azure from the Visual Studio or VS Code terminal before running the Function App locally. You need to run `az login` and `az account set --subscription YOUR_AZURE_SUBSCRIPTION_ID` first.

- You can find an `env.template` file inside the `ui` folder for the React web application. Copy this file and name the copy `.env.local`. While running the web app and Function App locally, you need to update the `NEXT_PUBLIC_API_URL` value to `http://localhost:7071/api`. In a later challenge when you deploy the web app and Function App, you need to update this value to the deployed Function App URL.
- There are hard-coded values on the web app for the non-manager and manager adjudicators displayed on the Adjudicators page of the site. Make sure these IDs match up with the IDs configured in your Function App's settings.

### Success Criteria

To complete this challenge successfully, you must:

- Complete all of the code in the solution that is marked with `TODO: Challenge 3` comments.
- Generate a small batch of claims through the `CoreClaims.Publisher` console application.
- Verify that the claims were processed by the Worker Service and written to Cosmos DB. You can do this by debugging the solution locally and setting breakpoints in the Event Hubs and Change Feed processors in the Worker Service to walk through the process. Use the Azure Cosmos DB Data Explorer to verify that the claims were written to the database.
- Acknowledge an assigned claim as an adjudicator in the UI.
- Deny a claim.
- Propose a claim for approval without applying discounts.
- Propose a claim for approval with discounts applied that are less than the threshold.
- Propose a claim for as an adjudicator manager.
- Approve or deny a claim as an adjudicator manager.

### Expected Claims Flow

- When a new claim is added:
  - If the member this claim belongs to doesn't have a Coverage record that is active for the `filingDate`, the claim should be `Rejected`
  - If the `totalAmount` value is less than 200.00 (configurable) it should be `Approved`
  - If the `totalAmount` value is greater than 200.00, it should be `Assigned`
  - Finally, if your initial ingestion run has a large volume of claims, it's possible the ChangeFeed triggers are still catching up, and the status may be `Initial`
- When the claim is assigned to an adjudicator:
  - Go to the Adjudicator page and select **Acknowledge Claim Assignment** and observe the flow of the claim through the system (change feed triggers, etc.). There should be claims assigned to both the Non-Manager and the Manager Adjudicators.
- When the claim is acknowledged:
  - The claim should be `Assigned` to the Adjudicator
  - Selecting **Deny Claim** will finalize the claim as `Denied` and publish the final status of the claim to a `ClaimDenied` topic on the event hub
  - Selecting **Propose Claim** without applying discounts on the Line Items, or changing them such that the difference between the total before and after is less than $500.00 (configurable) will trigger an automatic approval
  - Selecting **Propose Claim** while applying discounts on the Line Items so the total before and after differs by more than $500.00 will trigger manager approval, updating the status to `ApprovalRequired` and assigning a new adjudicator manager to the claim. Since we are hard-coding the Non-Manager and Manager Adjudicators, you should be able to select the Manager tab and see the claim assigned to the Manager Adjudicator.

  > **Note**: If you propose a claim as an adjudicator manager, the claim will always be approved, regardless of the total discount amount.

- Reviewing the claim history:
  - Select **View History** on a claim row to see the history of the claim
  - All claims start in the `Initial` state, from here they can transition to
    - `Denied` if the member is uninsured
    - `Approved` if the total is less than 200
    - `Assigned` if the total is more than 200
  - From `Assigned` it transitions to `Acknowledged` when the adjudicator acknowledges the claim
  - From `Acknowledge` to
    - `Denied` if the adjudicator declines the claim
    - `Proposed` if the adjudicator proposes some updates
  - From `Proposed`
    - `Approved` if the changes are under a configured threshold
    - `ApprovalRequired` if the changes are over a threshold
  - From `ApprovalRequired` to
    - `Denied` or `Proposed`
- Final claim state:
  - Once a Claim reaches the `Denied` or `Approved` state, it will get published to another pair of EventHub topics for hypothetical downstream processing
  - Note that when a Claim gets to a final `Approved` state, the associated Member document within the `Member` container will get updated with increments of the following two attributes:
    - `approvedCount` - the number of claims that have been approved for this member
    - `approvedTotal` - the total amount of all claims that have been approved for this member
