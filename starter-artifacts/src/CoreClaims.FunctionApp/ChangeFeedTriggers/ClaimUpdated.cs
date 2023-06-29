using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreClaims.FunctionApp.HttpTriggers.Claims;
using CoreClaims.Infrastructure;
using CoreClaims.Infrastructure.Domain.Entities;
using CoreClaims.Infrastructure.Events;
using CoreClaims.Infrastructure.Repository;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoreClaims.FunctionApp.ChangeFeedTriggers
{
    public class ClaimUpdated
    {
        private readonly IAdjudicatorRepository _adjudicatorRepository;
        private readonly IMemberRepository _memberRepository;

        public ClaimUpdated(
            IAdjudicatorRepository adjudicatorRepository,
            IMemberRepository memberRepository)
        {
            _adjudicatorRepository = adjudicatorRepository;
            _memberRepository = memberRepository;
        }

        [Function("ClaimUpdated")]
        public async Task Run(
            [CosmosDBTrigger(databaseName: Constants.Connections.CosmosDbName,
                containerName: "Claim",
                StartFromBeginning = true,
                Connection = Constants.Connections.CosmosDb,
                LeaseContainerName = "ClaimLeases",
                LeaseContainerPrefix = "PropagateClaimHeader")] IReadOnlyList<ClaimHeader> input,
            FunctionContext context)
        {
            /* TODO: Challenge 3.
             * Uncomment and complete the following lines as instructed.
             */
            var logger = context.GetLogger<ClaimUpdated>();
            using var logScope = logger.BeginScope("CosmosDbTrigger: ClaimUpdated");

            var headers = input.Where(i => i.Type == ClaimHeader.EntityName);

            foreach (var claim in headers)
            {
                if (!string.IsNullOrEmpty(claim.MemberId))
                {
                    // TODO: Upsert the claim in the Member repository.
                    //await __________;
                    logger.LogInformation($"Updating ClaimHeader/{claim.ClaimId}/{claim.AdjustmentId} for Member/{claim.MemberId}");
                }

                if (!string.IsNullOrEmpty(claim.AdjudicatorId))
                {
                    // TODO: Upsert the claim in the Adjudicator repository.
                    //await __________;
                    logger.LogInformation($"Updating ClaimHeader/{claim.ClaimId}/{claim.AdjustmentId} for Adjudicator/{claim.AdjudicatorId}");
                }
            }
        }
    }
}
