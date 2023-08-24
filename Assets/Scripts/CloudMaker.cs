using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMaker : MonoBehaviour {
    [SerializeField] private GameObject cloudPart;
    [SerializeField] private Vector2 startX;
    [SerializeField] private Vector2 startY;
    
    void Start() {
        for (int i = 0; i < Random.Range(3, 6); i++) {
            Instantiate(cloudPart, new Vector2(Random.Range(startX.x, startX.y), Random.Range(startY.x, startY.y)), cloudPart.transform.rotation);
        }
        StartCoroutine(Clouds());
    }

    private IEnumerator Clouds() {
        while (true) {
            Instantiate(cloudPart, new Vector2(startX.x, Random.Range(startY.x, startY.y)), cloudPart.transform.rotation);
            yield return new WaitForSeconds(Random.Range(7.5f, 12));
        }
    }
}
