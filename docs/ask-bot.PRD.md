---
ID: 'DEMO_ASK-BOT'
Curso: 'Copilot-CSharp'
Arquetipo: 'CSharp-CLI'
Autor: 'Alberto Basalo'
---
# PRD — AskBot CLI

## 1. Resumen Ejecutivo

CLI educativo que consulta APIs públicas para dar información básica de la IP del usuario y servicios asociados: localización, clima, moneda, hora y sol. El objetivo es mostrar a los alumnos un **programa de terminal** funcional que demuestre el consumo de servicios externos y el manejo de datos JSON.

### 1.1 Objetivos y Métricas

  * Desarrollar una aplicación CLI funcional con todos los **comandos** operativos de forma local.
  * Demostrar el consumo exitoso de **APIs REST** públicas y gratuitas.
  * Implementar manejo de errores para servicios externos no disponibles.

### 1.2 Audiencia

  * **Alumnos:** Para adquirir experiencia práctica en consumo de APIs.
  * **Instructores:** Para utilizar como guía y validar el aprendizaje.

### 1.3 Tecnologías
  * **Lenguaje:** C# con .NET 9+.
  * **Librerías:** Mínimas, preferiblemente solo `System.Net.Http` y `System.Text.Json`.
  * **Herramientas:** Visual Studio Code, o Visual Studio con .NET CLI
  * **Despliegue:** Aplicación ejecutable con el comando `dotnet run <command>`.

## 2. Alcance

  * **Incluido:** CLI con 4 comandos principales para consultar información de geolocalización, clima, moneda y datos solares.
  * **Excluido:** Interfaz gráfica, persistencia de datos, autenticación, notificaciones.

### 2.1 Historias de Usuario

  * Como usuario, quiero saber mi localización actual a partir de mi IP.
  * Como usuario, quiero conocer el pronóstico de hoy en mi ciudad.
  * Como usuario, quiero ver la moneda oficial de mi país y cotizaciones básicas.
  * Como usuario, quiero saber las horas de salida y puesta del sol.

### 2.2 Requisitos Funcionales

