using UnityEngine;

namespace RTSBuildingSystem
{
    public class CameraMovement : MonoBehaviour
    {
        public float speed = 35;
        public float rotationSpeed = 80;
        public float zoomSpeed = 5;
        public float smoothness = .125f;

        [Header("Camera limits")]
        public Vector2 minMaxX = new Vector2(-50, 50);
        public Vector2 minMaxZ = new Vector2(-50, 50);

        private Camera cam;
        private float currentZoom, currentRotation;
        private Transform holder;
        private Vector3 target = new Vector3(0, 25, 0);

        void Start()
        {
            cam = GetComponent<Camera>();
            currentZoom = cam.fieldOfView;
            holder = transform.parent;
        }

        private void Update()
        {
            // Zoom in/out
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f && currentZoom > 30) currentZoom -= zoomSpeed;
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f && currentZoom < 60) currentZoom += zoomSpeed;

            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, currentZoom, smoothness);

            // Rotate
            if (Input.GetAxisRaw("RotateCam") != 0) currentRotation += rotationSpeed * Input.GetAxisRaw("RotateCam") * Time.deltaTime;

            holder.rotation = Quaternion.Lerp(holder.rotation, Quaternion.Euler(0, currentRotation, 0), smoothness);

            // Movement relative to camera
            Vector3 camForward = transform.forward, camRight = transform.right;
            camForward.y = camRight.y = 0;
            Vector3 moveDir = Input.GetAxisRaw("Vertical") * camForward + Input.GetAxisRaw("Horizontal") * camRight;

            // Limit movement
            target.x = Mathf.Clamp(target.x, minMaxX.x, minMaxX.y);
            target.z = Mathf.Clamp(target.z, minMaxZ.x, minMaxZ.y);

            target += moveDir * speed * Time.deltaTime;
            holder.position = Vector3.Lerp(holder.position, target, smoothness);
        }
    }
}