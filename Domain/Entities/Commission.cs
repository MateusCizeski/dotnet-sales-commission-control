using ApiBase.Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities
{
    /// <summary>
    /// Commission of one invoice. It is part of the Invoice aggregate: it is created and
    /// recalculated by the invoice, so invoice and commission are always saved together.
    /// Status flow: Pending → Paid | Cancelled.
    /// </summary>
    public class Commission : EntityGuid
    {
        public Guid InvoiceId { get; private set; }

        /// <summary>Invoice total used as the calculation base.</summary>
        public decimal BaseAmount { get; private set; }

        /// <summary>Percentage applied (the seller's rate at calculation time).</summary>
        public decimal Rate { get; private set; }

        public decimal Amount { get; private set; }
        public CommissionStatus Status { get; private set; }
        public DateTime CalculatedAt { get; private set; }
        public DateTime? PaidAt { get; private set; }

        public Invoice? Invoice { get; private set; }

        protected Commission() { }

        internal Commission(Invoice invoice, decimal rate, DateTime calculatedAt)
        {
            Invoice = invoice;
            InvoiceId = invoice.Id;
            Status = CommissionStatus.Pending;
            Apply(invoice.TotalAmount, rate, calculatedAt);
        }

        /// <summary>The single place where the commission rule lives.</summary>
        public static decimal Calculate(decimal baseAmount, decimal rate)
        {
            return Math.Round(baseAmount * (rate / 100), 2);
        }

        public void Pay(DateTime paidAt)
        {
            if (Status == CommissionStatus.Paid)
            {
                throw new DomainException("Commission is already paid.");
            }

            if (Status == CommissionStatus.Cancelled)
            {
                throw new DomainException("A cancelled commission cannot be paid.");
            }

            Status = CommissionStatus.Paid;
            PaidAt = paidAt;
        }

        public void Cancel()
        {
            if (Status == CommissionStatus.Paid)
            {
                throw new DomainException("A paid commission cannot be cancelled.");
            }

            if (Status == CommissionStatus.Cancelled)
            {
                throw new DomainException("Commission is already cancelled.");
            }

            Status = CommissionStatus.Cancelled;
        }

        internal void Recalculate(decimal baseAmount, decimal rate, DateTime calculatedAt)
        {
            if (Status != CommissionStatus.Pending)
            {
                throw new DomainException("Only a pending commission can be recalculated.");
            }

            Apply(baseAmount, rate, calculatedAt);
        }

        private void Apply(decimal baseAmount, decimal rate, DateTime calculatedAt)
        {
            BaseAmount = baseAmount;
            Rate = rate;
            Amount = Calculate(baseAmount, rate);
            CalculatedAt = calculatedAt;
        }
    }
}
