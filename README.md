# Inetum Copilot CSharp
Demo laboratory Copilot course CSharp edition for Inetum

## Dev workflow

```bash
# Clone repository
git clone https://github.com/AlbertoBasaloAcademy/inetum_copilot-csharp.git
cd inetum_copilot-csharp
# Compile C# code
dotnet build ./src/Askbot
# Run C# code
dotnet run --project ./src/Askbot 
# The weather command
dotnet run --project ./src/Askbot -- weather
```

## AskBot CLI

A CLI educational tool that queries public APIs to provide basic information from the user's IP and associated services: 
- location, weather, currency, and sun.

Weather command
- `dotnet run --project ./src/Askbot -- weather` : shows weather for your current IP location (uses http://ip-api.com/json/)
- `dotnet run --project ./src/Askbot -- weather <lat> <lon>` : shows weather for explicit decimal coordinates (latitude longitude)

> [Ver PRD del proyecto AskBot](docs/ask-bot.PRD.md)

---

- Author: [Alberto Basalo](https://albertobasalo.dev)
- Academy: [Academy Organization](https://github.com/AlbertoBasaloAcademy)
- Laboratory: [Source Repository](https://github.com/AlbertoBasaloLabs/copilot-csharp)
