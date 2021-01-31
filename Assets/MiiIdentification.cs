using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class MiiIdentification : MonoBehaviour
{
    public KMAudio Audio;
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

    private static readonly string[] names = new string[]
    {
        "ABBY", "ABE", "AI", "AKIRA", "ALEX", "ALISHA", "ANDY", "ANNA", "ASAMI", "ASHLEY",
        "BARBARA", "CHIKA", "CHRIS", "COLE", "DAISUKE", "DAVID", "EDDY", "EDUARDO", "ELISA", "EMILY",
        "EMMA", "EVA", "FRITZ", "FUMIKO", "GABI", "GABRIELE", "GEORGE", "GIOVANNA", "GREG", "GWEN",
        "HARU", "HAYLEY", "HELEN", "HIROMASA", "HIROMI", "HIROSHI", "HOLLY", "IAN", "JACKIE", "JAKE",
        "JAMES", "JESSIE", "JULIE", "KATHRIN", "KEIKO", "KENTARO", "LUCA", "LUCIA", "MARCO", "MARIA",
        "MARISA", "MARTIN", "MATT", "MEGAN", "MIA", "MICHAEL", "MIDORI", "MIGUEL", "MIKE", "MISAKI",
        "MIYU", "NAOMI", "NELLY", "NICK", "OSCAR", "PABLO", "PATRICK", "PIERRE", "RACHEL", "RAINER",
        "REN", "RIN", "RYAN", "SABURO", "SAKURA", "SANDRA", "SARAH", "SHINNOSUKE", "SHINTA", "SHOHEI",
        "SHOUTA", "SILKE", "SIOBHAN", "SOTA", "STEPH", "STEPHANIE", "STEVE", "SUSANA", "TAKASHI", "TAKUMI",
        "TATSUAKI", "THEO", "TOMMY", "TOMOKO", "TYRONE", "URSULA", "VICTOR", "VINCENZO", "YOKO", "YOSHI"
    };

    private List<string> userinput;
    private string answer;
    private string combine;
    private int index;
    private bool strike;
    static int moduleIdCounter = 1;
    private enum State
    {
        ReadyToStart,
        Showing,
        Submitting
    }
    private State currentState;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        startButton.OnInteract += delegate
        {
            StartPress(startButton);
            return false;
        };
        wiimenu.OnInteract += delegate
        {
            WiiMenuPress(wiimenu);
            return false;
        };
        screen.OnInteract += delegate
        {
            ScreenPress();
            return false;
        };
        enterbutton.OnInteract += delegate
        {
            EnterPress(enterbutton);
            return false;
        };
        backbutton.OnInteract += delegate
        {
            BackPress(backbutton);
            return false;
        };
        foreach (KMSelectable letter in keyboard)
        {
            var l = letter;
            letter.OnInteract += delegate
            {
                KeyPress(l);
                return false;
            };
        }
    }

    void Start()
    {
        currentState = State.ReadyToStart;
        useranswer.GetComponentInChildren<TextMesh>().text = "";
        useranswer.gameObject.SetActive(false);
        foreach (Renderer style in aesthetics)
        {
            style.gameObject.SetActive(true);
        }

        foreach (KMSelectable key in keyboard)
        {
            key.gameObject.SetActive(false);
        }

        backbutton.gameObject.SetActive(false);
        enterbutton.gameObject.SetActive(false);
        screen.gameObject.SetActive(false);
    }

    private void StartPress(KMSelectable button)
    {
        button.AddInteractionPunch();
        Audio.PlaySoundAtTransform("Wii Start Sound Effect", transform);
        Debug.LogFormat("[Mii Identification #{0}] You pressed the start button!", moduleId);
        screen.gameObject.SetActive(true);
        startButton.gameObject.SetActive(false);
        wiimenu.gameObject.SetActive(false);
        foreach (Renderer aes in aesthetics)
        {
            aes.gameObject.SetActive(false);
        }

        index = Random.Range(0, 100);
        answer = names[index];
        MiiIdentificationPerform();
    }

    private void MiiIdentificationPerform()
    {
        foreach (KMSelectable key in keyboard)
        {
            key.gameObject.SetActive(false);
        }
        currentState = State.Showing;
        enterbutton.gameObject.SetActive(false);
        backbutton.gameObject.SetActive(false);
        Debug.LogFormat("[Mii Identification #{0}] The mii that shows up is {1}.", moduleId, answer);
        modulescreen.material.mainTexture = images[index];
        screen.gameObject.SetActive(true);
    }

    private void WiiMenuPress(KMSelectable button)
    {
        button.AddInteractionPunch();
        Audio.PlaySoundAtTransform("Wii Menu Sound Effect", transform);
    }


    private void ScreenPress()
    {
        modulescreen.material = white;
        currentState = State.Submitting;
        screen.gameObject.SetActive(false);
        Keyboard();
    }

    private void KeyPress(KMSelectable key)
    {
        Audio.PlaySoundAtTransform("Mii Channel Typewriter Sound Effect", transform);
        if (useranswer.text.Length > 9)
        {
            return;
        }
        combine += key.GetComponentInChildren<TextMesh>().text;
        useranswer.GetComponentInChildren<TextMesh>().text = combine;
    }

    private void EnterPress(KMSelectable enter)
    {
        enter.AddInteractionPunch();
        Audio.PlaySoundAtTransform("Mii Channel Enter_Back Sound Effect", transform);
        Debug.LogFormat("[Mii Identification #{0}] You entered {1}.", moduleId,
            useranswer.GetComponentInChildren<TextMesh>().text);
        if (Equals(combine, answer))
        {
            moduleSolved = true;
            GetComponent<KMBombModule>().HandlePass();
            foreach (KMSelectable key in keyboard)
            {
                key.gameObject.SetActive(false);
            }

            enterbutton.gameObject.SetActive(false);
            backbutton.gameObject.SetActive(false);
            useranswer.gameObject.SetActive(false);
            Debug.LogFormat("[Mii Identification #{0}] You got it! Module solved! :)", moduleId);
            modulescreen.material.mainTexture = defused;
            Audio.PlaySoundAtTransform("Mii Channel Defused Sound Effect", transform);
        }
        else
        {
            GetComponent<KMBombModule>().HandleStrike();
            Debug.LogFormat("[Mii Identification #{0}] That was not the answer we were looking for.", moduleId);
            combine = "";
            useranswer.GetComponentInChildren<TextMesh>().text = "";
            MiiIdentificationPerform();
        }
    }

    private void BackPress(KMSelectable back)
    {
        back.AddInteractionPunch();
        Audio.PlaySoundAtTransform("Mii Channel Enter_Back Sound Effect", transform);
        combine = "";
        useranswer.GetComponentInChildren<TextMesh>().text = "";
    }


    private void Keyboard()
    {
        foreach (KMSelectable key in keyboard)
        {
            key.gameObject.SetActive(true);
        }

        backbutton.gameObject.SetActive(true);
        enterbutton.gameObject.SetActive(true);
        useranswer.gameObject.SetActive(true);
    }
    
