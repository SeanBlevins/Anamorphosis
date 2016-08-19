using UnityEngine;
using System.Collections;

public class Mouse : MonoBehaviour {

    public float sensitivity;
	
	// Update is called once per frame
	void Update () {
        Vector3 mouseY = new Vector3(-Input.GetAxis("Mouse Y") * sensitivity, 0, 0);

        float camRotationX = transform.rotation.eulerAngles.x;
        float rotTo = camRotationX + mouseY.x;

        if (rotTo > 70 && rotTo < 300)
            return;

        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * sensitivity, 0), Space.World);
        transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * sensitivity, 0, 0));
	
	}
}
