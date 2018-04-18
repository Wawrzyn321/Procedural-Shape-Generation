using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float zommingSpeed = 0.2f;
    public float zommingDelta = 2f;
    public bool allowNegativeSize;

    private float currentZoom;

    public float panningSpeed = 0.5f;
    private Vector3 panPosition;
    private bool isPanning;


    void Awake ()
    {
        currentZoom = Camera.main.orthographicSize;
    }
	
	void Update ()
    {
        HandleZooming();
        HandlePanning();
    }

    private void HandleZooming()
    {
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zommingDelta;
        if (!allowNegativeSize && currentZoom < 0) currentZoom = 0;
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, currentZoom, zommingSpeed);
    }

    private void HandlePanning()
    {
        if (Input.GetMouseButtonDown(2))
        {
            panPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isPanning = true;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            isPanning = false;
        }
        if (isPanning)
        {
            Vector3 v = panPosition - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = Vector3.Lerp(transform.position,
                transform.position += new Vector3(v.x, v.y, 0),
                panningSpeed);
        }
    }
}
