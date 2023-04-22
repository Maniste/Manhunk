using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mlook : MonoBehaviour {

	[SerializeField]
	private float sensitivityX;
	[SerializeField]
	private float sensitivityY;

	private float yaw = 0.0f;
	private float pitch = 0.0f;

	[SerializeField]
	private GameObject _character = null;

    private void Awake()
    {
		Cursor.lockState = CursorLockMode.Locked;
		pitch = transform.rotation.y;
	}
		

	void Update()
	{
		yaw += sensitivityX * Time.deltaTime * Input.GetAxisRaw ("Mouse X");
		pitch -= sensitivityY * Time.deltaTime * Input.GetAxisRaw ("Mouse Y");
		pitch = Mathf.Clamp (pitch, -90f, 90f);
		transform.eulerAngles = new Vector3 (pitch, yaw);

		_character.transform.localRotation = Quaternion.AngleAxis (yaw, _character.transform.up);
	}

	private void OnGUI()
	{
		GUI.color = Color.red;
		GUI.Label(new Rect(10, 480, 200, 20), "Press escape to lock/unlock mouse");
	}

}