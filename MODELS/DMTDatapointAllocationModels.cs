using System;

namespace DMTDatapointAllocation.Models
{
    // Team DTO
    public class TeamDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }
    }

    // Quarter DTO
    public class QuarterDto
    {
        public int QuarterId { get; set; }
        public string QuarterName { get; set; }
    }

    // Budget DTO
    public class BudgetDto
    {
        public int BudgetId { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
    }

    // Request to insert/update allocation
    public class AllocationRequest
    {
        public int InitiatorEmpId { get; set; }    // @InitiatorMEMpID
        public int TeamId { get; set; }            // @TeamID
        public int QuarterId { get; set; }         // @QID
        public int BudgetId { get; set; }          // @BID
        public int DCPoints { get; set; }          // @DCPoints
        public int DAPoints { get; set; }          // @DAPoints
        /// <summary>
        /// 1 = Insert, 2 = Update
        /// </summary>
        public int Choice { get; set; } = 1;       // @Choice
        /// <summary>
        /// If updating, pass existing MasterID; otherwise 0 or null.
        /// </summary>
        public int? RID { get; set; }              // @RID (output for insert)
    }

    // Response after insert/update
    public class AllocationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? RID { get; set; }
    }

    // Master data returned for subsequent forms
    public class MasterDataDto
    {
        public int RID { get; set; }
        public int InitiatorEmpId { get; set; }
        public int TeamId { get; set; }
        public int QuarterId { get; set; }
        public int BudgetId { get; set; }
        public int DCPoints { get; set; }
        public int DAPoints { get; set; }
        public int Choice { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
