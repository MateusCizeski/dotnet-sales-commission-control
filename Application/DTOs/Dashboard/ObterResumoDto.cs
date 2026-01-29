using Domain.Enums;

namespace Application.DTOs.Dashboard
{
    public class ObterResumoDto
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public Guid? vendedorId { get; set; }
        public StatusInvoice? statusInvoice { get; set; }
    }
}
