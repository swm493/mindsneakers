using System;
using MindSneakers;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[Serializable]
public class DialogueData
{
    [JsonConverter(typeof(StringEnumConverter))]
    public Character character1;
    [JsonConverter(typeof(StringEnumConverter))]
    public Character character2;

    public Dialogue[] dialogues;
}

[Serializable]
public class Dialogue
{
    [JsonConverter(typeof(StringEnumConverter))]
    public Character speaker;
    [JsonConverter(typeof(StringEnumConverter))]
    public CharacterState state;
    
    [TextArea(2,5)]public string[] texts;
}