
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ValidZone : MonoBehaviour
{
    public string ZoneType;
    public bool OyeSiii;
    public void PlaceObject(RectTransform draggedObject)
    {
        draggedObject.SetParent(transform);
        draggedObject.anchoredPosition = Vector2.zero;
    }
}