using Domain.Enums;
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

        public string DocumentNumber { get; private set; }
        public string Speciality { get; private set; }
        public Address Address { get; private set; }

        public bool IsVerified { get; private set; }
        public string? DocumentFileUrl { get; private set; }
        public bool IsPremium { get; private set; }
        public string? SubscriptionId { get; private set; }
        public DateTime? PremiumActivatedAt { get; private set; }
        public DateTime? NextBillingDate { get; private set; }
        public SubscriptionStatus? SubscriptionStatus { get; private set; }
        public PremiumTier Tier { get; private set; } = PremiumTier.None;
        public int FailedAttempts { get; private set; }
        public DateTime? NextRetryAt { get; private set; }

        public void MarkPaymentFailedWithRetry()
        {
            FailedAttempts++;

            if (FailedAttempts == 1)
                NextRetryAt = DateTime.UtcNow.AddHours(24);

            else if (FailedAttempts == 2)
                NextRetryAt = DateTime.UtcNow.AddHours(48);

            else if (FailedAttempts >= 3)
                CancelPremium();
            else
                NextRetryAt = null;
        }

        public void MarkSubscriptionPending()
        {
            SubscriptionStatus = Domain.Enums.SubscriptionStatus.Pending;
            IsPremium = false;
            NextBillingDate = null;
            PremiumActivatedAt = null;
        }

        public void ResetPaymentFailures()
        {
            FailedAttempts = 0;
            NextRetryAt = null;
        }
        public void ActivatePremiumPlus(string subscriptionId, DateTime nextBillingDate)
        {
            IsPremium = true;
            SubscriptionId = subscriptionId;
            PremiumActivatedAt = DateTime.UtcNow;
            NextBillingDate = nextBillingDate;
            SubscriptionStatus = Domain.Enums.SubscriptionStatus.Active;

            Tier = PremiumTier.PremiumPlus;
        }
        public void MarkSubscriptionCreated(string subscriptionId, PremiumTier tier)
        {
            SubscriptionId = subscriptionId;
            Tier = tier;
            SubscriptionStatus = Domain.Enums.SubscriptionStatus.Pending;
        }
        public void ActivatePremium(string subscriptionId, DateTime nextBillingDate)
        {
            IsPremium = true;
            SubscriptionId = subscriptionId;
            PremiumActivatedAt = DateTime.UtcNow;
            NextBillingDate = nextBillingDate;
            Tier = PremiumTier.Premium;
            SubscriptionStatus = Domain.Enums.SubscriptionStatus.Active;
        }
        public void UpdateBilling(DateTime nextBillingDate)
        {
            NextBillingDate = nextBillingDate;
        }
        public void CancelPremium()
        {
            SubscriptionStatus = Domain.Enums.SubscriptionStatus.Cancelled;
            IsPremium = false;
            Tier = PremiumTier.None;
        }
        public void PausePremium()
        {
            IsPremium = false;
            SubscriptionStatus = Domain.Enums.SubscriptionStatus.Paused;
        }

        public void MarkChargeFailed()
        {
            IsPremium = false;
            SubscriptionStatus = Domain.Enums.SubscriptionStatus.ChargeFailed;
        }

        public void Verify(string documentFileUrl)
        {
            DocumentFileUrl = documentFileUrl;
            IsVerified = true;
        }

        public void UpdateProfile(string speciality,  Address address)
        {
            Address = address;
            Speciality = speciality;
        }
    }
}
