using UnityEngine;
using UnityEngine.UI;

public class ShopPreviewRotator : MonoBehaviour
{
    [Header("Preview Target")]
    [SerializeField] private GameObject prefabInstance;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 45f;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    private void Update()
    {
        if (prefabInstance == null) return;
        prefabInstance.transform.Rotate(rotationAxis, rotationSpeed * Time.unscaledDeltaTime);
    }

    public void SetTarget(GameObject instance)
    {
        if (prefabInstance != null) prefabInstance.SetActive(false);
        prefabInstance = instance;
        if (prefabInstance != null) prefabInstance.SetActive(true);
    }
}