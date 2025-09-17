---
Title: Weather command spec
ID: SPEC_WEATHER_001
---

# Weather command (brief spec)

Purpose
 - Provide current weather for the user's location (based on IP) or for explicit coordinates.

Usage
 - askbot weather            # weather for current IP location
 - askbot weather <lat> <lon>  # weather for specific latitude and longitude

Behavior
 - Resolve location: if no coordinates provided, call http://ip-api.com/json/ to obtain latitude and longitude.
 - Query weather: call Open-Meteo current weather API:
	 https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current_weather=true
 - Parse and show: temperature (°C), precipitation probability if available, and weather code/description.

Inputs
 - Optional: two floats (latitude, longitude). If omitted, use IP geolocation.

Outputs (human-readable)
 - Header with location (city, country) when available.
 - Temperature in °C (rounded to 1 decimal).
 - Precipitation chance (if provided) or "N/A".
 - Weather condition (map numeric weather code to short text).

Examples
 - askbot weather
	 🌤️ Weather in Madrid:
			Temperature: 22.0°C
			Precipitation: 15%
			Condition: Partly cloudy

Error handling
 - Network/API failure: show a friendly message ("Weather service unavailable — try again later") and exit non-zero.
 - Geolocation failure: if IP lookup fails and no coords provided, show "Unable to resolve location".
 - Invalid coordinates: validate lat/lon ranges and show usage help.

Performance
 - Target response < 3s with healthy connectivity.

Acceptance criteria
 - Returns temperature and condition for current IP location when called without args.
 - Returns weather for valid lat/lon arguments.
 - Handles network errors and invalid input with clear messages.

Notes
 - Keep external dependencies minimal; use System.Net.Http and System.Text.Json.
