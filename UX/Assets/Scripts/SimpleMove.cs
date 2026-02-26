using UnityEngine;

public class SimpleMove : MonoBehaviour {
    public float speed = 2f;
    void Update() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 right = new Vector3(transform.right.x, 0, transform.right.z).normalized;
        Vector3 dir = right * x + forward * z;
        transform.position += dir * speed * Time.deltaTime;
    }
}

