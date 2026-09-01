using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Formulario de registro (Opción 3 del laboratorio SMTP).
/// Responsabilidad única: capturar nombre + correo, validar, y avisar
/// mediante eventos si el registro fue exitoso o falló. NO conoce nada
/// sobre SMTP ni sobre el envío de correo — eso es responsabilidad de
/// quien se suscriba a los eventos (RegistrationEmailNotifier).
/// </summary>
public class RegistrationForm : MonoBehaviour
{
    [Header("UI (asignado por RegistrationUIBootstrapper)")]
    public TMP_InputField nameInput;
    public TMP_InputField emailInput;
    public Button submitButton;
    public TMP_Text validationStatusText;

    // Evento disparado cuando la validación pasa: (nombre, correo)
    public event Action<string, string> OnRegistrationSuccess;

    // Evento disparado cuando la validación falla: (nombre, correo, motivo)
    public event Action<string, string, string> OnRegistrationFailed;

    private static readonly Regex EmailRegex = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    private void Awake()
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(HandleSubmit);
        }
    }

    public void HandleSubmit()
    {
        string name = nameInput != null ? nameInput.text?.Trim() : string.Empty;
        string email = emailInput != null ? emailInput.text?.Trim() : string.Empty;

        if (string.IsNullOrEmpty(name))
        {
            Fail(name, email, "El nombre está vacío.");
            return;
        }

        if (string.IsNullOrEmpty(email))
        {
            Fail(name, email, "El correo está vacío.");
            return;
        }

        if (!EmailRegex.IsMatch(email))
        {
            Fail(name, email, "El correo no tiene un formato válido.");
            return;
        }

        Succeed(name, email);
    }

    private void Succeed(string name, string email)
    {
        SetStatus($"Registro válido para {name}.");
        OnRegistrationSuccess?.Invoke(name, email);
    }

    private void Fail(string name, string email, string reason)
    {
        SetStatus($"Registro rechazado: {reason}");
        OnRegistrationFailed?.Invoke(name, email, reason);
    }

    private void SetStatus(string message)
    {
        if (validationStatusText != null)
        {
            validationStatusText.text = message;
        }
        Debug.Log($"[RegistrationForm] {message}");
    }
}
