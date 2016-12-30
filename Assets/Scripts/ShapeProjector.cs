using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class ShapeProjector : MonoBehaviour {

    //projection
    public Camera cam;
    public GameObject planeObj;
    public GameObject keyPoint;
    private ArrayList textures = new ArrayList();
    private InkscapeWrapper inkscapeWrapper;

    void Start()
    {
        inkscapeWrapper = gameObject.GetComponent<InkscapeWrapper>();


        if(inkscapeWrapper.svgToPng("test.svg", "test.png", 512, 512))
        {
            //success
            System.Console.WriteLine("success");
        }
        else
        {
            //error
            System.Console.WriteLine("error");
        }
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            project_plane();
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            move_plane();
        }

        if (Input.GetButtonDown("ToggleShape"))
        {
            planeObj.transform.GetComponent<MeshRenderer>().enabled = !planeObj.transform.GetComponent<MeshRenderer>().enabled;
        }
    }

    //Create keypoint object to be used later to check if player is standing in correct positoin for perspective effect
    private void create_keypoint()
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        Instantiate(keyPoint, pos, rot);
    }

    //Move shape plane away or towards player
    private void move_plane()
    {
        float zPos = planeObj.transform.localPosition.z + Input.GetAxis("Mouse ScrollWheel");

        if (zPos < 0.8) zPos = 0.86f;
        if (zPos > 4) zPos = 4f;

        Vector3 newPlanePos = new Vector3(planeObj.transform.localPosition.x, planeObj.transform.localPosition.y, zPos);

        planeObj.transform.localPosition = newPlanePos; 
    }

    //Loop throuhg each pixel in the shape plane and raycast through it if pixel is not transparent
    private void project_plane()
    {
        MeshRenderer rend = planeObj.transform.GetComponent<MeshRenderer>();
        Texture2D shapeTex = rend.material.mainTexture as Texture2D;

        textures.Clear();
        
        for (int x = 0; x < shapeTex.width; x++)
        {
            for (int y = 0; y < shapeTex.height; y++)
            {
                Color pixel = shapeTex.GetPixel(x, y);

                if (pixel.a != 0f)
                {
                    double xu = (double)x / shapeTex.width;
                    double yv = (double)y / shapeTex.height;
                    float xu_pos = (float)(xu * 2) - 1;
                    float yv_pos = (1 - (float)(yv * 2)) * -1;
                    raycast_through_pixel(xu_pos, yv_pos, pixel);
                }
            }
        }

        //For any splashable material hit by a raycast create a new texture
        for (int i = 0; i < textures.Count; i++)
        {
            Texture2D tex = textures[i] as Texture2D;
            tex.Apply();

            // Encode texture into PNG
            byte[] bytes = tex.EncodeToPNG();

            String tex_name = planeObj.transform.GetComponent<MeshRenderer>().material.name + "_wall" + i;
            File.WriteAllBytes(Application.dataPath + "/../Assets/" + tex_name + ".png", bytes);
        }
    }

    private void raycast_through_pixel(float xu_pos, float yv_pos, Color pix_color)
    {
        //determine raycast direction using the shape plane game object
        Vector3 point_loc_pos = planeObj.transform.localPosition;
        point_loc_pos.x += xu_pos;
        point_loc_pos.y += yv_pos;

        Vector3 point_world_pos = planeObj.transform.TransformPoint(point_loc_pos);
        Vector3 point_screen_pos = cam.WorldToScreenPoint(point_world_pos);

        Ray point_ray = cam.ScreenPointToRay(point_screen_pos);

        RaycastHit hit;
        if (!Physics.Raycast(point_ray, out hit, Mathf.Infinity)) return;      
        
        MeshRenderer rend = hit.transform.GetComponent<MeshRenderer>();       
        if (rend == null) return;

        //If the object hit by the raycast does not have the splashable scirpt attached return
        if (!hit.transform.GetComponent<splashable>()) return;

        //Create a list of textures to create and save in the Assets folder
        Texture2D out_tex = null;
        if (!hit.transform.GetComponent<splashable>().writtenTo)
        {
            out_tex = new Texture2D(512, 512, TextureFormat.ARGB32, false);
            rend.material.mainTexture = out_tex;
            textures.Add(out_tex);
            hit.transform.GetComponent<splashable>().writtenTo = true;
            create_keypoint();
        }

        //Set the pixel on the texture where the raycast hit
        Vector2 pixelUV = new Vector2();

        Texture2D tex = rend.material.mainTexture as Texture2D;
        pixelUV = hit.textureCoord;
        pixelUV.x *= tex.width;
        pixelUV.y *= tex.height;
        tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, pix_color);
    }

}
