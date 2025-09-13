---
ID: 'PRACTICA_ACTIVITY-BOOKINGS'
Curso: 'Copilot-CSharp'
Arquetipo: 'CSharp-API'
Autor: 'Alumnos'
---
# PRD — Activity Bookings API

## 1. Resumen Ejecutivo

API REST para gestionar reservas de actividades turísticas. El objetivo es que los alumnos implementen un **CRUD** (Crear, Leer, Actualizar, Borrar) básico, persistan los datos en archivos (JSON, CSV o XML). La solución no requerirá autenticación, servicios de terceros ni el uso de una base de datos.

### 1.1 Objetivos y Métricas

  * Desarrollar una API REST funcional con todos los **endpoints** operativos de forma local.
  * Lograr que la aplicación sea ejecutable con el comando `dotnet run`.
  * Implementar **tests unitarios** y de integración para validar el comportamiento de la API.

### 1.2 Audiencia

  * **Alumnos:** Para adquirir experiencia práctica.
  * **Instructores:** Para utilizar como guía y validar el aprendizaje.

### 1.3 Tecnologías
  * **Lenguaje:** C# con .NET 9+. [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-9.0&tabs=visual-studio)
  * **Librerías:** Mínimas, preferiblemente solo `System.Net.Http` y `System.Text.Json`.
  * **Herramientas:** Visual Studio Code, o Visual Studio con .NET CLI
  * **Despliegue:** Servicio ejecutable en localhost con el comando `dotnet run`.

## 2. Alcance

  * **Incluido:** API REST con un CRUD básico sobre archivos JSON, CSV o XML.
  * **Excluido:** Autenticación, pagos, notificaciones, bases de datos.

### 2.1 Historias de Usuario

  * Como usuario, quiero listar todas las actividades ofertadas.
  * Como usuario, quiero ver detalles de una actividad específica.
  * Como usuario, quiero crear una nueva reserva para una actividad.
  * Como usuario, quiero ver todas mis reservas.
  * Como usuario, quiero cancelar una reserva existente.

### 2.2 Requisitos Funcionales

1.  **RF-1:** `GET /activities` - Lista todas las actividades disponibles.
2.  **RF-2:** `GET /activities/{id}` - Muestra detalles de una actividad específica.
3.  **RF-3:** `POST /bookings` - Crea una nueva reserva.
4.  **RF-4:** `GET /bookings` - Lista todas las reservas existentes.
5.  **RF-5:** `DELETE /bookings/{id}` - Cancela una reserva.

### 2.3 Requisitos Funcionales Adicionales

6.  **RF-6:** `GET /activities/{id}/bookings` - Lista todas las reservas de una actividad específica.
7.  **RF-7:** `PUT /bookings/{id}` - Actualiza una reserva existente.
8.  **RF-8:** `GET /activities?status={status}` - Filtra las actividades por estado (ej. `disponible`, `finalizada`).

### 2.4 Requisitos No Funcionales

  * **Simplicidad:** Priorizar la claridad y la simplicidad en la implementación.
  * **Diseño:** La API debe seguir un diseño RESTful con rutas claras y consistentes.
  * **Rendimiento:** Las respuestas de la API deben ser rápidas, preferiblemente en menos de 1 segundo.
  * **Persistencia:** Utilizar ficheros JSON, CSV o XML para almacenar los datos.
  * **Tecnologías:** Utilizar Dotnet 9+ y librerías NuGet mínimas.

## 3. Modelo de Datos y Reglas de Negocio

### 3.1 Esquema de Datos

  * **Actividad:**

      * `id` (int)
      * `nombre` (string)
      * `precio` (float)
      * `fecha` (datetime)
      * `estado` (string: `disponible`, `finalizada`, `cancelada`, `completo`)
      * `capacidad` (opcional, int): número máximo de reservas.
      * `quorum` (opcional, int): número mínimo de reservas para que la actividad se realice.

  * **Reserva:**

      * `id` (int)
      * `actividadId` (int)
      * `nombreCliente` (string)
      * `fechaReserva` (datetime)
      * `estado` (string: `confirmada`, `cancelada`)

### 3.2 Relaciones

  * Una **Actividad** puede tener múltiples **Reservas**.
  * Una **Reserva** está asociada a una única **Actividad**.

