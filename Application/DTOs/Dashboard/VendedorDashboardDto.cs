namespace Application.DTOs.Dashboard
{
    public class VendedorDashboardDto
    {
        public Guid VendedorId { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public decimal ComissaoTotal { get; set; }
    }
}
