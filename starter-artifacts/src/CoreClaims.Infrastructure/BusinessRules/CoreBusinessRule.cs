using CoreClaims.Infrastructure.Domain.Entities;
using CoreClaims.Infrastructure.Domain.Enums;
using CoreClaims.Infrastructure.Repository;
using Microsoft.Extensions.Options;

namespace CoreClaims.Infrastructure.BusinessRules
{
    public class CoreBusinessRule : ICoreBusinessRule
    {
        private readonly IAdjudicatorRepository _adjudicatorRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IOptions<BusinessRuleOptions> _options;
        private readonly Random random;

        public CoreBusinessRule(
            IAdjudicatorRepository adjudicatorRepository,
            IMemberRepository memberRepository,
            IOptions<BusinessRuleOptions> options)
        {
            _adjudicatorRepository = adjudicatorRepository;
            _memberRepository = memberRepository;
            _options = options;
            random = new Random();
        }

        public async Task<ClaimDetail> AssignClaim(ClaimDetail claim)
        {
            // If claim has no member, set status to Pending.
            if (string.IsNullOrEmpty(claim.MemberId))
            {
                claim.ClaimStatus = ClaimStatus.Pending;
                return claim;
            }

            // Check if member has active coverage.
            // If member has active coverage, check if claim is still covered based on dates.
            // If claim is not valid, set status to Rejected.
            var coverages = await _memberRepository.GetMemberCoverage(claim.MemberId);
            if (!coverages.Any(c => c.StartDate <= claim.FilingDate && c.EndDate >= claim.FilingDate))
            {
                claim.ClaimStatus = ClaimStatus.Denied;
                claim.Comment = "[Automatic] Rejected: Coverage expired or missing";
                return claim;
            }

            // If claim's total amount is less than 200, set status to Approved.
            if (claim.TotalAmount < _options.Value.AutoApproveThreshold)
            {
                claim.ClaimStatus = ClaimStatus.Approved;
                claim.Comment = $"[Automatic] Approved: Less than threshold of {_options.Value.AutoApproveThreshold}";
                return claim;
            }

            // Select random adjudicator.
            if (string.IsNullOrEmpty(claim.AdjudicatorId))
            {
                Adjudicator adjudicator;
                if (_options.Value.DemoMode)
                {
                    var demoAdjudicatorId = _options.Value.DemoAdjudicatorId;
                    var demoManagerAdjudicatorId = _options.Value.DemoManagerAdjudicatorId;

                    if (!string.IsNullOrWhiteSpace(demoAdjudicatorId) && !string.IsNullOrWhiteSpace(demoManagerAdjudicatorId))
                    {
                        var randomIndex = random.Next(0, 2);

                        var selectedAdjudicatorId = randomIndex == 0 ? demoAdjudicatorId : demoManagerAdjudicatorId;

                        adjudicator = await _adjudicatorRepository.GetAdjudicator(selectedAdjudicatorId) ?? await _adjudicatorRepository.GetRandomAdjudicator();
                    }
                    else if (!string.IsNullOrWhiteSpace(demoAdjudicatorId))
                    {
                        adjudicator = await _adjudicatorRepository.GetAdjudicator(demoAdjudicatorId) ?? await _adjudicatorRepository.GetRandomAdjudicator();
                    }
                    else if (!string.IsNullOrWhiteSpace(demoManagerAdjudicatorId))
                    {
                        adjudicator = await _adjudicatorRepository.GetAdjudicator(demoManagerAdjudicatorId) ?? await _adjudicatorRepository.GetRandomAdjudicator();
                    }
                    else
                    {
                        adjudicator = await _adjudicatorRepository.GetRandomAdjudicator();
                    }
                }
                else
                {
                    adjudicator = await _adjudicatorRepository.GetRandomAdjudicator();
                }
                
                if (adjudicator != null)
                {
                    // Set Adjudicator to the claim and set status to Assigned.
                    claim.AdjudicatorId = adjudicator.AdjudicatorId;
                    claim.ClaimStatus = ClaimStatus.Assigned;
                    claim.Comment = $"[Automatic] Assigned: Automatically assigned to Adjudicator {adjudicator.AdjudicatorId} ({adjudicator.Role})";
                }
            }

            return claim;
        }

        public async Task<ClaimDetail> AdjudicateClaim(ClaimDetail claim)
        {
            /* TODO: Challenge 3.
             * Uncomment and complete the following lines as instructed.
             */

            // TODO: Retrieve the Adjudicator from the AdjudicatorRepository based on the AdjudicatorId assigned to the claim:
            //var adjudicator = _________________________;

            // TODO: Check if the adjudicator has a Manager role. If so, set status to Approved, add a comment to the claim, and return the claim.
            //if (__ == __)
            //{
            //    claim.ClaimStatus = _____________;
            //    claim.Comment = "[Automatic] Approved: Manager Proposed adjustment";
            //    return ________;
            //}

            if (claim.LastAmount.HasValue)
            {
                // TODO: Check if the difference between the LastAmount and TotalAmount is less than or equal to the RequireManagerApproval threshold.
                // If so, set status to Approved, add a comment to the claim, and return the claim.
                decimal difference = Math.Abs(claim.LastAmount.Value - claim.TotalAmount);
                //if (difference <= ______)
                // {
                //     claim.ClaimStatus = _____________;
                //     claim.Comment = $"[Automatic] Approved: Proposed adjustment below approval threshold {____PRINT_THRESHOLD_AMOUNT_HERE____}";
                //     return ________;
                // }
            }

            Adjudicator manager;
            if (_options.Value.DemoMode && !string.IsNullOrWhiteSpace(_options.Value.DemoManagerAdjudicatorId))
            {
                // TODO: If DemoMode is enabled and a DemoManagerAdjudicatorId is set, retrieve the Adjudicator from the AdjudicatorRepository based on the DemoManagerAdjudicatorId:
                //manager = _________________________;
            }
            else
            {
                // TODO: Retrieve a random Adjudicator from the AdjudicatorRepository with a role of "Manager":
                //manager = _________________________;
            }

            // TODO: If a manager was found, set the claim's PreviousAdjudicatorId to the claim's AdjudicatorId, set the claim's AdjudicatorId to the manager's AdjudicatorId,
            // if (manager == null)
            //     throw new NullReferenceException("Unable to find an appropriate manager to assign approval to");

            // claim.PreviousAdjudicatorId = _____________;
            // claim.AdjudicatorId = _____________;
            // claim.ClaimStatus = ClaimStatus.ApprovalRequired;
            // claim.Comment = "[Automatic] Reassigned: Adjustment requires manager approval";

            return claim;
        }
    }
}
