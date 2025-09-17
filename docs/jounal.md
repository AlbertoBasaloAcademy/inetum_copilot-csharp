Summary of activity: Implement Weather command for AskBot

- Goal: Implement the `weather` command per `docs/specs/1-Weather forecast.spec.md`, following C# best practices and the project's clean-code guidelines.

- Actions performed:
  - Inspected project files: `Program.cs`, `IpApi.cs`, `IpClient.cs`, and `Weather.cs`.
  - Implemented an async `weather` CLI flow in `Program.cs` that accepts either no args (use IP location) or `weather <lat> <lon>`.
  - Implemented `Weather.FetchWeatherAsync(...)` to call Open-Meteo current weather API, parse current temperature and weather code, try to obtain hourly precipitation probability for the current hour, and produce a human-readable output.
  - Mapped Open-Meteo/WMO numeric weather codes to short textual descriptions.
  - Added input validation and friendly error messages with specific non-zero exit codes for common failure modes.
  - Updated `README.md` with usage examples for the `weather` command.
  - Committed changes with two logical commits: `feat(weather): ...` and `docs(readme): ...`.

- Files changed:
  - `src/Askbot/Program.cs` — CLI wiring and argument parsing.
  - `src/Askbot/Weather.cs` — Open-Meteo client, parsing and formatting.
  - `README.md` — usage documentation.

- Build & test summary:
  - `dotnet build` completed successfully (3 nullable warnings in `IpClient.cs`).
  - Smoke test `dotnet run -- weather` executed and printed location and weather output.

- Next steps / Recommendations:
  - Convert `IpClient.FetchIp()` to async and remove blocking `.Result` calls and nullable warnings.
  - Add unit tests (xUnit) that mock `HttpClient` to validate parsing and error handling.
  - Consider adding small integration tests or CI steps to run the smoke test when network is available.

- Note: The filename requested was `docs/jounal.md` (exact spelling preserved).