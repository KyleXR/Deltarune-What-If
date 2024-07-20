using System.Collections;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    public float timeToDestroy = 5.0f; // Time in seconds before the object is destroyed

    private void Start()
    {
        StartCoroutine(DestroyAfterDelay(timeToDestroy));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
