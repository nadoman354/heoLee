using System.Collections.Generic;
using System;
using UnityEngine;
public interface IAimProvider { Vector2 AimDir { get; } }
public interface IAnchors { Vector2 MuzzlePos { get; } Quaternion MuzzleRot { get; } }
public interface ITimeSource { float Now { get; } }
public interface ISpawner { GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent); }
public interface ICollisionQuery { RaycastHit2D[] OverlapBox(Vector2 center, Vector2 size, float angle, int mask); }

public sealed class ViewAnchors : IAnchors
{
    readonly Transform _muzzle, _owner;
    public ViewAnchors(Transform muzzle, Transform owner) { _muzzle = muzzle; _owner = owner; }
    public Vector2 MuzzlePos => (_muzzle ? _muzzle : _owner).position;
    public Quaternion MuzzleRot => (_muzzle ? _muzzle : _owner).rotation;
}

// 마우스 조준
public sealed class MouseAimProvider : IAimProvider
{
    readonly Camera _cam; readonly Func<Vector2> _originGetter;
    public MouseAimProvider(Camera cam, Func<Vector2> originGetter) { _cam = cam; _originGetter = originGetter; }
    public Vector2 AimDir
    {
        get
        {
            var origin = _originGetter();
            var mouse = (Vector2)_cam.ScreenToWorldPoint(Input.mousePosition);
            var dir = mouse - origin; return dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.right;
        }
    }
}

// 스틱 조준(신 InputSystem에서 읽어오는 람다를 주입하는 형태)
public sealed class StickAimProvider : IAimProvider
{
    readonly Func<Vector2> _stickGetter;
    public StickAimProvider(Func<Vector2> stickGetter) { _stickGetter = stickGetter; }
    public Vector2 AimDir
    {
        get { var v = _stickGetter(); return v.sqrMagnitude > 0.0001f ? v.normalized : Vector2.zero; }
    }
}

// 둘 중 유효한 걸 선택
public sealed class CombinedAimProvider : IAimProvider
{
    readonly IAimProvider _stick, _mouse; readonly float _deadZone2;
    public CombinedAimProvider(IAimProvider stick, IAimProvider mouse, float deadZone = 0.2f) { _stick = stick; _mouse = mouse; _deadZone2 = deadZone * deadZone; }
    public Vector2 AimDir => _stick.AimDir.sqrMagnitude > _deadZone2 ? _stick.AimDir : _mouse.AimDir;
}

public sealed class UnityTimeSource : ITimeSource { public float Now => Time.time; }

public sealed class DefaultSpawner : ISpawner
{
    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent = null)
        => UnityEngine.Object.Instantiate(prefab, pos, rot, parent);
}

public sealed class Physics2DQuery : ICollisionQuery
{
    public RaycastHit2D[] OverlapBox(Vector2 center, Vector2 size, float angle, int mask)
        => Physics2D.BoxCastAll(center, size, angle, Vector2.right, 0f, mask); // 필요시 OverlapBoxAll로 변경
}

public interface ICapabilities
{
    bool TryGet<T>(out T service) where T : class;
    T Require<T>() where T : class; // 없으면 명확한 예외
}

public sealed class Capabilities : ICapabilities
{
    private readonly Dictionary<Type, object> _map = new();
    public Capabilities Add<T>(T svc) where T : class { _map[typeof(T)] = svc; return this; }
    public bool TryGet<T>(out T svc) where T : class
    {
        if(_map.TryGetValue(typeof(T), out var o))
        {
            if(o != null && o is T)
            {
                svc = (o as T);
                return true;
            }
        }
        svc = null;
        return false;
    }
    public T Require<T>() where T : class
        => TryGet<T>(out var svc) ? svc : throw new InvalidOperationException($"Missing capability: {typeof(T).Name}");
}
