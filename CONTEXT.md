# TrackYourLife — Domain Context

## Glossary

### Home

The `/home` route is a **daily hub**: stacked sections for Trainings, Nutrition, and Youtube. Each section shows a compact, actionable snapshot (not full overview charts). Deeper analytics and lists live on module overview pages.

Section order: Trainings → Reading → Nutrition + Youtube (grid). When an ongoing workout exists, the Trainings section shows only a resume card; otherwise it shows the active workout plan top section (same as the Workouts page).

### Book

A book tracked by the user in the Reading module (`/books`). Has progress (`currentPage` / `totalPages`), status (`NotStarted`, `Ongoing`, `Finished`), and optional rating when finished.

### Reading Session

A live reading session: started from a book, tracked with a client-side timer, finished by recording the end page and optional notes. At most one active session per user. Completed sessions drive book progress, daily page totals, and streaks.

### Daily Reading Target

Personal goal of pages to read per day. Stored as a `Users.Goal` with `GoalType.ReadingPages` and `GoalPeriod.Day`.

### Reading Streak

Consecutive UTC days on which the user met or exceeded their daily reading target. Computed from finished session page totals, not stored separately.

### Subscribed Channel

A YouTube channel the user has added to TrackYourLife. Stored as a `YoutubeChannel`, assigned to exactly one **Category** for organization and daily watch limits. The UI uses "Subscribe" when adding a channel.

### Favorite Channel

A **Subscribed Channel** marked with `IsFavorite`. Favorites are independent of category assignment. Only subscribed channels can be favorites. There is no cap on how many channels can be favorited.

### Favorites (Youtube list filter)

A virtual filter on **Channels** and **Videos** (`youtubeCategoryId=favorites`, API `favoritesOnly=true`), not a stored **Category**. The tab appears only when the user has at least one **Favorite Channel**. It lists those channels and their latest videos only.

### Home Recommendation

A single video in the Youtube section of **Home**, chosen server-side on each API request. Selection is two-step:

1. For each **Favorite Channel**, pick one video from its two most recent uploads (prefer unwatched; random if both are watched).
2. Randomly pick one video from those per-channel winners.

If the user has no favorite channels, the home page shows an empty state linking to `/youtube/channels`.
