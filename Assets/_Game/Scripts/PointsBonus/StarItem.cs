using UnityEngine;

public class StarItem : PointItemBase
{
    private void Reset()
    {
        pointValue = 50;
        disappearTime = 2f;
    }

    protected override void OnCollected()
    {
        base.OnCollected();
        // Ví dụ: thêm flash effect
    }
}