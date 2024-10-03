using System;
using System.Collections;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    //스피드 조정 변수
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float crouchSpeed;
    private float applySpeed;

    [SerializeField]
    private float jumpForce;

    //상태 변수
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;

    //앉았을 때 얼마나 앉을지 결정하는 변수
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;
    private float applyCrouchY;
    //땅 착지 여부
    private CapsuleCollider capsuleCollider;


    //카메라 민감도
    [SerializeField]
    private float lookSensitivity;

    //카메라 한계
    [SerializeField]
    private float CameraRotationLimit;
    private float currentCameraRotationX = 0.0f;

    //필요한 컴포넌트
    [SerializeField]
    private Camera theCamera;
    private Rigidbody myRigid;

    private GunController theGunController;
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        applySpeed = walkSpeed;
        theGunController = FindObjectOfType<GunController>();


        //초기화
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
    }


    void Update()
    {
        IsGround();
        TryJump();
        TryRun();
        TryCrouch();
        Move();
        CameraRotation();
        CharacterRotation();
    }

    //앉기 시도
    private void TryCrouch()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    //앉기 실행
    private void Crouch()
{
    if (isCrouch)
    {
        // 이미 앉아있는 경우 일어남
        applySpeed = walkSpeed;
        applyCrouchPosY = originPosY;
        StartCoroutine(CrouchCoroutine(originPosY));
    }
    else
    {
        // 앉는 경우
        applySpeed = crouchSpeed;
        applyCrouchPosY = crouchPosY;
        StartCoroutine(CrouchCoroutine(crouchPosY));
    }

    isCrouch = !isCrouch; // 상태 반전
}

//앉은 상태 카메라 설정 보간법
IEnumerator CrouchCoroutine(float targetPosY)
{
    float _posY = theCamera.transform.localPosition.y;

    while (!Mathf.Approximately(_posY, targetPosY))
    {
        _posY = Mathf.Lerp(_posY, targetPosY, 0.3f);
        theCamera.transform.localPosition = new Vector3(0, _posY, 0);
        yield return null;
    }

    // 최종 위치 설정
    theCamera.transform.localPosition = new Vector3(0, targetPosY, 0);
}

    //땅인지 확인(점프용)
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down , capsuleCollider.bounds.extents.y + 0.1f);
    }
    private void TryJump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround == true)
        {
            Jump();
        }
    }

    private void Jump()
    {
        //앉은 상태에서 점프 시 앉은 상태 해제
        if(isCrouch){
            Crouch();
        }
        myRigid.velocity = transform.up * jumpForce;
    }

    private void TryRun()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            Running();
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancel();
        }
    }

    private void Running()
    {
        // if(isCrouch)
        //     Crouch();

        theGunController.CancelFineSight();

        
        isRun = true;
        applySpeed = runSpeed;
    }

    private void RunningCancel()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }

    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f , _yRotation , 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }
    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX , -CameraRotationLimit, CameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX , 0,0);
    }

}
