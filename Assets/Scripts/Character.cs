using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Warp
{
    public Util.CharacterName character_name;
    static Dictionary<Util.CharacterName, Character> characters = new();
    Animator animator;
    public static Character GetCharacter(Util.CharacterName name)
    {
        Debug.Assert(characters.ContainsKey(name), $"Character {name} not set");
        return characters[name];
    }
    public static Character Piggy { get => GetCharacter(Util.CharacterName.Piggy); }
	public static Character Partner { get => GetCharacter(Util.CharacterName.Partner); }
	private void Start()
	{
        Debug.Assert(!characters.ContainsKey(character_name), $"Character {name} already set");
        characters[character_name] = this;
        animator = GetComponent<Animator>();
        animator.speed = 0.0f;
	}
	private void OnDestroy()
	{
		characters.Clear();
	}
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void StartTalking()
    {
        animator.speed = 1.0f;
    }
    public void StopTalking()
    {
        animator.speed = 0.0f;
    }
}
