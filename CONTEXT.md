# TrackYourLife — Domain Context

## Glossary

### Subscribed Channel

A YouTube channel the user has added to TrackYourLife. Stored as a `YoutubeChannel`, assigned to exactly one **Category** for organization and daily watch limits. The UI uses "Subscribe" when adding a channel.

### Favorite Channel

A **Subscribed Channel** marked with `IsFavorite`. Favorites are independent of category assignment. Only subscribed channels can be favorites. There is no cap on how many channels can be favorited.

### Home Recommendation

A single video shown on `/home`, chosen server-side on each API request. Selection is two-step:

1. For each **Favorite Channel**, pick one video from its two most recent uploads (prefer unwatched; random if both are watched).
2. Randomly pick one video from those per-channel winners.

If the user has no favorite channels, the home page shows an empty state linking to `/youtube/channels`.
