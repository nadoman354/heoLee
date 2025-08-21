using UnityEngine;

public class BulletTestCode : MonoBehaviour
{
    [SerializeField]
    SO_BulletConfig[] config;
    [SerializeField]
    BulletPool pool;
    int cnt = 0;
    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            Bullet bullet = pool.Get();
            bullet.Init(config[cnt], transform.position, Vector2.zero);
            cnt++;
            cnt %=config.Length;
        }
    }
}
