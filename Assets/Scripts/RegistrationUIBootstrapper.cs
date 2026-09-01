using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Arma toda la interfaz mínima (campo nombre, campo correo, campo correo
/// destino, botón enviar, texto de estado de validación, texto de estado
/// de envío) por código al iniciar la escena, y conecta entre sí las 3
/// piezas de la arquitectura: RegistrationForm, SimpleEmailSender y
/// RegistrationEmailNotifier. No requiere arrastrar nada a mano en el
/// Editor: basta con reproducir la escena.
/// </summary>
public static class RegistrationUIBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        // Canvas
        GameObject canvasGO = new GameObject("RegistrationCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        RectTransform panel = CreatePanel(canvasGO.transform);

        TMP_InputField nameInput = CreateInputField(panel, "NameInput", "Nombre", 260f);
        TMP_InputField emailInput = CreateInputField(panel, "EmailInput", "Correo", 210f);
        TMP_InputField destInput = CreateInputField(panel, "DestinationEmailInput", "Correo destino (a donde llega la notificación)", 160f);

        Button submitButton = CreateButton(panel, "SubmitButton", "Registrar", 110f);
        TMP_Text validationText = CreateLabel(panel, "ValidationStatusText", "Estado de validación: —", 60f);
        TMP_Text sendResultText = CreateLabel(panel, "SendResultText", "Estado de envío: —", 15f);

        GameObject logicGO = new GameObject("RegistrationLogic");
        RegistrationForm form = logicGO.AddComponent<RegistrationForm>();
        SimpleEmailSender sender = logicGO.AddComponent<SimpleEmailSender>();
        RegistrationEmailNotifier notifier = logicGO.AddComponent<RegistrationEmailNotifier>();

        form.nameInput = nameInput;
        form.emailInput = emailInput;
        form.submitButton = submitButton;
        form.validationStatusText = validationText;

        notifier.form = form;
        notifier.emailSender = sender;
        notifier.destinationEmailInput = destInput;
        notifier.sendResultText = sendResultText;

        // Reenganchar el listener del botón ahora que form ya existe
        // (Awake de RegistrationForm corrió antes de asignar submitButton).
        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(form.HandleSubmit);
    }

    private static RectTransform CreatePanel(Transform parent)
    {
        GameObject panelGO = new GameObject("Panel", typeof(RectTransform));
        panelGO.transform.SetParent(parent, false);
        RectTransform rt = panelGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(500, 400);
        rt.anchoredPosition = Vector2.zero;
        return rt;
    }

    private static TMP_InputField CreateInputField(RectTransform parent, string name, string placeholder, float yPos)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(TMP_InputField));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(460, 40);
        rt.anchoredPosition = new Vector2(0, yPos);

        TMP_InputField input = go.GetComponent<TMP_InputField>();

        GameObject textGO = new GameObject("Text", typeof(RectTransform));
        textGO.transform.SetParent(go.transform, false);
        TextMeshProUGUI text = textGO.AddComponent<TextMeshProUGUI>();
        text.fontSize = 18;
        text.color = Color.black;
        RectTransform textRt = textGO.GetComponent<RectTransform>();
        StretchFull(textRt);

        GameObject placeholderGO = new GameObject("Placeholder", typeof(RectTransform));
        placeholderGO.transform.SetParent(go.transform, false);
        TextMeshProUGUI placeholderText = placeholderGO.AddComponent<TextMeshProUGUI>();
        placeholderText.text = placeholder;
        placeholderText.fontSize = 18;
        placeholderText.color = new Color(0, 0, 0, 0.5f);
        placeholderText.fontStyle = FontStyles.Italic;
        RectTransform placeholderRt = placeholderGO.GetComponent<RectTransform>();
        StretchFull(placeholderRt);

        input.textViewport = rt;
        input.textComponent = text;
        input.placeholder = placeholderText;

        return input;
    }

    private static Button CreateButton(RectTransform parent, string name, string label, float yPos)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 45);
        rt.anchoredPosition = new Vector2(0, yPos);
        go.GetComponent<Image>().color = new Color(0.2f, 0.6f, 0.9f);

        GameObject textGO = new GameObject("Text", typeof(RectTransform));
        textGO.transform.SetParent(go.transform, false);
        TextMeshProUGUI text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 20;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        StretchFull(textGO.GetComponent<RectTransform>());

        return go.GetComponent<Button>();
    }

    private static TMP_Text CreateLabel(RectTransform parent, string name, string initialText, float yPos)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(460, 40);
        rt.anchoredPosition = new Vector2(0, yPos);

        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        text.text = initialText;
        text.fontSize = 16;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.black;
        text.enableWordWrapping = true;
        return text;
    }

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(8, 4);
        rt.offsetMax = new Vector2(-8, -4);
    }
}
