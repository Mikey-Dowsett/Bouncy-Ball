using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    Vector3 startPos;

    private void Awake() {
        startPos = transform.position;
    }

    //Shakes the camera for a set duration with a set magnitude
    public IEnumerator Shake(float duration, float magnitude) {
        float counter = 0;
        while (counter < duration) {
            counter += Time.deltaTime;

            transform.position = new Vector3(startPos.x + Random.Range(-0.1f, 0.1f) * magnitude, startPos.y + Random.Range(-0.1f, 0.1f) * magnitude, -10);

            yield return null;
        }
        transform.position = startPos;
    }
}
