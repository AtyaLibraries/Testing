# Atya.Governance.Testing

Small test-only helpers for Atya packages and applications.

This package is intended for test projects only. Production projects should
not reference it.

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
|-- Testing.sln
|-- src/Testing/Testing.csproj
|-- tests/Testing.UnitTests/
|-- samples/Testing.Samples.Console/
|-- docs/RELEASING.md
|-- .github/
`-- Directory.Packages.props
```

`Atya.Governance.Testing` is the public package, assembly, root namespace, and
C# namespace identity. `Testing` is used only for local solution, project,
folder, workflow, and non-shipping assembly names.

## Development

```powershell
dotnet restore .\Testing.sln
dotnet format .\Testing.sln --verify-no-changes --no-restore
dotnet build .\Testing.sln --configuration Release --no-restore
dotnet test .\tests\Testing.UnitTests\Testing.UnitTests.csproj --configuration Release --no-build
dotnet pack .\src\Testing\Testing.csproj --configuration Release --no-build --output .\artifacts\packages -p:EnablePackageValidation=true
```

The first restore creates `packages.lock.json` files. CI restores in locked
mode, verifies formatting, builds on Linux and Windows, enforces 80% line
coverage, validates the package, and uploads package and symbol artifacts.

## GitHub setup

Push `development` and `master`, authenticate the GitHub CLI, and run:

```powershell
./bootstrap.ps1 -RepoOwner AtyaLibraries -RepoName Testing
```

Set the `NUGET_API_KEY`, `NUGET_SIGN_CERT_BASE64`, and
`NUGET_SIGN_CERT_PASSWORD` secrets. Set `REQUIRE_SIGNED_PACKAGES` to `false`
only for an explicit unsigned-publishing exception.

## Versioning

MinVer derives package versions from `vMAJOR.MINOR.PATCH` tags. Releases are
promoted from `development` to `master`; the publish workflow signs and pushes
the package, creates the version tag, and creates the GitHub Release. See
[docs/RELEASING.md](docs/RELEASING.md) for the full process.
