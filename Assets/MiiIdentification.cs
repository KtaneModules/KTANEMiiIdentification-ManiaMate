using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using KModkit;

public class MiiIdentification : MonoBehaviour
{
	public KMAudio audio;
	public KMBombInfo bomb;
	public KMSelectable[] keyboard;
	public KMSelectable backbutton;
	public KMSelectable enterbutton;
	public KMSelectable startButton;
	public KMSelectable wiimenu;
	public KMSelectable screen;
	public Renderer modulescreen;
  public Renderer[] aesthetics;
	public Texture[] images;
	public Texture defused;
	public Material white;
	public TextMesh useranswer;
	private String[] names = new String[100]
	{"ABBY", "ABE", "AI", "AKIRA", "ALEX", "ALISHA", "ANDY", "ANNA", "ASAMI", "ASHLEY",
	 "BARBARA", "CHIKA","CHRIS", "COLE", "DAISUKE", "DAVID", "EDDY", "EDUARDO", "ELISA", "EMILY",
	 "EMMA", "EVA", "FRITZ",  "FUMIKO", "GABI", "GABRIELE", "GEORGE", "GIOVANNA", "GREG", "GWEN",
	 "HARU", "HAYLEY", "HELEN", "HIROMASA", "HIROMI", "HIROSHI", "HOLLY", "IAN", "JACKIE", "JAKE",
	 "JAMES", "JESSIE", "JULIE", "KATHRIN", "KEIKO", "KENTARO", "LUCA", "LUCIA", "MARCO", "MARIA",
	 "MARISA", "MARTIN", "MATT", "MEGAN", "MIA", "MICHAEL", "MIDORI", "MIGUEL", "MIKE", "MISAKI",
	 "MIYU", "NAOMI", "NELLY", "NICK",	"OSCAR", "PABLO", "PATRICK", "PIERRE", "RACHEL", "RAINER",
	 "REN", "RIN", "RYAN", "SABURO", "SAKURA", "SANDRA", "SARAH", "SHINNOSUKE", "SHINTA", "SHOHEI",
	 "SHOUTA", "SILKE", "SIOBHAN", "SOTA", "STEPH", "STEPHANIE", "STEVE", "SUSANA", "TAKASHI", "TAKUMI",
	 "TATSUAKI", "THEO", "TOMMY", "TOMOKO", "TYRONE", "URSULA", "VICTOR", "VINCENZO", "YOKO", "YOSHI"};
	private List<String> userinput;
  private String answer;
	private String combine;
	private int index;
	private bool strike;
  static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved;

	void Awake()
	{
		moduleId = moduleIdCounter++;
		startButton.OnInteract += delegate(){ StartPress(startButton); return false; };
		wiimenu.OnInteract += delegate() {WiiMenuPress(wiimenu); return false;};
    screen.OnInteract += delegate(){ ScreenPress(screen); return false; };
		enterbutton.OnInteract += delegate(){ EnterPress(enterbutton); return false; };
		backbutton.OnInteract += delegate(){ BackPress(backbutton); return false; };
		foreach(KMSelectable letter in keyboard)
		{
			letter.OnInteract += delegate(){ KeyPress(letter); return false; };
		}
	}
	void Start ()
	{
		useranswer.GetComponentInChildren<TextMesh>().text = "";
		useranswer.gameObject.SetActive(false);
		foreach(Renderer style in aesthetics)
		{
			style.gameObject.SetActive(true);
		}
		foreach(KMSelectable key in keyboard)
		{
			key.gameObject.SetActive(false);
		}
		backbutton.gameObject.SetActive(false);
		enterbutton.gameObject.SetActive(false);
		screen.gameObject.SetActive(false);
	}
		public void StartPress(KMSelectable button)
		{
				button.AddInteractionPunch();
				audio.PlaySoundAtTransform("Wii Start Sound Effect", transform);
				Debug.LogFormat("[Mii Identification #{0}] You pressed the start button!", moduleId);
				screen.gameObject.SetActive(true);
				startButton.gameObject.SetActive(false);
				wiimenu.gameObject.SetActive(false);
				foreach(Renderer aes in aesthetics)
				{
					aes.gameObject.SetActive(false);
				}
				index = UnityEngine.Random.Range(0,100);
			  answer = names[index];
				MiiIdentificationPerform();
		}

	 void MiiIdentificationPerform()
 	 {
		  foreach(KMSelectable key in keyboard)
 		  {
 			  key.gameObject.SetActive(false);
 		  }
			enterbutton.gameObject.SetActive(false);
			backbutton.gameObject.SetActive(false);
		  Debug.LogFormat("[Mii Identification #{0}] The mii that shows up is {1}.", moduleId, answer);
      modulescreen.material.mainTexture = images[index];
			screen.gameObject.SetActive(true);
   }

	 public void WiiMenuPress(KMSelectable button)
	 {
			 button.AddInteractionPunch();
			 audio.PlaySoundAtTransform("Wii Menu Sound Effect", transform);
	}


   public void ScreenPress(KMSelectable button)
	 {
			modulescreen.material = white;
			screen.gameObject.SetActive(false);
			Keyboard();
	 }

	 public void KeyPress(KMSelectable key)
	 {
		 audio.PlaySoundAtTransform("Mii Channel Typewriter Sound Effect", transform);
		 combine += key.GetComponentInChildren<TextMesh>().text;
		 useranswer.GetComponentInChildren<TextMesh>().text = combine;
	 }

	 public void EnterPress(KMSelectable enter)
	 {
		 enter.AddInteractionPunch();
		 audio.PlaySoundAtTransform("Mii Channel Enter_Back Sound Effect", transform);
		 Debug.LogFormat("[Mii Identification #{0}] You entered {1}.", moduleId, useranswer.GetComponentInChildren<TextMesh>().text);
		 if(Equals(combine, answer))
		 {
			 moduleSolved = true;
			 GetComponent<KMBombModule>().HandlePass();
			 foreach(KMSelectable key in keyboard)
  		  {
  			  key.gameObject.SetActive(false);
  		  }
 			 enterbutton.gameObject.SetActive(false);
 			 backbutton.gameObject.SetActive(false);
			 useranswer.gameObject.SetActive(false);
			 Debug.LogFormat("[Mii Identification #{0}] You got it! Module solved! :)", moduleId);
			 modulescreen.material.mainTexture = defused;
			 audio.PlaySoundAtTransform("Mii Channel Defused Sound Effect", transform);
		 }
		 else{
			 GetComponent<KMBombModule>().HandleStrike();
			 Debug.LogFormat("[Mii Identification #{0}] That was not the answer we were looking for.", moduleId);
			 combine = "";
			 useranswer.GetComponentInChildren<TextMesh>().text = "";
			 MiiIdentificationPerform();
		 }
	 }

	 public void BackPress(KMSelectable back)
	 {
		 back.AddInteractionPunch();
		 audio.PlaySoundAtTransform("Mii Channel Enter_Back Sound Effect", transform);
		 combine = "";
		 useranswer.GetComponentInChildren<TextMesh>().text = "";
	 }


  void Keyboard()
	{
     foreach(KMSelectable key in keyboard)
		 {
			 key.gameObject.SetActive(true);
		 }
		 backbutton.gameObject.SetActive(true);
		 enterbutton.gameObject.SetActive(true);
		 useranswer.gameObject.SetActive(true);
	}

}
