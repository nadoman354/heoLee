using UnityEngine;

public class DroppedCurrencyFactory : MonoBehaviour
{
    [SerializeField] private GameObject prefab; // DroppedCurrency ÇÁ¸®ÆÕ
    [SerializeField] private ResourcesItemMetaCatalog metaCatalog;

    private void Awake()
    {
        if (!metaCatalog) metaCatalog = ResourcesItemMetaCatalog.LoadDefault();
    }

    public DroppedItem CreateFromId(string id, Vector3 spawnPos)
    {
        if (!metaCatalog || !metaCatalog.TryGetCurrency(id, out var meta))
        {
            Debug.LogError($"[DroppedCurrencyFactory] Currency meta not found: {id}");
            return null;
        }
        var go = PoolManager.Spawn(prefab, spawnPos, Quaternion.identity);
        var d = go.GetComponent<DroppedCurrency>();
        d.Setup(meta);
        return d;
    }
}
