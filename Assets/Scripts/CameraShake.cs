using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class CameraShake : MonoBehaviour {
    [SerializeField] private AudioSource musicSource;
    private Vector3 startPos;
    private bool screenShakeEnable = true;
    private bool musicEnable = true;
    private bool soundEnable = true;

    [SerializeField] private Texture2D closedMouse;

    private void Awake() {
        startPos = transform.position;
        musicEnable = true;
        soundEnable = true;
        screenShakeEnable = true;
    }

    private void Update() {
        ChangeSound();

        if (Input.GetMouseButton(0)) {
            Cursor.SetCursor(closedMouse, Vector2.zero, CursorMode.Auto);
        }
        else {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    private void ChangeSound() {
        if (!soundEnable) {
            AudioSource[] audioSources = GameObject.FindObjectsOfType<AudioSource>();
            foreach (AudioSource source in audioSources) {
                if (source != musicSource) {
                    source.enabled = false;
                }
            }
        } else {
            AudioSource[] audioSources = GameObject.FindObjectsOfType<AudioSource>();
            foreach (AudioSource source in audioSources) {
                if (source != musicSource) {
                    source.enabled = true;
                }
            }
        }
    }

    //Shakes the camera for a set duration with a set magnitude
    public IEnumerator Shake(float duration, float magnitude) {
        if(screenShakeEnable) {
            float counter = 0;
            while (counter < duration) {
                counter += Time.deltaTime;

                transform.position = new Vector3(startPos.x + Random.Range(-0.1f, 0.1f) * magnitude, startPos.y + Random.Range(-0.1f, 0.1f) * magnitude, -10);

                yield return null;
            }
            transform.position = startPos;
        } else {
            yield return null;
        }
        StopCoroutine("Shake");
    }

    public void ChangeShakeEnable() {
        screenShakeEnable = !screenShakeEnable;
    }

    public void ChangeMusicEnable() {
        musicEnable = !musicEnable;
        musicSource.enabled = musicEnable;
    }

    public void ChangeSoundEnable() {
        soundEnable = !soundEnable;
    }
}
