using System.Collections.Generic;
using UnityEngine;

public class SearchInteractableTarget : MonoBehaviour
{
    private readonly List<IInteractable> nearbyObjects = new();
    private IInteractable currentTarget;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interactable") && other.TryGetComponent(out IInteractable interactable))
        {
            if (!nearbyObjects.Contains(interactable))
            {
                nearbyObjects.Add(interactable);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Interactable") && other.TryGetComponent(out IInteractable interactable))
        {
            if (nearbyObjects.Contains(interactable))
            {
                nearbyObjects.Remove(interactable);
            }
        }
    }

    private void Update()
    {
        IInteractable nearest = GetClosestInteractable();

        if (nearest != currentTarget)
        {
            currentTarget = nearest;

            if (currentTarget != null)
                GameEvents.OnShowInteractKey?.Invoke(((MonoBehaviour)currentTarget).transform, currentTarget.GetInteractDescription());
            else
                GameEvents.OnHideInteractKey?.Invoke();
        }
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