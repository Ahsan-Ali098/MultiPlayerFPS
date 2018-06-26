using UnityEngine;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f;

    [SerializeField]
    private float thursterForce = 2000f;

    [SerializeField]
    private float thruesterFuelBurnSpeed = 1f;

    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f;
    private float thrusterFuelAmount = 1f;

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;  
    }
    [SerializeField]
    private LayerMask environmentMask;
    
    

    [Header("Spring settings:")]


     
    [SerializeField]
    
    private float jointSpring=20f;

    [SerializeField]
    private float jointMaxForce= 40f;

    //component caching
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();

        SetJointSettings(jointSpring);
    }
    void Update()
    {
        //Setting target position for spring
        //this makes the physics act make ryt 
        //when it comes to applying gravity while flying

        RaycastHit _hit;
        if(Physics.Raycast(transform.position,Vector3.down,out _hit,100f,environmentMask))
        {
            joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }


        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");


        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;

        //Final movement Vector
        Vector3 _velocity = (_movHorizontal + _movVertical) * speed;

        //animate movement
        animator.SetFloat("ForwardVelocity", _zMov);

        //Applying movement
        motor.Move(_velocity);

        //calculate rotation as 3d vector(Turning around)
        float _yRot = Input.GetAxisRaw("Mouse X");
        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity;

        //applying Rotation
        motor.Rotate(_rotation);

        //calculate camera rotation as 3d vector(Turning around)

        float _xRot = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRot * lookSensitivity;

        //apply camera rotation
        motor.RotateCamera(_cameraRotationX);


        Vector3 _thursterForce = Vector3.zero;
        
        //calculate the thruster force based on the player input
        if(Input.GetButton("Jump") && thrusterFuelAmount > 0f)
        {

            thrusterFuelAmount -= thruesterFuelBurnSpeed * Time.deltaTime;

            if(thrusterFuelAmount>=0.01f)
            {
                _thursterForce = Vector3.up * thursterForce;
                SetJointSettings(0f);
            }
            
        }
        else
        {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
            SetJointSettings(jointSpring);
        }
        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

        //apply the thurster force
        motor.ApplyThurster(_thursterForce);
    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive
        {
                positionSpring=jointSpring,
                maximumForce=jointMaxForce
        };
    }
}
