using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class FlexibleLayoutElement : MonoBehaviour
    {
        [Space(20)]
        [Header("LayoutElement Coordinates (X:Y) ")]
        public int X;
        public int Y;

        public void SetCoordinates(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}