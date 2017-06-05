using UnityEngine;
using System.Collections;

public class CharaController : MonoBehaviour
{



    private Animator targetAnimator;
    private Transform cameraTransform;
    private Transform targetTransform;
    public float speed = 25f;
    private Vector3 rotation;
    private Vector3 direction;
    private string rotating = "Rotation";
    private string moving = "Moving";
    private string horizontal = "Horizontal";
    private string vertical = "Vertical";
    private bool isMoved = false;
    private bool isRotation = false;

    #region Members
    private CharacterController characterController;
    #endregion


    void Start()
    {
        InitMembers();
    }
    // 初始化成员变量
    public void InitMembers()
    {
        targetTransform = GameObject.Find("Player").transform;
        cameraTransform = targetTransform.FindChild("MainCamera");
        characterController = targetTransform.GetComponent<CharacterController>();
        targetAnimator = targetTransform.GetComponent<Animator>();
    }
    public void LateUpdate()
    {
        if (isMoved)
        {
            //this.target.Translate(new Vector3 (this.direction.x * Time.deltaTime * this.speed,0,  direction.y * Time.deltaTime * this.speed),Space.World);
            this.targetAnimator.SetFloat(horizontal, this.direction.x);
            this.targetAnimator.SetFloat(vertical, this.direction.z);
        }


        if (isRotation)
        {

            this.targetAnimator.SetFloat(horizontal, this.rotation.x);
            this.targetAnimator.SetFloat(vertical, this.rotation.y);
            
            //变动的距离
            //float changeDistance = Input.GetAxis("Mouse ScrollWheel");
            ////单前的距离
            //float currentDistance = originVector.magnitude;
            ////单位向量
            //Vector3 miniVector = originVector.normalized;
            ////获取新的向量
            ////originVector = miniVector * (currentDistance - changeDistance * Time.deltaTime * 100);
            ////transform.position = target.position - originVector;

            //float rotationAmount = yRot * mouseTurnedSpeed * Time.deltaTime;
            //    //最终的旋转读书
            //transform.RotateAround(target.position, Vector3.up, rotationAmount * 360);
            //    //人物也旋转 保证镜头始终对着人物背面
            //    //target.RotateAround(target.position, Vector3.up, rotationAmount * 360);

            ////纵轴
            //float rotationAmountY = xRot * mouseTurnedSpeed * Time.deltaTime;
            //Vector3 yCenter = new Vector3(-originVector.z / originVector.x, target.position.y, 1);

            //transform.RotateAround(target.position, Vector3.right, rotationAmountY * 360);
            //Vector3.right
            //lastFrameTargetRoation = target.rotation.eulerAngles;
            //originVector = new Vector3(target.position.x - transform.position.x, target.position.y - transform.position.y, target.position.z - transform.position.z);
            //transform.LookAt(target);
            //Debug.Log("  "+ yRot + "   "+ xRot);
            //m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            //m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);
            //m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);
            ////target.localRotation = Quaternion.Slerp(target.localRotation, m_CharacterTargetRot,
            ////        2 * Time.deltaTime);
            ////cameraTran.localRotation = Quaternion.Slerp(cameraTran.localRotation, m_CameraTargetRot,
            ////    2 * Time.deltaTime);
            //target.localRotation = m_CharacterTargetRot;
            //cameraTran.localRotation = m_CameraTargetRot;

        }
    }

    public void BeginMove()
    {
        this.targetAnimator.SetBool(moving, true);
        isMoved = true;
    }
    public void EndMove()
    {
        this.targetAnimator.SetBool(moving, false);
        isMoved = false;
    }
    public void UpdateDirection(Vector3 direction)
    {
        this.direction = direction;
    }
    public void BeginRotation()
    {
        this.targetAnimator.SetBool(rotating, true);
        isRotation = true;
    }
    public void EndRotation()
    {
        this.targetAnimator.SetBool(rotating, false);
        isRotation = false;
    }
    public void UpdateRation(Vector3 rotation)
    {
        this.rotation = rotation;
    }
    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, -90, 90);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}