### 3.3 Reglas de Negocio

  * No se pueden reservar actividades con estado `finalizada` o `completo`.
  * Una reserva no puede ser cancelada si faltan menos de 48 horas para el inicio de la actividad.
  * Si se alcanza la `capacidad` máxima de una actividad, su estado debe cambiar a `completo`.
  * Si no se alcanza el `quorum` 48 horas antes de la actividad, esta debe cancelarse automáticamente y notificar a los usuarios (simulado con un log).

## 4. Criterios de Aceptación y Riesgos

### 4.1 Criterios de Aceptación

  * Cada endpoint devuelve datos legibles en formato JSON.
  * La API puede ser ejecutada sin errores.
  * Los tests unitarios y de integración pasan correctamente.
  * Se manejan adecuadamente los errores de I/O (lectura/escritura de archivos).

### 4.2 Riesgos

  * Errores en la manipulación de ficheros (lectura/escritura) que pueden corromper los datos.

## 5. Datos de Ejemplo (Seed Data)

### 5.1 Actividades

```json
[
  {"id":1, "nombre":"Tour por la ciudad", "precio":50.0, "fecha":"2026-07-15T10:00:00", "estado":"disponible", "capacidad": 10},
  {"id":2, "nombre":"Excursión a la montaña", "precio":80.0, "fecha":"2026-07-20T08:00:00", "estado":"disponible", "quorum": 5},
  {"id":3, "nombre":"Visita al museo", "precio":30.0, "fecha":"2024-07-18T14:00:00", "estado":"finalizada"}
]
```

```csv
id,nombre,precio,fecha,estado,capacidad,quorum
1,Tour por la ciudad,50.0,2026-07-15T10:00:00,disponible,10,
2,Excursión a la montaña,80.0,2026-07-20T08:00:00,disponible,,5
3,Visita al museo,30.0,2024-07-18T14:00:00,finalizada,,
```

```xml
<actividades>
  <actividad>
    <id>1</id>
    <nombre>Tour por la ciudad</nombre>
    <precio>50.0</precio>
    <fecha>2026-07-15T10:00:00</fecha>
    <estado>disponible</estado>
    <capacidad>10</capacidad>
  </actividad>
  <actividad>
    <id>2</id>
    <nombre>Excursión a la montaña</nombre>
    <precio>80.0</precio>
    <fecha>2026-07-20T08:00:00</fecha>
    <estado>disponible</estado>
    <quorum>5</quorum>
  </actividad>
  <actividad>
    <id>3</id>
    <nombre>Visita al museo</nombre>
    <precio>30.0</precio>
    <fecha>2024-07-18T14:00:00</fecha>
    <estado>finalizada</estado>
  </actividad>
</actividades>
```

### 5.2 Reservas

```json
[
  {"id":1, "actividadId":1, "nombreCliente":"Juan Pérez", "fechaReserva":"2025-06-01T12:00:00", "estado":"confirmada"},
  {"id":2, "actividadId":2, "nombreCliente":"María Gómez", "fechaReserva":"2025-06-02T15:30:00", "estado":"confirmada"},
  {"id":3, "actividadId":1, "nombreCliente":"Luis Martínez", "fechaReserva":"2025-06-03T09:45:00", "estado":"cancelada"}
]
```

```csv
id,actividadId,nombreCliente,fechaReserva,estado
1,1,Juan Pérez,2025-06-01T12:00:00,confirmada
2,2,María Gómez,2025-06-02T15:30:00,confirmada
3,1,Luis Martínez,2025-06-03T09:45:00,cancelada
```

```xml  
<reservas>
  <reserva>
    <id>1</id>
    <actividadId>1</actividadId>
    <nombreCliente>Juan Pérez</nombreCliente>
    <fechaReserva>2025-06-01T12:00:00</fechaReserva>
    <estado>confirmada</estado>
  </reserva>
  <reserva>
    <id>2</id>
    <actividadId>2</actividadId>
    <nombreCliente>María Gómez</nombreCliente>
    <fechaReserva>2025-06-02T15:30:00</fechaReserva>
    <estado>confirmada</estado>
  </reserva>
  <reserva>
    <id>3</id>
    <actividadId>1</actividadId>
    <nombreCliente>Luis Martínez</nombreCliente>
    <fechaReserva>2025-06-03T09:45:00</fechaReserva>
    <estado>cancelada</estado>
  </reserva>
</reservas>
```