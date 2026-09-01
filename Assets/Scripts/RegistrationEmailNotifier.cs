using UnityEngine;
using TMPro;

/// <summary>
/// Suscriptor del formulario. Es la ÚNICA pieza que conoce tanto
/// RegistrationForm como SimpleEmailSender. El formulario nunca llama
/// directamente al envío de correo: este notifier se suscribe a sus
/// eventos (OnRegistrationSuccess / OnRegistrationFailed) y decide qué
/// asunto/cuerpo construir y cuándo enviar.
/// </summary>
public class RegistrationEmailNotifier : MonoBehaviour
{
    public RegistrationForm form;
    public SimpleEmailSender emailSender;
    public TMP_InputField destinationEmailInput;
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

    private string DestinationEmail =>
        destinationEmailInput != null && !string.IsNullOrEmpty(destinationEmailInput.text)
            ? destinationEmailInput.text.Trim()
            : "mailto@example.com";

    private void HandleSuccess(string name, string email)
    {
        string subject = "Registro completado";
        string body =
            $"Registro exitoso.\n\n" +
            $"Nombre: {name}\n" +
            $"Correo ingresado: {email}\n";

        Send(subject, body);
    }

    private void HandleFailed(string name, string email, string reason)
    {
        string subject = "Registro rechazado: formato inválido";
        string body =
            $"El registro fue rechazado.\n\n" +
            $"Nombre ingresado: {name}\n" +
            $"Correo ingresado: {email}\n" +
            $"Motivo de la validación que falló: {reason}\n";

        Send(subject, body);
    }

    private void Send(string subject, string body)
    {
        bool ok = emailSender.SendEmail(DestinationEmail, subject, body, out string resultMessage);
        if (sendResultText != null)
        {
            sendResultText.text = ok
                ? $"Correo enviado ✔ ({resultMessage})"
                : $"Error al enviar ✘ ({resultMessage})";
        }
    }
}
