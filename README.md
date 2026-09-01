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
  eventos del formulario y decide asunto/cuerpo según éxito o fallo. El
  correo de notificación se envía al mismo correo escrito en el
  formulario.

La UI **se diseña manualmente en el Editor de Unity** (Canvas, campos de
nombre/correo, botón, textos de estado). Los scripts no crean ninguna UI
por código — solo se les arrastran las referencias existentes.

## Conexión manual en el Editor (una sola vez)

1. Crear un GameObject vacío, ej. `RegistrationLogic`, y agregarle los
   componentes `RegistrationForm`, `SimpleEmailSender` y
   `RegistrationEmailNotifier`.
2. En `RegistrationForm` (Inspector):
   - `Name Input` → tu campo de nombre (TMP_InputField).
   - `Email Input` → tu campo de correo (TMP_InputField).
   - `Submit Button` → tu botón de enviar.
   - `Validation Status Text` → un `TMP_Text` de tu UI para mostrar el
     resultado de la validación.
3. En `RegistrationEmailNotifier` (Inspector):
   - `Form` → el mismo `RegistrationForm` del paso 2.
   - `Email Sender` → el `SimpleEmailSender` del mismo GameObject.
   - `Send Result Text` (opcional) → otro `TMP_Text` para el resultado del
     envío SMTP (éxito/error).

No hace falta enganchar nada más al `OnClick()` del botón desde el
Inspector: `RegistrationForm` ya se suscribe a su propio botón en
`Awake()`.

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

## Si no llega el correo — cómo diagnosticar

`SimpleEmailSender.SendEmail` nunca falla en silencio: siempre devuelve
`resultMessage` con el texto exacto de éxito o del `Exception.Message`.
Revisa ese texto en el `Send Result Text` de tu UI (o en la consola de
Unity, se loguea igual). Las causas más comunes:
- `email_config.json` no existe o tiene la clave equivocada.
- La clave de aplicación de Gmail fue revocada/expiró — hay que generar
  una nueva.
- Firewall/red bloqueando el puerto 587 saliente.

## Ejecución

Abrir el proyecto en Unity 6, abrir `Assets/Scenes/SampleScene.unity`,
verificar las referencias del paso "Conexión manual" y dar Play. Escribir
nombre + correo y presionar el botón de registro para ver la validación y
el envío real de correo.
