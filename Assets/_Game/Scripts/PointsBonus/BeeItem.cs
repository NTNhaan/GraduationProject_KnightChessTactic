using UnityEngine;

public class BeeItem : PointItemBase
{
    private void Reset()
    {
        pointValue = 20;
        disappearTime = 3f;
    }

    protected override void OnCollected()
    {
        base.OnCollected();
        // Có thể thêm hiệu ứng đặc biệt khi thu ong
    }
}