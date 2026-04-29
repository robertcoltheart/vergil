## About

A thing that does something.

## Usage
Install the package from NuGet with `dotnet add package Vergil`.

```csharp
Example code
```

### Versioning Strategy
- If current commit has a version tag
  - Use the version as-is
- If current commit does not have a version tag
  - Search commit history for highest version tag
    - No tag found
      - Use 0.0.0 with height
    - Tag found
      - Use tag with height

```mermaid
flowchart TD
    A[Current commit] --> |Has tag| B[Use tag]
    A --> |No tag| C[Search history]
    C --> |No tags| D[0.0.0]
    C --> |Highest tag| E[Tag + height]
```

#### Options
1. Choose which part of the version to increment if height needs to be added
2. Use a custom label for pre-release versions, or use the current branch name

## Documentation
See the [wiki](https://github.com/robertcoltheart/vergil/wiki) for examples and help using Vergil.

## Get in touch
Discuss with us on [Discussions](https://github.com/robertcoltheart/vergil/discussions), or raise an [issue](https://github.com/robertcoltheart/vergil/issues).

[![Discussions](https://img.shields.io/badge/DISCUSS-ON%20GITHUB-yellow?style=for-the-badge)](https://github.com/robertcoltheart/vergil/discussions)

## Contributing
Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on how to contribute to this project.
