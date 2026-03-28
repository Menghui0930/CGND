using UnityEngine;

public class DestroyOverTime : MonoBehaviour
{
    [SerializeField] private float lifetime;
    void Start()
    {
        Destroy(gameObject,lifetime);
    }
}
