using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private Camera mainCamera;

    private readonly List<IInteractable> nearbyObjects = new();
    private IInteractable currentTarget;

    public Vector2  moveDirection       { get; private set; }
    public Vector2  mouseWorldPosition  { get; private set; }
    public bool     attackPressed       { get; private set; }
    public bool     skill1Pressed       { get; private set; }
    public bool     skill2Pressed       { get; private set; }
    public bool     dashPressed         { get; private set; }
    public bool     interactPressed     { get; private set; }
    public bool     item1Pressed        { get; private set; }
    public bool     item2Pressed        { get; private set; }
    public bool     item3Pressed        { get; private set; }
    public bool     item4Pressed        { get; private set; }

    private bool    isInputLocked = false;


    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (isInputLocked)
        {
            moveDirection   = Vector2.zero;

            attackPressed   = false;
            skill1Pressed   = false;
            skill2Pressed   = false;
            dashPressed     = false;
            interactPressed = false;
            item1Pressed    = false;
            item2Pressed    = false;
            item3Pressed    = false;
            item4Pressed    = false;
            return;
        }

        if(mainCamera == null) mainCamera = Camera.main;

        mouseWorldPosition  = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
        moveDirection       = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        attackPressed   = Input.GetMouseButton(0);
        skill1Pressed   = Input.GetMouseButtonDown(1);
        skill2Pressed   = Input.GetKeyDown(KeyCode.Q);
        dashPressed     = Input.GetKeyDown(KeyCode.Space);
        interactPressed = Input.GetKeyDown(KeyCode.F);
        item1Pressed    = Input.GetKeyDown(KeyCode.Alpha1);
        item2Pressed    = Input.GetKeyDown(KeyCode.Alpha2);
        item3Pressed    = Input.GetKeyDown(KeyCode.Alpha3);
        item4Pressed    = Input.GetKeyDown(KeyCode.Alpha4);

        IInteractable nearest = GetClosestInteractable();

        if (nearest != currentTarget)
        {
            currentTarget = nearest;

            if (currentTarget != null)
                GameEvents.OnShowInteractKey?.Invoke(((MonoBehaviour)currentTarget).transform, currentTarget.GetInteractDescription());
            else
                GameEvents.OnHideInteractKey?.Invoke();
        }

        // ��ȣ�ۿ� Ű(F)�� ������ ��, �� �� ���� ����
        if (interactPressed && nearest != null)
        {
            nearest.OnPlayerInteract(gameObject.GetComponent<Player>());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable") && collision.TryGetComponent(out IInteractable interactable))
        {
            if (!nearbyObjects.Contains(interactable))
            {
                nearbyObjects.Add(interactable);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable") && collision.TryGetComponent(out IInteractable interactable))
        {
            if (nearbyObjects.Contains(interactable))
            {
                nearbyObjects.Remove(interactable);
            }
        }
    }

    public void InputLocker(bool state)
    {
        isInputLocked = state;
    }

    public void MoveStop()
    {
        moveDirection = Vector2.zero;
    }

    private IInteractable GetClosestInteractable()
    {
        if (nearbyObjects.Count == 0) return null;

        float closestDist = float.MaxValue;
        IInteractable closestObj = null;

        foreach (var obj in nearbyObjects) //가장 가까운 오브젝트 선정
        {
            float dist = Vector2.Distance(transform.position, ((MonoBehaviour)obj).transform.position);

            if (dist < closestDist)
            {
                closestDist = dist;
                closestObj = obj;
            }
        }

        return closestObj;
    }
}
