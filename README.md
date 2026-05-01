# Atya.Governance.Testing

Small test-only helpers for Atya packages and applications.

This package is intended for test projects only. Production projects should not reference it.

## Helpers

- `FakeClock` exposes controllable `UtcNow`, `Now`, and `Today` values.
- `JsonAssert` compares JSON structurally, ignoring formatting and property order.
- `FakeCorrelationIdAccessor` provides a mutable correlation id for diagnostics tests.
- `FakeCurrentUser` provides a mutable authenticated or anonymous current user.
- `ResultAssertions` checks common result-like shapes without referencing a specific result package.
- `ValidationFailureBuilder` creates framework-neutral validation failure data.

## Development

Restore, build, and test from the repository root:

```powershell
dotnet restore .\Testing.sln
dotnet build .\Testing.sln --configuration Release --no-restore -m:1
dotnet test .\Testing.sln --configuration Release --no-build -m:1
```

Pack the library:

```powershell
dotnet pack .\src\Testing\Testing.csproj --configuration Release --output .\artifacts\packages
```

For local package-feed publishing, see [LOCAL_NUGET.md](LOCAL_NUGET.md).
