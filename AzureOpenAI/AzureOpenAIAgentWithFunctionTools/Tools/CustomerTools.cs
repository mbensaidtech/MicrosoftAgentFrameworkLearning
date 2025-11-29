using AzureOpenAIShared;
using System.ComponentModel;

namespace AzureOpenAIAgentWithFunctionTools.Tools;

/// <summary>
/// Tools class containing public methods for customer operations.
/// These methods can be used by the AI agent to perform customer-related actions.
/// </summary>
public class CustomerTools
{
    /// <summary>
    /// Edits the personal information of a customer.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer.</param>
    /// <param name="firstName">The customer's first name.</param>
    /// <param name="lastName">The customer's last name.</param>
    /// <param name="email">The customer's email address.</param>
    /// <param name="phoneNumber">The customer's phone number.</param>
    /// <returns>A message indicating the result of the operation.</returns>
    [Description("Edit the personal information of a customer.")]
    public string EditPersonalInformation(
        string customerId,
        string? firstName = null,
        string? lastName = null,
        string? email = null,
        string? phoneNumber = null)
    {
        var updates = new List<string>();
        
        if (!string.IsNullOrWhiteSpace(firstName))
            updates.Add($"First Name: {firstName}");
        if (!string.IsNullOrWhiteSpace(lastName))
            updates.Add($"Last Name: {lastName}");
        if (!string.IsNullOrWhiteSpace(email))
            updates.Add($"Email: {email}");
        if (!string.IsNullOrWhiteSpace(phoneNumber))
            updates.Add($"Phone Number: {phoneNumber}");

        if (updates.Count == 0)
        {
            return $"No updates provided for customer {customerId}. Personal information remains unchanged.";
        }

        return $"Successfully updated personal information for customer {customerId}. Updated fields: {string.Join(", ", updates)}";
    }

    /// <summary>
    /// Edits the address information for a customer.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer.</param>
    /// <param name="streetAddress">The street address.</param>
    /// <param name="city">The city.</param>
    /// <param name="state">The state or province.</param>
    /// <param name="postalCode">The postal or zip code.</param>
    /// <param name="country">The country.</param>
    /// <returns>A message indicating the result of the address update operation.</returns>
    [Description("Edit the address information for a customer.")]
    public string EditAddress(
        string customerId,
        string? streetAddress = null,
        string? city = null,
        string? state = null,
        string? postalCode = null,
        string? country = null)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            return "Error: Customer ID is required to update address.";
        }

        var addressParts = new List<string>();
        
        if (!string.IsNullOrWhiteSpace(streetAddress))
            addressParts.Add(streetAddress);
        if (!string.IsNullOrWhiteSpace(city))
            addressParts.Add(city);
        if (!string.IsNullOrWhiteSpace(state))
            addressParts.Add(state);
        if (!string.IsNullOrWhiteSpace(postalCode))
            addressParts.Add(postalCode);
        if (!string.IsNullOrWhiteSpace(country))
            addressParts.Add(country);

        if (addressParts.Count == 0)
        {
            return $"No address updates provided for customer {customerId}. Address remains unchanged.";
        }

        var newAddress = string.Join(", ", addressParts);
        return $"Successfully updated address for customer {customerId}. New address: {newAddress}";
    }


    /// <summary>
    /// Updates the payment method for a customer.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer.</param>
    /// <param name="paymentMethodType">The type of payment method (e.g., CreditCard, PayPal, BankAccount).</param>
    /// <param name="paymentMethodDetails">The details of the payment method.</param>
    /// <returns>A message indicating the result of the payment method update operation.</returns>
    [Description("Update the payment method for a customer.")]
    public string UpdatePaymentMethod(
        string customerId,
        string paymentMethodType,
        string paymentMethodDetails)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            return "Error: Customer ID is required to update payment method.";
        }

        if (string.IsNullOrWhiteSpace(paymentMethodType))
        {
            return "Error: Payment method type is required.";
        }

        return $"Payment method successfully updated for customer {customerId}. Payment method type: {paymentMethodType}";
    }

    /// <summary>
    /// Updates the subscription preferences for a customer.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer.</param>
    /// <param name="emailNotifications">Whether to receive email notifications.</param>
    /// <param name="smsNotifications">Whether to receive SMS notifications.</param>
    /// <param name="marketingEmails">Whether to receive marketing emails.</param>
    /// <returns>A message indicating the result of the subscription preferences update operation.</returns>
    [Description("Update the subscription preferences for a customer.")]
    public string UpdateSubscriptionPreferences(
        string customerId,
        bool? emailNotifications = null,
        bool? smsNotifications = null,
        bool? marketingEmails = null)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            return "Error: Customer ID is required to update subscription preferences.";
        }

        var preferences = new List<string>();
        
        if (emailNotifications.HasValue)
            preferences.Add($"Email Notifications: {emailNotifications.Value}");
        if (smsNotifications.HasValue)
            preferences.Add($"SMS Notifications: {smsNotifications.Value}");
        if (marketingEmails.HasValue)
            preferences.Add($"Marketing Emails: {marketingEmails.Value}");

        if (preferences.Count == 0)
        {
            return $"No preference updates provided for customer {customerId}. Preferences remain unchanged.";
        }

        return $"Subscription preferences successfully updated for customer {customerId}. Updated preferences: {string.Join(", ", preferences)}";
    }
}

