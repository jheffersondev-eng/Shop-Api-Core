namespace Shop.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string CognitoSub { get; }
}
