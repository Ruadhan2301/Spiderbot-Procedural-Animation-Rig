using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author - Rowan Goswell
/// Date - 02/02/2021
/// </summary>
public class ControllerScript : MonoBehaviour
{
    private MultiLegWalkerCode walkerScript;

    public Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        walkerScript = GetComponent<MultiLegWalkerCode>();
    }

    // Update is called once per frame
    void Update()
    {
        
		float dirX = Input.GetAxisRaw("Horizontal");
		float dirZ = Input.GetAxisRaw("Vertical");
		if(dirX != 0f || dirZ != 0f){
			direction = new Vector3(dirX, 0f, dirZ);
				
			walkerScript.MoveDirection = direction;
		
			walkerScript.FaceDirection = direction;
		}else{
			walkerScript.MoveDirection = Vector3.zero;
		}
        
    }

}