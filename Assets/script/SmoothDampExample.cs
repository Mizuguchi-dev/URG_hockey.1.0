using UnityEngine;

// <summary>
/// ターゲットのz座標に追従させるスクリプト
// </summary>
public class SmoothDampExample : MonoBehaviour
{
    // ターゲット
    [SerializeField] private Transform _target;

    // 追従させるオブジェクト
    [SerializeField] private Transform _follower;

    // 目標値に到達するまでのおおよその時間[s]
    [SerializeField] private float _smoothTime = 0.3f;

    // 最高速度
    [SerializeField] private float _maxSpeed = float.PositiveInfinity;

    // 現在速度(SmoothDampの計算のために必要)
    private float _currentVelocity = 0;

    // z座標をターゲットのz座標に追従させる
    private void Update()
    {
        // 現在位置取得
        var currentPos = _follower.position;


        // 次フレームの位置を計算
        currentPos.z = Mathf.SmoothDamp(
            currentPos.z,
            _target.position.z,
            ref _currentVelocity,
            _smoothTime,
            _maxSpeed
        );

        // 現在位置のz座標を更新
        _follower.position = currentPos;
    }
}