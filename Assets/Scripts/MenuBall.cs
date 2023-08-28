using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBall : MonoBehaviour
{
    [Header("Ball")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] AudioSource audioSource;
    [SerializeField] private ParticleSystem wallHitPart;
    [SerializeField] float springPower;
    [SerializeField] private float camSize = 5;
    private Camera cam;

    [Header("Menu")]
    [SerializeField] GameObject levelMenu;
    [SerializeField] GameObject mainMenu;
    [SerializeField] Animator loadingScreenAnim;
    [SerializeField] private bool isEndScreen;

    void Start() {
        cam = Camera.main;
        cam.orthographicSize = camSize;
        rb.AddForce(new Vector2(Random.Range(-5, 5), Random.Range(-5, 5)) * Random.Range(2.5f, 5f), ForceMode2D.Impulse);
        if(rb.velocity.x > -0.1f && rb.velocity.x < 0.1f) {
            rb.velocity = new Vector2(2.5f, rb.velocity.y);
        }
        if(rb.velocity.y > -0.1f && rb.velocity.y < 0.1f) {
            rb.velocity = new Vector2(rb.velocity.x, 2.5f);
        }
    }

    private void Update() {
        if (rb.velocity.magnitude < 5f) {
            rb.AddForce(new Vector2(Random.Range(-5, 5), Random.Range(-5, 5)) * Random.Range(2.5f, 5f), ForceMode2D.Impulse);
        }

        if (isEndScreen && Input.GetMouseButtonDown(0)) {
            LoadLevel(0);
            isEndScreen = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D col) {
        //Plays a sound when hitting a wall;
        if(col.collider.CompareTag("Wall")) {
            Instantiate(wallHitPart, col.contacts[0].point, Quaternion.identity);
            audioSource.pitch = 1 + Random.Range(-0.1f, 0.1f);
            audioSource.Play();
        }

        //Increase velocity when hitting a spring;
        if(col.collider.CompareTag("Spring")) {
            col.collider.GetComponent<ParticleSystem>().Play();
            col.collider.GetComponent<Animator>().SetTrigger("Launch");
            col.collider.GetComponent<AudioSource>().pitch = 1 + Random.Range(-0.1f, 0.1f);
            col.collider.GetComponent<AudioSource>().Play();
            StartCoroutine(cam.GetComponent<CameraShake>().Shake(0.1f, rb.velocity.magnitude/15));
            rb.velocity *= springPower;
        }
    }

    public void OpenLevelSelect() {
        mainMenu.SetActive(false);
        levelMenu.SetActive(true);
    }

    public void CloseLevelSelect() {
        mainMenu.SetActive(true);
        levelMenu.SetActive(false);
    }

    public void LoadLevel(int levelName) {
        StartCoroutine(LoadingScreen($"Level{levelName}"));
    }

    private IEnumerator LoadingScreen(string levelName) {
        loadingScreenAnim.SetTrigger("Close");
        yield return new WaitForSeconds(1.25f);
        SceneManager.LoadScene(levelName);
    }
}
