using System;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;

// This is a super bare bones example of how to play and display a ink story in Unity.
public class BasicInkExample : MonoBehaviour {
    public static event Action<Story> OnCreateStory;

    public Story CreateStory(TextAsset jsonStory)
    {
	    inkJSONAsset = jsonStory;
	    story = new Story(inkJSONAsset.text);
	    OnCreateStory?.Invoke(story);
	    return story;
    }
    
    public void StartCharacterDialog(Story chosenStory, string knot)
    {
	    if (chosenStory == null) return;
	    chosenStory.ChoosePathString(knot);
	    RefreshView(chosenStory);
    }
    
    
	
	// This is the main function called every time the story changes. It does a few things:
	// Destroys all the old content and choices.
	// Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
	void RefreshView (Story chosenStory) {
		// Remove all the UI on screen
		RemoveChildren ();
		
		// Read all the content until we can't continue any more
		while (chosenStory.canContinue) {
			// Continue gets the next line of the story
			string text = chosenStory.Continue ();
			// This removes any white space from the text.
			text = text.Trim();
			// Display the text on screen!
			CreateContentView(text);
		}

		// Display all the choices, if there are any!
		if(chosenStory.currentChoices.Count > 0) {
			for (int i = 0; i < chosenStory.currentChoices.Count; i++) {
				Choice choice = chosenStory.currentChoices [i];
				Button button = CreateChoiceView (choice.text.Trim ());
				// Tell the button what to do when we press it
				button.onClick.AddListener (delegate {
					OnClickChoiceButton (choice, chosenStory);
				});
			}
		}
		// If we've read all the content and there's no choices, the story is finished!
		else {
			
			Button choice = CreateChoiceView("Done");
			choice.onClick.AddListener(delegate{
				RemoveChildren();
			});
		}
	}

	// When we click the choice button, tell the story to choose that choice!
	void OnClickChoiceButton (Choice choice, Story currentStory) {
		currentStory.ChooseChoiceIndex (choice.index);
		RefreshView(currentStory);
	}

	// Creates a textbox showing the the line of text
	void CreateContentView (string text) {
		Text storyText = Instantiate (textPrefab) as Text;
		storyText.text = text;
		storyText.transform.SetParent (canvas.transform, false);
	}

	// Creates a button showing the choice text
	Button CreateChoiceView (string text) {
		// Creates the button from a prefab
		Button choice = Instantiate (buttonPrefab) as Button;
		choice.transform.SetParent (canvas.transform, false);
		
		// Gets the text from the button prefab
		Text choiceText = choice.GetComponentInChildren<Text> ();
		choiceText.text = text;

		// Make the button expand to fit the text
		HorizontalLayoutGroup layoutGroup = choice.GetComponent <HorizontalLayoutGroup> ();
		layoutGroup.childForceExpandHeight = false;

		return choice;
	}

	// Destroys all the children of this gameobject (all the UI)
	void RemoveChildren () {
		int childCount = canvas.transform.childCount;
		for (int i = childCount - 1; i >= 0; --i) {
			Destroy (canvas.transform.GetChild (i).gameObject);
		}
	}

	private void SetStoryVariables()
	{
		foreach (var x in story.variablesState)
		{
			storyVariables.Add(x);
		}
	}
	
	public void SetInkStoryVariable( Story _story, string variable, object value, bool log = true )
	{
		if (story == null || !story.variablesState.GlobalVariableExistsWithName(variable))
		{
			return;
		}

		if( log )
		{
			Debug.Log( $"[Ink] Set variable: {variable} = {value}" );
		}

		_story.variablesState[variable] = value;

	}

	public object GetVariablesState(Story _story, string variable)
	{
		return _story.variablesState[variable];
	}
	
	[SerializeField]
	private TextAsset inkJSONAsset = null;
	public Story story;

	[SerializeField] private List<string> storyVariables;
	
	[SerializeField]
	private Canvas canvas = null;

	// UI Prefabs
	[SerializeField]
	private Text textPrefab = null;
	[SerializeField]
	private Button buttonPrefab = null;
}
