using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace _3DashUtils.UnityScripts
{
    public class IconScript : MonoBehaviour
    {
        public float rotateSpeed = 50f;
        private Transform TargetObject;
        public float lookOffset = 98f;
        public float lerpSpeed = 1f;
        public float snapRange = 3f;
        public bool hasFocus = false;
        private Quaternion targetRotation;

        // Start is called before the first frame update
        void Start()
        {
            if (transform.childCount < 1)
            {
                Debug.LogError("No child object found to manipulate.");
                this.enabled = false;
                return;
            }
            if (transform.childCount > 1)
            {
                Debug.LogWarning("Multiple children found. Only the first one will be affected.");
            }
            TargetObject = transform.GetChild(0);
            targetRotation = TargetObject.localRotation;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 worldMouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, lookOffset));

            Vector2 flatPos = new Vector2(transform.position.x, transform.position.y);
            Vector2 flatMouse = new Vector2(worldMouse.x, worldMouse.y);
            if ((flatPos - flatMouse).sqrMagnitude < snapRange)
            {
                targetRotation = Quaternion.LookRotation(transform.position - worldMouse);
            }
            else
            {
                targetRotation.ToAngleAxis(out var angle, out var axis);
                var newAngle = Mathf.Repeat(angle + (rotateSpeed * Time.deltaTime), 360);
                targetRotation = Quaternion.AngleAxis(newAngle, Vector3.up);
            }
            transform.rotation = Quaternion.Slerp(this.transform.localRotation, targetRotation, Time.deltaTime * lerpSpeed);
        }
    }
}