using Plooto.Payments.Domain.Interfaces;

namespace Plooto.Payments.Domain.Entities
{
    public class Bill : IEntity<int>
    {
        public int Id { get; set; }
        public string VendorName { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
    }
}
