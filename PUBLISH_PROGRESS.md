# Publish progress

The project was written locally first. It is published to
https://github.com/DIREG502/pf2e-campaign-tracker one stage per day.

How it works: the full history lives on the local `main` branch. Each day the next slice of
commits is cherry-picked onto the local `publish` branch and pushed to GitHub `main`.
Author dates are the real dates the work was done, committer dates are the real dates it
was published. Nothing is backdated.

## Schedule

| Stage | Content | Commits from local `main` |
| --- | --- | --- |
| 1 | Repo setup, solution skeleton, entities, DbContext, initial migration | 1–12 (`caa648b`..`69a674c`) |
| 2 | Backend API: auth, Swagger, campaign and expense endpoints, CSV export, fixes | 13–31 (`7983533`..`b84505f`) |
| 3 | Frontend: scaffold, API client, auth, pages for all features | 32–47 (`d457cab`..`51099b5`) |
| 4 | Charts, demo seed script, UI polish | 48–56 (`3f34153`..`2243478`) |
| 5 | xUnit tests and the fixes found while testing | 57–64 (`2b749fb`..`91d4f39`) |
| 6 | Screenshots, READMEs (EN + DE), CI, new release workflow | 65–69 (`eff08e2`..`56c6934`) + new |
| 7 | Last docs commit, release v1.0.0 and verification | 70 (`355368d`) + release |

## Completed stages

### Stage 1 – 2026-11-05

- Created the public repository `DIREG502/pf2e-campaign-tracker`.
- Pushed commits 1–12: gitignore/gitattributes, MIT license, editorconfig, .NET solution and
  Web API project, dotnet-ef tool, all EF Core entities, `AppDbContext` with the SQLite/PostgreSQL
  switch, and the initial migration.
- Added this file.

### Stage 2 – 2026-11-07

- Pushed commits 13–31: JWT authentication with register/login/me, Swagger "Authorize" support,
  ProblemDetails error responses, endpoints for campaigns, characters, inventory, session notes,
  encounters/combatants (with the PF2e initiative tie rule), categories (with seeded defaults),
  expenses with date filter, expense summary and CSV export, CORS, and two fixes
  (amount validation on non-English cultures, UTC timestamps from SQLite).
- Checked that the pushed tree matches the original commit 31 and builds without warnings.

### Stage 3 – 2026-11-08

- Pushed commits 32–47: Vite + React scaffold, fetch-based API client with JWT handling, auth context,
  routing and layout, login/register, base styles, and pages for characters (list, form, detail with
  inventory), campaigns, session notes, encounters, the initiative tracker, categories and expenses
  with date filter and CSV export button.
- Checked that the pushed tree matches the original commit 47, that the backend builds without
  warnings, and that the frontend builds after a clean `npm ci`.
- Known until stage 4: the "Charts" link in the navigation has no page yet and redirects to the start page.

**Next:** Stage 4 – charts, demo seed script, UI polish (commits 48–56).
