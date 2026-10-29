using ApiBase.Domain.Entities;
using Domain.Exceptions;
using Domain.Validation;
using System.Text.RegularExpressions;

namespace Domain.Entities
{
    public class Seller : EntityGuid
    {
        public const int FullNameMaxLength = 200;
        public const decimal MaxCommissionRate = 15;

        public string FullName { get; private set; } = string.Empty;

        /// <summary>Brazilian individual tax ID (CPF), digits only.</summary>
        public string Cpf { get; private set; } = string.Empty;

        public string Email { get; private set; } = string.Empty;
        public string? Phone { get; private set; }

        /// <summary>Commission percentage applied to the seller's invoices (0 to 15).</summary>
        public decimal CommissionRate { get; private set; }

        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected Seller() { }

        public Seller(string fullName, string cpf, string email, decimal commissionRate, DateTime createdAt, string? phone = null)
        {
            ValidateFullName(fullName);

            if (string.IsNullOrWhiteSpace(cpf))
            {
                throw new DomainException("CPF is invalid.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new DomainException("Email is invalid.");
            }

            cpf = NormalizeCpf(cpf);
            ValidateCpf(cpf);
            ValidateEmail(email);
            ValidateCommissionRate(commissionRate);

            FullName = fullName;
            Cpf = cpf;
            Email = email;
            CommissionRate = commissionRate;
            Phone = phone;
            IsActive = true;
            CreatedAt = createdAt;
        }

        public void UpdateDetails(string fullName, decimal commissionRate)
        {
            ValidateFullName(fullName);
            ValidateCommissionRate(commissionRate);

            FullName = fullName;
            CommissionRate = commissionRate;
        }

        public void Deactivate() => IsActive = false;

        public void Activate() => IsActive = true;

        public void EnsureIsActive()
        {
            if (!IsActive)
            {
                throw new DomainException("Seller is inactive.");
            }
        }

        public static string NormalizeCpf(string cpf)
        {
            return Regex.Replace(cpf ?? string.Empty, "[^0-9]", "");
        }

        private static void ValidateFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new DomainException("Full name is required.");
            }

            if (fullName.Length > FullNameMaxLength)
            {
                throw new DomainException($"Full name must have at most {FullNameMaxLength} characters.");
            }
        }

        private static void ValidateCpf(string cpf)
        {
            if (!CpfValidator.CpfIsValid(cpf))
            {
                throw new DomainException("CPF is invalid.");
            }
        }

        private static void ValidateEmail(string email)
        {
            if (!EmailValidator.EmailIsValid(email))
            {
                throw new DomainException("Email is invalid.");
            }
        }

        private static void ValidateCommissionRate(decimal commissionRate)
        {
            if (commissionRate < 0 || commissionRate > MaxCommissionRate)
            {
                throw new DomainException($"Commission rate must be between 0% and {MaxCommissionRate}%.");
            }
        }
    }
}
