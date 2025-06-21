using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GemboardCameraFollow : MonoBehaviour
{
    [SerializeField] private Gemboard gemboard;
    [SerializeField] private float padding = 1f; // để camera không cắt sát mép

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (gemboard == null)
        {
            gemboard = FindAnyObjectByType<Gemboard>();
        }
        CenterCameraOnBoard();
    }

    public void CenterCameraOnBoard()
    {
        if (gemboard == null) return;

        // Vị trí giữa bảng: do đã trừ spacingX, spacingY nên trung tâm nằm tại (0,0)
        Vector3 boardCenter = new Vector3(0f, 0f, -10f); // camera Z = -10
        transform.position = boardCenter;

        float boardWidth = gemboard.width;
        float boardHeight = gemboard.height;

        float screenRatio = (float)Screen.width / Screen.height;
        float targetRatio = boardWidth / boardHeight;

        if (screenRatio >= targetRatio)
        {
            cam.orthographicSize = boardHeight / 2f + padding;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            cam.orthographicSize = (boardHeight / 2f) * differenceInSize + padding;
        }
    }
}