# Quiz-applicatie (simpele versie)

Zelfde Windows Forms desktopapp als `QuizApp.Desktop`, maar dan herschreven met eenvoudigere,
klassieke C#-code — zodat elke regel makkelijk uit te leggen is.

Gebruikt dezelfde MySQL-database (`quizapp`, via Laragon) als de andere versies.

## Draaien

1. Laragon aan (MySQL).
2. Openen in Visual Studio (`QuizApp.Simple.csproj`) en op **F5** drukken, of `dotnet run` in deze map.

## Wat is er anders dan `QuizApp.Desktop`?

- **Geen Entity Framework.** In plaats daarvan `Data/Database.cs`: gewone SQL-strings met
  `MySqlConnection` / `MySqlCommand` / `MySqlDataReader` (via het pakket `MySqlConnector`).
  Je kan dus letterlijk elke `SELECT`/`INSERT`/`UPDATE`/`DELETE` teruglezen in de code.
- **Geen LINQ.** Overal gewone `foreach`-loops en `if`-statements in plaats van
  `.Where(...)`, `.Select(...)`, `.OrderBy(...)` enzovoort. Sorteren gebeurt met `ORDER BY` in de SQL zelf.
- **Geen tuples.** In plaats van `(Question, List<RadioButton>, TextBox?)` is er een kleine, benoemde
  klasse `AnswerControl` (in `QuizForm.cs`) met duidelijke veldnamen.
- **Benoemde event-handlers.** Knoppen roepen een methode aan zoals `btnStart_Click(...)` op,
  in plaats van een anonieme `(s, e) => ...`-lambda. Dat is ook precies hoe Visual Studio's
  eigen Form-designer knoppen aan code koppelt.
- **Alleen CSV-import** (geen JSON) — één simpel, handmatig geparst bestandsformaat
  (`id;question;answer_a;answer_b;answer_c;correct_answer`), zonder externe CsvHelper-library.
- **Nullable reference types staan uit** (`<Nullable>disable</Nullable>`), zodat er geen `?`/`??`
  door de code staat — gewone `null`-checks met `if (... == null)`.

## Structuur

- `Models/` — `Question`, `QuestionType`, `QuizResult`, `QuizAnswer`: kale data-klassen, geen logica.
- `Data/Database.cs` — alle database-code op één plek, elke methode doet één ding
  (bv. `GetAllQuestions()`, `AddQuestion(...)`, `SaveQuizResult(...)`).
- `MainForm.cs` — startscherm met 3 knoppen.
- `QuizForm.cs` — student: naam invullen → vragen beantwoorden → score bekijken (één Form, drie panelen).
- `QuestionsForm.cs` + `QuestionEditForm.cs` — docent: vragen toevoegen/wijzigen/verwijderen/importeren.
- `ResultsForm.cs` + `ResultDetailsForm.cs` — docent: overzicht van alle pogingen + details per poging.

## Let op

De connectiestring in `Data/Database.cs` gaat ervan uit dat de tabellen `Questions`, `QuizResults`
en `QuizAnswers` er al staan (zoals aangemaakt door de webversie via Entity Framework). Als je met
een lege database begint, moet je die tabellen eerst zelf aanmaken (bv. via de webversie draaien,
of handmatig in phpMyAdmin) voordat deze app data kan opslaan of ophalen.
