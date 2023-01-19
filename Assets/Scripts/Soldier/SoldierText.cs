using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierText : MonoBehaviour
{
	private TextMesh _textMesh;

	private void Start()
	{
		_textMesh = transform.GetChild(1).GetComponent<TextMesh>();
	}

	private void Update()
	{
		if (GetComponent<Soldier>().DetectObject(Camera.main.transform.position + Vector3.down * 2, 0))
			UpdateText();
		else _textMesh.text = "";
	}

	private void UpdateText()
	{
		
		switch (GetComponent<Soldier>().GetSoldierMode())
		{
			case SoldierMode.Attack:
				_textMesh.text = "Attacking";
				break;

			case SoldierMode.Search:
				_textMesh.text = "Searching";
				break;

			case SoldierMode.Patrol:
				_textMesh.text = "Patrolling";
				break;
		}

		transform.GetChild(1).transform.rotation = GameObject.FindWithTag("Player").transform.rotation;
	}
}
