using UnityEditor.EditorTools;
using UnityEngine;

public class DroppedWeaponFactory : MonoBehaviour
{
    [SerializeField] private GameObject prefab; // DroppedWeapon ������
    [SerializeField] private ResourcesItemMetaCatalog metaCatalog;

    private void Awake()
    {
        if (!metaCatalog) metaCatalog = ResourcesItemMetaCatalog.LoadDefault();
    }

    public DroppedItem CreateFromId(string id, Vector3 spawnPos)
    {
        if (!metaCatalog || !metaCatalog.TryGetWeapon(id, out var meta))
        {
            Debug.LogError($"[DroppedWeaponFactory] IWeaponLogic meta not found: {id}");
            return null;
        }
        var go = PoolManager.Spawn(prefab, spawnPos, Quaternion.identity);
        var d = go.GetComponent<DroppedWeapon>();
        d.Setup(meta);
        return d;
    }

    public DroppedItem CreateFromWeapon(IWeaponLogic w, Vector3 spawnPos)
    {
        var go = PoolManager.Spawn(prefab, spawnPos, Quaternion.identity);
        var d = go.GetComponent<DroppedWeapon>();
        d.SetupOriginal(w);
        return d;
    }
}
