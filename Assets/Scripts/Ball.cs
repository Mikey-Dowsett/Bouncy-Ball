using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    [Header("General")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Camera cam;
    [SerializeField] bool screenShakeEnable;
    
    [Header("Power")]
    [SerializeField] GameObject launchDirDisplay;
    [SerializeField] Image launchTriangle;
    [SerializeField] Gradient powerGradient;
    [SerializeField] float powerMultiplier;

    [Header("Trail")]
    [SerializeField] TrailRenderer tr;
    [SerializeField] float trailLengthModifier;

    private bool startLaunch;

    private void Update() {
        if(startLaunch) {
            //Gets the mouse position
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
                print(power);

                launchDirDisplay.SetActive(false);
                startLaunch = false;
            }
        }
        
        tr.emitting = rb.velocity.magnitude > trailLengthModifier / 2;
        tr.time = rb.velocity.magnitude / trailLengthModifier;
    }

    //Tracks the player clicking the ball;
    private void OnMouseOver(){
        if(Input.GetMouseButtonDown(0)) {
            startLaunch = true;
            launchDirDisplay.SetActive(true);
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

    //Shakes the camera when hitting a wall;
    private void OnCollisionEnter2D(Collision2D col) {
        if(screenShakeEnable)
            StartCoroutine(cam.GetComponent<CameraShake>().Shake(0.1f, rb.velocity.magnitude/trailLengthModifier));
    }
}
