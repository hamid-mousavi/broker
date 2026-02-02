using System.ComponentModel.DataAnnotations;

namespace Broker.DTOs.CargoOwner
{
    public class CargoOwnerDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? CompanyName { get; set; }
        public string? NationalId { get; set; }
        public string? EconomicCode { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCargoOwnerDto
    {
        [StringLength(200)]
        public string? CompanyName { get; set; }

        [StringLength(20)]
        public string? NationalId { get; set; }

        [StringLength(20)]
        public string? EconomicCode { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? Province { get; set; }
    }

    public class UpdateCargoOwnerDto
    {
        [StringLength(200)]
        public string? CompanyName { get; set; }

        [StringLength(20)]
        public string? NationalId { get; set; }

        [StringLength(20)]
        public string? EconomicCode { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? Province { get; set; }
    }
}


