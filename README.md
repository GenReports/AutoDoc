# AutoDoc - Git to Report 📚

**AutoDoc** is a simple but powerful tool that automatically analyzes Git commits and generates structured daily CSV reports.  
It leverages [LM Studio](https://lmstudio.ai/) as the AI engine to transform commit history into meaningful documentation with context-aware summaries.

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
  "OutputPath": "AutoDoc-Reports",
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
This file is user-editable and allows you to context.

You can follows the template:
```text
The project is called [Your Project Name], an [Your Project description].

[Talk about Your Project features]...

The project is developed in/with: [Your Project languages and tools].
```

---

## 📦 Requirements
LM Studio installed and running locally.
Start LM Studio server with your preferred model.
Ensure it listens on the same port as configured in appsettings.json (default: http://localhost:1234).

---

## ▶️ Usage

1 - Go to releases: [Releases](https://github.com/IseduardoRezende/AutoDoc/releases)

2 - Install the zip  

3 - Edit appsettings.json with your repository path and preferences.

4 - Edit Context.txt with your project context.

5 - Make sure LM Studio server is running.

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
nice-api-2025-08-14-0198b3ccd15b77a295e7e23516fd3217.csv

The example was generated in [pt-BR] because I change appsettings.json

```csv
Date;Step;Activity;Description;Motivation;Process;Result;Participants
14/08/2025 13:50:16;Testes de unidade;Adição de testes para PersonService;Implementação de testes unitários para a camada de serviço 'PersonService' no Nice Acesso.;Garantir a qualidade e a correção das operações realizadas pela entidade 'Pessoa' no sistema de controle de acesso.;Uso da ferramenta xUnit para criação e execução dos testes unitários.;Adição de testes que validam as operações básicas da entidade 'Pessoa' no sistema.;Eduardo Rezende
14/08/2025 13:51:08;Testes;Adição de Testes para PersonVehicleService;Implementação de testes unitários para a camada de serviço PersonVehicleService do sistema Nice Acesso.;Garantir a qualidade e estabilidade das funcionalidades relacionadas à associação entre pessoas e veículos no sistema.;Uso da biblioteca de testes xUnit, Visual Studio e .NET framework;Adição de um conjunto de testes que verificam o comportamento esperado da camada de serviço PersonVehicleService.;Eduardo Rezende
14/08/2025 13:51:30;Testes de Unidade;Adição de Testes para PersonTriggerService;Criação e implementação de testes unitários para a classe PersonTriggerService, responsável por controlar as interações entre pessoas e acionadores no sistema Nice Acesso.;Garantir a qualidade do código e prevenir bugs potenciais na lógica de negócios.;Utilização da biblioteca NUnit para criação e execução dos testes unitários, além da IDE Visual Studio.;Adição de novos testes que podem ser executados para validar a funcionalidade da classe PersonTriggerService.;Eduardo Rezende
14/08/2025 15:53:42;Desenvolvimento;Refatorar: Atualizar PersonService;Atualização do método GetByType da classe PersonService para evitar retorno de erro quando não há registros.;Melhoria na experiência do usuário ao prevenir exceções indesejadas.;Uso da linguagem C# (.NET) e do framework Entity Framework.;Maior estabilidade na chamada do método GetByType, evitando retornos de erro inesperados.;Eduardo Rezende
14/08/2025 15:56:58;Desenvolvimento;Refatorar: Atualizar PersonTypeService;Atualização dos métodos Delete e Disable da classe PersonTypeService, para que eles não retornem erros ao remover ou desabilitar um tipo de pessoa sem pessoas associadas.;Melhorar a segurança do sistema evitando erros inesperados ao excluir ou desativar tipos de pessoa com registros associados.;Linguagem C#, Visual Studio, Entity Framework;Métodos Delete e Disable atualizados para lidarem com casos em que um tipo de pessoa não possui associações.;Eduardo Rezende

```
---

## 🤝 Contributing
Contributions are welcome! Feel free to open issues or submit PRs with improvements.

Please for commits pattern follows: [pattern](https://github.com/IseduardoRezende/commit-pattern)

---

## 📜 License
This project is open source and available under the MIT License.
