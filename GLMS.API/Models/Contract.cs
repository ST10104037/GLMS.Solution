using System.ComponentModel.DataAnnotations;

namespace GLMS.API.Models
{
    public enum ContractStatus { Draft, Active, Expired, OnHold }

    public class Contract
    {
        public int Id { get; set; }

        public int ClientId { get; set; }
        public Client? Client { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public ContractStatus Status { get; set; } = ContractStatus.Draft;

        [Required]
        public string ServiceLevel { get; set; } = string.Empty;

        // Path to uploaded PDF on the server
        public string? SignedAgreementPath { get; set; }

        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}
