using UnityEngine;
using System.Collections;

namespace UnityStandardAssets.Utility
{
    public class LookForward : MonoBehaviour
    {

        // The target we are following
        [SerializeField]
        private Transform target;
        // The distance in the x-z plane to the target

        [SerializeField]
        private Transform lookAtTarget;
        [SerializeField]
        private Vector3 lookAtOffset;
        //初始向量差 用来保持距离不变
        private Vector3 originVector;
        //记录上一frame的旋转叫
        private Vector3 lastFrameTargetRoation;
        //鼠标旋转视野的速度
        [SerializeField]
        private float mouseTurnedSpeed = 0.3f;
        //鼠标右键控制镜头旋转的代码
        private bool rightButtonDonwed;

        // Use this for initialization
        void Start()
        {
            //获取当前的distance和height
            originVector = new Vector3(target.position.x - transform.position.x, target.position.y - transform.position.y, target.position.z - transform.position.z);
            rightButtonDonwed = false;
            lastFrameTargetRoation = target.rotation.eulerAngles;
        }

        // Update is called once per frame
        void Update()
        {

            //变动的距离
            float changeDistance = Input.GetAxis("Mouse ScrollWheel");
            //单前的距离
            float currentDistance = originVector.magnitude;
            //单位向量
            Vector3 miniVector = originVector.normalized;
            //获取新的向量
            originVector = miniVector * (currentDistance - changeDistance * Time.deltaTime * 100);


            //记录鼠标右键是否按下的状态
            if (Input.GetMouseButton(1) && Input.GetMouseButtonDown(1))
            {
                rightButtonDonwed = true;
            }
            if (Input.GetMouseButtonUp(1))
            {
                rightButtonDonwed = false;
            }
            transform.position = target.position - originVector;
            print(rightButtonDonwed);
            if (rightButtonDonwed)
            {
                //获取鼠标旋转的度数 横轴
                float rotationAmount = Input.GetAxis("Mouse X") * mouseTurnedSpeed * Time.deltaTime;
                //最终的旋转读书
                transform.RotateAround(target.position, Vector3.up, rotationAmount * 360);
                //人物也旋转 保证镜头始终对着人物背面
                target.RotateAround(target.position, Vector3.up, rotationAmount * 360);

                //纵轴
                float rotationAmountY = Input.GetAxis("Mouse Y") * mouseTurnedSpeed * Time.deltaTime;
                Vector3 yCenter = new Vector3(-originVector.z / originVector.x, target.position.y, 1);
              
                transform.RotateAround(target.position, yCenter, rotationAmountY * 360);
            }
            else
            {
                transform.RotateAround(target.position, Vector3.up, target.rotation.eulerAngles.y - lastFrameTargetRoation.y);

            }
            lastFrameTargetRoation = target.rotation.eulerAngles;
            originVector = new Vector3(target.position.x - transform.position.x, target.position.y - transform.position.y, target.position.z - transform.position.z);

        }
    }
}