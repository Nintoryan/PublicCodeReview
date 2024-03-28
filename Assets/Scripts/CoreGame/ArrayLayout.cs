using System;

[Serializable]
public class ArrayLayout
{
    [Serializable]
    public struct RowData
    {
        public bool[] Row;
    }

    public RowData[] Rows = new RowData[7];
}
