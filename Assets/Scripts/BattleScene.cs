using UnityEngine;
using System.Collections;

public enum BattleCommand
{
	zero,
	block,
	bit,
	die
};

public class BattleScene : MonoBehaviour
{
	public Animator[] anim;
//	public  BattleCommand[] command;
	//int opponentIndex;
	static int attackState = Animator.StringToHash ("Base Layer.Attack");
	static int bitState = Animator.StringToHash ("Base Layer.Bit");
	static int dieState = Animator.StringToHash ("Base Layer.Die");
	static int blockState = Animator.StringToHash ("Base Layer.Block");
	static int idleState = Animator.StringToHash ("Base Layer.Idle");
	bool once = false;
	AnimatorStateInfo baseState;
	bool suicide = false;
	int currentIndex = 0;
	BattleCommand secondCommand;
	
	void OnEnabled()
	{
		anim [0].SetInteger ("animIndex", 0);
		anim [1].SetInteger ("animIndex", 0);
	}
	
	void Start ()
	{
		//setCommand (BattleCommand.die, BattleCommand.bit);
	}
	
	public void setCommand (BattleCommand command1, BattleCommand command2)
	{
		currentIndex = 0;
		suicide = false;
		if (command1 == command2 && command2 == BattleCommand.die) {
			anim [0].SetInteger ("animIndex", 1);
			anim [1].SetInteger ("animIndex", 1);
			suicide = true;
		} else if (command1 == BattleCommand.die) {
			currentIndex = 1;
			setAnim (command2);
			currentIndex = 0;
			secondCommand = command1;
			Invoke("secondPhase",2);
		} else {
			setAnim (command1);
			currentIndex = 1;
			secondCommand = command2;
			Invoke("secondPhase",2);
		}
	}
	
	void secondPhase()
	{
		setAnim (secondCommand);
		secondCommand = BattleCommand.zero;
	}
	
	void setAnim (BattleCommand command)
	{
		int alterIndex = Mathf.Abs (currentIndex - 1);
		if (command == BattleCommand.block) {
			anim [currentIndex].SetInteger ("animIndex", 1);
			anim [alterIndex].SetInteger ("animIndex", 3);
		} else if (command == BattleCommand.bit) {
			anim [currentIndex].SetInteger ("animIndex", 1);
			anim [alterIndex].SetInteger ("animIndex", 2);
		} else if (command == BattleCommand.die) {
			anim [currentIndex].SetInteger ("animIndex", 1);
			anim [alterIndex].SetInteger ("animIndex", 4);
		}
	}
	
	void Update ()
	{
		for (int i=0; i<anim.Length; i++) {
			baseState = anim [i].GetCurrentAnimatorStateInfo (0);
			if (baseState.nameHash == attackState || baseState.nameHash == bitState || baseState.nameHash == blockState) {
				if (!anim [i].IsInTransition (0)) {
					if (suicide) {
						anim [i].SetInteger ("animIndex", 5);
						Client.status = Statuses.play;
					} else {
							anim [i].SetInteger ("animIndex", 0);
						if(secondCommand == BattleCommand.zero)
							Client.status = Statuses.play;
					}
				}
			}
		}
		
		/*for (int i=0; i<anim.Length; i++) {
			opponentIndex = Mathf.Abs (i - 1);
			baseState = anim [i].GetCurrentAnimatorStateInfo (0);
			
			if (currentTurn == i||currentTurn<-1) {
				if(currentTurn==i)
				currentTurn = -1;
				else
				{
					actionCode = Constants.bothDie;
					currentTurn++;
				}
				anim [i].SetInteger ("animState", 1);
				if (actionCode == Constants.block)
					anim [opponentIndex].SetInteger ("animState", 2);
			}
			if (baseState.nameHash == attackState) {
				
				if (!anim [i].IsInTransition (0)) {
					anim [i].SetInteger ("animState", 0);

				}
				
				if (anim [i].GetFloat ("bit") > 0.1f) {
					if (actionCode == Constants.dmg) {
						anim [opponentIndex].SetInteger ("animState", 3);
						actionCode = 0;
					} else if (actionCode == Constants.kill) {
						anim [opponentIndex].SetInteger ("animState", 4);
						actionCode = 0;
					}	
					
				}
				if(anim [i].GetFloat ("bit") > 1.1f&&actionCode == Constants.bothDie)
					{
						anim [i].SetInteger ("animState", 4);
					}
				
			}
			if (baseState.nameHash == defState) {
				if (!anim [i].IsInTransition (0)) {
					anim [i].SetInteger ("animState", 0);

				}
			}
			if (baseState.nameHash == dmgState) {
				if (!anim [i].IsInTransition (0)) {
					anim [i].SetInteger ("animState", 0);

				}
			}
		}*/
				
	}
}
