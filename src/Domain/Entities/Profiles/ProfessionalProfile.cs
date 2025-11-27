using Domain.ValueObjects;

namespace Domain.Entities.Profiles
{
    public class ProfessionalProfile
    {
        private ProfessionalProfile() { }

        public ProfessionalProfile(
            Guid userId,
            string documentNumber,
            string speciality,
            Address address,
            bool isPremium = false)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            DocumentNumber = documentNumber;
            Speciality = speciality;
            Address = address;
            IsVerified = false;
            IsPremium = isPremium;
        }

        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        public string DocumentNumber { get; private set; }  // CRM/CRP
        public string Speciality { get; private set; }
        public Address Address { get; private set; }

        public bool IsVerified { get; private set; }
        public string? DocumentFileUrl { get; private set; }

        public bool IsPremium { get; private set; }
        public void Verify(string documentFileUrl)
        {
            DocumentFileUrl = documentFileUrl;
            IsVerified = true;
        }

        public void UpgradeToPremium()
        {
            IsPremium = true;
        }
    }
}
