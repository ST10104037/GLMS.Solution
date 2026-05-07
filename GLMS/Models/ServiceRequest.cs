using System.ComponentModel.DataAnnotations;

namespace GLMS.Models
{
    public enum ServiceRequestStatus { Pending, InProgress, Completed, Cancelled }

    public class ServiceRequest
    {
        public int Id { get; set; }

        public int ContractId { get; set; }
        public Contract? Contract { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        // Amount entered in USD
        public decimal CostUSD { get; set; }

        // Converted to ZAR (saved to database)
        public decimal CostZAR { get; set; }

        public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;
    }
}
