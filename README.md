# Azure Cosmos DB & OpenAI Reference Architecture: Medical Claims Management Hackathon

Contoso Insurance manages medical insurance claims through a custom-built application. The application is built on a legacy platform that is difficult to maintain and is not scalable. They want to modernize their application and move it to the cloud. They have decided to use Azure Cosmos DB as the database for their new application. They have also decided to use OpenAI to help with the claims management process. They have asked you to help them build a proof of concept (POC) for their new application.

Currently, Contoso has a very tedious process for managing and processing claims. This process involves an extensive amount of human interaction and, therefore, clerical errors are imminent. The manual nature of the process, including the relatively high amount of issues that arise from data introduced by human entry alone, adds unnecessary costs to the company. These costs are realized in the inputting and the processing of the claim. Contoso is seeking help as they search for a better way to process claims.

Contoso would like to build a pilot atop of a version of their current claims submission solution to experiment with various tools in Azure for improving and optimizing operations around claims submission and processing. Ideally, Contoso would like to be able to accomplish the following with the proposed solution:

- Reduce the amount of time it takes to process a claim
- Implement business rules to automatically approve or deny claims
- Run their application at a global scale, enabling them to ingest high volumes of medical claims and apply automated processing logic
- Add a layer of intelligence to their application to help with the claims management process by providing an adjudicator with AI generated guidance on what action to take

Their long-term goal after accomplishing the above, which lays the ground work for automated decisions, is to harness AI to automatically review, approve, deny or forward the claim to a manager, replacing the hardcoded rule logic used by the solution.
