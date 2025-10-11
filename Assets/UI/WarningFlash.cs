using UnityEngine;
using TMPro;

public class WarningFlash : MonoBehaviour
{
    public static WarningFlash Instance;
    public TextMeshProUGUI warningText;
    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FlashWarning(string text, int fontSize)
    {
        warningText.text = text;
        warningText.fontSize = fontSize;
        animator.SetTrigger("Flash");
    }
}
