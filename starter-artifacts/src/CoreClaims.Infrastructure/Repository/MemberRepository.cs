﻿using CoreClaims.Infrastructure.Domain.Entities;
using CoreClaims.Infrastructure.Models;
using Microsoft.Azure.Cosmos;
using System.Security.Claims;

namespace CoreClaims.Infrastructure.Repository
{
    public class MemberRepository : CosmosDbRepository, IMemberRepository
    {
        public MemberRepository(CosmosClient client) :
            base(client, "Member")
        {
        }

        public async Task<IPageResult<ClaimHeader>> ListMemberClaims(
            string memberId,
            int offset = 0,
            int limit = Constants.DefaultPageSize,
            DateTime? startDate = null,
            DateTime? endDate = null,
            bool includeDenied = false,
            string sortColumn = Constants.DefaultSortColumn,
            string sortDirection = "asc")
        {
            sortColumn ??= Constants.DefaultSortColumn;
            string sql;
            int count = 0;
            QueryDefinition query;
            var filterClause = includeDenied ? string.Empty : " AND m.claimStatus != 'Denied'";

            if (startDate != null && endDate != null)
            {
                var countSql = @$"
                            SELECT VALUE COUNT(1) FROM m
                            WHERE m.memberId = @memberId AND m.type = 'ClaimHeader'{filterClause} AND
                                  m.filingDate >= @startDate AND m.filingDate <= @endDate";

                var countQuery = new QueryDefinition(countSql)
                    .WithParameter("@memberId", memberId)
                    .WithParameter("@startDate", startDate)
                    .WithParameter("@endDate", endDate);

                var countResult = await Container.GetItemQueryIterator<int>(countQuery).ReadNextAsync();
                count = countResult.Resource.FirstOrDefault();

                sql = @$"
                    SELECT * FROM m
                    WHERE m.memberId = @memberId AND m.type = 'ClaimHeader'{filterClause} AND
                          m.filingDate >= @startDate AND m.filingDate <= @endDate
                    ORDER BY m.{sortColumn} {sortDirection}
                    OFFSET @offset LIMIT @limit";

                query = new QueryDefinition(sql)
                    .WithParameter("@memberId", memberId)
                    .WithParameter("@offset", offset)
                    .WithParameter("@limit", limit)
                    .WithParameter("@startDate", startDate)
                    .WithParameter("@endDate", endDate);
            }
            else
            {
                var countSql = @$"
                            SELECT VALUE COUNT(1) FROM m
                            WHERE m.memberId = @memberId AND m.type = 'ClaimHeader'{filterClause}";

                var countQuery = new QueryDefinition(countSql)
                    .WithParameter("@memberId", memberId);

                var countResult = await Container.GetItemQueryIterator<int>(countQuery).ReadNextAsync();
                count = countResult.Resource.FirstOrDefault();

                sql = @$"
                    SELECT * FROM m
                    WHERE m.memberId = @memberId AND m.type = 'ClaimHeader'{filterClause}
                    ORDER BY m.{sortColumn} {sortDirection}
                    OFFSET @offset LIMIT @limit";

                query = new QueryDefinition(sql)
                    .WithParameter("@memberId", memberId)
                    .WithParameter("@offset", offset)
                    .WithParameter("@limit", limit);
            }

            var result = await Query<ClaimHeader>(query);
            return new PageResult<ClaimHeader>(count, offset, limit, result);
        }

        public async Task<IEnumerable<Coverage>> GetMemberCoverage(string memberId)
        {
            var query = new QueryDefinition("SELECT * FROM m WHERE m.memberId = @memberId AND m.type = 'Coverage'")
                .WithParameter("@memberId", memberId);

            return await Query<Coverage>(query);
        }

        public async Task<IPageResult<Member>> ListMembers(int offset = 0, int limit = Constants.DefaultPageSize,
            string sortColumn = Constants.DefaultSortColumn,
            string sortDirection = "asc")
        {
            sortColumn ??= Constants.DefaultSortColumn;

            const string countSql = @"
                SELECT VALUE COUNT(1) FROM m WHERE m.type = 'Member'";

            var countQuery = new QueryDefinition(countSql);

            var countResult = await Container.GetItemQueryIterator<int>(countQuery).ReadNextAsync();
            var count = countResult.Resource.FirstOrDefault();

            QueryDefinition query = new QueryDefinition($"SELECT * FROM m WHERE m.type = 'Member' ORDER BY m.{sortColumn} {sortDirection} OFFSET @offset LIMIT @limit")
                .WithParameter("@offset", offset)
                .WithParameter("@limit", limit);

            var result = await Query<Member>(query);
            return new PageResult<Member>(count, offset, limit, result);
        }

        public async Task<Member> IncrementMemberTotals(string memberId, int count, decimal amount)
        {
            /* TODO: Challenge 3.
             * Uncomment and complete the following lines as instructed.
             */
            var response = await Container.PatchItemAsync<Member>(memberId, new PartitionKey(memberId),
                patchOperations: new[]
                {
                    // TODO: Increment the approvedCount and approvedTotal properties with patch operations.
                    //       Convert the amount to a double using the decimal.ToDouble method.
                    //PatchOperation.______("/approvedCount", _______),
                    //PatchOperation.______("/approvedTotal", _______),
                    PatchOperation.Replace("/modifiedBy", "System/IncrementTotals"),
                    PatchOperation.Replace("/modifiedOn", DateTime.UtcNow), 
                });

            return response.Resource;
        }

        public async Task UpsertClaim(ClaimHeader claim)
        {
            await Container.UpsertItemAsync(claim);
        }

        public async Task<Member> Get(string memberId)
        {
            var response = await ReadItem<Member>(memberId, memberId);
            return response;
        }
        
    }
}
