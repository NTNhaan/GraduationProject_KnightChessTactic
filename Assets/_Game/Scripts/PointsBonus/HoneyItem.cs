using UnityEngine;

public class HoneyItem : PointItemBase
{
    private void Reset()
    {
        pointValue = 10;
        disappearTime = 5f;
    }

    protected override void OnCollected()
    {
        base.OnCollected();
        // Có thể thêm hiệu ứng particle
        // Vd: EffectManager.Instance.Play("HoneyCollect", transform.position);
    }
}