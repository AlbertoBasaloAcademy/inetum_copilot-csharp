# Changelog for Askbot

## Version 1.0.0 - October 2, 2025
### Added
- Location display: Shows user's current location based on IP address (city, country, coordinates)
- Weather command: Displays current weather information using Open-Meteo API, including temperature, precipitation probability, and weather condition
- Money command: Shows country's official currency and exchange rates against EUR, USD, GBP, CHF using Frankfurter API
- Unit tests: Added xUnit tests for weather functionality with mocked HTTP responses
- CLI structure: Implemented command-line interface with help messages and error handling
- API clients: Created reusable classes for IP geolocation, weather, and currency services

### Changed
- Project structure: Organized code into separate classes for better maintainability
- Error handling: Added graceful handling of network failures and invalid inputs

### Fixed
- None

> End of CHANGELOG for Askbot, last updated October 2, 2025.