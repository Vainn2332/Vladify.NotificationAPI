# Vladify.NotificationAPI

Notification microservice for the Vladify platform. It stores per-user notification
preferences, reacts to domain events coming over RabbitMQ, and delivers email
notifications over SMTP. The public surface is a **GraphQL** API secured with
**Auth0** JWTs.

![CI](https://github.com/Vainn2332/Vladify.NotificationAPI/actions/workflows/ci.yml/badge.svg)

## Features

- **GraphQL API** (HotChocolate) for reading and managing user notification settings.
- **JWT authentication** via Auth0 — every query and mutation requires a valid bearer token.
- **Event-driven** processing with MassTransit + RabbitMQ:
  - `UserCreatedMessage` → creates notification settings for the new user (email subscription on by default).
  - `SongCreatedMessage` → emails every email-subscribed user about the new song.
- **Email delivery** with MailKit over SMTP (STARTTLS), batched and sent in parallel.
- **MongoDB** persistence for notification settings.
- **OpenAPI + Scalar** interactive API reference (Development environment only).

## Tech stack

| Area            | Technology                                   |
| --------------- | -------------------------------------------- |
| Runtime         | .NET 9 / ASP.NET Core                        |
| API             | HotChocolate (GraphQL) 15                    |
| Messaging       | MassTransit 8.2 + RabbitMQ                   |
| Persistence     | MongoDB (MongoDB.Driver 3.7)                 |
| Email           | MailKit / MimeKit                            |
| Auth            | Microsoft.AspNetCore.Authentication.JwtBearer (Auth0) |
| Mapping         | AutoMapper 15                                |
| Config          | DotNetEnv (`.env`) + user secrets            |
| Testing         | xUnit-based unit & integration tests, code coverage |

## Solution layout

The solution follows a layered architecture:

```
Vladify.NotificationAPI/       # Presentation: GraphQL, auth, DI wiring, entry point
Vladify.BusinessLogic/         # Services, MassTransit consumers, options, AutoMapper profiles
Vladify.DataAccess/            # MongoDB repository, entities, options
Vladify.UnitTests/             # Unit tests
Vladify.IntegrationTests/      # Integration tests (GraphQL end-to-end, JWT builder, seeding)
```

Dependency direction: `NotificationAPI → BusinessLogic → DataAccess`.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (for the local RabbitMQ instance)
- A **MongoDB** instance (local or hosted)
- An **Auth0** tenant (for issuing/validating JWTs)
- A mailbox that supports SMTP with an app password (e.g. a Gmail account with an
  [app password](https://support.google.com/accounts/answer/185833))

## Getting started

1. **Clone the repository**

   ```bash
   git clone https://github.com/Vainn2332/Vladify.NotificationAPI.git
   cd Vladify.NotificationAPI
   ```

2. **Create your environment file**

   Copy the template and fill in the values (see [Configuration](#configuration)):

   ```bash
   cp .env.example .env
   ```

   The `.env` file is git-ignored. Alternatively you can use
   [.NET user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets)
   with the same `Section__Key` keys.

3. **Start RabbitMQ**

   `compose.yml` provides a RabbitMQ container with the management UI. It reads
   `RABBITMQ_USERNAME` / `RABBITMQ_PASSWORD` from your environment (set them in `.env`
   or export them first, and use the same values for `RabbitMqOptions__*`):

   ```bash
   docker compose up -d
   ```

   - AMQP: `localhost:5672`
   - Management UI: <http://localhost:15672>

4. **Run the API**

   ```bash
   dotnet restore
   dotnet run --project Vladify.NotificationAPI
   ```

   In the `Development` environment you get:

   - GraphQL endpoint: `/graphql`
   - Scalar API reference: `/scalar`
   - OpenAPI document: `/openapi/v1.json`

## Configuration

Configuration is bound from environment variables (or user secrets). Nested keys use
the double-underscore (`__`) separator. All options are validated on startup, so the
app will fail fast if a required value is missing.

| Variable | Description |
| --- | --- |
| `MongoDbOptions__ConnectionString` | MongoDB connection string, e.g. `mongodb://localhost:27017`. |
| `MongoDbOptions__DatabaseName` | Database name to use. |
| `EmailNotificationOptions__SMTPClientUrl` | SMTP **server host name** (not a URL, no `http://`). Gmail: `smtp.gmail.com`. |
| `EmailNotificationOptions__Port` | SMTP port. The client uses STARTTLS, so use the STARTTLS port. Gmail: `587`. |
| `EmailNotificationOptions__SenderEmail` | Sender mailbox — also the SMTP login username. |
| `EmailNotificationOptions__SenderName` | Display name shown in the "From" field. |
| `EmailNotificationOptions__ApplicationPassword` | SMTP app password (e.g. a Gmail 16-digit app password). |
| `RabbitMqOptions__ServerHost` | RabbitMQ host (`localhost`, or the compose service/container name). |
| `RabbitMqOptions__Username` | RabbitMQ username. |
| `RabbitMqOptions__Password` | RabbitMQ password. |
| `Auth0Options__Domain` | Auth0 domain, e.g. `your-tenant.eu.auth0.com` (the authority becomes `https://<domain>`). |
| `Auth0Options__Audience` | Expected JWT audience, e.g. `https://Vladify/musicAPI`. |
| `ASPNETCORE_ENVIRONMENT` | `Development`, `Staging`, or `Production`. |

> **Never commit `.env`.** It is excluded by `.gitignore` by default.

## GraphQL API

The endpoint is served at `/graphql`. All operations require a valid Auth0 JWT
(`Authorization: Bearer <token>`).

### Queries

- `getNotificationById(id)` — a single user's notification settings.
- `getNotifications(pageNumber, pageSize)` — paged list of all notification settings.
- `getEmailSubscribers(pageNumber, pageSize)` — paged list of email-subscribed users.

### Mutations

- `updateNotificationSettings(input)` — replace a user's notification settings.
- `patchSubscription(input)` — patch a user's subscription flags.

## Messaging

The service consumes the following messages from RabbitMQ (via MassTransit):

| Message | Consumer | Effect |
| --- | --- | --- |
| `UserCreatedMessage` | `CreateUserNotificationSettingsConsumer` | Creates notification settings for the user (email subscription enabled). |
| `SongCreatedMessage` | `EmailSenderConsumer` | Sends a "new song" email to all email subscribers. |

## Testing

Run the full test suite:

```bash
dotnet test
```

- `Vladify.UnitTests` — unit tests for services, consumers, factories, and the GraphQL error filter.
- `Vladify.IntegrationTests` — end-to-end GraphQL tests with a test JWT builder and data seeding.

## Continuous integration

GitHub Actions ([.github/workflows/ci.yml](.github/workflows/ci.yml)) runs on pushes and
pull requests to `main`:

1. Restore, build (`Release`).
2. Run tests with OpenCover code coverage.
3. Run SonarCloud static analysis.

Required repository secrets: `SONAR_TOKEN`, `TEST_JWT_SECRETKEY`.
