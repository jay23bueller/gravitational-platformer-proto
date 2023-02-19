using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour, IPullable
{
    private Transform _planetTransform;
    private float _currentGravitySpeed;
    [SerializeField]
    private float _defaultGravitySpeed;
    [SerializeField]
    private float _movementSpeed;
    [SerializeField]
    private float _dragMultipler;
    [SerializeField]
    private float _jumpForce = 15f;
    [SerializeField]
    private float _groundTraceDistance = .7f;
    private GravityVolume.VolumeShape _volumeShape;
    private Rigidbody _rigidbody;
    private Vector3 _inputDirection = Vector3.zero;

    private bool _buttonHeld;
    private float _directionality = 0f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _currentGravitySpeed = _defaultGravitySpeed;
    }

    private void Update()
    {
        if(!_buttonHeld && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)))
        {
            Debug.Log("Started!");
            
            Debug.Log(_inputDirection);
            if (Vector3.Project(transform.up, Vector3.up).y < 0.24f)
                _directionality = -1f;
            else
                _directionality = 1f;
            _buttonHeld = true;
        }

        if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            Debug.Log("Ended!");
            _directionality = 0f;
            _buttonHeld = false;
        }

        _inputDirection = transform.right * _directionality * Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && Physics.Raycast(new Ray(transform.position, -transform.up),_groundTraceDistance, LayerMask.GetMask("Planet")))
        {
            _rigidbody.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        Vector3 gravityDirection = Vector3.down;
        if(_planetTransform != null)
        {
            switch(_volumeShape)
            {
                case GravityVolume.VolumeShape.Sphere:
                    gravityDirection = (_planetTransform.position - _rigidbody.position).normalized;
                    break;
                case GravityVolume.VolumeShape.Cube:
                    Vector3 dir = (_planetTransform.position - _rigidbody.position).normalized;
                    float upDot = Vector3.Dot(dir, _planetTransform.up);
                    float downDot = Vector3.Dot(dir, -_planetTransform.up);
                    float rightDot =  Vector3.Dot(dir, _planetTransform.right);
                    float leftDot =  Vector3.Dot(dir, -_planetTransform.right);
                    //Debug.Log($"Up dot:{upDot}");
                    //Debug.Log($"downDot:{downDot}");
                    //Debug.Log($"rightDot:{rightDot}");
                    //Debug.Log($"leftDot:{leftDot}");
                    Vector3 projectionOnRight = Vector3.Project(dir, _planetTransform.right);

                    Vector3 projectionOnUp = Vector3.Project(dir, _planetTransform.up);
                    if (projectionOnUp.magnitude > projectionOnRight.magnitude)
                        gravityDirection = projectionOnUp;
                    else
                        gravityDirection = projectionOnRight;
                    Debug.Log($"projectionOnRight: {projectionOnRight}");
                    Debug.Log($"projectionOnUp: {projectionOnUp}");

                    //if (upDot > .6f)
                    //    gravityDirection = _planetTransform.up;
                    //else if (downDot > .6f)
                    //    gravityDirection = -_planetTransform.up;

                    //if (rightDot > .6f)
                    //    gravityDirection += _planetTransform.right;
                    //else if (leftDot > .6f)
                    //    gravityDirection += -_planetTransform.right;

                    break;
            }
           
            Quaternion upToGravityDirecitonRotation = Quaternion.LookRotation(transform.forward, -gravityDirection);
            _rigidbody.rotation = Quaternion.RotateTowards(_rigidbody.rotation, upToGravityDirecitonRotation, 5f);
        }
        _rigidbody.AddForce(_inputDirection * _movementSpeed, ForceMode.Acceleration);
        
        _rigidbody.AddForce(gravityDirection * _currentGravitySpeed, ForceMode.Acceleration);

        _rigidbody.drag = _rigidbody.velocity.magnitude * _dragMultipler / _movementSpeed;
        
    }


    public void PullTowardsTransform(Transform t, float gravitySpeed, GravityVolume.VolumeShape volumeShape)
    {
        _planetTransform = t;
        _currentGravitySpeed = gravitySpeed;
        _volumeShape = volumeShape;
    }
}
