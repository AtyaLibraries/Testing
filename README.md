# Atya.Governance.Testing

Small test-only helpers for Atya packages and applications.

This package is intended for test projects only. Production projects should not reference it.

| | |
| --- | --- |
| Repository | [https://github.com/AtyaLibraries/Testing](https://github.com/AtyaLibraries/Testing) |
| NuGet | `Atya.Governance.Testing` |
| License | MIT |

## Helpers

- `FakeClock` exposes controllable `UtcNow`, `Now`, and `Today` values.
- `JsonAssert` compares JSON structurally, ignoring formatting and property order.
- `FakeCorrelationIdAccessor` provides a mutable correlation id for diagnostics tests.
- `FakeCurrentUser` provides a mutable authenticated or anonymous current user.
- `ResultAssertions` checks common result-like shapes without referencing a specific result package.
- `ValidationFailureBuilder` creates framework-neutral validation failure data.

## Layout

```text
.
|-- src/Atya.Governance.Testing/                    # Shipped library
|-- tests/Atya.Governance.Testing.UnitTests/        # Unit tests
|-- samples/Atya.Governance.Testing.Samples.UnitTests/ # Usage examples
|-- docs/RELEASING.md                               # Release and recovery flow
|-- .github/                                        # GitHub automation
|-- bootstrap.ps1                                   # Repository setup
`-- Directory.Packages.props                        # Central package versions
```

## Development

```bash
dotnet restore ./Atya.Governance.Testing.sln
dotnet format ./Atya.Governance.Testing.sln --verify-no-changes
dotnet build ./Atya.Governance.Testing.sln --configuration Release --no-restore
dotnet test ./Atya.Governance.Testing.sln --configuration Release --no-build
dotnet pack ./src/Atya.Governance.Testing/Atya.Governance.Testing.csproj \
  --configuration Release \
  --no-build \
  --output artifacts/packages \
  -p:EnablePackageValidation=true
```

The first restore creates fresh `packages.lock.json` files for each project.
CI restores in locked mode, verifies formatting, builds on Linux and Windows,
enforces 80% line coverage, validates the package, and uploads symbols.

## GitHub setup

Push `development` and `master`, authenticate the GitHub CLI, and run:

```powershell
./bootstrap.ps1 -RepoOwner AtyaLibraries -RepoName Testing
```

The script configures `development` as the default branch, branch rulesets,
required CI checks, merge methods, and repository labels. Publishing also
requires `NUGET_API_KEY`, `NUGET_SIGN_CERT_BASE64`,
`NUGET_SIGN_CERT_PASSWORD`, and the `REQUIRE_SIGNED_PACKAGES` variable.

## Versioning and releases

MinVer derives package versions from `vMAJOR.MINOR.PATCH` tags. Releases are
promoted from `development` to `master`; the publish workflow supports an
explicit stable SemVer input for controlled releases. See
[`docs/RELEASING.md`](docs/RELEASING.md) for the complete flow.
