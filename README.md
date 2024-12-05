# Automation-Tests-Aperture

Automation testing project covering API and UI tests for the Aperture assignment. Includes, reporting, and CI/CD integration.

## **Structure**

This repository contains the following projects:

- **API_Tests**: Automated tests for the [FakeStoreAPI](https://fakestoreapi.com/docs) endpoints.
- **UI_Tests**: Selenium-based UI tests for [Sauce Demo](https://www.saucedemo.com).

## **Installation**

You need to install:

- [Visual Studio 2022](https://visualstudio.microsoft.com/) (or later)
- [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Google Chrome](https://www.google.com/chrome/) (Latest version)
- [Git](https://git-scm.com/)

## **Usage**

### **Clone the Repository**

```bash
git clone https://github.com/suwitbuy123/Automation-Tests-Aperture.git
cd Automation-Tests-Aperture
```

### **Restore and Build**

1. Restore the required NuGet packages:

```bash
dotnet restore
```

2. Build the solution:

```bash
dotnet build
```

### **Running the Tests**

1. Run API Tests
   To execute the API tests, use the following command:

```bash
dotnet test API_Tests/API_Tests.csproj --no-build --verbosity normal
```

2. Run UI Tests
   To execute the UI tests, use the following command:

```bash
dotnet test UI_Tests/UI_Tests.csproj --no-build --verbosity normal
```

### **Test Data Configuration**

TestCredentials.json
Ensure the file TestCredentials.json is available in the UI_Tests/TestData folder. This file contains credentials for the Sauce Demo login:

```json
{
  "Usernames": [
    "standard_user",
    "locked_out_user",
    "problem_user",
    "performance_glitch_user",
    "error_user",
    "visual_user"
  ],
  "Password": "secret_sauce"
}
```

### **Troubleshooting**

1. ChromeDriver Version Mismatch

- If the workflow fails with a 404 Not Found error during ChromeDriver installation:

  - Verify the installed Chrome version:

  ```bash
  google-chrome --version
  ```

  - Adjust the workflow to use an automated Chrome and ChromeDriver setup:

  ```yaml
  - name: Set up Chrome and ChromeDriver
    uses: browser-actions/setup-chrome@v1
  ```

2. Missing Test Data

   - If TestCredentials.json is not found, ensure it exists in:

   ```bash
   UI_Tests/TestData/TestCredentials.json
   ```

3. Debugging Tests Locally

   - Use the Test Explorer in Visual Studio to debug individual tests.
   - For failed tests, check logs in UI_Tests/bin/Debug/net9.0/Screenshots/.

### **Contact**

For questions or issues:

- Open a GitHub Issue in this repository.
- Reach out to the repository owner.
