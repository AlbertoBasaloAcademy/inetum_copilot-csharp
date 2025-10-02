# Backlog for Askbot

> Epic Priority Legend: ‼️ Critical | ❗ High  |❕ Normal

> Feature Status Legend: ⛔ BLOCKED | ⏳ PENDING | ✨ CODED | ✅ TESTED

## ‼️ Core CLI Features {E1}

Core functionality for the Askbot CLI application to query public APIs for user IP information.

### ✨ Location Display {F1.1}

- **Dependencies:** None
- **Project Requirements:** RF-1 from ask-bot.PRD.md

Displays user's current location based on IP address, including city, country, coordinates.

### ✅ Weather Command {F1.2}

- **Dependencies:** F1.1
- **Project Requirements:** RF-2 from ask-bot.PRD.md

Shows current weather information for user's location or specified coordinates using Open-Meteo API.

### ✨ Money Command {F1.3}

- **Dependencies:** F1.1
- **Project Requirements:** RF-3 from ask-bot.PRD.md

Displays country's official currency and exchange rates against EUR, USD, GBP, CHF using Frankfurter API.

### ⛔ Sun Command {F1.4}

- **Dependencies:** F1.1
- **Project Requirements:** RF-4 from ask-bot.PRD.md

Shows sunrise and sunset times for user's location using Open-Meteo API.

> End of BACKLOG for Askbot, last updated October 2, 2025.