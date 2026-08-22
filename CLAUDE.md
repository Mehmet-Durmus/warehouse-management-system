# Working Agreement for AI-Assisted Changes

## Project history and boundary

This project was designed and built entirely by hand (Mehmet) up to the commit tagged
`v1.0-handwritten`. That tag marks the last commit written without AI assistance and
**must never be moved, deleted, or have its history rewritten** (no force-push, no
rebase across it).

All AI-assisted work happens on branches prefixed `ai-assisted/` and is merged into
`main` only after the human owner has reviewed the diff and explicitly approved it.
Never merge, push to `main`, push tags, or delete branches without asking first.

Commit messages produced during AI-assisted work keep their `Co-Authored-By: Claude`
trailer — it is the audit trail that makes the split between handwritten and
AI-assisted work verifiable in `git log`. Do not strip it.

## Current phase: Phase 1 — characterization tests

We are adding a single test project, `WHMS.Tests`, at the solution root, with
subfolders mirroring the existing projects (`Domain/`, `Application/`, `Api/`,
`Infrastructure/` as needed) — **not** one test project per layer.

- Stack: xUnit + Moq.
- Naming: `{ClassUnderTest}Tests.cs`, placed at the path that mirrors the source
  file's path within its project. Test method names follow
  `MethodName_Scenario_ExpectedResult`.
- Structure: Arrange-Act-Assert, one behavior per test.
- Review cadence: after the first several handlers were reviewed one at a time and
  approved, the project owner relaxed this to continuous mode - write tests for the
  remaining handlers in the target list without stopping for approval after each one,
  running `dotnet test` after every file and committing per handler as usual. Only
  interrupt this flow to flag something that needs a decision: a suspicious/buggy
  behavior found while writing a characterization test (per the rule below), a
  namespace collision or similar structural surprise worth a heads-up, or finishing
  a whole feature group.
- Goal: lock in the **current** behavior of command/query handlers, validators,
  filter extensions, the validation pipeline behavior, and the two authorization
  handlers — before anything moves. See the "Test target list" section below.
- **No business logic changes in this phase.** `WHMS.Domain`, `WHMS.Application`
  handler logic, `WHMS.Persistence`, and `WHMS.Api` behavior must stay exactly as
  they are. If a test reveals a suspicious or seemingly buggy behavior (e.g. a
  missing guard), do not fix it — write the test to document current behavior as-is
  and flag it to the user instead of silently changing it.
- Do not touch EF Core migrations, seed data, or repository implementations in
  `WHMS.Persistence` — those are out of scope for unit testing and are not part of
  this phase.

## Phase 2 (later, requires separate go-ahead): moving business rules to Domain

Once Phase 1 tests are green for a given area, business rules move into the
corresponding domain entity one aggregate at a time (e.g. `Delivery`, then
`Shipment`, then `InventoryCount`, ...). For each aggregate:

1. Add the behavior method(s) to the entity (e.g. `Delivery.Receive(...)`), replacing
   public setters with `private set` where the invariant requires it.
2. Write new domain-level unit tests for that method directly (no mocks needed).
3. Update the corresponding Application handler to call the new domain method instead
   of inlining the check, and update/adjust its existing Phase-1 test only as much as
   the new call shape requires — the observable behavior (return value / exception /
   error message) should not change unless that change was explicitly agreed with the
   user first.
4. Rules that require querying another aggregate or the database (e.g. SKU name
   uniqueness) do not move into a single entity method — model them as a domain
   service interface (defined in `WHMS.Domain`, implemented in
   `WHMS.Infrastructure`/`WHMS.Persistence`) instead. Ask before introducing a new
   interface if it's not obvious which project should implement it.

Do not start Phase 2 for an aggregate until its Phase 1 tests exist and pass.

## Always stop and ask before

- Any git operation that touches shared/remote state: push, merge to `main`, tag,
  force-push, branch deletion.
- Any change to `WHMS.Persistence` (migrations, DbContext, repository
  implementations) or to API contracts (routes, request/response DTO shapes, status
  codes) — these are outside the current phases entirely.
- "Fixing" a bug encountered while writing a characterization test, instead of just
  documenting the current behavior in the test and reporting it.
- Any change to `appsettings.json`, secrets, or Docker/CI configuration.

## House rules

- Commit messages follow **Conventional Commits** (`type: subject`), matching the
  style already used in this repo's history (`docs:`, `refactor:`, ...). Use `test:`
  for Phase 1 characterization tests, `refactor:` for Phase 2 domain moves, `chore:`
  for project/tooling setup (e.g. creating `WHMS.Tests`). Keep the subject in the
  imperative mood, no trailing period.
- Small, single-purpose commits (roughly one handler/entity/aggregate per commit),
  not large sweeping diffs.
- Run `dotnet build` and `dotnet test` after every change; do not proceed to the next
  file/aggregate if either is red.
- No speculative abstractions, no unrelated cleanup, no renames outside the scope of
  the current task — see the general engineering guidelines already in effect for
  this session.

## Test target list (Phase 1)

Full list agreed with the user, grouped by priority:

1. **Command handlers** (highest priority — these hold the business rules being
   moved): all handlers under `WHMS.Application/Features/Command/**` (Delivery,
   Shipment, InventoryCount, Catalog, WasteRecord, Warehouse, Store, Employee, Auth).
   Cover: happy path, not-found/wrong-warehouse branch, invalid state-transition
   branch (already received/sent/completed), uniqueness/conflict branch, and
   warning-collection branches (e.g. `AssignEmployeesCommandHandler`).
2. **Filter extensions**: `WHMS.Application/Common/Filtering/Extensions/*` — pure
   `IQueryable` predicate logic, no mocks needed.
3. **Query handlers with real logic** (pagination/sorting), lower priority than 1-2.
4. **`ValidationBehavior`**: aggregates validator failures correctly; passes through
   when there are no validators for a request.
5. **Authorization handlers**: `PasswordChangedHandler`, `WarehouseAssignedHandler`.
6. **Validators**: all classes under `WHMS.Application/Validators/**` — table-driven,
   one valid case + one case per rule.
7. **`PasswordCreator`**: generated password satisfies its length/character-class
   constraints.

Out of scope for Phase 1: `WHMS.Persistence` repositories/DbContext, `WHMS.Api`
controllers, `TokenService` signing logic.
