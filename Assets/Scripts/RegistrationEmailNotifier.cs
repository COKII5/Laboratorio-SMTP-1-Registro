using UnityEngine;
using TMPro;

/// <summary>
/// Suscriptor del formulario. Es la ÚNICA pieza que conoce tanto
/// RegistrationForm como SimpleEmailSender. El formulario nunca llama
/// directamente al envío de correo: este notifier se suscribe a sus
/// eventos (OnRegistrationSuccess / OnRegistrationFailed) y decide qué
/// asunto/cuerpo construir y cuándo enviar. El correo notificado llega
/// al mismo correo que la persona escribió en el formulario (InputCorreo).
/// </summary>
public class RegistrationEmailNotifier : MonoBehaviour
{
    public RegistrationForm form;
    public SimpleEmailSender emailSender;

    [Tooltip("Opcional: un TMP_Text de tu UI donde mostrar el resultado del envío SMTP. Puede quedar vacío.")]
    public TMP_Text sendResultText;

    private void OnEnable()
    {
        if (form == null) return;
        form.OnRegistrationSuccess += HandleSuccess;
        form.OnRegistrationFailed += HandleFailed;
    }

    private void OnDisable()
    {
        if (form == null) return;
        form.OnRegistrationSuccess -= HandleSuccess;
        form.OnRegistrationFailed -= HandleFailed;
    }

    private void HandleSuccess(string name, string email)
    {
        string subject = "Registro completado";
        string body =
            $"Registro exitoso.\n\n" +
            $"Nombre: {name}\n" +
            $"Correo ingresado: {email}\n";

        Send(email, subject, body);
    }

    private void HandleFailed(string name, string email, string reason)
    {
        string subject = "Registro rechazado: formato inválido";
        string body =
            $"El registro fue rechazado.\n\n" +
            $"Nombre ingresado: {name}\n" +
            $"Correo ingresado: {email}\n" +
            $"Motivo de la validación que falló: {reason}\n";

        // Si el correo llegó vacío o inválido no hay a dónde enviar la
        // notificación de fallo; se deja constancia en consola/UI.
        string destination = string.IsNullOrEmpty(email) ? "mailto@example.com" : email;
        Send(destination, subject, body);
    }

    private void Send(string toEmail, string subject, string body)
    {
        bool ok = emailSender.SendEmail(toEmail, subject, body, out string resultMessage);
        if (sendResultText != null)
        {
            sendResultText.text = ok
                ? $"Correo enviado ✔ ({resultMessage})"
                : $"Error al enviar ✘ ({resultMessage})";
        }
    }
}
