using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MinimapClickHandler : MonoBehaviour, IPointerClickHandler
{
    public Camera miniMapCam;
    public Minimap minimap;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!FindObjectOfType<DoorPlacer>().FirstDoorPlaced)
        {
            return;
        }

        Vector2 cursor = new Vector2(0, 0);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RawImage>().rectTransform,
            eventData.pressPosition, eventData.pressEventCamera, out cursor))
        {

            Texture texture = GetComponent<RawImage>().texture;
            Rect rect = GetComponent<RawImage>().rectTransform.rect;

            float coordX = Mathf.Clamp(0, (((cursor.x - rect.x) * texture.width) / rect.width), texture.width);
            float coordY = Mathf.Clamp(0, (((cursor.y - rect.y) * texture.height) / rect.height), texture.height);

            float calX = coordX / texture.width;
            float calY = coordY / texture.height;

       
            cursor = new Vector2(calX, calY);
            
            CastRayToWorld(cursor);
        }
        
        
    }
    
    private void CastRayToWorld(Vector2 vec)
    {
        Ray MapRay = miniMapCam.ScreenPointToRay(new Vector2(vec.x * miniMapCam.pixelWidth,
            vec.y * miniMapCam.pixelHeight));

        RaycastHit miniMapHit;

        if (Physics.Raycast(MapRay, out miniMapHit, Mathf.Infinity))
        {
            Debug.Log("Hit tag: " + miniMapHit.collider.tag);
            if (miniMapHit.collider.tag == "Outside")
            {
                WarningFlash.Instance.FlashWarning("OUT OF BOUNDS", 50);
                return;
            }
            if (miniMapHit.collider.tag == "Haze")
            {
                WarningFlash.Instance.FlashWarning("ERROR: HAZE", 50);
                return;
            }
            if (miniMapHit.collider.tag == "Wall")
            {
                WarningFlash.Instance.FlashWarning("TOO CLOSE TO WALLS", 50);
                return;
            }
            if (miniMapHit.collider.tag == "Door")
            {
                WarningFlash.Instance.FlashWarning("TOO CLOSE TO DOOR", 50);
                return;
            }
            if (miniMapHit.collider.tag == "Player" || miniMapHit.collider.tag == "NPC")
            {
                WarningFlash.Instance.FlashWarning("TOO CLOSE TO CHARACTER", 50);
                return;
            }
            minimap.MinimapClick(miniMapHit.point);
        }
        
    }
}