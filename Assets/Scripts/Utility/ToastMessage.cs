using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToastMessage : MonoBehaviour
{
    public static ToastMessage Instance {  get; private set; }

    [SerializeField] private GameObject toastPrefab;
    [SerializeField] private Transform messageContainer;
    [SerializeField] private float displayTime = 2.5f;
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private int maxMessage = 5;

    private Queue<GameObject> messageQueue = new Queue<GameObject>();
    private List<GameObject> activeMessage = new List<GameObject>();
    private bool isProcessingQueue = false;

    public enum MessageType
    {
        Normal,
        Success,
        Warning,
        Error,
        info
    }


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowMessage(string message, MessageType type = MessageType.Normal)
    {
        if (toastPrefab == null || messageContainer == null) return;

        //�޼��� �ν��Ͻ� ���� 
        GameObject toastInstance = Instantiate(toastPrefab, messageContainer);
        toastInstance.SetActive(false);

        //�ؽ�Ʈ ������Ʈ ã��
        TextMeshProUGUI textComponent = toastInstance.GetComponentInChildren<TextMeshProUGUI>();
        Image backgroundImage = toastInstance.GetComponentInChildren<Image>();

        if (textComponent != null)
        {
            textComponent.text = message;               //�޼��� ���� ����

            Color textColor;                            //�޼��� Ÿ�Կ� ���� ���� ���� 
            Color backgroundColor;

            switch (type)
            {
                case MessageType.Success:
                    textColor = Color.green;
                    backgroundColor = new Color(0.2f, 0.6f, 0.2f, 0.8f);
                    break;
                case MessageType.Warning:
                    textColor = Color.yellow;
                    backgroundColor = new Color(0.8f, 0.6f, 0.2f, 0.8f);
                    break;
                case MessageType.Error:
                    textColor = Color.red;
                    backgroundColor = new Color(0.2f, 0.6f, 0.2f, 0.8f);
                    break;
                case MessageType.info:
                    textColor = Color.blue;
                    backgroundColor = new Color(0.2f, 0.6f, 0.2f, 0.8f);
                    break;
                default:
                    textColor = Color.white;
                    backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                    break;
            }

            textComponent.color = textColor;
            if(backgroundColor != null)
            {
                backgroundImage.color = backgroundColor;
            }
        }

        messageQueue.Enqueue(toastInstance);            //�޼��� ť�� �߰�

        if(!isProcessingQueue)                          //ť ó�� ����
        {
            StartCoroutine(ProcessMessageQueue());
        }
    }

    private IEnumerator ProcessMessageQueue()
    {
        isProcessingQueue = true;

        while(messageQueue.Count > 0)
        {
            GameObject toast = messageQueue.Dequeue();              //ť���� �޼��� �������� 

            if (activeMessage.Count >= maxMessage && activeMessage.Count > 0) //Ȱ�� �޼����� �ִ� ������ �����ϸ� ���� ������ �޼��� ���� 
            {
                Destroy(activeMessage[0]);
                activeMessage.RemoveAt(0);
            }

            //�޼��� ǥ��
            toast.SetActive(true);
            activeMessage.Add(toast);

            //ĵ���� �׷� ��������
            CanvasGroup canvasGroup = toast.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = toast.AddComponent<CanvasGroup>();
            }

            //���̵� ��
            canvasGroup.alpha = 0;
            float elapedTime = 0;
            while( elapedTime < fadeTime)
            {
                canvasGroup.alpha = Mathf.Lerp(0, 1, elapedTime / fadeTime);
                elapedTime += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = 1;

            yield return new WaitForSeconds(displayTime);

            //���̵� �ƿ� 
            elapedTime = 0;
            while(elapedTime < fadeTime)
            {
                canvasGroup.alpha = Mathf.Lerp(1, 0, elapedTime / fadeTime);
                elapedTime += Time.deltaTime;
                yield return null;
            }

            activeMessage.Remove(toast);
            Destroy(toast);

            yield return new WaitForSeconds(0.1f);
        }

        isProcessingQueue = false;
    }

    
}
