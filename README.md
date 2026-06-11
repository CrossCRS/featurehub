# FeatureHub

A WIP(!) feature flag management system built with **.NET 10** using **Clean Architecture** and **CQRS** with support for multiple projects and environments. The API is designed for use with client SDKs to enable dynamic feature toggling in applications.

## Technology Stack

| Layer | Technology |
|---|---|
| **Runtime** | .NET 10.0 |
| **Database** | PostgreSQL 15+ (via Npgsql) |
| **ORM** | Entity Framework Core |
| **CQRS** | Paramore.Brighter (commands), Paramore.Darker (queries) |
| **Validation** | FluentValidation |
| **Auth** | ASP.NET Core Identity + JWT Bearer |
| **API Docs** | OpenAPI + Swashbuckle/SwaggerUI |
| **Tests** | xUnit, Moq, MockQueryable |

## REST API endpoints

| Endpoint | Method | Auth | Description |
|---|---|---|---|
| `GET /public/{etoken}` | Public | No | Get environment flags by token (for client SDKs) |
| `POST /api/auth/login` | Public | No | Login with username/password |
| `POST /api/auth/refresh` | Public | No | Refresh expired JWT |
| `GET /api/projects` | Authorized | Yes | List user's projects |
| `GET /api/projects/{id}` | Authorized | Yes | Get project by ID |
| `POST /api/projects` | Authorized | Yes | Create project |
| `PATCH /api/projects/{id}` | Authorized | Yes | Update project |
| `DELETE /api/projects/{id}` | Authorized | Yes | Delete project |
| `GET /api/projects/{pid}/environments` | Authorized | Yes | List environments |
| `GET /api/projects/{pid}/environments/{id}` | Authorized | Yes | Get environment by ID |
| `POST /api/projects/{pid}/environments` | Authorized | Yes | Create environment |
| `PATCH /api/projects/{pid}/environments/{id}` | Authorized | Yes | Update environment |
| `DELETE /api/projects/{pid}/environments/{id}` | Authorized | Yes | Delete environment |
| `POST .../environments/{id}/regenerate-token` | Authorized | Yes | Regenerate environment token |
| `GET .../environments/{eid}/featureflags` | Authorized | Yes | List feature flags |
| `GET .../environments/{eid}/featureflags/{id}` | Authorized | Yes | Get feature flag by ID |
| `POST .../environments/{eid}/featureflags` | Authorized | Yes | Create feature flag |
| `PATCH .../environments/{eid}/featureflags/{id}` | Authorized | Yes | Update feature flag |
| `DELETE .../environments/{eid}/featureflags/{id}` | Authorized | Yes | Delete feature flag |

## How It Works

Feature flags are organized into a hierarchy: **Projects -> Environments -> Feature Flags**. Each environment gets a unique token that client SDKs use to fetch flags at runtime.

### Hierarchy

```
Project (e.g. "My App")
├── Environment: "Production"  (token: abc123...)
│   ├── new-checkout       -> { value: true }
│   └── legacy-banner      -> { value: false, no data -> excluded from response }
└── Environment: "Staging"   (token: def456...)
    ├── new-theme          -> { value: true }
    └── new-checkout       -> { value: true, data: '{"variant":"B"}' }
```

### Setting up flags (management API)

All management endpoints require JWT authentication. The typical workflow (frontend dashboard TODO :)):

1. **Create a project** – `POST /api/projects` with `{ "name": "My App" }`
2. **Create an environment** – `POST /api/projects/{pid}/environments` with `{ "name": "Staging" }`
3. **Copy the environment token** – returned in the environment response (e.g. `def456...`). This is the SDK credential.
4. **Create feature flags** – `POST .../environments/{eid}/featureflags` with `{ "name": "new-theme", "value": false }`
5. **Toggle a flag** – `PATCH .../featureflags/{id}` with `{ "value": true }`

### Fetching flags (client SDK)

Client SDKs call a single public endpoint with the environment token:

```
GET /api/public/{environmentToken}
```

Example response:

```json
[
  { "name": "new-theme",      "value": true },
  { "name": "new-checkout",   "value": true, "data": "{\"variant\":\"B\"}" }
]
```

**Key behaviors:**
- Only flags that are **active** and have `value: true` or a `data` payload are returned - disabled flags are excluded entirely, client SDKs should treat missing flags as off.
- The optional `?clientHash=...` query parameter is used for future percentage rollouts and targeting rules (TODO :))

### Using the flag data in your app

A typical client SDK integration:

```python
import requests
import json

FEATUREHUB_TOKEN = "def456..."
flags = requests.get(f"https://your-api/api/public/{FEATUREHUB_TOKEN}").json()

# Build a lookup map
flags_map = {f["name"]: f for f in flags}

# Check a boolean flag
if flags_map.get("new-theme", {}).get("value"):
    enable_new_theme()

# Use structured data from a flag
checkout_data = flags_map.get("new-checkout", {}).get("data")
if checkout_data:
    variant = json.loads(checkout_data)["variant"]
    render_checkout(variant)
```

If a flag is absent from the response, it is off and has no data - treat it as disabled.

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/CrossCRS/featurehub.git
cd FeatureHub
```

### 2. Configure the database connection and JWT secret

Update `ConnectionStrings:DefaultConnection` and `JwtSettings:SecretKey` in user secrets.

### 3. Apply migrations

```bash
dotnet ef database update --project FeatureHub.Infrastructure --startup-project FeatureHub.Api
```

### 4. Run the API

```bash
dotnet run --project FeatureHub.Api
```

The API starts on `https://localhost:7156` (swagger available on `https://localhost:7156/swagger`).

## Seeded data

On first run in development, the database is automatically seeded with:

| User | Password | Role |
|---|---|---|
| `administrator` | `P@ssw0rd!` | Administrator |
| `user` | `P@ssw0rd!` | User |
| `demo` | `P@ssw0rd!` | Demo |

## Tests
xUnit test project (partially) covering the Application layer. More tests to come :)

Uses Moq for mocking and MockQueryable for EF Core IQueryable mocking.