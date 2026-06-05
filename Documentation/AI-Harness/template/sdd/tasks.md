# Tasks — [Feature name]

> Inputs: [specify.md](./specify.md), [design.md](./design.md)

## Checklist

- [ ] Task 1 — description
  - **Files:** `path/to/file.cs`
  - **Done when:** ...
- [ ] Task 2
  - **Depends on:** Task 1
  - **Done when:** ...

## Parallelization

| Can run in parallel | Must be sequential |
|---------------------|-------------------|
| | |

## Verification

```bash
dotnet build
dotnet test
# manual: Swagger / Blazor
```

## Commit guidance

- Small atomic commits
- Message format: `feat(scope): description` or `fix(scope): description`

## Post-merge

- [ ] Update `Documentation/State.md`
