using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using RDKitForUnity;
public class NoClipController : MonoBehaviour
{
    public RenderingData renderingData;
    public float speed = 10f;
    private bool isNoClipEnabled = false;
    public static bool userisTyping = false;

    public Vector3 origin = new Vector3(0, 0, -10);
    private RDKitUnityFuncs rDKitUnityFuncs;
    void Start()
    {
        rDKitUnityFuncs = new RDKitUnityFuncs();
    }
    void Update()
    {
        if (userisTyping == false)
        {
            if (isNoClipEnabled)
            {
                float moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
                float moveY = Input.GetAxis("Jump") * 5 * Time.deltaTime;
                float moveZ = Input.GetAxis("Vertical") * speed * Time.deltaTime;
                //Use localRotation rather than rotation so that we rotate relative to the camera's up
                Vector3 angles = transform.localRotation.eulerAngles;
                float yRot = (angles.x + 180f) % 360f - 180f;
                yRot -= Input.GetAxis("Mouse Y") * speed;
                float xRot = (angles.y + 180f) % 360f - 180f;
                xRot += Input.GetAxis("Mouse X") * speed;
                //Clamp between a min and max rotation (replace these with whatever you want the extents to be). The extents shouldn't be outside the range of [-180,180] and for a camera controller also probably should stay within [-89,89] to avoid incorrect rotation
                yRot = Mathf.Clamp(yRot, -90f, 90f);


                //Apply the rotation by converting back to a quaternion. We can't directly set a single value of eulerAngles, which is why we stored all of the quaternion's angles and only updated one
                transform.localRotation = Quaternion.Euler(new Vector3(yRot, xRot, 0));
                transform.Translate(new Vector3(moveX, moveY, moveZ));
            }
            else
            {
                //Functions for orbiting around the protein when noclip is disabled
                //The ? functions you see are basically converting a boolean to an integer, sidestepping a nested if statement
                transform.RotateAround(new Vector3(0, 0, 0), transform.up, Input.GetAxis("Mouse X") * 30 * (Input.GetMouseButton(0) ? 1 : 0));
                transform.RotateAround(new Vector3(0, 0, 0), transform.right, -Input.GetAxis("Mouse Y") * 30 * (Input.GetMouseButton(0) ? 1 : 0));

                //Function for panning when noclip is disabled
                //Same ? function as above is used here as well
                transform.Translate(-Input.GetAxis("Mouse X") * (Input.GetMouseButton(1) ? 1 : 0), -Input.GetAxis("Mouse Y") * (Input.GetMouseButton(1) ? 1 : 0), 0);

            }
            transform.Translate(0, 0, Input.mouseScrollDelta.y);

            //Button to reset camera to original position and rotation 
            if (Input.GetKeyDown("o") == true)
            {
                transform.position = origin;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                isNoClipEnabled = false;

            }
            if (Input.GetKeyDown(KeyCode.N)) // Toggle noclip with 'N'
            {
                isNoClipEnabled = !isNoClipEnabled;
            }
            /*if (Input.GetKeyDown(KeyCode.P))
            {
                rDKitUnityFuncs.GenerateBaseMolecule("Assets/9IZ1.pdb",false,false,renderingData.renderMode,0.1f,0.055f,0.0095f);

            }*/

        }

    }
    public void TypeInvert(bool typetype)
    {
        userisTyping = typetype;
        print(typetype);
    }
}
