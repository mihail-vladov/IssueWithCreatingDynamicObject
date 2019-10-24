# IssueWithCreatingDynamicObject

This repo is created to represent a problem with creating a dynamic object in .NET Core 3.0.

The solution contains one .NET Core 2.0 project that contains the logic for creating the dynamic object. And one multitargeting test project targeting .NET Core 2.2 and 3.0.

Steps to reproduce:
1. Open and build the solution.
2. Run the tests.

**Actual Result**: The test for .NET Core 3.0 will fail.
**Expected Result**: The test for .NET Core 3.0 should pass as the one for 2.2 as they are the same.
