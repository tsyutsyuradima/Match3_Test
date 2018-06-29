using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [ExecuteInEditMode]
    public class ColliderFitImage : MonoBehaviour
    {
        public BoxCollider2D MyCollider;
        public Image MyImage;

        void Awake()
        {
            //runInEditMode = true;
        }

        void Update()
        {
            MyCollider.offset = new Vector2(0, 0);
            MyCollider.size = new Vector3(MyImage.rectTransform.rect.width,
                                          MyImage.rectTransform.rect.height, 1);
        }
    }
}