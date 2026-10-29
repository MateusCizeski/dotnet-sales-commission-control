namespace Domain.Enums
{
    public enum UserRole
    {
        /// <summary>Manages sellers, invoices and commissions.</summary>
        Admin = 1,

        /// <summary>Reads only their own invoices, commissions and dashboard.</summary>
        Seller = 2
    }
}
