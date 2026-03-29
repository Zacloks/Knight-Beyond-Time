using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float cameraSpeed = 2f;
    public Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = new Vector3(player.position.x,player.position.y,-10f);
        transform.position = Vector3.Slerp(transform.position,newPos,cameraSpeed*Time.deltaTime);
    }
}