using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [Header("Camera Movement Settings")]
    [SerializeField] private float cameraSpeed = 0.1f;
    [SerializeField] private Vector3 offset;

    [Header("Bounding Box Settings")]
    [SerializeField] private Vector2 boundingBoxSize = new Vector2(2f, 2f); // Kích thước vùng an toàn

    private Transform player;

    private void Start()
    {
        if (PlayerMovement.Instance != null)
        {
            player = PlayerMovement.Instance.transform;
        }
        else
        {
            Debug.LogError("PlayerMovement.Instance is not initialized. Please ensure PlayerMovement script is attached to the player.");
        }
    }

    private void Update()
    {
        if (player != null)
        {
            FollowPlayerWithBoundingBox();
        }
    }

    private void FollowPlayerWithBoundingBox()
    {
        // Tính vị trí của vùng an toàn
        Vector3 targetPosition = player.position + offset;

        Vector3 cameraPosition = transform.position;

        // Kiểm tra nếu nhân vật vượt qua vùng an toàn (bounding box)
        if (Mathf.Abs(targetPosition.x - cameraPosition.x) > boundingBoxSize.x / 2)
        {
            cameraPosition.x = Mathf.Lerp(cameraPosition.x, targetPosition.x, cameraSpeed);
        }

        if (Mathf.Abs(targetPosition.y - cameraPosition.y) > boundingBoxSize.y / 2)
        {
            cameraPosition.y = Mathf.Lerp(cameraPosition.y, targetPosition.y, cameraSpeed);
        }

        // Gán lại vị trí camera
        transform.position = new Vector3(cameraPosition.x, cameraPosition.y, cameraPosition.z);
    }

    private void OnDrawGizmosSelected()
    {
        // Vẽ vùng an toàn trong Scene View để tiện debug
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(player.position + offset, new Vector3(boundingBoxSize.x, boundingBoxSize.y, 0));
        }
    }
}
