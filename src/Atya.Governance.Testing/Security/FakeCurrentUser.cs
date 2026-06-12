using System.Security.Claims;

namespace Atya.Governance.Testing.Security;

/// <summary>
/// Mutable test double for code that depends on the current user.
/// </summary>
public sealed class FakeCurrentUser
{
    private readonly List<Claim> _claims = [];
    private readonly HashSet<string> _roles = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets a value indicating whether the user is authenticated.
    /// </summary>
    public bool IsAuthenticated => UserId is not null;

    /// <summary>
    /// Gets the current user id.
    /// </summary>
    public string? UserId { get; private set; }

    /// <summary>
    /// Gets the current user name.
    /// </summary>
    public string? UserName { get; private set; }

    /// <summary>
    /// Gets the current user roles.
    /// </summary>
    public IReadOnlyCollection<string> Roles => _roles.ToArray();

    /// <summary>
    /// Gets the current user claims.
    /// </summary>
    public IReadOnlyCollection<Claim> Claims => _claims.ToArray();

    /// <summary>
    /// Gets a claims principal for the configured user.
    /// </summary>
    public ClaimsPrincipal Principal
    {
        get
        {
            ClaimsIdentity identity = IsAuthenticated
                ? new ClaimsIdentity(_claims, "Test")
                : new ClaimsIdentity();

            return new ClaimsPrincipal(identity);
        }
    }

    /// <summary>
    /// Configures an authenticated user.
    /// </summary>
    /// <param name="userId">User id.</param>
    /// <param name="userName">Optional user name.</param>
    /// <param name="roles">Optional roles.</param>
    /// <returns>The current fake user.</returns>
    public FakeCurrentUser SetAuthenticated(string userId, string? userName = null, IEnumerable<string>? roles = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        SetAnonymous();

        UserId = userId;
        UserName = userName;
        _claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));

        if (!string.IsNullOrWhiteSpace(userName))
        {
            _claims.Add(new Claim(ClaimTypes.Name, userName));
        }

        if (roles is not null)
        {
            foreach (string role in roles)
            {
                AddRole(role);
            }
        }

        return this;
    }

    /// <summary>
    /// Adds a role to the current user.
    /// </summary>
    /// <param name="role">Role name.</param>
    /// <returns>The current fake user.</returns>
    public FakeCurrentUser AddRole(string role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(role);

        if (_roles.Add(role))
        {
            _claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return this;
    }

    /// <summary>
    /// Adds a claim to the current user.
    /// </summary>
    /// <param name="type">Claim type.</param>
    /// <param name="value">Claim value.</param>
    /// <returns>The current fake user.</returns>
    public FakeCurrentUser AddClaim(string type, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        ArgumentNullException.ThrowIfNull(value);

        _claims.Add(new Claim(type, value));

        return this;
    }

    /// <summary>
    /// Returns whether the current user has the supplied role.
    /// </summary>
    /// <param name="role">Role name.</param>
    /// <returns><see langword="true"/> when the role is present.</returns>
    public bool IsInRole(string role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(role);

        return _roles.Contains(role);
    }

    /// <summary>
    /// Resets the fake user to anonymous.
    /// </summary>
    public void SetAnonymous()
    {
        UserId = null;
        UserName = null;
        _roles.Clear();
        _claims.Clear();
    }
}
