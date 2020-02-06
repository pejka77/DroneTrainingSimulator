using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAir : MonoBehaviour {

    public static bool useAirCam = false;
    public GameObject cameraAir;

    void Start()
    {
        var audio = GameObject.FindObjectOfType<AudioSource>();
        audio.Play();
    }

	void Update () {
		if(Input.GetMouseButtonDown(0))
        {
            useAirCam = !useAirCam;
            var drones = GameObject.FindObjectsOfType<DroneController>();
            if (useAirCam)
            {
                foreach (var drone in drones)
                {
                    var camera = drone.GetComponentInChildren<CameraFollow>(true);
                    camera.gameObject.SetActive(false);
                }
                cameraAir.SetActive(true);
            }
            else
            {
                if (TrackManager.Instance.BestDrone != null)
                {
                    cameraAir.SetActive(false);
                    TrackManager.Instance.BestDrone.gameObject.GetComponentInChildren<CameraFollow>(true).gameObject.SetActive(true);
                }
            }
        }
	}
}
