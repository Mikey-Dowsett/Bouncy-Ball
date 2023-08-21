using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Ball : MonoBehaviour
{
    [Header("General")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Camera cam;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Animator ballAnim;
    [SerializeField] GameObject tutorialWindow;
    
    [Header("Power")]
    [SerializeField] GameObject launchDirDisplay;
    [SerializeField] Image launchTriangle;
    [SerializeField] Gradient powerGradient;
    [SerializeField] float powerMultiplier;
    [SerializeField] float springPower;
    private bool startLaunch;

    [Header("Trail")]
    [SerializeField] TrailRenderer tr;
    [SerializeField] float trailLengthModifier;

    [Header("Coins")]
    [SerializeField] ParticleSystem coinCollected;
    private float totalCoins;

    [Header("Timer")]
    [SerializeField] TMP_Text timerText;
    [SerializeField] float timeInSeconds;
    bool timerStarted;

    [Header("GameOver")]
    [SerializeField] GameObject menuUI;
    [SerializeField] ParticleSystem loseParticles;
    [SerializeField] ParticleSystem winParticles;
    [SerializeField] AudioClip deflateAudio;
    [SerializeField] AudioClip victoryAudio;
    [SerializeField] TMP_Text VorDText;
    [SerializeField] Button continueButton;
    [SerializeField] Animator loadingScreenAnim;
    [SerializeField] string homeScene;
    [SerializeField] string currentScene;
    [SerializeField] string nextScene;
    private bool gameOver;
    
    private void Start() {
        totalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;
        ShowTime();
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0) && !gameOver) {
            startLaunch = true;
            launchDirDisplay.SetActive(true);
        }
        
        if(startLaunch && !gameOver) {
            //Gets the mouse position;
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            
            //Rotate the arrow in direction of launch;
            Vector2 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            launchDirDisplay.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);

            //Find power of ball launch;
            float power = Vector2.Distance(transform.position, mousePos);
            power = Mathf.Clamp(power * 2, 0, 10);

            //Find rotation to launch ball at
            Vector2 launchDir = mousePos - (Vector2)transform.position;
            launchDir = new Vector2(-Mathf.Clamp(launchDir.x, -2, 2), -Mathf.Clamp(launchDir.y, -2, 2));

            //Sets the color of the launch arrow;
            SetPowerColor(launchDir, power);

            //Launches the ball when the player stops clicking;
            if(!Input.GetMouseButton(0)){
                rb.AddForce(launchDir * power * powerMultiplier, ForceMode2D.Impulse);

                launchDirDisplay.SetActive(false);
                startLaunch = false;

                if(!timerStarted) StartCoroutine(Timer());

                if(tutorialWindow) tutorialWindow.SetActive(false);
            }
        }
        
        //Controls the size of the trail;
        tr.emitting = rb.velocity.magnitude > trailLengthModifier / 2;
        tr.time = rb.velocity.magnitude / trailLengthModifier;

        if(totalCoins <= 0 && !gameOver) {
            StopCoroutine("Timer");
            StartCoroutine(Win());
        }
    }

    //Calculates the color of the power arrow;
    private void SetPowerColor(Vector2 launchDir, float power) {
        Vector2 fullPower = launchDir.normalized * power * powerMultiplier;
        if(fullPower.x < 0) fullPower = new Vector2(-fullPower.x, fullPower.y);
        if(fullPower.y < 0) fullPower = new Vector2(fullPower.x, -fullPower.y);

        float avgPower = fullPower.x + fullPower.y / 2;
        float percent = avgPower / 22;
        launchTriangle.color = powerGradient.Evaluate(percent);
    }

    
    private void OnCollisionEnter2D(Collision2D col) {
        //Plays a sound when hitting a wall;
        if(col.collider.CompareTag("Wall") && !gameOver) {
            audioSource.pitch = 1 + Random.Range(-0.1f, 0.1f);
            audioSource.Play();
        }

        //Increase velocity when hitting a spring;
        if(col.collider.CompareTag("Spring") && !gameOver) {
            col.collider.GetComponent<Animator>().SetTrigger("Launch");
            col.collider.GetComponent<AudioSource>().pitch = 1 + Random.Range(-0.1f, 0.1f);
            col.collider.GetComponent<AudioSource>().Play();
            StartCoroutine(cam.GetComponent<CameraShake>().Shake(0.1f, rb.velocity.magnitude/trailLengthModifier));
            rb.velocity *= springPower;
        }
    }

    //Remove coin from count when hitting one;
    private void OnTriggerEnter2D(Collider2D col) {
        if(col.CompareTag("Coin") && !gameOver) {
            Instantiate(coinCollected, col.transform.position, Quaternion.identity);
            Destroy(col.gameObject);
            totalCoins--;
        }
    }

    //Counts down the time until the player hits 0;
    private IEnumerator Timer() {
        timerStarted = true;
        ShowTime();
        while(timeInSeconds > 0) {
            if(totalCoins <= 0) {
                break;
            }
            yield return new WaitForSeconds(1f);
            timeInSeconds--;
            ShowTime();        
        }

        if(totalCoins > 0) {
            gameOver = true;
            loseParticles.Play();
            audioSource.clip = deflateAudio;
            audioSource.Play();
            rb.velocity *= 0.1f;

            VorDText.text = "Defeat";
            continueButton.interactable = false;

            yield return new WaitForSeconds(2.5f);
            ballAnim.SetTrigger("Death");
            yield return new WaitForSeconds(0.5f);
            menuUI.SetActive(true);
        }
    }

    //Runs through the victory sequence;
    private IEnumerator Win() {
        gameOver = true;
        rb.velocity *= 0.1f;
        winParticles.Play();
        audioSource.clip = victoryAudio;
        audioSource.volume = 0.25f;
        audioSource.Play();

        yield return new WaitForSeconds(3f);
        VorDText.text = "Victory";
        continueButton.interactable = true;
        menuUI.SetActive(true);
    }

    //Displays the time in UI;
    private void ShowTime() {
        int minutes = (int)(timeInSeconds / 60);
        float seconds = timeInSeconds % 60;
        if(seconds < 10){
            timerText.text = $"{minutes}:0{seconds}";
        } else {
            timerText.text = $"{minutes}:{seconds}";
        }
    }

    private IEnumerator LoadingScreen(string levelName) {
        loadingScreenAnim.SetTrigger("Close");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(levelName);
    }
    
    public void Home() {
        StartCoroutine(LoadingScreen(homeScene));
    }

    public void Replay() {
        StartCoroutine(LoadingScreen(currentScene));
    }

    public void NextLevel() {
        StartCoroutine(LoadingScreen(nextScene));
    }
}
