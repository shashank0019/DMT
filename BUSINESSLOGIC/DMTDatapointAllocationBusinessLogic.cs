using DMTDatapointAllocation.DATAACCESS;
using DMTDatapointAllocation.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMTDatapointAllocation.BUSINESSLOGIC
{
    public class DMTDatapointAllocationBusinessLogic
    {
        private readonly DMTDatapointAllocationDataAccess _dataAccess;

        public DMTDatapointAllocationBusinessLogic(DMTDatapointAllocationDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public Task<List<TeamDto>> GetAllTeamsAsync()
        {
            return _dataAccess.GetAllTeamsAsync();
        }

        public Task<List<QuarterDto>> GetQuartersAsync()
        {
            return _dataAccess.GetQuartersAsync();
        }

        public Task<List<BudgetDto>> GetBudgetAsync()
        {
            return _dataAccess.GetBudgetAsync();
        }


        public async Task<AllocationResponse> InsertOrUpdateAsync(AllocationRequest request)
        {
            // simple validation example
            if (request.TeamId <= 0)
                return new AllocationResponse { Success = false, Message = "Invalid TeamId" };

            if (request.QuarterId <= 0)
                return new AllocationResponse { Success = false, Message = "Invalid QuarterId" };

            var rid = await _dataAccess.InsertOrUpdateAllocationAsync(request);
            return new AllocationResponse
            {
                Success = rid.HasValue,
                Message = rid.HasValue ? "Saved" : "Failed",
                RID = rid
            };
        }

        public async Task<List<MasterDataDto>> GetMasterDataAsync(int choice, int rid)
        {
            return await _dataAccess.GetMasterDataAsync(choice, rid);
        }


    }
}
