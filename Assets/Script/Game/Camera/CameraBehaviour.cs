using Assets.Script.Tools;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public bool isMapCamera;

    [Header("Stats")]
    public float ymax, ymin;

    [SerializeField] private float speedOfDeplacement = 1;
    [SerializeField] private float speedOfZoom = 10;
    [SerializeField] private float IncrementSpeed = 2;


    [HideInInspector] public float fov;
    [HideInInspector] public Rigidbody _rb;
    private float _lastY;
    private Camera _camera;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _camera = GetComponent<Camera>();
        fov = _camera.fieldOfView;
        if(isMapCamera)
        {
            ymin = ymax;
            gameObject.transform.position = new Vector3(transform.position.x,ymax,transform.position.z);
        }
    }

    public void Zoom(float y, bool acceleratieIsActive)
    {
        if (transform.position.y >= ymin && y >= 0 ||
            transform.position.y <= ymax && y <= 0)
        {
            Vector3 newPosition = new Vector3(y / speedOfZoom * transform.forward.x,
                y / speedOfZoom * transform.forward.y,
                y / speedOfZoom * transform.forward.z) * (Time.deltaTime * speedOfDeplacement);

            if (acceleratieIsActive) { newPosition *= IncrementSpeed; }

            _rb.MovePosition(transform.position + newPosition);
        }
    }
    private void VerifyIfOutOfBorder(float[] maxmin)
    {
        if (transform.position.x >= maxmin[0] || 
            transform.position.x <= maxmin[1] || 
            transform.position.z >= maxmin[2] || 
            transform.position.z <= maxmin[3])
        {
            if (_camera.fieldOfView < fov + 10) { _camera.fieldOfView += 0.1f; }

        }
        else { _camera.fieldOfView = fov; }
    }
    // maxmin : [0] MaxX, [1] MinX, [2] MaxZ, [3] MinZ
    public void MoveCamera(float y, float x, bool acceleratieIsActive, float[] maxmin)
    {
        Vector3 newPosition = new( y * transform.up.x + x * transform.right.x
            , 0
            , y * transform.up.z + x * transform.right.z);

        newPosition *= 10;

        if (acceleratieIsActive) { newPosition *= IncrementSpeed; }

        _rb.velocity = newPosition;

        Vector3 distanceGround = RayCast.RaycastForGround(gameObject, transform.position);
        ymin += distanceGround.y - _lastY;
        ymax += distanceGround.y - _lastY;

        transform.position = new Vector3(
            transform.position.x > maxmin[0] ? maxmin[0] : transform.position.x < maxmin[1] ? maxmin[1] : transform.position.x
            , !isMapCamera ? transform.position.y + distanceGround.y - _lastY : transform.position.y
            , transform.position.z > maxmin[2] ? maxmin[2] : transform.position.z < maxmin[3] ? maxmin[3] : transform.position.z);

        VerifyIfOutOfBorder(maxmin);

        _lastY = distanceGround.y;
    }

    public void RotateCameraY(float x)
    {
        Quaternion rotation = transform.rotation;
        rotation.eulerAngles += new Vector3(0, x / 5, 0);
        _rb.MoveRotation(rotation);
    }
}
