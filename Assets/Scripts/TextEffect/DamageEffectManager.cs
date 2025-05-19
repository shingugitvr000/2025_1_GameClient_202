using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class DamageEffectManager : MonoBehaviour
{
    public static DamageEffectManager Instance { get; private set; }

    [SerializeField] private GameObject textPrefab; // UI ������ �ؽ�Ʈ ������
    [SerializeField] private Canvas uiCanvas; // UI ĵ���� ����

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // ĵ������ ������ �ڵ����� ã��
        if (uiCanvas == null)
        {
            uiCanvas = FindObjectOfType<Canvas>();
            if (uiCanvas == null)
            {
                Debug.LogError("UI ĵ������ ã�� �� �����ϴ�. ���� ĵ������ �ִ��� Ȯ���ϼ���.");
            }
        }
    }

    public void ShowDamageText(Vector3 position, string text, Color color, bool isCritical = false, bool isStatusEffect = false)
    {
        if (textPrefab == null || uiCanvas == null) return;

        // ���� ��ǥ�� ��ũ�� ��ǥ�� ��ȯ
        Vector3 screenPos = Camera.main.WorldToScreenPoint(position);

        // UI�� ī�޶� �ڿ� �ִ� ��� ǥ������ ����
        if (screenPos.z < 0) return;

        // ������ �ؽ�Ʈ UI ����
        GameObject damageText = Instantiate(textPrefab, uiCanvas.transform);

        // ��ũ�� ��ġ ����
        RectTransform rectTransform = damageText.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.position = screenPos;
        }

        // �ؽ�Ʈ ������Ʈ ����
        TextMeshProUGUI tmp = damageText.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            // �ؽ�Ʈ ����
            tmp.text = text;

            // ���� ����
            tmp.color = color;

            // �ƿ����� ���� ����
            tmp.outlineColor = new Color(
                Mathf.Clamp01(color.r - 0.3f),
                Mathf.Clamp01(color.g - 0.3f),
                Mathf.Clamp01(color.b - 0.3f),
                color.a
            );

            // ũ�� ����
            float scale = 1.0f;

            // �ؽ�Ʈ�� ������ ��� ���� ���� ũ�� ����
            int numericValue;
            if (int.TryParse(text.Replace("+", "").Replace("CRIT!", "").Replace("HEAL CRIT!", ""), out numericValue))
            {
                scale = Mathf.Clamp(numericValue / 15f, 0.8f, 2.5f);
            }

            // ũ��Ƽ���̸� ũ�� ����
            if (isCritical) scale *= 1.4f;

            // ���� ȿ���� �ణ �۰�
            if (isStatusEffect) scale *= 0.8f;

            damageText.transform.localScale = new Vector3(scale, scale, scale);
        }

        // ȿ�� ����
        DamageTextEffect effect = damageText.AddComponent<DamageTextEffect>();
        if (effect != null)
        {
            effect.Initialize(isCritical, isStatusEffect);

            // ���� ȿ���� �������� �ʰ� ���� �ö�
            if (isStatusEffect)
            {
                effect.SetVerticalMovement();
            }
        }
    }

    // ������ �Լ�
    public void ShowDamage(Vector3 position, int amount, bool isCritical = false)
    {
        string text = amount.ToString();
        Color color = isCritical ? new Color(1.0f, 0.8f, 0.0f) : new Color(1.0f, 0.3f, 0.3f);

        if (isCritical)
        {
            text = "CRIT!\n" + text;
        }

        ShowDamageText(position, text, color, isCritical);
    }

    // ���� �Լ�
    public void ShowHeal(Vector3 position, int amount, bool isCritical = false)
    {
        string text = "+" + amount.ToString();
        Color color = isCritical ? new Color(0.4f, 1.0f, 0.4f) : new Color(0.3f, 0.9f, 0.3f);

        if (isCritical)
        {
            text = "HEAL CRIT!\n" + text;
        }

        ShowDamageText(position, text, color, isCritical);
    }

    // �̽� �Լ�
    public void ShowMiss(Vector3 position)
    {
        ShowDamageText(position, "MISS", Color.gray, false);
    }

    // ���� ȿ�� �Լ�
    public void ShowStatusEffect(Vector3 position, string effectName)
    {
        Color color;

        // ���� ȿ���� ���� ���� ����
        switch (effectName.ToLower())
        {
            case "poison":
                color = new Color(0.5f, 0.1f, 0.5f); // �����
                break;
            case "burn":
                color = new Color(1f, 0.4f, 0f); // ��Ȳ��
                break;
            case "freeze":
                color = new Color(0.5f, 0.8f, 1f); // �ϴû�
                break;
            case "stun":
                color = new Color(1f, 1f, 0f); // �����
                break;
            default:
                color = new Color(1f, 1f, 1f); // �⺻ ���
                break;
        }

        ShowDamageText(position, effectName.ToUpper(), color, false, true);
    }
}