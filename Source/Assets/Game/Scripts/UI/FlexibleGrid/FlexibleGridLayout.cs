using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class FlexibleGridLayout : GridLayoutGroup
    {
        int row = 3;
        int col = 3;

        public void CreateGrid(int col, int row)
        {
            this.col = col;
            this.row = row;
            UpdateCellSize();
        }


        protected override void OnRectTransformDimensionsChange()
        {
            UpdateCellSize();
        }

        public override void SetLayoutHorizontal()
        {
            UpdateCellSize();
            base.SetLayoutHorizontal();
        }

        public override void SetLayoutVertical()
        {
            UpdateCellSize();
            base.SetLayoutVertical();
        }

        private void UpdateCellSize()
        {
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;
            Vector2 newSize = new Vector2(width / col, height / row);
            cellSize = newSize;
        }
    }
}