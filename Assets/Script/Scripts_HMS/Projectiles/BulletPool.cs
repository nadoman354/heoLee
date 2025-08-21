using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    private static BulletPool instance;
    public static BulletPool Instance=>instance;

    [SerializeField] Bullet prefab;
    [SerializeField] int prewarmCount = 256;
    [SerializeField] int expandChunk = 64;

    // LIFO
    readonly Stack<Bullet> pool = new Stack<Bullet>(1024);

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Prewarm();
    }
    void Prewarm()
    {
        for (int i = 0; i < prewarmCount; i++)
        {
            var b = Instantiate(prefab, transform);
            b.SetPool(this);
            b.gameObject.SetActive(false);
            pool.Push(b);
        }
    }

    void Expand(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var b = Instantiate(prefab, transform);
            b.SetPool(this);
            b.gameObject.SetActive(false);
            pool.Push(b);
        }
    }

    public Bullet Get()
    {
        if (pool.Count == 0) Expand(expandChunk);
        var b = pool.Pop();
        b.gameObject.SetActive(true);
        return b;
    }

    public void Return(Bullet b)
    {
        // 상태 리셋은 여기에서 (Trail, Animator 등)
        b.gameObject.SetActive(false);
        pool.Push(b);
    }
}
