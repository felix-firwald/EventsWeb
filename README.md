# EventsWeb

Простой REST API для управления событиями, реализованный на ASP.NET Core Web API.

## Требования

- .NET 9 SDK или выше

## Запуск проекта

Клонировать репозиторий:

```bash
git clone <repository-url>
```

Восстановить зависимости:

```bash
dotnet restore
```

Собрать проект:

```bash
dotnet build
```

Запустить API:

```bash
dotnet run --project EventsWeb.Api
```

После запуска адрес приложения будет указан в консоли.

Swagger UI доступен по адресу:

```text
/swagger
```

Например:

```text
https://localhost:xxxx/swagger
```

## API

### Получить все события

```http
GET /events
```

Возвращает список всех событий.

### Получить событие

```http
GET /events/{id}
```

Возвращает событие с указанным `id`. Id является uuid (System.Guid).

Если событие не найдено, возвращается:

```text
404 Not Found
```

### Создать событие

```http
POST /events
```

Пример тела запроса:

```json
{
  "title": "Название события",
  "description": "Описание (необязательно)",
  "startAt": "2026-09-10T12:00:00",
  "endAt": "2026-09-10T13:00:00"
}
```

### Обновить событие

```http
PUT /events/{id}
```

Пример тела запроса аналогичный с POST /events.

### Удалить событие

```http
DELETE /events/{id}
```

## Структура проекта

- `EventsWeb.Api` — API слой; ASP.NET Core Web API и контроллеры
- `EventsWeb.Core` — Domain слой; сущности, DTO и интерфейсы
- `EventsWeb.Services` — реализация бизнес-логики и хранение данных.

## Хранение данных

События пока что хранятся в памяти приложения.

После перезапуска экземпляра приложения созданные события удаляются.