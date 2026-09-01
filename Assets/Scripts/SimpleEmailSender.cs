using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using UnityEngine;

/// <summary>
/// Código SMTP entregado por el profesor (uso obligatorio), adaptado
/// únicamente para:
///  1. Recibir destinatario/asunto/cuerpo como parámetros en vez de
///     tenerlos hardcodeados (así sirve para éxito y para fallo).
///  2. Leer la clave de aplicación desde un archivo LOCAL que NO se sube
///     al repositorio (ver .gitignore -> email_config.json), en vez de
///     dejarla escrita en el código fuente.
/// La lógica de conexión SMTP (host, puerto, EnableSsl, MailMessage,
/// SmtpClient) es la misma que la entregada.
/// </summary>
public class SimpleEmailSender : MonoBehaviour
{
    // Mismo remitente entregado por el profesor para el laboratorio.
    private const string FromEmail = "ingmultimediausbbog@gmail.com";

    [Serializable]
    private class EmailConfig
    {
        public string appPassword;
    }

    /// <summary>
    /// Ruta del archivo de configuración local, fuera de control de
    /// versiones. Vive en la raíz del proyecto (junto a Assets/), no
    /// dentro de Assets, para que ni siquiera Unity lo trate como asset.
    /// </summary>
    private static string ConfigPath =>
        Path.Combine(Application.dataPath, "..", "email_config.json");

    private string LoadAppPassword()
    {
        string path = ConfigPath;
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "No se encontró email_config.json en la raíz del proyecto. " +
                "Crea el archivo con: {\"appPassword\": \"xxxx xxxx xxxx xxxx\"} " +
                "(clave de aplicación de Gmail, NUNCA la contraseña real de la cuenta).",
                path);
        }

        string json = File.ReadAllText(path);
        EmailConfig config = JsonUtility.FromJson<EmailConfig>(json);
        if (config == null || string.IsNullOrEmpty(config.appPassword))
        {
            throw new InvalidOperationException(
                "email_config.json existe pero no tiene un campo 'appPassword' válido.");
        }
        return config.appPassword;
    }

    /// <summary>
    /// Envía un correo real. Devuelve true/false según el resultado, y
    /// nunca lanza excepción hacia afuera (se captura y se reporta),
    /// para que el llamador pueda mostrar el resultado en la UI.
    /// </summary>
    public bool SendEmail(string toEmail, string subject, string body, out string resultMessage)
    {
        try
        {
            string password = LoadAppPassword();

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(FromEmail);
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body = body;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(FromEmail, password),
                EnableSsl = true
            };

            smtp.Send(mail);
            resultMessage = "Email sended succesfuly";
            Debug.Log("[SimpleEmailSender] " + resultMessage);
            return true;
        }
        catch (Exception ex)
        {
            resultMessage = "Error: " + ex.Message;
            Debug.Log("[SimpleEmailSender] " + resultMessage);
            return false;
        }
    }
}
