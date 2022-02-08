using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kalkatos.DottedArrow;

public class ChangeArrow : MonoBehaviour
{
	[SerializeField] private Arrow[] arrows;
	[SerializeField] private CombatManager combatManager;

	private int currentArrow;

	private void Update ()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow))
			combatManager.Arrow = arrows[currentArrow++ % arrows.Length];
	}

}
