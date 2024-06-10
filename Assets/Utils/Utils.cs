using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Caiiru.Utils
{
    public static class Utils
    {

        public static TextMeshProUGUI CreateWorldText(string text, Transform parent = null,
            Vector3 localPosition = default(Vector3), int fontSize = 1, Color? color = null,
            TextContainerAnchors textAnchor =  TextContainerAnchors.Middle , TMPro.TextAlignmentOptions textAlignment = TMPro.TextAlignmentOptions.Center,
            int sortOrdering = 500)
        {
            if (color == null)
                color = Color.white;
            
            return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment,
                sortOrdering);
        }

        public static TextMeshProUGUI CreateWorldText(Transform parent, string text, Vector3 localPosition,
            int fontSize, Color color, TextContainerAnchors textAnchor,  TMPro.TextAlignmentOptions textAlignment, int sortOrder)
        {
            GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            TextMeshProUGUI textMesh = gameObject.GetComponent<TextMeshProUGUI>(); 
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortOrder;
            return textMesh;
        }
        public static Vector3 GetMouseWorldPosition()
        {
            Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
        }

        public static Vector3 GetMouseWorldPositionWithZ()
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
        {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }
    }
}