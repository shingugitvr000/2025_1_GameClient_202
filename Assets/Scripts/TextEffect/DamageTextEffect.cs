using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class DamageTextEffect : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 100f; // UI������ �� ū ��
    [SerializeField] private float lifeTime = 1.5f;

    private TextMeshProUGUI textMesh;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Color originalColor;
    private Vector2 moveDirection;
    private float timer = 0f;

    private bool isCritical = false;
    private bool isStatusEffect = false;
    private bool useGravity = true;
    private float verticalVelocity = 100f;

    public void Initialize(bool critical, bool statusEffect)
    {
        isCritical = critical;
        isStatusEffect = statusEffect;

        // ���� ȿ���� �߷� ��� �� ��
        if (isStatusEffect)
        {
            useGravity = false;
        }

        Start(); // �ʱ�ȭ ��� ����
    }

    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (textMesh != null)
        {
            originalColor = textMesh.color;

            // ���� ���� ���� (UI �����̽���)
            float randomX = Random.Range(-0.5f, 0.5f);
            float randomY = useGravity ? Random.Range(0.5f, 1.0f) : Random.Range(0.8f, 1.5f);
            moveDirection = new Vector2(randomX, randomY).normalized;

            // ���� ȸ�� (UI��)
            if (rectTransform != null)
            {
                rectTransform.rotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));
            }

            // �ʱ� ���� �ӵ� (�߷� ��� ��)
            if (useGravity)
            {
                verticalVelocity = Random.Range(100f, 200f); // UI������ �� ū ��
            }

            // ��ġ ������ ȿ��
            StartCoroutine(PunchScale(isCritical ? 1.5f : 1.2f));

            // ũ��Ƽ���� ��� �߰� ȿ��
            if (isCritical)
            {
                StartCoroutine(FlashText());
                StartCoroutine(CreateFlashEffect());
            }
        }
    }

    // ���� �̵� ���� (�߷� ������� �ʰ� ���� �ö󰡴� ȿ��)
    public void SetVerticalMovement()
    {
        useGravity = false;
    }

    private void Update()
    {
        if (rectTransform == null) return;

        // �̵� ó��
        if (useGravity)
        {
            // �߷� ȿ�� (������ �̵�) - UI��
            verticalVelocity -= 300f * Time.deltaTime; // UI�� �߷°�
            rectTransform.anchoredPosition += new Vector2(0, verticalVelocity * Time.deltaTime);
            rectTransform.anchoredPosition += new Vector2(moveDirection.x * moveSpeed * Time.deltaTime, 0);
        }
        else
        {
            // ���� �̵� - UI��
            rectTransform.anchoredPosition += (Vector2)(moveDirection * moveSpeed * Time.deltaTime);
        }

        // ���̵� �ƿ�
        timer += Time.deltaTime;
        if (timer >= lifeTime * 0.5f)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, (timer - lifeTime * 0.5f) / (lifeTime * 0.5f));
            }
            else if (textMesh != null)
            {
                float alpha = Mathf.Lerp(originalColor.a, 0f, (timer - lifeTime * 0.5f) / (lifeTime * 0.5f));
                textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            }

            // �ӵ� ����
            moveSpeed = Mathf.Lerp(moveSpeed, 20f, Time.deltaTime * 2f);

            // ������ ���������� �ı�
            if ((canvasGroup != null && canvasGroup.alpha <= 0.05f) ||
                (textMesh != null && textMesh.color.a <= 0.05f))
            {
                Destroy(gameObject);
            }
        }
    }

    // ������ ��ġ ȿ��
    private IEnumerator PunchScale(float intensity)
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * intensity;

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    // ��½�� ȿ��
    private IEnumerator FlashText()
    {
        if (textMesh == null) yield break;

        Color flashColor = Color.white;
        float flashDuration = 0.2f;

        // ���� ���� ����
        Color startColor = textMesh.color;

        // ��½�� �������� ����
        textMesh.color = flashColor;

        // ���
        yield return new WaitForSeconds(flashDuration);

        // ���� �������� ����
        textMesh.color = startColor;
    }

    // �ܻ� ȿ���� UI�� ������ ȿ���� ��ü
    private IEnumerator CreateFlashEffect()
    {
        if (textMesh == null) yield break;

        float interval = 0.05f;
        int flashCount = 3;

        for (int i = 0; i < flashCount; i++)
        {
            // ���İ� �������� ������ ����
            textMesh.alpha = 0.5f;
            yield return new WaitForSeconds(interval);
            textMesh.alpha = 1.0f;
            yield return new WaitForSeconds(interval);
        }
    }
}