#pragma warning disable 414
    private const string TwitchHelpMessage = "Start the module with !{0} start. Press the displayed mii with !{0} mii. Submit the name of a mii using !{0} submit NAME.";
#pragma warning restore 414
    
    private IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant().Trim();
        switch (currentState)
        {
            case State.ReadyToStart:
                if (command.EqualsAny("start", "press start"))
                {
                    yield return null;
                    startButton.OnInteract();
                    yield break;
                }
                yield break;
            case State.Showing:
                if (command.EqualsAny("mii", "press mii"))
                {
                    yield return null;
                    screen.OnInteract();
                    yield break;
                }
                yield break;
            case State.Submitting:
                if (command.StartsWith("submit"))
                {
                    var parsedCommand = command.Split(' ');
                    if (parsedCommand.Length != 2)
                    {
                        yield return "sendtochaterror Invalid number of parameters!";
                        yield break;
                    }

                    yield return null;
                    
                    var nameToSubmit = parsedCommand[1];
                    if (nameToSubmit.Any(x => !"abcdefghijklmnopqrstuvwxyz".Contains(x)))
                    {
                        yield return "sendtochaterror There is an invalid character in the name!";
                        yield break;
                    }
                    backbutton.OnInteract();
                    yield return new WaitForSeconds(.1f);
                    foreach (var letter in nameToSubmit)
                    {
                        keyboard["abcdefghijklmnopqrstuvwxyz".IndexOf(letter)].OnInteract();
                        yield return new WaitForSeconds(.1f);
                    }
                    enterbutton.OnInteract();
                    yield break;
                }
                yield return "sendtochaterror Every command in this state must start with submit!";
                yield break;
        }
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        if (currentState == State.ReadyToStart)
        {
            startButton.OnInteract();
            yield return new WaitForSeconds(.1f);
        }

        if (currentState == State.Showing)
        {
            screen.OnInteract();
            yield return new WaitForSeconds(.1f);
        }
        backbutton.OnInteract();
        yield return new WaitForSeconds(.1f);
        foreach (var letter in answer.ToLowerInvariant())
        {
            keyboard["abcdefghijklmnopqrstuvwxyz".IndexOf(letter)].OnInteract();
            yield return new WaitForSeconds(.1f);
        }
        enterbutton.OnInteract();
    }
}

