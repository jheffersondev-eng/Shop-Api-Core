namespace Shop.Application.Common.Options;

public sealed class CognitoOptions
{
    public string Region { get; set; }
    public string UserPoolId { get; set; }
    public string AwsAccessKeyId { get; set; }
    public string AwsSecretAccessKey { get; set; }
    public string AwsSessionToken { get; set; }
    public string AwsProfile { get; set; }
    public bool SkipUserProvisioning { get; set; }
}
