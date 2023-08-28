using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Ball : MonoBehaviour
{
    [Header("General")] 
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject tutorialWindow;
    [SerializeField] private TMP_Text showLevelText;
    [SerializeField] private ParticleSystem wallHitPart;
    [SerializeField] private float camSize = 5;
    
    [Header("Power")]
    [SerializeField] private GameObject launchDirDisplay;
    [SerializeField] private Image launchTriangle;
    [SerializeField] private Gradient powerGradient;
    [SerializeField] private float powerMultiplier;
    [SerializeField] private float springPower;
    
    [Header("GameOver")]
    [SerializeField] private ParticleSystem winParticles;
    [SerializeField] private AudioClip victoryAudio;
    [SerializeField] private Animator loadingScreenAnim;
    [SerializeField] private int currentScene;
    [SerializeField] private int nextScene;
    
    private Camera cam;
    private float totalCoins;
    private bool startLaunch;
    private bool gameOver;
    private bool paused;
    
    private void Awake() {
        totalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;
        showLevelText.text = $"Level {currentScene}";
        cam = Camera.main;
        cam.orthographicSize = camSize;
    }

    private void Update() {
        //Tracks when the player starts to fire the ball;
        if(Input.GetMouseButtonDown(0) && !gameOver && !paused) {
            startLaunch = true;
            launchDirDisplay.SetActive(true);
        }
        
        if(startLaunch && !gameOver) {
            //Gets the mouse position;
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 curPos = transform.position;
            
            //Rotate the arrow in direction of launch;
            Vector2 dir = Input.mousePosition - cam.WorldToScreenPoint(curPos);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            launchDirDisplay.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);

            //Find power of ball launch;
            float power = Vector2.Distance(curPos, mousePos);
            power = Mathf.Clamp(power * 4f, 0, 10);

            //Find rotation to launch ball at
            Vector2 launchDir = mousePos - (Vector2)curPos;
            launchDir = new Vector2(-Mathf.Clamp(launchDir.x, -2, 2), -Mathf.Clamp(launchDir.y, -2, 2));

            //Sets the color of the launch arrow;
            SetPowerColor(launchDir, power);

            //Launches the ball when the player stops clicking;
            if(!Input.GetMouseButton(0)){
                rb.AddForce(power * powerMultiplier * launchDir, ForceMode2D.Impulse);

                launchDirDisplay.SetActive(false);
                startLaunch = false;
                
                if(tutorialWindow) tutorialWindow.SetActive(false);
            }
        }

        //Ends the game when all coins are collected;
        if(totalCoins <= 0 && !gameOver) {
            StartCoroutine(Win());
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            StartCoroutine(LoadingScreen("Level0"));
        }
    }

    //Calculates the color of the power arrow;
    private void SetPowerColor(Vector2 launchDir, float power) {
        Vector2 fullPower =  power * powerMultiplier * launchDir.normalized;
        if(fullPower.x < 0) fullPower = new Vector2(-fullPower.x, fullPower.y);
        if(fullPower.y < 0) fullPower = new Vector2(fullPower.x, -fullPower.y);

        float avgPower = fullPower.x + fullPower.y / 2;
        float percent = avgPower / 20;
        launchTriangle.color = powerGradient.Evaluate(percent);
    }
    
    private void OnCollisionEnter2D(Collision2D col) {
        //Plays a sound when hitting a wall;
        if(col.collider.CompareTag("Wall") && !gameOver) {
            Instantiate(wallHitPart, col.contacts[0].point, Quaternion.identity);
            audioSource.pitch = 1 + Random.Range(-0.1f, 0.1f);
            audioSource.Play();
        }

        //Increase velocity when hitting a spring;
        if(col.collider.CompareTag("Spring") && !gameOver) {
            col.collider.GetComponent<ParticleSystem>().Play();
            col.collider.GetComponent<Animator>().SetTrigger("Launch");
            col.collider.GetComponent<AudioSource>().pitch = 1 + Random.Range(-0.1f, 0.1f);
            col.collider.GetComponent<AudioSource>().Play();
            StartCoroutine(cam.GetComponent<CameraShake>().Shake(0.1f, springPower));
            rb.velocity *= springPower;
        }
    }

    //Remove coin from count when hitting one;
    private void OnTriggerEnter2D(Collider2D col) {
        if(col.CompareTag("Coin") && !gameOver) {
            col.GetComponent<ParticleSystem>().Stop();
            col.GetComponent<AudioSource>().Play();
            col.GetComponent<BoxCollider2D>().enabled = false;
            totalCoins--;
        }
    }

    //Runs through the victory sequence;
    private IEnumerator Win() {
        gameOver = true;
        launchDirDisplay.SetActive(false);
        rb.velocity *= 0.25f;
        winParticles.Play();
        audioSource.clip = victoryAudio;
        audioSource.volume = 0.25f;
        audioSource.Play();

        yield return new WaitForSeconds(3.25f);
        NextLevel();
    }

    private IEnumerator LoadingScreen(string levelName) {
        loadingScreenAnim.SetTrigger("Close");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(levelName);
    }
    
    public void NextLevel() {
        StartCoroutine(LoadingScreen($"Level{nextScene}"));
    }
}
