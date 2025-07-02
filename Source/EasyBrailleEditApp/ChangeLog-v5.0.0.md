# v5.0.0

## User features

- Upgrade .NET Framework from 7.0 to 9.0.
- Upgrade dependent NuGet packages to latest versions.

## Breaking changes
 
- The minimum supported version is Windows 10. The application might be able to run on Windows 7, but is not guaranteed to work correctly.

## Developer features

- Use GitHub Actions for CI/CD.
- 使用 Central Package Management (CPM) 來統一管理相依套件的版本。

## Release instructions

Tag a specific commit with a version:

```bash
git tag v5.0.0
git push origin v5.0.0
```		
