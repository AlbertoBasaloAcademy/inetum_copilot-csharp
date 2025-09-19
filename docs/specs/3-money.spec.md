# RF-3 — Money command (spec)

## 1. Problem Specification (user POV)

As a user I want to know my country's official currency and convenient exchange rates so I can quickly understand the value of money in my location compared to common reference currencies (EUR, USD, GBP, CHF).

I expect a simple, human-readable CLI output that works with no extra configuration and degrades gracefully when network or API problems occur.

Constraints and assumptions:
- The CLI runs locally and will determine the user's country via the `loc` command data (IP-based geolocation) or accept a country code override.
- Use free public APIs; responses should be quick (<3s) in normal network conditions.

## 2. Solution Overview (dev POV)

Inputs
- Optional: country code (ISO 3166-1 alpha-2) or country name as CLI argument.
- If not provided, obtain country code from existing geolocation logic (RF-1) which queries ip-api.co or ip-api.com.

Processing
- Map the country code to its official currency code (ISO 4217). Use a minimal internal mapping table embedded in the app (fallback to a small JSON resource) to avoid extra external dependencies.
- Query exchange rates from the Frankfurter API: `https://api.frankfurter.dev/v1/latest?from={currency}&to=EUR,USD,GBP,CHF`.
  - If the target currency equals the base (e.g. EUR), Frankfurter returns 1 for that currency; still show it.
- Format rates relative to 1 unit of the country's currency and also show reciprocal rates where helpful (e.g., 1 EUR = X local).

Outputs
- Human-friendly text block with:
  - Currency name and ISO code
  - Symbol if known (optional) and decimal precision recommendation
  - Exchange rates for EUR, USD, GBP, CHF (only available ones)
  - A short note when the rate is unavailable or the API failed

Error handling and fallbacks
- If mapping from country to currency fails, show an explanatory message and suggest providing a currency code manually.
- If the Frankfurter API is unreachable or returns error, show the currency name and a clear "rates unavailable" message; optionally show cached or partial data if available.
- Respect timeouts (e.g., 2s) and avoid blocking other commands.

Implementation notes
- Keep dependencies minimal: use `System.Net.Http` and `System.Text.Json`.
- Implement a small, testable CurrencyMapper class with unit tests for common country codes.
- Add user-facing help text for `askbot money --help`.

Edge cases
- Territories or countries with multiple currencies (use the de facto official currency; allow override).
- Unrecognized country codes; case-insensitive matching for inputs.
- API returns partial rates (omit missing targets but still show available ones).

## 3. Acceptance Criteria (QA)

Given the app installed and network available
- When I run `askbot money` (no args) the CLI uses my detected country and prints a block like:
  "💰 Currency: Euro (EUR)\n   1 EUR = 1.00 EUR\n   1 EUR = 1.08 USD\n   1 EUR = 0.86 GBP\n   1 EUR = 0.97 CHF"
- When I run `askbot money ES` it shows Spain's currency (EUR) and the same rates as above.
- When I run `askbot money --currency=USD` it treats USD as the base and shows rates vs EUR/GBP/CHF.
- If the Frankfurter API is unreachable, the CLI shows the currency and a clear message: "Exchange rates unavailable (network/API error)" and exits with a non-zero status code.
- Response time under 3 seconds with normal connectivity.
- The CurrencyMapper component has unit tests covering at least 10 common country-to-currency mappings and edge-case inputs.

Test data and steps
- Unit tests for mapping: ES -> EUR, GB -> GBP, US -> USD, CH -> CHF, JP -> JPY, IN -> INR, CN -> CNY, RU -> RUB, BR -> BRL, AU -> AUD.
- Integration test (manual): run `askbot money` with a mocked Frankfurter response to verify formatting.

Notes
- Keep spec concise and focused on functionality described in the PRD. Prioritize reliability and clear error messages over exhaustive currency features.
