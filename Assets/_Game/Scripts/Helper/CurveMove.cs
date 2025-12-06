using System;
using UnityEngine;

namespace Data
{
    public class CurveMove : MonoBehaviour
    {
        private float _time = 0;
        private float _moveTime = 0;
        private Vector3 _startPos;
        private Vector3 _endPos;
        private Vector3 _randomPos;
        private int _direct = 1;
        private Action _callBack;

        private bool _isStarted = false;

        public void CurveMoveVer1(Vector3 startPos, Vector3 endPosition, float t, Action callBack)
        {
            _startPos = startPos;
            _moveTime = t;
            _endPos = endPosition;
            _callBack = callBack;
            Vector3 midPoint = (_startPos + _endPos) / 2f;
            float offsetX = UnityEngine.Random.Range(-500f, 500f);
            float offsetY = UnityEngine.Random.Range(100f, 250f);

            _randomPos = new Vector3(
                _startPos.x + offsetX,
                _startPos.y + offsetY,
                _startPos.z
            );
            _isStarted = true;
        }
        public void CurveMoveVer2(Vector3 startPos, Vector3 endPosition, float time, int direct, Action callBack)
        {
            _startPos = startPos;
            _moveTime = time;
            _endPos = endPosition;
            _direct = direct;
            var posX = UnityEngine.Random.Range(-UnityEngine.Random.Range(_endPos.x, _startPos.x),
                UnityEngine.Random.Range(_endPos.x, _startPos.x));
            //var posX = (-UnityEngine.Random.Range(_endPos.x, _startPos.x) * _direct) * 2;
            //_randomPos = new Vector3(posX, UnityEngine.Random.Range(_endPos.y, _startPos.y));
            _randomPos = new Vector3(posX, _startPos.y);
            _isStarted = true;
            _callBack = callBack;
        }
        public Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            return a + (b - a) * t;
        }

        public Vector3 QuadraticCurve(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            Vector3 p0 = Lerp(a, b, t);
            Vector3 p1 = Lerp(b, c, t);
            return Lerp(p0, p1, t);
        }
        void Update()
        {
            if (_isStarted)
            {
                _time += Time.deltaTime;
                var curveTime = _time / _moveTime;
                transform.position = QuadraticCurve(_startPos, _randomPos, _endPos, curveTime);

                if (_time > _moveTime)
                {
                    _isStarted = false;
                    _callBack?.Invoke();
                }
            }
        }
    }
}