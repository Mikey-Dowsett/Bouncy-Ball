using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Sprite[] crateLooks;
    [SerializeField] AudioSource audioSource;
    [SerializeField] ParticleSystem crateBreak;
    
    private int hits;

    private void OnCollisionEnter2D(Collision2D col) {
        if(col.collider.CompareTag("Ball")) {
            hits++;
            sr.sprite = crateLooks[hits];
            audioSource.Play();
            if(hits >= crateLooks.Length - 1) {
                Instantiate(crateBreak, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
