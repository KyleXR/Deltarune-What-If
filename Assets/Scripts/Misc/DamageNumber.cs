using UnityEngine;
using TMPro;
using System.Collections;

public class DamageNumber : MonoBehaviour
{
    public TextMeshProUGUI damageText; // Assign this in the inspector
    public float floatSpeed = 1.0f;
    public float fadeSpeed = 1.0f;

    private void Start()
    {
        StartCoroutine(FloatAndFade());
    }

    public void Initialize(int damageAmount)
    {
        damageText.text = damageAmount.ToString();
    }

    private IEnumerator FloatAndFade()
    {
        float elapsedTime = 0;
        Color startColor = damageText.color;
        Vector3 startPosition = transform.position;

        while (elapsedTime < 1)
        {
            float t = elapsedTime / 1;
            transform.position = startPosition + Vector3.up * floatSpeed * t;
            damageText.color = Color.Lerp(startColor, Color.clear, t);
            elapsedTime += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        Destroy(gameObject); // Clean up after animation
    }
}
