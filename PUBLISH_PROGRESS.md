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

**Next:** Stage 2 – backend API (commits 13–31).
