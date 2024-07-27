//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PulseHandler : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//    IEnumerator PulseLaserCharge()
//    {
//        Vector3 originalScale = laserCharge.transform.localScale;
//        Vector3 pulseScale = originalScale * 1.5f;

//        // Pulse effect
//        while (isShooting)
//        {
//            float elapsedTime = 0f;
//            // Scale up
//            while (elapsedTime < pulseDuration / 2)
//            {
//                float progress = elapsedTime / (pulseDuration / 2);
//                laserCharge.transform.localScale = Vector3.Lerp(originalScale, pulseScale, progress);
//                elapsedTime += Time.deltaTime;
//                yield return null;
//            }
//            // Scale down
//            elapsedTime = 0f;
//            while (elapsedTime < pulseDuration / 2)
//            {
//                float progress = elapsedTime / (pulseDuration / 2);
//                laserCharge.transform.localScale = Vector3.Lerp(pulseScale, originalScale, progress);
//                elapsedTime += Time.deltaTime;
//                yield return null;
//            }
//        }
//        DestroyComponent(0);
//    }
//}
