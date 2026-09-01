# Laboratorio SMTP — Opción 3: Formulario de registro con validación

Proyecto Unity 6 que implementa la **Opción 3** del laboratorio: un
formulario de registro (nombre + correo) que valida los campos y notifica
por correo real (SMTP/Gmail) el resultado, usando el código SMTP entregado
por el profesor, sin modificarlo en su lógica de conexión.

## Arquitectura (criterio de mayor peso de la rúbrica)

El disparo del correo se hace por **suscripción a eventos**, nunca por
llamado directo. El formulario no conoce el envío de correo.

```
RegistrationForm (declara y dispara los eventos, no sabe que existe SMTP)
    │  event OnRegistrationSuccess(name, email)
    │  event OnRegistrationFailed(name, email, reason)
    ▼
RegistrationEmailNotifier (único que conoce ambas puntas)
    │  se suscribe a los 2 eventos, arma asunto/cuerpo dinámico
    ▼
SimpleEmailSender (código SMTP entregado, adaptado a parámetros)
```

- `Assets/Scripts/RegistrationForm.cs` — captura nombre/correo, valida
  (campos vacíos, formato de correo con regex), dispara
  `OnRegistrationSuccess` u `OnRegistrationFailed` con el motivo real de
  la validación que falló. Sin ninguna referencia a SMTP.
- `Assets/Scripts/SimpleEmailSender.cs` — código SMTP obligatorio del
  profesor, adaptado solo para recibir destinatario/asunto/cuerpo por
  parámetro y leer la clave de aplicación desde un archivo local
  (`email_config.json`, fuera del repo).
- `Assets/Scripts/RegistrationEmailNotifier.cs` — se suscribe a los
  eventos del formulario y decide asunto/cuerpo según éxito o fallo.
- `Assets/Scripts/RegistrationUIBootstrapper.cs` — arma la UI mínima
  (campos, botón, textos de estado) por código al iniciar la escena, para
  que el proyecto funcione directamente al reproducir sin pasos manuales
  extra en el editor.

## Manejo de error específico de la opción

El correo de fallo **no** usa un mensaje genérico: el cuerpo incluye el
motivo real de la validación (`campo vacío` / `formato de correo
inválido`), tomado directamente de la causa detectada en
`RegistrationForm`.

## Configuración de la clave de aplicación (obligatorio, no se sube)

1. Crear en la raíz del proyecto (junto a `Assets/`) un archivo
   `email_config.json`:
   ```json
   { "appPassword": "xxxx xxxx xxxx xxxx" }
   ```
2. Ese archivo está en `.gitignore` — nunca se sube al repositorio ni a
   su historial.

## Ejecución

Abrir el proyecto en Unity 6, abrir `Assets/Scenes/SampleScene.unity` y
dar Play. La UI se genera automáticamente. Escribir nombre + correo +
correo destino, y presionar "Registrar" para ver la validación y el envío
real de correo.
