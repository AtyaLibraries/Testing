# Testing

Small test-only helpers for Atya packages and applications.

## Scope

This package should contain reusable testing helpers that are broadly useful across repositories:

- deterministic time helpers
- JSON test assertions
- fake diagnostics and security accessors
- framework-neutral result assertions
- small builders for common test data

## What does not belong

Keep this package intentionally boring. Do not add:

- helpers tied to one package's private internals
- production runtime dependencies
- large test frameworks or mocking frameworks
- random one-off helpers that do not repeat across packages
- web, host, or DI abstractions before a shared need is proven

Production projects must not reference this package. It is for test projects only.

## Helpers

- `FakeClock` exposes controllable `UtcNow`, `Now`, and `Today` values.
- `JsonAssert` compares JSON structurally, ignoring formatting and property order.
- `FakeCorrelationIdAccessor` provides a mutable correlation id for diagnostics tests.
- `FakeCurrentUser` provides a mutable authenticated or anonymous current user.
- `ResultAssertions` checks common result-like shapes without referencing a specific result package.
- `ValidationFailureBuilder` creates framework-neutral validation failure data.
