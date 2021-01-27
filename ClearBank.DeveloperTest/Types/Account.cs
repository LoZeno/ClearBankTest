namespace ClearBank.DeveloperTest.Types
{
    public record Account(
        string AccountNumber,
        decimal Balance,
        AccountStatus Status,
        AllowedPaymentSchemes AllowedPaymentSchemes)
    {
    }
}