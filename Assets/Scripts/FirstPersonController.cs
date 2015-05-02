using UnityEngine;
using System.Collections;
using System;

public class FirstPersonController : MonoBehaviour {

    public float speed;
    public float jumpHeight;
    public float maxVelocityChange;
    Boolean grounded;
    public Camera cam;

	// Use this for initialization
	void Start () {

	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (grounded)
        {
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetVelocity = cam.transform.TransformDirection(targetVelocity);
            targetVelocity *= speed;
            Vector3 v = GetComponent<Rigidbody>().velocity;
            Vector3 velocityChange = (targetVelocity - v);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            
            GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);

            if (Input.GetButtonDown("Jump"))
            {
                print("Jump");
                print(transform.up * jumpHeight);
                GetComponent<Rigidbody>().AddForce(transform.up * jumpHeight);
            }
        }

        grounded = false;
	
	}

    void Update()
    {
        if (!Input.GetMouseButton(0))
            return;

        Vector2 topPix;
        Vector2 bottomPix;

        Texture2D tex;

        Vector3 rotation = cam.transform.forward;
        tex = raycast_get_tex(rotation);
        paint_at_raycast(rotation);
        //paint_at_raycast(position + new Vector3(0.1f, 0, 0));
        topPix = raycast_get_pix(rotation + new Vector3(0, 0.1f, 0));
        paint_at_raycast(rotation + new Vector3(0, 0.1f, 0));
        //paint_at_raycast(position + new Vector3(-0.1f, 0, 0));
        bottomPix = raycast_get_pix(rotation + new Vector3(0, -0.1f, 0));
        paint_at_raycast(rotation + new Vector3(0, -0.1f, 0));
        //paint_at_raycast(position + new Vector3(0, 0, 0.1f));
        //paint_at_raycast(position + new Vector3(0, 0, -0.1f));
        //paint_at_raycast(cam.transform.position);        

        //print(topPix);
        //print(bottomPix);

        //tex.SetPixel((int)topPix.x, (int)topPix.y, Color.black);
        //tex.SetPixel((int)bottomPix.x, (int)bottomPix.y, Color.black);
        //tex.Apply();

        //print("Drawing line between" + topPix + " and " + bottomPix);
        paint_line(topPix, bottomPix, tex);
    }

    private void paint_at_raycast(Vector3 rotation)
    {
        print(rotation);
        Vector2 pixelUV = new Vector2();

        RaycastHit hit;
        if (!Physics.Raycast(cam.transform.position, rotation, out hit))
        {
            return;
        }

        MeshRenderer rend = hit.transform.GetComponent<MeshRenderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
        {
            return;
        }

        Texture2D tex = rend.material.mainTexture as Texture2D;
        pixelUV = hit.textureCoord;
        pixelUV.x *= tex.width;
        pixelUV.y *= tex.height;
        tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.black);
        tex.Apply();
    }

    private Texture2D raycast_get_tex(Vector3 rotation)
    {
        RaycastHit hit;
        Physics.Raycast(cam.transform.position, rotation, out hit);  

        MeshRenderer rend = hit.transform.GetComponent<MeshRenderer>();

        return rend.material.mainTexture as Texture2D;
    }

    private void paint_line(Vector2 topPix, Vector2 bottomPix, Texture2D tex)
    {
        if ((int)topPix.x == (int)bottomPix.x)
        {
            for (float min = bottomPix.y; min <= topPix.y; min++)
            {
                print("Drawing point (" + min + "," + topPix.x + ")");
                tex.SetPixel((int)topPix.x, (int)min, Color.black);
            }
        }
        else
        {
            float gradient = (topPix.y - bottomPix.y) / (topPix.x - bottomPix.x);
            print("Gradient is " + gradient);
            for (float min = bottomPix.x; min <= topPix.x; min++)
            {
                int y_pixel = (int)((min - bottomPix.x) * gradient + bottomPix.y);
                print("Drawing point (" + y_pixel + "," + min + ")");
                tex.SetPixel((int)min, y_pixel, Color.black);
            }
        }
        //tex.SetPixel((int)topPix.x, (int)topPix.y, Color.black);
        //tex.SetPixel((int)bottomPix.x, (int)bottomPix.y, Color.black);
        tex.Apply();
    }

    private Vector2 raycast_get_pix(Vector3 rotation)
    {
        //print(rotation);
        Vector2 pixelUV = new Vector2();

        RaycastHit hit;
        if (!Physics.Raycast(cam.transform.position, rotation, out hit))
        {
            return pixelUV;
        }

        MeshRenderer rend = hit.transform.GetComponent<MeshRenderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
        {
            return pixelUV;
        }

        Texture2D tex = rend.material.mainTexture as Texture2D;
        pixelUV = hit.textureCoord;
        pixelUV.x *= tex.width;
        pixelUV.y *= tex.height;

        return pixelUV;
    }

    void OnCollisionStay(Collision collision) {
        if (collision.transform.tag == "Ground"){
                grounded=true;
        }
    }
}
