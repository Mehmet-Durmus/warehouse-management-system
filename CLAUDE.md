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

## Phase 1 — characterization tests (complete, merged to `main`)

Merged via PR #1 on the `ai-assisted/test-project-setup` branch (kept, not deleted,
for reference). 256 tests across every Command handler and every Query handler with
guard logic. Six real bugs were found and fixed along the way, each in its own `fix:`
commit separate from the `test:` commit that first characterized the buggy behavior
— see that branch's history for the details. Kept below for reference; do not restart
this phase.

We added a single test project, `WHMS.Tests`, at the solution root, with
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
- Goal: lock in the **current** behavior of command handlers, the query handlers that
  have guard/throw logic, and the two authorization handlers — before anything moves.
  See the "Test target list" section below.
- **No business logic changes in this phase.** `WHMS.Domain`, `WHMS.Application`
  handler logic, `WHMS.Persistence`, and `WHMS.Api` behavior must stay exactly as
  they are. If a test reveals a suspicious or seemingly buggy behavior (e.g. a
  missing guard), do not fix it — write the test to document current behavior as-is
  and flag it to the user instead of silently changing it.
- Do not touch EF Core migrations, seed data, or repository implementations in
  `WHMS.Persistence` — those are out of scope for unit testing and are not part of
  this phase.

## Current phase: Phase 2 — moving business rules to Domain

Branch: `ai-assisted/move-business-rules-to-domain`, branched from `main` (not from
the old `ai-assisted/test-project-setup` branch — that one is a frozen, merged
snapshot; `main` is the live base).

A full catalog of every business rule found in every Command/Query handler — which
entity/entities each concerns, and whether it's genuinely cross-aggregate or just an
aggregate-root-plus-child check — was worked out with the user before starting this
phase. Reference that discussion in conversation history if the shape of a rule is
unclear; it is not duplicated here.

### Structure

- `WHMS.Domain/BusinessRules/<Aggregate>/` mirroring the folder names already used
  under `WHMS.Application/Features/Command/` (`Catalog`, `Store`, `WasteRecord`,
  `Employee`, `Warehouse`, `Delivery`, `Shipment`, `InventoryCount`).
- `WHMS.Domain/BusinessRules/Shared/` for rules that don't belong to a single
  aggregate — e.g. `AddressRules`, since `Address` is a value object used by both
  `Warehouse` and `Store`, not owned by either.
- **Naming principle:** a Rules class is named after the aggregate the rule is
  fundamentally *about*, not the handler that happens to call it. E.g.
  `WarehouseRules.EnsureExists(...)` lives under `BusinessRules/Warehouse/` even
  though `Delivery`/`Shipment`/`Employee` handlers call into it. A single handler may
  call into more than one aggregate's Rules class when a check is a composite of
  checks about different aggregates (e.g. manager assignment: `WarehouseRules` for
  "no manager yet", `EmployeeRules` for "has the Manager role").
- **Rule classes are static, pure, and synchronous — no repository/DB access, ever.**
  The Application handler keeps doing the I/O (repository calls) to gather whatever
  the rule needs (a bool, a count, an already-fetched entity); the Rule method
  receives that data and either throws a typed domain exception or returns a computed
  value (e.g. `InventoryCountRules.CalculateVariance(quantity, currentStock)` returns
  an `int`, no throw). This is what makes Rule methods testable with zero mocks.

### Exception hierarchy (`WHMS.Domain/Exceptions/`)

Start minimal, split further only when a real case demands it:

- `DomainException` — abstract base.
- `NotFoundException` — a referenced aggregate/entity doesn't exist (or isn't visible
  to the caller — cross-warehouse access denial reuses this, matching existing
  "hide as not-found" behavior).
- `BusinessRuleViolationException` — everything else: uniqueness conflicts, invalid
  state transitions (already sent/received/completed), insufficient stock, etc.

