# Front Architecture — DocFront.Web

## Stack

- **Blazor Server** (`AddServerSideBlazor`, `MapBlazorHub`)
- ASP.NET Core hosts UI; SignalR to browser
- Not Blazor WebAssembly

## Data flow

```text
DocAPI (HTTPS)
    ↓ HttpClient "DocApi"
Services (PacienteService, ProntuarioService, AgendamentoService)
    ↓
State (PacienteState, ProntuarioState, AgendamentoState)
    ↓
Pages / Components
```

## Folders

| Folder | Role |
|--------|------|
| `Pages/Pacientes/` | Gerenciar, Criar, Detalhes |
| `Pages/Pacientes/Components/` | Tabs, cards, lists |
| `Services/` | HTTP + `ApiService` base |
| `State/` | Cache, loading, `OnChange` events |
| `Mappers/` | DTO ↔ ViewModel |
| `Models/Dtos/` | API shapes |
| `Models/ViewModels/` | UI shapes |

## Configuration

`appsettings.json`:

```json
"ApiSettings": { "BaseUrl": "https://localhost:7004" }
```

## MVP2 planning gaps

- Atendimento not integrated (no Service/State yet).
- Global error boundary / toasts.
- `ApiResponse<T>` consistency.
- Cache invalidation and debounce where useful.
- Service/component tests for stable flows.
- Auth-aware UI states after backend decision.

## Conventions

- UI does not call API directly.
- List pages use State cache before refetching.
- Match API routes: `paciente`, `prontuario`, `agendamento`.
- Avoid large frontend architecture changes before backend contracts stabilize.
- Atendimento is not integrated in front yet — do not add UI without API stability.
