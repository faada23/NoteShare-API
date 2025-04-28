# Noteshare API

RESTful API для платформы обмена заметками между пользователями

## Особенности

- 🔐 Аутентификация JWT + авторизация ролей (User/Moderator)
- 📝 CRUD операции для заметок (публичные/приватные)
- 🚀 Кэширование популярных заметок с Redis
- ⚙️ Фоновые задачи для обновления кэша (BackgroundService)
- ✅ Валидация запросов через FluentValidation
- 📊 Пагинация и сортировка результатов
- 📂 Паттерны:
  - Repository + Unit of Work
  - Result
- 🐳 Docker-контейнеризация 
- 📈 Логирование через Serilog (консоль + файлы)

## Технологии

- .NET 8
- ASP.NET Core
- Entity Framework Core 8
- PostgreSQL
- Redis
- Docker + Docker Compose
- FluentValidation
- Swagger
- Serilog
- BackgroundService
- JWT

