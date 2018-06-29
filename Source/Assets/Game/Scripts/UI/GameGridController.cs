using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameGridController : MonoBehaviour
    {
        public FlexibleGridLayout GridLayout;
        public FlexibleLayoutElement LayoutElementPrefab;

        Transform[,] layoutElements;

        public void GenerateNewGrid(int col, int row)
        {
            GridLayout.CreateGrid(col, row);
            layoutElements = new Transform[col, row];
            for (int x = 0; x < col; ++x)
            {
                for (int y = 0; y < row; ++y)
                {
                    FlexibleLayoutElement tmp = Instantiate(LayoutElementPrefab, GridLayout.transform);
                    layoutElements[x, y] = tmp.transform;
                    tmp.SetCoordinates(x, y);
                }
            }
        }

        public Transform GetTransformByIndex(int x, int y)
        {
            return layoutElements[x, y].transform;
        }
    }
}