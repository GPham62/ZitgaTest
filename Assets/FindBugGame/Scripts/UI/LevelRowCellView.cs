using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class LevelRowCellView : EnhancedScrollerCellView
    {
        public LevelCellView[] cellViews;
        public void SetData(ref SmallList<ScrollerLevelData> data, int startingIndex)
        {
            for (var i = 0; i < cellViews.Length; i++)
            {
                var dataIndex = startingIndex + i;
                cellViews[i].SetData(dataIndex, dataIndex < data.Count ? data[dataIndex] : null);
            }
        }
    }
}

