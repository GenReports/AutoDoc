# AutoDoc - Git to Report 📚

**AutoDoc** is a simple but powerful multiplatform tool that automatically analyzes Git commits and generates structured daily CSV reports.  
It leverages AI to transform commit history into meaningful documentation with context-aware summaries.

---

## 🚀 How It Works

1. **Collect Commits**  
   AutoDoc scans a Git repository for commits within a given period.

2. **Context Injection**  
   A custom `Context.txt` file allows you to provide your project background informations for the AI model.

3. **Commit Processing**  
   - Commits are grouped by day.  
   - If a day contains many commits, they are split into smaller parts.  
   - Each part is sent to the model for structured summarization.

4. **AI Integration**  
   - Can use OpenAI APIs like (`/v1/chat/completions`).  
   - Automatically retries failed requests and waits between calls to avoid memory overload.

5. **CSV Generation**  
   After processing all commits for a day, AutoDoc generates a uniquely named CSV file with the results.

---

## ⚙️ Configuration

All settings are handled in the `appsettings.json` file:

```json
{
  "RepositoryPath": "C:\\Path\\To\\Your\\Repository",
  "OwnerEmail": "example@email.com",
  "OwnerName": "Your Name",
  "Culture": "en-US",
  "OutputPath": "AutoDoc-Reports",
  "CompletionsUri": "http://localhost:1234/v1/chat/completions",
  "ApiKey": "your_api_key_here",
  "DelayMilliseconds": 20000,
  "ModelTemperature": 0.5,
  "MaxRetries": 2
}
```

🔢 Explanation of Parameters:
- RepositoryPath       → Path to the Git repository to analyze.
- OwnerEmail           → Git email address for filtering commits.
- OwnerName            → Who is responsible for the commits.
- Culture              → Defines reporting culture/locale (e.g., pt-BR, en-US).
- OutputPath           → Directory where daily CSV files are saved.
- CompletionsUri       → API endpoint.
- ApiKey               → API Key if applicable.
- DelayMilliseconds    → Delay between requests (prevents RAM overload).
- ModelTemperature     → Controls creativity of the model (0 = focused, 1 = creative).
- MaxRetries           → How many times to retry if a request fails.

---

## 📝 Context File
Inside the project, you will find Context.txt.
This file is user-editable and allows you to context.

You can follows the template:
```text
The project is called [Your Project Name], an [Your Project description].

[Talk about Your Project features]...

The project is developed in/with: [Your Project languages and tools].
```

---

## 📦 Requirements
Access to an AI server.
Ensure it listens on the same port as configured in appsettings.json (default: http://localhost:1234).

---

## ▶️ Usage

1 - Go to releases: [Releases](https://github.com/IseduardoRezende/AutoDoc/releases)

2 - Install the zip  

3 - Edit appsettings.json with your repository path and preferences.

4 - Edit Context.txt with your project context.

5 - Make sure AI server is reachable.

6 - Execute AutoDoc.exe 

  - Option 1: [StartDate] [EndDate] | Explanation: The folllowing arguments will generate 1 month of reports.
  ```text
 PS C:\Users\eduar> C:\Users\eduar\Desktop\AutoDoc-win-x64\AutoDoc.exe 2025-01-01 2025-02-01
  ```
  - Option 2: [Days] | Explanation: The folllowing argument will generate 1 week of reports from the current day.
   ```text
 PS C:\Users\eduar> C:\Users\eduar\Desktop\AutoDoc-win-x64\AutoDoc.exe 7
  ```
  - Option 3: No arguments | Explanation: Will generate reports from the current day.
 ```text
 PS C:\Users\eduar> C:\Users\eduar\Desktop\AutoDoc-win-x64\AutoDoc.exe
  ```

7 - Check the output folder for generated CSV reports.

---

## 📊 Example Output

After processing, AutoDoc will generate CSV files named by [repoName]-[date]-[guidV7].
Each row contains structured commit summaries enriched with AI-generated insights.

Example file:
nice-api-08-14-2025-0198b3ccd15b77a295e7e23516fd3217.csv

Example file content output:

```csv
Date;Step;Activity;Description;Motivation;Process;Result;Participants
08/14/2025 13:50:16;Unit Tests;Addition of tests for PersonService;Implementation of unit tests for the 'PersonService' service layer in Nice Acesso.;Ensure the quality and correctness of the operations performed by the 'Person' entity in the access control system.;Use of the xUnit tool for the creation and execution of unit tests.;Addition of tests that validate the basic operations of the 'Person' entity in the system.;Eduardo Rezende
08/14/2025 13:51:08;Tests;Addition of tests for PersonVehicleService;Implementation of unit tests for the PersonVehicleService service layer of the Nice Acesso system.;Ensure the quality and stability of the functionalities related to the association between people and vehicles in the system.;Use of the xUnit testing library, Visual Studio, and .NET framework;Addition of a set of tests that verify the expected behavior of the PersonVehicleService service layer.;Eduardo Rezende
08/14/2025 13:51:30;Unit Tests;Addition of tests for PersonTriggerService;Creation and implementation of unit tests for the PersonTriggerService class, responsible for controlling the interactions between people and triggers in the Nice Acesso system.;Ensure code quality and prevent potential bugs in business logic.;Use of the NUnit library for the creation and execution of unit tests, along with the Visual Studio IDE.;Addition of new tests that can be executed to validate the functionality of the PersonTriggerService class.;Eduardo Rezende
08/14/2025 15:53:42;Development;Refactor: Update PersonService;Update of the GetByType method of the PersonService class to avoid returning errors when no records are found.;Improvement of the user experience by preventing unwanted exceptions.;Use of the C# language (.NET) and the Entity Framework.;Greater stability in the GetByType method call, avoiding unexpected error returns.;Eduardo Rezende
08/14/2025 15:56:58;Development;Refactor: Update PersonTypeService;Update of the Delete and Disable methods of the PersonTypeService class, so that they do not return errors when removing or disabling a person type without associated people.;Improve system safety by avoiding unexpected errors when deleting or disabling person types with associated records.;C# language, Visual Studio, Entity Framework;Delete and Disable methods updated to handle cases where a person type has no associations.;Eduardo Rezende

```
---

## 💡 Note

🧰 The hardware settings used for the test: 

- 12th Generation Intel(R) Core(TM) i5-12450HX Processor (2.40 GHz)

- 16.0 GB Installed RAM DDR5

- 512.0 GB SSD

- NVDIA GeForce RTX 3050 6.0 GB

🤖 The model settings used for the test:

- Publisher: NousResearch

- Model: nous-hermes-2-mistral-7b-dpo

- Params: 8B

- Quant: Q5_K_M

- Size: 5.13 GB

---

## 🤝 Contributing
Contributions are welcome! Feel free to open issues or submit PRs with improvements.

Please for commits pattern follows: [pattern](https://github.com/IseduardoRezende/commit-pattern)

---

## 📜 License
This project is open source and available under the MIT License.
