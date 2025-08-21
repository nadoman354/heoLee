using UnityEngine;

public class DroppedConsumableFactory : MonoBehaviour
{
    [SerializeField] private GameObject prefab; // DroppedConsumable ÇÁ¸®ÆÕ
    [SerializeField] private ResourcesItemMetaCatalog metaCatalog;

    private void Awake()
    {
        if (!metaCatalog) metaCatalog = ResourcesItemMetaCatalog.LoadDefault();
    }

    public DroppedItem CreateFromId(string id, Vector3 spawnPos)
    {
        if (!metaCatalog || !metaCatalog.TryGetConsumable(id, out var meta))
        {
            Debug.LogError($"[DroppedConsumableFactory] Consumable meta not found: {id}");
            return null;
        }
        var go = PoolManager.Spawn(prefab, spawnPos, Quaternion.identity);
        var d = go.GetComponent<DroppedConsumable>();
        d.Setup(meta);
        return d;
    }
}
