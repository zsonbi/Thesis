using UnityEngine;

public class ThreadSafeMonoBehaviour : MonoBehaviour
{
    public bool Destroyed { get; private set; }

    protected void OnDestroy()
    {
        this.Destroyed = true;
        StopAllCoroutines();
    }
}