using Plooto.Payments.Domain.Enums;
using Plooto.Payments.Domain.Interfaces;

namespace Plooto.Payments.Domain.Entities
{
    public class Payment : IEntity<int>
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DebitDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
