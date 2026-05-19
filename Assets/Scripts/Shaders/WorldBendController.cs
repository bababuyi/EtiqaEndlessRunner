using UnityEngine;

public class WorldBendController : MonoBehaviour
{
    [SerializeField] private float curvature = 0.001f;

    void Awake()
    {
        Shader.SetGlobalFloat("_Curvature", curvature);
    }
}