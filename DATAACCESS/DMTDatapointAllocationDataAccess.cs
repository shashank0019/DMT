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
        public async Task<List<MasterDataDto>> GetMasterDataAsync(int choice, int rid)
        {
            var results = new List<MasterDataDto>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("DMTDatapointAllocation_GetMasterData", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Choice", choice);
                cmd.Parameters.AddWithValue("@RID", rid);

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var dto = new MasterDataDto
                        {
                            RID = reader.GetInt32(reader.GetOrdinal("RID")),
                            InstanceID = reader.GetInt64(reader.GetOrdinal("InstanceID")),

                            // ✅ Corrected: matches DB column exactly from SP
                            InitiatorMEmplID = reader.GetInt32(reader.GetOrdinal("InitiatorMEmplID")),

                            WFStatus = reader.GetInt32(reader.GetOrdinal("WFStatus")),
                            TeamID = reader.GetInt32(reader.GetOrdinal("TeamID")),
                            Quarter = reader.GetInt32(reader.GetOrdinal("Quarter")),
                            Budget = reader.GetDecimal(reader.GetOrdinal("Budget")),
                            DCPoints = reader.GetDecimal(reader.GetOrdinal("DCPoints")),
                            DAPoints = reader.GetDecimal(reader.GetOrdinal("DAPoints")),
                            EntryDate = reader.GetDateTime(reader.GetOrdinal("EntryDate")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                        };

                        results.Add(dto);
                    }
                }
            }

            return results;
        }


    }
}
