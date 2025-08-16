# AutoDoc - Git to Report 📚

**AutoDoc** is a simple but powerful tool that automatically analyzes Git commits and generates structured daily CSV reports.  
It leverages [LM Studio](https://lmstudio.ai/) as the AI engine to transform commit history into meaningful documentation with context-aware summaries.

---

## 🚀 How It Works

1. **Collect Commits**  
   AutoDoc scans a Git repository for commits within a given period.

2. **Context Injection**  
   A custom `Context.txt` file allows you to provide background information or instructions for the AI model without touching the source code.

3. **Commit Processing**  
   - Commits are grouped by day.  
   - If a day contains many commits, they are split into smaller parts.  
   - Each part is sent to the model for structured summarization.

4. **AI Integration**  
   - Uses LM Studio's local API (`/v1/chat/completions`).  
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
  "OutputPath": "Datas",
  "CompletionsUri": "http://localhost:1234/v1/chat/completions",
  "DelayMilliseconds": 20000,
  "ModelTemperature": 0.5,
  "MaxRetries": 2
}
```

🔢 Explanation of Parameters:
- RepositoryPath    → Path to the Git repository to analyze.
- OwnerEmail        → Git email address for filtering commits.
- OwnerName         → Who is responsible for the commits.
- Culture           → Defines reporting culture/locale (e.g., pt-BR, en-US).
- OutputPath        → Directory where daily CSV files are saved.
- CompletionsUri    → API endpoint of LM Studio.
- DelayMilliseconds → Delay between requests (prevents RAM overload).
- ModelTemperature  → Controls creativity of the model (0 = focused, 1 = creative).
- MaxRetries        → How many times to retry if a request fails.

---

## 📝 Context File
Inside the project, you will find Context.txt.
This file is user-editable and allows you to guide the model with instructions, tone, or context.
For example, you can describe how you want commits summarized or what reporting style to follow.

---

## 📦 Requirements
LM Studio installed and running locally.
Start LM Studio server with your preferred model.
Ensure it listens on the same port as configured in appsettings.json (default: http://localhost:1234).

---

## ▶️ Usage

1 - Go to releases: https://github.com/IseduardoRezende/AutoDoc/releases

2 - Install the zip  

3 - Edit appsettings.json with your repository path and preferences.

4 - Add/edit Context.txt with your reporting instructions.

5 - Make sure LM Studio server is running.

6 - Execute AutoDoc.exe 
  - Option 1: [StartDate] [EndDate] | Example: 2025-01-01 2025-02-01 | Explanation: Will generate 1 month of reports.
  - Option 2: [Days]                | Example: 7 | Explanation: Will generate 1 week of reports from the current day.
  - Option 3: No arguments          | Explanation: Will generate reports from the current day.

7 - Check the output folder for generated CSV reports.

---

## 📊 Example Output

After processing, AutoDoc will generate CSV files named by [project]-[date]-[guidV7].
Each row contains structured commit summaries enriched with AI-generated insights.

Example file:
book-api-2025-08-16-0198b3ccd15b77a295e7e23516fd3217.csv

---

## 🤝 Contributing
Contributions are welcome! Feel free to open issues or submit PRs with improvements.

---

## 📜 License
This project is open source and available under the MIT License.
