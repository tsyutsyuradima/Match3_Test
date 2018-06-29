using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Game
{
    public class MatchElement : SwipeComponent
    {
        public event PointerHandler OnHideElement;
        public Text TEXT;
        public MatchBase match = new MatchBase();
        public Image Background;

        public void Initialize(int x, int y, Color color)
        {
            match.X = x;
            match.Y = y;
            match.MyColor = color;
            Background.color = color;
            transform.DOScale(1f, 0.1f).SetEase(Ease.Linear);
        }

        public void HideElement()
        {
            transform.DOScale(0f, 0.1f).SetEase(Ease.InOutBack).OnComplete(() =>
            {
                if (OnHideElement != null)
                    OnHideElement(this);
            });
        }

        public void MoveTo(Transform newParent)
        {
            transform.DOMove(newParent.position, 0.1f).OnComplete(() =>
            {
                transform.SetParent(newParent, true);
            });
        }

        public void StartTweenScale()
        {
            transform.DOScale(0.95f, 1f).SetEase(Ease.InOutBack).SetLoops(-1,LoopType.Yoyo);
        }
        public void StopTween()
        {
            transform.DOKill();
            transform.localScale = Vector3.one;
        }

        //ONLY FOR DEBUGING
        //private void Update()
        //{
        //    TEXT.text = match.X + "x" + match.Y;
        //}
    }
}