using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class SwipeComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public delegate void PointerHandler(SwipeComponent obj);
        public event PointerHandler OnRightSwipeEvent;
        public event PointerHandler OnLeftSwipeEvent;
        public event PointerHandler OnUpSwipeEvent;
        public event PointerHandler OnDownSwipeEvent;
        public event PointerHandler OnClickEvent;

        private Vector3 FirstPos;
        private Vector3 LastPos;
        private float DragDistance;  //minimum distance for a SWIPE

        void Start()
        {
            DragDistance = Screen.height * 10 / 100; // 10% height of the screen
        }

        void OnMouseDown()
        {
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            FirstPos = eventData.position;
            LastPos = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            LastPos = eventData.position;

            if (Mathf.Abs(LastPos.x - FirstPos.x) > DragDistance || Mathf.Abs(LastPos.y - FirstPos.y) > DragDistance)
            {
                if (Mathf.Abs(LastPos.x - FirstPos.x) > Mathf.Abs(LastPos.y - FirstPos.y))
                {
                    if ((LastPos.x > FirstPos.x) && OnRightSwipeEvent != null)
                        OnRightSwipeEvent(this);
                    else if ((LastPos.x < FirstPos.x) && OnLeftSwipeEvent != null)
                        OnLeftSwipeEvent(this);
                }
                else
                {
                    if (LastPos.y > FirstPos.y && OnUpSwipeEvent != null)
                        OnUpSwipeEvent(this);
                    else if (LastPos.y < FirstPos.y && OnDownSwipeEvent != null)
                        OnDownSwipeEvent(this);
                }
            }
            else if (OnClickEvent != null)
                OnClickEvent(this);
        }
    }
}