### Global exception handling (`WHMS.Api`, new — deliberately in scope for this branch)

Typed domain exceptions are only useful once something converts them to a proper HTTP
response — today nothing does (verified: no `UseExceptionHandler`, no exception
filters, no try/catch anywhere in `WHMS.Api`; every thrown exception, including
FluentValidation's `ValidationException`, currently reaches the client as a raw
unhandled-exception response). Added via `IExceptionHandler`
(`AddExceptionHandler<T>()` + `app.UseExceptionHandler()` in `Program.cs`), mapping:

- `NotFoundException` → 404
- `BusinessRuleViolationException` → 400 (revisit to 409 for conflict-shaped cases if
  it turns out to matter)
- FluentValidation `ValidationException` → 400
- anything else → 500

All responses wrapped in the existing `ApiResponse`/`ApiResponse<T>`
(`WHMS.Api/Common/Models/`) shape via `ApiResponse.Failure(...)`.

### Per-aggregate migration steps

Once the exception hierarchy and global handler exist (first commits on this branch),
migrate one aggregate at a time:

1. Write the `<Aggregate>Rules` class with one method per rule, plus a Domain-level
   unit test per rule (no mocks — that's the point).
2. Update the Application handler(s) to call the Rule method instead of inlining the
   check; the repository call(s) that gather input data stay in the handler.
3. Update the handler's existing Phase 1 test: assert the new exception *type*
   (`NotFoundException`/`BusinessRuleViolationException`) instead of a bare
   `Exception`. Message text may also change if the Rule class phrases it
   differently — this is a deliberate, already-agreed change, not a silent one.
4. Genuinely cross-aggregate rules (e.g. Shipment's projected-stock calculation) take
   multiple already-fetched parameters; they still never call a repository directly.

Do not start an aggregate's migration until its Rule class + Domain test exist and its
handler test has been updated to match. Do not start Phase 2 work on an aggregate
whose Phase 1 characterization tests don't already exist and pass (they all do, as of
the merge above).

## Always stop and ask before

- Any git operation that touches shared/remote state: push, merge to `main`, tag,
  force-push, branch deletion.
- Any change to `WHMS.Persistence` (migrations, DbContext, repository
  implementations) or to API contracts (routes, request/response DTO shapes) — these
  are outside the current phases entirely. The one deliberate exception is the global
  exception-handling middleware described under Phase 2 above (error-path status
  codes and response shape only) — everything else about `WHMS.Api` still requires
  asking first, including success-path status codes and any controller/route change.
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

Command handlers are done (all groups under `WHMS.Application/Features/Command/**`).
Revised list, agreed with the user after that pass:

1. **Query handlers with a guard/throw branch** (not-found, cross-warehouse access,
   role-based visibility): `GetDelivery`, `GetDeliveries`, `GetDeliveryItem`,
   `GetDeliveryItems`, `GetEmployee`, `GetInventoryCount`, `GetInventoryCountLines`,
   `GetShipment`, `GetShipmentItem`, `GetShipmentItems`, `GetStore`, `GetWarehouse`,
   `GetWasteRecords`. Same guard-focused coverage style as command handlers.

Explicitly out of scope (decided after the command-handler pass, not just
deprioritized): `ValidationBehavior`, all `WHMS.Application/Validators/**`,
`WHMS.Application/Common/Filtering/Extensions/*`, `PasswordCreator`, and the two
authorization handlers (`PasswordChangedHandler`, `WarehouseAssignedHandler`) under
`WHMS.Application/Authorization/`. Also out of
scope: query handlers with no guard/throw branch (pure list/count/lookup
pass-throughs — e.g. `GetCatalogData`, `GetAllEmployees`, `GetLocationData`, the
`*Count` queries) — not enough behavior to characterize.

Out of scope for Phase 1 regardless: `WHMS.Persistence` repositories/DbContext,
`WHMS.Api` controllers, `TokenService` signing logic.
