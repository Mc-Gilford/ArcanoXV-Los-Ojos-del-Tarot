using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScript : MonoBehaviour
{
    private Vector2 input;

    private float distancetoPlayer;
    
    [SerializeField] private Transform target;

    [SerializeField] private MouseSensitivity mouseSensitivity;

    [SerializeField] private CameraAngle cameraAngle;

    private CameraRotation cameraRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Awake()
    {
        distancetoPlayer = Vector3.Distance(transform.position, target.position);
        target = GameObject.Find("Player").transform;
        setCameraConfig();
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cameraRotation.yaw += input.x * mouseSensitivity.horizontal * Time.deltaTime;
        cameraRotation.pitch += -input.y * mouseSensitivity.vertical * Time.deltaTime;   
        cameraRotation.pitch = Mathf.Clamp(cameraRotation.pitch, cameraAngle.min, cameraAngle.max);
    }

    private void LateUpdate()
    {
        transform.eulerAngles = new Vector3(cameraRotation.pitch, cameraRotation.yaw, 0.0f);
        transform.position = target.position - transform.forward * distancetoPlayer;
    }

    public void Look(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }

    [Serializable]
    public struct MouseSensitivity
    {
        public float horizontal;
        public float vertical;
    }

    public struct CameraRotation
    {
        public float pitch;
        public float yaw;

    }

    [Serializable] 
    public struct CameraAngle
    {
        public float min;
        public float max;
    }

    private void setCameraConfig()
    {
        cameraAngle.min = -10f;
        cameraAngle.max = 40f;
        mouseSensitivity.horizontal = 20f;
        mouseSensitivity.vertical = 10f;
    }
}
