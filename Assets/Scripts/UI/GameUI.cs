using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUI : MonoBehaviour
{
	private BasicRuleset rules;
	private GameController gameController;

	[Header("Menu")]
	public GameObject MenuPanel;
	[Header("Rules")]
	public GameObject RulesPanel;
	public Text RulesDescription;
	public GameObject OptionsMenu;

	void Start()
	{
		gameController = GetComponent<GameController>();
		CreateNewRules();
	}

	void Update() 
	{
	
	}

	public void CreateNewRules()
	{
		rules = new BasicRuleset();
		RulesDescription.text = "";
		string[] ruleDescriptions = rules.GetRules();
		foreach(string description in ruleDescriptions)
		{
			RulesDescription.text += "* " + description + "\n \n";
		}
	}

	// Send the rules to the GameController to start the game.
	public void SendRules()
	{
		gameController.SetupGame( rules );
		MenuPanel.SetActive(false);
		RulesPanel.SetActive(false);
	}

	private void HideUI()
	{
		
	}

}
