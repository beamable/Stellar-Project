Beamable WebSDK 0.6 — Local Notes

- Local cache: `docs/beamable-websdk-0.6.html`
- Version: 0.6 (Web SDK)

Overview
- This file summarizes what’s discoverable from the locally saved docs homepage and lists priority pages to cache for full offline reference.
- The homepage includes links to Getting Started, API Reference Modules, Services, and many endpoint function docs.

Key Entry Points (links visible on homepage)
- Getting Started: `https://help.beamable.com/WebSDK-0.6/web/getting-started/getting-started/`
- API Reference Modules: `https://help.beamable.com/WebSDK-0.6/web/user-reference/api-reference/modules/`

Service Classes (from sidebar links)
- AccountService: `https://help.beamable.com/WebSDK-0.6/web/user-reference/api-reference/services/classes/AccountService/`
- AnnouncementsService: `https://help.beamable.com/WebSDK-0.6/web/user-reference/api-reference/services/classes/AnnouncementsService/`
- ApiService: `https://help.beamable.com/WebSDK-0.6/web/user-reference/api-reference/services/classes/ApiService/`
- AuthService: `https://help.beamable.com/WebSDK-0.6/web/user-reference/api-reference/services/classes/AuthService/`
- ContentService: `https://help.beamable.com/WebSDK-0.6/web/user-reference/api-reference/services/classes/ContentService/`
- LeaderboardsService: `https://help.beamable.com/WebSDK-0.6/web/user-reference/api-reference/services/classes/LeaderboardsService/`
- PlayerService: `https://help.beamable.com/WebSDK-0.6/web/user-reference/api-reference/services/classes/PlayerService/`
- StatsService: `https://help.beamable.com/WebSDK-0.6/web/user-reference/api-reference/services/classes/StatsService/`

Common Functional Areas (endpoint docs present via links)
- Accounts: many endpoints under `.../api/functions/accounts...`
- Announcements: `.../api/functions/announcements...`
- Content/Remote Config: `.../api/functions/content...`
- Events: `.../api/functions/events...` and `.../api/functions/eventPlayers...`
- Groups/Clans: `.../api/functions/groups...` and `.../api/functions/groupUsers...`
- Inventory: `.../api/functions/inventory...`
- Leaderboards: `.../api/functions/leaderboards...` and types under `.../contents/types/...Leaderboard...` and schemas under `.../schema/type-aliases/...`
- Stats: likely via `StatsService` and related endpoints in `ApiService`

What to Cache Next (for complete offline use)
- Getting Started guide (install/init/auth examples)
- Modules root page (package-level exports and top-level API)
- Each Service class page listed above
- Frequently used function pages for your game flow, e.g.:
  - Accounts: register/login, external identity, roles
  - Inventory: get/add items, currencies, transactions
  - Leaderboards: set score, get view/friends/ranks
  - Events: calendar/list/details, submit score, claim
  - Content: manifest get/public/private, text/binary

Tips for Offline Mirroring
- Save each linked page as “Webpage, HTML only” into `docs/beamable-websdk-0.6/` subfolder.
- Keep filenames readable (e.g., `services-AuthService.html`).
- Update this cheat sheet with the local file paths for the saved pages.

Local Grep Helpers (PowerShell)
- List all Beamable WebSDK links in the homepage:
  `Select-String -Path docs/beamable-websdk-0.6.html -AllMatches -Pattern 'href="https://help.beamable.com/WebSDK-0.6/[^\"]+"' | % Matches | % Value | Sort-Object -Unique`
- Service classes only:
  `Select-String -Path docs/beamable-websdk-0.6.html -AllMatches -Pattern 'services/classes/[^\"]+"' | % Matches | % Value | Sort-Object -Unique`

Notes
- The saved homepage shows the structure and deep links, but not the content of linked pages. To avoid guesswork, please cache the specific pages you’ll use so we can include exact method names, params, and examples.

