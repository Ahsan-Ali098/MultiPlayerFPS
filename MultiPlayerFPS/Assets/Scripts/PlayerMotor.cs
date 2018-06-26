using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {


    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float  cameraRotationX = 0;
    private float currentCameraRotationX = 0f; 
    private Vector3 thursterForce = Vector3.zero;
    [SerializeField]
    private float cameraRotationLimit = 55;



    private Rigidbody rb;

	// Use this for initialization
	void Start () 
    {
        rb = GetComponent<Rigidbody>();		
	}
	
    //gets a movement vector
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;

    }


    //gets  a rotational vector
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;

    }


    //gets a rotational vector for camera
    public void RotateCamera(float _cameraRotationX)
    {
        cameraRotationX = _cameraRotationX;

    }


    //get a force character for thursters
    public void ApplyThurster(Vector3 _thursterForce)
    {
        thursterForce = _thursterForce;
    }


	// Update is called once per frame
	void FixedUpdate ()
    {
		PerformMovement();
        PerformRotation();
	}

    //performs mmovement based on velocity vector
    void PerformMovement()
    {
        if(velocity!=Vector3.zero)
        {
            rb.MovePosition(rb.position+velocity*Time.fixedDeltaTime);
        }
        if(thursterForce!=Vector3.zero)
        {
            rb.AddForce(thursterForce*Time.fixedDeltaTime,ForceMode.Acceleration);
        }

    }

    //perform Rotation
    void PerformRotation()
    {
        rb.MoveRotation(rb.rotation*Quaternion.Euler(rotation));
        if(cam!=null)
        {
            //cam.transform.Rotate(-cameraRotation);

            //new rotational calculation
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);

        }
    }
}
