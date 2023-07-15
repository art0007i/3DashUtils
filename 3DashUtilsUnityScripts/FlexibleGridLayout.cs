using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace _3DashUtils.UnityScripts
{
    [AddComponentMenu("Layout/Flexible Grid Layout", 152)]
    public class FlexibleGridLayout : LayoutGroup
    {
        // Code taken from video https://www.youtube.com/watch?v=CGsEJToeXmA

        public enum FitType
        {
            Uniform,
            Width,
            Height,
            FixedRows,
            FixedColumns,
            // addon
            FixedBoth
        }

        public FitType fitType;
        public int rows = 3;
        public int columns = 3;
        public Vector2 cellSize = new(100, 100);
        public Vector2 spacing = new(0, 0);
        public bool fitX;
        public bool fitY;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            if (fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform)
            {
                float sqrRt = Mathf.Sqrt(transform.childCount);
                rows = Mathf.CeilToInt(sqrRt);
                columns = Mathf.CeilToInt(sqrRt);
            }

            if (fitType == FitType.Width || fitType == FitType.FixedColumns)
            {
                rows = Mathf.CeilToInt(transform.childCount / (float)columns);
            }
            if (fitType == FitType.Height || fitType == FitType.FixedRows)
            {
                columns = Mathf.CeilToInt(transform.childCount / (float)rows);
            }

            float parentWidth = rectTransform.rect.width;
            float parentHeight = rectTransform.rect.height;

            float cellWidth =
                (parentWidth / (float)columns)
                - ((spacing.x / (float)columns) * 2)
                - (padding.left / (float)columns)
                - (padding.right / (float)columns);
            float cellHeight =
                (parentHeight / (float)rows)
                - ((spacing.y / (float)rows) * 2)
                - (padding.top / (float)rows)
                - (padding.bottom / (float)rows);

            cellSize.x = fitX ? cellWidth : cellSize.x;
            cellSize.y = fitY ? cellHeight : cellSize.y;

            int columnCount = 0;
            int rowCount = 0;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                rowCount = i / columns;
                columnCount = i % columns;

                var item = rectChildren[i];

                var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
                var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

                SetChildAlongAxis(item, 0, xPos, cellSize.x);
                SetChildAlongAxis(item, 1, yPos, cellSize.y);
            }
        }

        public override void CalculateLayoutInputVertical()
        {
        }

        public override void SetLayoutHorizontal()
        {
        }

        public override void SetLayoutVertical()
        {
        }
    }
}