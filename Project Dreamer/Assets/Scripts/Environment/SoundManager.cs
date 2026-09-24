using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    //Really just for ai behavior testing purposes right now. Holds a list of current heard sounds for the behavior tree to access.
    public List<Sound> HeardSounds = new List<Sound>();
}
