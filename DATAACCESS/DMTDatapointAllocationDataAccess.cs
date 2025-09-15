using DMTDatapointAllocation.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DMTDatapointAllocation.DATAACCESS
{
    public class DMTDatapointAllocationDataAccess
    {
        private readonly string _connectionString;

        public DMTDatapointAllocationDataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<TeamDto>> GetAllTeamsAsync()
        {
            var result = new List<TeamDto>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("DMTDatapointAllocation_GetAllTeam", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await conn.OpenAsync();
                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    while (await rdr.ReadAsync())
                    {
                        result.Add(new TeamDto
                        {
                            TeamId = rdr.GetInt32(rdr.GetOrdinal("TeamID")),
                            TeamName = rdr.GetString(rdr.GetOrdinal("Team"))
                        });
                    }
                }
            }

            return result;
        }

        public async Task<List<QuarterDto>> GetQuartersAsync()
        {
            var result = new List<QuarterDto>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("DMTDatapointAllocation_GetQuarter", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await conn.OpenAsync();
                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    while (await rdr.ReadAsync())
                    {
                        result.Add(new QuarterDto
                        {
                            QuarterId = rdr.GetInt32(rdr.GetOrdinal("QID")),
                            QuarterName = rdr.GetString(rdr.GetOrdinal("Quarter"))
                        });
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets budget for the selected team+quarter. Adjust returned fields per your SP.
        /// </summary>
        public async Task<List<BudgetDto>> GetBudgetAsync()
        {
            var result = new List<BudgetDto>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("DMTDatapointAllocation_GetBudget", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await conn.OpenAsync();

                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    while (await rdr.ReadAsync())
                    {
                        result.Add(new BudgetDto
                        {
                            BudgetId = rdr.GetInt32(rdr.GetOrdinal("BID")),
                            BudgetName = rdr.GetString(rdr.GetOrdinal("Budget"))
                        });
                    }
                }
            }

            return result;
        }



        /// <summary>
        /// Insert or update allocation. Returns RID (master id) produced by SP.
        /// </summary>
        public async Task<int?> InsertOrUpdateAllocationAsync(AllocationRequest request)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("DMTDatapointAllocation_InsertUpdate", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Add parameters following your SP contract
                cmd.Parameters.AddWithValue("@InitiatorMEMpID", request.InitiatorEmpId);
                cmd.Parameters.AddWithValue("@TeamID", request.TeamId);
                cmd.Parameters.AddWithValue("@QID", request.QuarterId);
                cmd.Parameters.AddWithValue("@BID", request.BudgetId);
                cmd.Parameters.AddWithValue("@DCPoints", request.DCPoints);
                cmd.Parameters.AddWithValue("@DAPoints", request.DAPoints);
                cmd.Parameters.AddWithValue("@Choice", request.Choice);

                // Output param for RID (Master ID)
                var ridParam = new SqlParameter("@RID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(ridParam);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                if (ridParam.Value != DBNull.Value)
                    return Convert.ToInt32(ridParam.Value);

                return null;
            }
        }

        /// <summary>
        /// Get master data for subsequent forms (choice=1, RID)
        /// </summary>
        public async Task<MasterDataDto> GetMasterDataAsync(int rid)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("DMTDatapointAllocation_GetMasterData", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Choice", 1);
                cmd.Parameters.AddWithValue("@RID", rid);

                await conn.OpenAsync();
                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    if (await rdr.ReadAsync())
                    {
                        return new MasterDataDto
                        {
                            RID = rdr.GetInt32(rdr.GetOrdinal("RID")),
                            InitiatorEmpId = rdr.GetInt32(rdr.GetOrdinal("InitiatorEmpId")),
                            TeamId = rdr.GetInt32(rdr.GetOrdinal("TeamID")),
                            QuarterId = rdr.GetInt32(rdr.GetOrdinal("QID")),
                            BudgetId = rdr.GetInt32(rdr.GetOrdinal("BID")),
                            DCPoints = rdr.GetInt32(rdr.GetOrdinal("DCPoints")),
                            DAPoints = rdr.GetInt32(rdr.GetOrdinal("DAPoints")),
                            Choice = rdr.GetInt32(rdr.GetOrdinal("Choice")),
                            CreatedOn = rdr.IsDBNull(rdr.GetOrdinal("CreatedOn")) ? DateTime.MinValue : rdr.GetDateTime(rdr.GetOrdinal("CreatedOn"))
                        };
                    }
                }
            }

            return null;
        }
    }
}
