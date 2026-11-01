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

### Guitar Tuner

The first item in the Guitar Tabs menu, labeled Tuner (`/guitar-tabs/tuner`), and the only way to open it. The tab page does not link to it. It listens to a played note and shows how sharp or flat it is against the **Selected Tuning**, and can play one of that tuning's pitches. It does not belong to a saved tab. **Custom Tunings** and the last **Selected Tuning** stay in this browser. A visit still starts in **Nearest mode**.

_Avoid_: Tunner, pitch detector

### Tuning Preset

One of Standard (E2 A2 D3 G3 B3 E4), Half-step down (Eb2 Ab2 Db3 Gb3 Bb3 Eb4), Whole-step down (D2 G2 C3 F3 A3 D4), Drop D (D2 A2 D3 G3 B3 E4), or Drop C (C2 G2 C3 F3 A3 D4), written from the low string to the high string. These five cannot be removed. Every preset is built from **Concert pitch**.

_Avoid_: open tuning

### Custom Tuning

A named six-string tuning the user adds on the **Guitar Tuner**, such as C# Tuning (C#2 F#2 B2 E3 G#3 C#4). Notes are written from the low string to the high string, keeping the spelling the user typed (`C#`, not `Db`). The first string sits at or above C2, and each following string is the next higher occurrence of its note. It stays in this browser so it can be chosen again. Its name and notes can be edited. Removing it is possible; the five **Tuning Presets** are not. If the removed tuning was selected, the tuner returns to Standard.

_Avoid_: Tab Tuning

### Concert pitch

A4 = 440 Hz. The **Guitar Tuner** has no calibration control.

_Avoid_: 432 Hz, tune reference

### Selected Tuning

The tuning currently chosen on the **Guitar Tuner**, either a **Tuning Preset** or a **Custom Tuning**, from a dropdown above the **Meter**. The string row sits below the **Meter**. The tuner judges and plays only those six pitches. The last choice is kept in this browser and is what the tuner opens on. With nothing saved, that choice is Standard.

_Avoid_: Tab Tuning, song tuning

### Meter

The **Guitar Tuner** display of the judged string's note name with no octave number, signed cents, and a needle from 50 cents flat to 50 cents sharp. Past that range the needle pegs. The number and the needle move in steps of 5 cents. Within 5 cents both sit at zero. A step holds until the pitch has moved 5 cents away from it. A reading within 20 cents of the one on the meter replaces it at once. A farther reading, such as a brief jump to 150 cents flat, is shown only after that pitch holds for three readings in a row. Silence clears the meter at once. With no stable pitch it idles: needle centered, no cents, and **Nearest mode** highlights no string. The note is the judged string's pitch, not the name of whatever was heard.

_Avoid_: chromatic readout

### In tune

Within 5 cents of the judged string's pitch, on either side.

_Avoid_: close enough, ±10 cents

### Listening

The **Guitar Tuner** asks for the microphone when the page opens. Until access is granted, and if it is denied, the **Meter** idles while a **Reference pluck** still plays. The control that asks again sits in the **Meter** area.

_Avoid_: required microphone

### Nearest mode

The **Guitar Tuner** state in which the meter targets the **String** whose pitch, or that pitch one octave up or down, is closest to the played note. Displayed cents are that same distance, so one octave of error still reads **in tune**. If two strings tie, the one whose true pitch is closer wins. This is how the tuner starts. While a stable pitch matches a string, that string is highlighted. A **Locked string** uses the same cents rule.

_Avoid_: chromatic, auto-detect all notes

### String

One of the six open-string positions on the **Guitar Tuner**, ordered from low to high and shown in one row with the low string on the left. Each is labeled with its note name and no octave number. Its pitch comes from the **Selected Tuning**.

_Avoid_: note, pitch

### Locked string

The **String** the meter judges exclusively. Tapping a string locks it and plays that pitch once. Tapping it again plays the pitch again and stays locked. Tapping another string moves the lock and plays that pitch. Changing the **Selected Tuning** keeps the same **String**, updates its pitch, and does not play a **Reference pluck**. The locked string stays highlighted until **Auto**, including while no note is playing.

_Avoid_: manual mode

### Reference pluck

A synthesized string tone, about one and a half seconds, of one pitch in the **Selected Tuning**. A string tap plays it, and the meter idles until it ends.

_Avoid_: drone, sine, sample

### Auto

The control at the end of the string row that clears the **Locked string** and returns the **Guitar Tuner** to **Nearest mode**. It looks selected while the tuner is in **Nearest mode**.

_Avoid_: unlock, manual off

### Tab Tuning

The tuning a guitar tab declares: a name, a pitch list such as `E A D G B E`, and an optional capo. Shown on the tab page. The **Guitar Tuner** does not read it.

_Avoid_: Selected Tuning

### Flagged ambiguities

- "Tuning" means both **Tab Tuning** and **Selected Tuning**. The tuner uses only **Selected Tuning**.
- The note on the **Meter** is the judged string, not the chromatic name of the note that was played.
- A pitch one octave off the target still counts as that **String**. Displayed cents ignore that octave.

## Example dialogue

> **Dev:** "This tab is Drop D. Does the Tuner open in Drop D?"
> **Domain expert:** "No. The Tuner opens on the last **Selected Tuning**, which starts as Standard, in **Nearest mode**. **Tab Tuning** stays on the tab page. A tuning you create, like C# Tuning, stays in this browser."
> **Dev:** "The low E comes through an octave high."
> **Domain expert:** "That is still the low E, and it can read in tune. Lock a string when Drop D's two D strings are both close."