1.  **RF-1:** `loc` o `[vacío]` - Muestra ciudad, país, latitud/longitud obtenidos de [ipapi.co](https://ipapi.co/json/) o [ip-api.com](http://ip-api.com/json/).
2.  **RF-2:** `weather` - Muestra temperatura actual, probabilidad de lluvia y código meteorológico desde [open-meteo.com](https://open-meteo.com/).
3.  **RF-3:** `money` - Muestra moneda oficial del país y cotización en EUR/USD/GBP/CHF desde [frankfurter.dev](https://api.frankfurter.dev/v1/latest).
4.  **RF-4:** `sun` - Muestra horas de salida y puesta del sol desde [open-meteo.com](https://open-meteo.com/).

### 2.3 Requisitos Funcionales Adicionales

5.  **RF-5:** `askbot help` - Muestra ayuda con la lista de comandos disponibles y su descripción.
6.  **RF-6:** `askbot --version` - Muestra la versión actual de la aplicación.
7.  **RF-7:** Todos los comandos deben manejar errores de conectividad y mostrar mensajes informativos.

### 2.4 Requisitos No Funcionales

  * **Simplicidad:** Priorizar la claridad y la simplicidad en la implementación y uso.
  * **Rendimiento:** Las respuestas de la CLI deben ser rápidas, preferiblemente en menos de 1 segundo con conexión activa.
  * **Disponibilidad:** Manejar amigablemente la no disponibilidad de servicios externos.
  * **Usabilidad:** Comandos intuitivos y mensajes de error claros y útiles.


## 3. Modelo de Datos y APIs

### 3.1 Geolocalización:

- `curl http://ip-api.com/json/` 

```json
{
  "status": "success",
  "country": "Spain",
  "countryCode": "ES",
  "region": "GA",
  "regionName": "Galicia",
  "city": "A Coruña",
  "zip": "15009",
  "lat": 43.3626,
  "lon": -8.4012,
  "timezone": "Europe/Madrid",
  "isp": "Telefonica de Espana SAU",
  "org": "RIMA (Red IP Multi Acceso)",
  "as": "AS3352 TELEFONICA DE ESPANA S.A.U.",
  "query": "81.39.197.0"
}
```

### 3.2 Clima:
- `curl "https://api.open-meteo.com/v1/forecast?latitude=43.3626&longitude=-8.4012&current_weather=true"`
```json
{
  "latitude": 43.3626,
  "longitude": -8.4012,
  "generationtime_ms": 0.02491474151611328,
  "utc_offset_seconds": 0,
  "timezone": "GMT",
  "timezone_abbreviation": "GMT",
  "elevation": 0,
  "current_units": {
    "time": "iso8601",
    "interval": "seconds",
    "temperature": "°C"
  },
  "current": {
    "time": "2025-09-13T06:45",
    "interval": 900,
    "temperature": 14.3
  }
}
```

### 3.3 Moneda:
- `curl https://api.frankfurter.dev/v1/latest` - Tasas de cambio de divisas

```json
{
  "amount": 1.0,
  "base": "EUR",
  "date": "2024-09-13",
  "rates": {
    "USD": 1.08,
    "GBP": 0.86,
    "CHF": 0.97
  }
}
```

### 3.4 Sol:
- `curl "https://api.open-meteo.com/v1/forecast?latitude=52.52&longitude=13.419998&daily=sunrise,sunset&timezone=Europe%2FBerlin"` - Información de salida y puesta del sol

```json
{
  "latitude": 52.52,
  "longitude": 13.419998,
  "generationtime_ms": 0.006198883056640625,
  "utc_offset_seconds": 7200,
  "timezone": "Europe/Berlin",
  "timezone_abbreviation": "GMT+2",
  "elevation": 38,
  "daily_units": {
    "time": "iso8601",
    "sunrise": "iso8601",
    "sunset": "iso8601"
  },
  "daily": {
    "time": [
      "2025-09-13"
    ],
    "sunrise": [
      "2025-09-13T06:37"
    ],
    "sunset": [
      "2025-09-13T19:26"
    ]
  }
}
```

## 4. Criterios de Aceptación y Riesgos

### 4.1 Criterios de Aceptación

  * Cada comando devuelve datos legibles en formato texto estructurado.
  * La CLI puede ser ejecutada desde terminal sin errores.
  * Se manejan adecuadamente los errores de conectividad y APIs no disponibles.
  * Los comandos responden en menos de 3 segundos en condiciones normales.
  * La aplicación muestra mensajes de ayuda claros cuando se ejecuta sin parámetros.

### 4.2 Riesgos

  * **APIs externas no disponibles:** Implementar mensajes de fallback informativos.
  * **Límites de uso de APIs:** Informar al usuario sobre posibles restricciones.
  * **Conectividad de red:** Manejar timeouts y errores de conexión graciosamente.
  * **Cambios en APIs:** Las APIs públicas pueden cambiar su estructura sin previo aviso.

## 5. Ejemplos de Salida

### 5.1 Comando `loc`
```
📍 Ubicación:
   Ciudad: Madrid
   País: España (ES)
   Coordenadas: 40.4168, -3.7038
```

### 5.2 Comando `weather`
```
🌤️ Clima en Madrid:
   Temperatura: 22°C
   Precipitación: 15%
   Condición: Parcialmente nublado
```

### 5.3 Comando `money`
```
💰 Moneda: Euro (EUR)
   1 EUR = 1.00 EUR
   1 EUR = 1.08 USD
   1 EUR = 0.86 GBP
   1 EUR = 0.97 CHF
```

### 5.4 Comando `time`
```
🕐 Hora Local:
   Actual: 14:30:25
   Zona: Europe/Madrid (CET)
   UTC+2 (Horario de verano)
```

### 5.5 Comando `sun`
```
☀️ Info Solar:
   Amanecer: 07:45
   Atardecer: 19:20
   Duración del día: 11h 35m
```


