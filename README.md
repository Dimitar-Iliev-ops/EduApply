# EduApply

Дипломен проект на Димитър Иванов Илиев: **Система за онлайн кандидатстване и класиране при прием в училище**.

Текущ обхват: само етап 1, срок **16.11.2026**. Приложението е скелет с Identity, многослойна структура и MS SQL Server база. Наличните модули за заявления, желания, проверка, класиране и записване са начални защитени страници.

## Документация

- [Увод](docs/Introduction.md) — актуалност, обект, предмет, цел и задачи.
- [Глава 1: проучване](docs/Research.md) — три системи, сравнение, предимства, ограничения, избор и източници.
- [Концепция и бележки за скелета](docs/Concept.md).
- [Контролен списък за 16.11](docs/Stage-1-checklist.md).
- [Проверки на първия етап](docs/Verification.md).

## Технологии и структура

.NET 8, ASP.NET Core MVC/Razor, Identity, EF Core Code First и MS SQL Server. SDK версията е зададена в `global.json`. Изисква се SDK 8.0.425 или по-нова корекция от същата feature лента.

```text
src/EduApply.Web       Контролери, изгледи, Identity UI, стартиране
src/EduApply.Services  Услуги и бизнес логика
src/EduApply.Data      Entities, DbContext, конфигурации, миграции, seed
src/EduApply.Models    ViewModels / DTOs
docs/                 Документация за първи етап
tools/                Помощни инструменти за локална разработка
```

## Стартиране с Windows и SQL Server LocalDB

Необходими са .NET 8 SDK и SQL Server Express LocalDB. Наличието на .NET SDK не инсталира SQL Server. Изпълнете от основната папка:

```powershell
dotnet restore EduApply.sln
dotnet tool restore
dotnet build EduApply.sln --no-restore
./tools/Initialize-Development.ps1
dotnet run --no-build --project src/EduApply.Web --launch-profile https
```

Приложението е на https://localhost:7143. Ако сертификатът още не е доверен, използвайте `dotnet dev-certs https --trust` на собствената си машина.

Помощният скрипт създава базата **EduApplyStage1**, прилага `InitialCreate` и зарежда начални данни. При нужда задайте друго свободно име с `-DatabaseName EduApplyStage1Review`. Скриптът пази настройките и отделни случайни пароли в User Secrets на Web проекта. Не променяйте connection string към чужда база.

| Роля | Демо имейл | Ключ на локалната парола |
|---|---|---|
| Кандидат | applicant@eduapply.example | Seed:Passwords:Applicant |
| Комисия | committee@eduapply.example | Seed:Passwords:Committee |
| Администратор | admin@eduapply.example | Seed:Passwords:Administrator |

За да видите паролите **само локално**, изпълнете `dotnet user-secrets list --project src/EduApply.Web`. Не публикувайте изхода. Не използвайте демо акаунтите или тези настройки за реален прием.

## Друг MS SQL Server

Задайте собствения connection string извън репото:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<connection-string>" --project src/EduApply.Web
```

Миграциите се прилагат чрез `dotnet ef database update --project src/EduApply.Data --startup-project src/EduApply.Web`. За демо seed задайте трите `Seed:Passwords:*` ключа, `Seed:Enabled=true` и изпълнете в Development:

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run --no-build --project src/EduApply.Web -- --initialize-database
```

При нормално стартиране приложението не променя автоматично схемата. Production отказва демо инициализация. За бъдещо внедряване трябва да се зададат реалните `AllowedHosts`, съхранение на производствени тайни и отделен процес за миграции.

## Проверка

```powershell
dotnet build EduApply.sln --no-restore
dotnet ef migrations has-pending-model-changes --no-build --project src/EduApply.Data --startup-project src/EduApply.Web
```

Функционалните тестове и измерването на покритието на алгоритъма са в следващите етапи. Няма заявени 65% покритие, работещо класиране или публично внедряване за етап 1. Достъпът на консултанта и рецензента е потвърден от собственика на репото.
