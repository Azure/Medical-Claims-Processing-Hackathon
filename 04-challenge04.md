# Challenge 4: Not Your High School Guidance Counselor

Now that you understand how business rules are used to evaluate a claim, you will augment the functionality of the Claim Details pane by providing the adjudicator with AI generated guidance on what action to take. This will be accomplished by using the Microsoft Semantic Kernel with Azure OpenAI to create a SemanticFunction that will respond with guidance for the claim. To accomplish this, examine the `RulesEngine` function in the `CoreClaims.SemanticKernel` project, which has some placeholder code for you to complete.

After creating the SemanticFunction, you will run the solution, select a claim to adjudicate and view the guidance provided to you by the language model.

## Challenge

Your team must:

1. Complete the semantic kernel code.
2. Ask a question through the prompt interface and see if it returns an answer about the new data you loaded.

### Hints

- Search through the solution for the `TODO: Challenge 4` comments and follow the instructions provided.
- The HTTP-triggered function in `GetClaimRecommendation.cs` is already created for you. This is invoked when you select the **Make Recommendation** button when viewing Claim Details in the UI.

### Success Criteria

To complete this challenge successfully, you must:

- Complete the semantic kernel code.
- Select a claim to adjudicate and view the guidance provided by the language model.

### Resources

- [What is Semantic Kernel?](https://learn.microsoft.com/semantic-kernel/overview/)
