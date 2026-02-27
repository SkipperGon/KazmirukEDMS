# KazmirukEDMS

KazmirukEDMS — учебный проект (дипломная работа): система электронного документооборота (СЭД) для администрации.

Ключевые возможности (реализовано):
- Авторизация через ASP.NET Core Identity
- Хранение метаданных документов и версионирование (EF Core, Postgres)
- Локальное файловое хранилище для бинарных файлов (настраиваемый путь)
- POC цифровой подписи (BouncyCastle) с абстракцией для замены на CryptoPro
- Razor Pages для CRUD операций с документами и загрузки версий

Требования
- .NET 8
- PostgreSQL 17 (или совместимая)

Быстрый старт

1. Настройте строку подключения в `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=...;Port=5432;Database=kazmiruk_edms;Username=...;Password=..."
}
```

2. Настройте локальное хранилище и (опционально) ключи подписи в `appsettings.json`:

```json
"LocalStorage": { "RootPath": "C:\\edms_storage" },
"Signature": { "PrivateKeyPath": "path/to/private.pem", "PublicKeyPath": "path/to/public.pem", "Algorithm": "GOST3411WITHECGOST3410" }
```

3. Создайте и примените миграции (если необходимо):

```bash
dotnet tool install --global dotnet-ef --version 8.0.0
dotnet ef migrations add InitialCreate --context ApplicationDbContext
dotnet ef database update --context ApplicationDbContext
```

4. Запустите проект:

```bash
dotnet run
```

5. Перейдите в браузере по адресу https://localhost:5001/ и введите учетные данные. Страница входа: `/Account/Login`.

Дальнейшее развитие
- Перенос хранения файлов в защищённое файловое хранилище или объектное хранилище (S3/MinIO)
- Интеграция с CryptoPro / SKZI для подписей в реальной системе
- Аудит действий (AuditLog), полнотекстовый поиск, workflow согласования

Лицензия: MIT

