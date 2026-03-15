using UnityEngine;

public class CameraMouseLook : MonoBehaviour
{
    [Header("Paramètres de rotation")]
    public float sensitivity = 100f;
    public float clampAngle = 80f;

    [Header("Paramètres de déplacement")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 2f;

    [Header("Hauteur de la caméra")]
    public float fixedHeight = 2f; // Hauteur figée (modifiable dans l’inspecteur)

    [Header("Mire personnalisée")]
    public Texture2D crosshairTexture;
    public float crosshairSize = 32f;

    private float rotX = 0f;
    private float rotY = 0f;
    private bool aiming = false;

    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotX = rot.x;
        rotY = rot.y;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Applique la hauteur fixe au départ
        Vector3 pos = transform.position;
        pos.y = fixedHeight;
        transform.position = pos;
    }

    void Update()
    {
        // Gestion clic droit
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            aiming = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            aiming = false;
        }

        // Rotation caméra
        if (aiming)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            rotY += mouseX * sensitivity * Time.deltaTime;
            rotX -= mouseY * sensitivity * Time.deltaTime;

            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

            Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0f);
            transform.rotation = localRotation;
        }

        // Déplacement sur le plan X/Z
        float moveForward = Input.GetAxis("Vertical");   // W/S ou Z/S
        float moveRight = Input.GetAxis("Horizontal");   // A/D ou Q/D

        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);

        Vector3 move = (transform.forward * moveForward + transform.right * moveRight);
        move.y = 0f; // empêche tout déplacement vertical
        transform.position += move * speed * Time.deltaTime;

        // Forcer la hauteur Y à fixedHeight
        Vector3 posFixed = transform.position;
        posFixed.y = fixedHeight;
        transform.position = posFixed;
    }

    void OnGUI()
    {
        if (aiming && crosshairTexture != null)
        {
            float xMin = (Screen.width - crosshairSize) / 2;
            float yMin = (Screen.height - crosshairSize) / 2;

            GUI.DrawTexture(new Rect(xMin, yMin, crosshairSize, crosshairSize), crosshairTexture);
        }
    }
}
