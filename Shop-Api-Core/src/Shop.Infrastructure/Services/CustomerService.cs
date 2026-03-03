using Shop.Application.Common.Interfaces;
using Shop.Application.Common.Options;
using Shop.Application.Customers.Commands;
using Shop.Application.Customers.Interfaces;
using Shop.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Shop.Infrastructure.Services;

public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICognitoUserService _cognitoUserService;
    private readonly CognitoOptions _cognitoOptions;

    public CustomerService(
        ICustomerRepository customerRepository,
        ICurrentUserService currentUserService,
        ICognitoUserService cognitoUserService,
        IOptions<CognitoOptions> cognitoOptions)
    {
        _customerRepository = customerRepository;
        _currentUserService = currentUserService;
        _cognitoUserService = cognitoUserService;
        _cognitoOptions = cognitoOptions.Value;
    }

    public async Task<Customer> CreateAsync(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var cognitoSub = _currentUserService.CognitoSub;
        if (string.IsNullOrWhiteSpace(cognitoSub))
        {
            if (_cognitoOptions.SkipUserProvisioning)
            {
                cognitoSub = $"local-dev-{Guid.NewGuid():N}";
            }
            else
            {
                cognitoSub = await _cognitoUserService.CreateUserAsync(request, cancellationToken);
            }
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Email is required.", nameof(request.Email));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name is required.", nameof(request.Name));
        }

        var normalizedSub = cognitoSub.Trim();
        var normalizedEmail = request.Email.Trim();
        var normalizedName = request.Name.Trim();
        var normalizedPhone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();

        var existingBySub = await _customerRepository.GetByCognitoSubAsync(normalizedSub, cancellationToken);
        if (existingBySub is not null)
        {
            throw new InvalidOperationException("CognitoSub already exists.");
        }

        var existingByEmail = await _customerRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existingByEmail is not null)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            CognitoSub = normalizedSub,
            Email = normalizedEmail,
            Name = normalizedName,
            Phone = normalizedPhone,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _customerRepository.SaveChangesAsync(cancellationToken);
        return customer;
    }

    public Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _customerRepository.GetByIdAsync(id, cancellationToken);
    }
}
