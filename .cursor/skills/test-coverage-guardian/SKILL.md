---
name: test-coverage-guardian
description: Audits git diff changes for test coverage across backend unit, frontend unit, backend functional, and e2e tests; creates or updates missing tests and runs relevant suites until passing. Use when validating change coverage, strengthening tests for modified code, or when the user asks to check if tests fully cover current repo changes.
disable-model-invocation: true
---

# Test Coverage Guardian

## Goal

Given current repository changes, ensure relevant behavior is covered by tests. If coverage is missing, add or update tests, then run suites repeatedly until they pass.

## Scope

- Backend unit tests
- Frontend unit tests
- Backend functional tests
- E2E tests

## Workflow

Copy this checklist and keep it updated while working:

```text
Coverage Guardian Progress
- [ ] Inspect git diff and changed files
- [ ] Map each code change to expected test coverage
- [ ] Identify missing/weak tests by scope
- [ ] Implement or update tests
- [ ] Run relevant test suites
- [ ] Fix failures and rerun until all pass
- [ ] Report coverage decisions and final status
```

### 1) Inspect changes

1. Collect changed files from git diff (tracked + untracked relevant source files).
2. Group changes by area:
   - Backend application/domain/infrastructure changes
   - Frontend feature/component/hook/query changes
   - API contract or integration boundary changes
3. For each changed area, state the behavior that can regress.

### 2) Map behavior to test layers

Use this default mapping:

- Pure backend business logic -> backend unit tests
- Frontend component logic and helpers -> frontend unit tests
- Cross-component or backend integration flows -> backend functional tests
- User-critical journeys and UI/backend interaction -> e2e tests

Coverage must focus on changed behavior and adjacent edge cases, not only happy paths.

### 3) Detect gaps

Treat coverage as insufficient when any changed behavior has no direct assertion in at least one appropriate layer.

Gap signals:

- Modified conditionals without new/updated assertions for branches
- New endpoints/queries/commands without functional or integration validation
- UI state transitions changed without component/unit verification
- User flows changed without e2e checks for critical path and failure mode

### 4) Create or update tests

1. Prefer editing existing nearby tests first.
2. Add new tests only when existing files cannot express the scenario clearly.
3. Keep test names behavior-focused and explicit.
4. Include negative and edge cases when logic changed.
5. Avoid broad rewrites unrelated to current diff.

## Test execution strategy

Auto-detect commands from repo conventions and run only relevant suites first, then broader suites when needed.

### Command discovery order

1. Backend:
   - Prefer solution/project-based `dotnet test` targeting affected test projects.
   - If multiple test projects are impacted, run the minimal relevant subset first, then full backend test set if needed.
2. Frontend unit:
   - Prefer package scripts (`test`, `test:unit`, or equivalent) in the frontend workspace.
3. E2E:
   - Prefer dedicated e2e scripts (for example Playwright/Cypress scripts in package config).

If discovery is ambiguous, inspect repository scripts/configuration and choose the command matching the changed area.

## Pass criteria

All relevant updated/added tests pass, and no failing tests remain in selected suites.

If failures occur:

1. Diagnose whether test or implementation is wrong.
2. Fix the minimal correct target.
3. Rerun affected suite.
4. Repeat until green.

Do not stop at first failure report.

## Guardrails

- Do not create migrations.
- Do not manually update generated API clients or swagger artifacts.
- Do not revert unrelated user changes.
- Keep edits focused on coverage for the current diff.

## Output format

Return results in this structure:

```markdown
Coverage assessment
- Changed areas: <short bullets>
- Coverage added/updated:
  - Backend unit: <files/tests>
  - Frontend unit: <files/tests>
  - Backend functional: <files/tests>
  - E2E: <files/tests>
- Remaining gaps: <none or explicit list>

Execution results
- Commands run: <list>
- Final status: <pass/fail>
- If fail: <blocking failures and next action>
```
