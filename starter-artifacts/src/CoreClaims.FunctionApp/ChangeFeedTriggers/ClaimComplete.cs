using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreClaims.FunctionApp.HttpTriggers.Claims;
using CoreClaims.Infrastructure;
using CoreClaims.Infrastructure.Domain.Entities;
using CoreClaims.Infrastructure.Domain.Enums;
using CoreClaims.Infrastructure.Events;
using CoreClaims.Infrastructure.Repository;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoreClaims.FunctionApp.ChangeFeedTriggers
{
    public class ClaimComplete
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IEventHubService _eventHub;

        public ClaimComplete(
            IMemberRepository memberRepository,
            IEventHubService eventHub)
        {
            _memberRepository = memberRepository;
            _eventHub = eventHub;
        }

        [Function("ClaimComplete")]
        public async Task Run(
            [CosmosDBTrigger(
                databaseName: Constants.Connections.CosmosDbName,
                containerName: "Claim",
                StartFromBeginning = true,
                Connection = Constants.Connections.CosmosDb,
                LeaseContainerName = "ClaimLeases",
                LeaseContainerPrefix = "ClaimComplete")] IReadOnlyList<ClaimDetail> input,
            FunctionContext context
            )
        {
            /* TODO: Challenge 3.
             * Uncomment and complete the following lines as instructed.
             */
            var logger = context.GetLogger<ClaimComplete>();
            using var logScope = logger.BeginScope("CosmosDbTrigger: ClaimComplete");

            try
            {
                foreach (var claim in input.Where(c =>
                             c.Type == ClaimDetail.EntityName &&
                             c.ClaimStatus is ClaimStatus.Approved or ClaimStatus.Denied))
                {
                    switch (claim.ClaimStatus)
                    {
                        case ClaimStatus.Approved:
                            //TODO: Increment the member totals by calling a method on the Member repository.
                            //      Pass to the method the MemberId, a count of 1, and the total amount from the claim.
                            //await _memberRepository._______(______, ______, ______);
                            await _eventHub.TriggerEventAsync(claim, Constants.EventHubTopics.Approved);
                            break;
                        case ClaimStatus.Denied:
                            await _eventHub.TriggerEventAsync(claim, Constants.EventHubTopics.Denied);
                            break;
                    }

                    logger.LogInformation($"Claim {claim.ClaimId} published to EventHub/{claim.ClaimStatus}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to publish ClaimComplete events");
                throw;
            }
        }
    }
}
