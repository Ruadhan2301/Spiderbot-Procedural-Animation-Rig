using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author - Rowan Goswell
/// Date - 02/02/2021
/// </summary>
public class WandererScript : MonoBehaviour
{
    private MultiLegWalkerCode walkerScript;

    public Vector3 destination;
    public Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        walkerScript = GetComponent<MultiLegWalkerCode>();
        UpdateDestination();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 lateralDestination = new Vector2(destination.x,destination.z);
        Vector2 lateralPosition = new Vector2(transform.position.x,transform.position.z);
        if((lateralDestination - lateralPosition).magnitude < 5f)
        {
            UpdateDestination();
        }
        direction = destination - transform.position;
        walkerScript.MoveDirection = direction;
        walkerScript.FaceDirection = direction;
        
    }

    private void UpdateDestination()
    {
        Vector2 offsetPosition = Random.insideUnitCircle * 30f;

        destination = new Vector3(500,transform.position.y,500f) + new Vector3(offsetPosition.x, 0f, offsetPosition.y);
    }

}
