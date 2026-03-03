using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using System.Text;
using Microsoft.Extensions.Options;
using Shop.Application.Common.Options;
using Shop.Application.Customers.Commands;
using Shop.Application.Customers.Interfaces;

namespace Shop.Infrastructure.Services;

public sealed class CognitoUserService : ICognitoUserService
{
    private readonly IAmazonCognitoIdentityProvider _cognito;
    private readonly CognitoOptions _options;

    public CognitoUserService(IOptions<CognitoOptions> options)
    {
        _options = options.Value;
        var credentials = ResolveCredentials(_options);
        _cognito = new AmazonCognitoIdentityProviderClient(
            credentials,
            RegionEndpoint.GetBySystemName(_options.Region));
    }

    public async Task<string> CreateUserAsync(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var username = BuildUsername(request.Email);

        var createRequest = new AdminCreateUserRequest
        {
            UserPoolId = _options.UserPoolId,
            Username = username,
            DesiredDeliveryMediums = new List<string> { DeliveryMediumType.EMAIL },
            UserAttributes = BuildAttributes(request)
        };

        try
        {
            var response = await _cognito.AdminCreateUserAsync(createRequest, cancellationToken);
            return GetSub(response.User.Attributes);
        }
        catch (UsernameExistsException)
        {
            var existing = await _cognito.AdminGetUserAsync(new AdminGetUserRequest
            {
                UserPoolId = _options.UserPoolId,
                Username = username
            }, cancellationToken);

            return GetSub(existing.UserAttributes);
        }
        catch (AmazonServiceException ex) when (ex.Message.Contains("Unable to get IAM security credentials from EC2 Instance Metadata Service."))
        {
            throw new InvalidOperationException(
                "AWS credentials are not configured for Cognito access. " +
                "Set Authentication:Cognito:AwsAccessKeyId and Authentication:Cognito:AwsSecretAccessKey, " +
                "or configure an AWS profile and set Authentication:Cognito:AwsProfile.",
                ex);
        }
        catch (AmazonServiceException ex)
        {
            throw new InvalidOperationException(
                $"Cognito request failed ({ex.ErrorCode}): {ex.Message}",
                ex);
        }
    }

    private static AWSCredentials ResolveCredentials(CognitoOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.AwsAccessKeyId)
            && !string.IsNullOrWhiteSpace(options.AwsSecretAccessKey))
        {
            if (!string.IsNullOrWhiteSpace(options.AwsSessionToken))
            {
                return new SessionAWSCredentials(
                    options.AwsAccessKeyId,
                    options.AwsSecretAccessKey,
                    options.AwsSessionToken);
            }

            return new BasicAWSCredentials(
                options.AwsAccessKeyId,
                options.AwsSecretAccessKey);
        }

        if (!string.IsNullOrWhiteSpace(options.AwsProfile)
            && new CredentialProfileStoreChain().TryGetAWSCredentials(options.AwsProfile, out var profileCredentials))
        {
            return profileCredentials;
        }

        return FallbackCredentialsFactory.GetCredentials();
    }

    private static List<AttributeType> BuildAttributes(CreateCustomerCommand request)
    {
        var attributes = new List<AttributeType>
        {
            new AttributeType { Name = "email", Value = request.Email }
        };

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            attributes.Add(new AttributeType { Name = "name", Value = request.Name });
        }

        if (!string.IsNullOrWhiteSpace(request.Phone))
        {
            attributes.Add(new AttributeType { Name = "phone_number", Value = request.Phone });
        }

        return attributes;
    }

    private static string BuildUsername(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var builder = new StringBuilder(normalizedEmail.Length + 5);
        builder.Append("usr-");

        foreach (var ch in normalizedEmail)
        {
            if (char.IsLetterOrDigit(ch))
            {
                builder.Append(ch);
                continue;
            }

            if (ch is '-' or '_' or '.')
            {
                builder.Append(ch);
                continue;
            }

            builder.Append('-');
        }

        return builder.ToString();
    }

    private static string GetSub(List<AttributeType> attributes)
    {
        foreach (var attribute in attributes)
        {
            if (attribute.Name == "sub")
            {
                return attribute.Value;
            }
        }

        throw new InvalidOperationException("Cognito did not return a sub attribute.");
    }
}
