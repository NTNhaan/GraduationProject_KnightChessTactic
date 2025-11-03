using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

    public class RotateSelf : MonoBehaviour {
        [Header("1 direction")]
        public Vector3 oneDirection = Vector3.back;

        [Header("random direction")]
        public Vector3 randomDirection = Vector3.back;
        Vector3 rotate;
        public float speed = 50;
        // Start is called before the first frame update
        private void Start() {
            if(oneDirection == Vector3.zero)
                oneDirection = Vector3.back;
            rotate = new Vector3(randomDirection.x, randomDirection.y, Random.Range(-randomDirection.z, randomDirection.z));
        }
        // Update is called once per frame
        void Update() {
            if(oneDirection.z != 0) {
                transform.Rotate(new Vector3(oneDirection.x, oneDirection.y, oneDirection.z) * speed * Time.deltaTime, Space.Self);
            }

            if(randomDirection.z != 0) {
                transform.Rotate(rotate * speed * Time.deltaTime, Space.Self);
            }
        }

        private void OnDrawGizmos()
        {
            oneDirection = oneDirection.normalized;
        }
    } 