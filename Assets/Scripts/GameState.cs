using System.Collections;
using System.Collections.Generic;

public class GameState
{
    // ROLL_SUCCESS determines if the previous dice roll in conversation was successful or not
    public bool ROLL_SUCCESS { get; set; } = false;

    // BODY PART OWNERSHIP
    public bool HAS_NOSE { get; set; } = true;
    public bool HAS_EYES { get; set; } = true;
    public bool HAS_EARS { get; set; } = false;
    public bool HAS_HAND { get; set; } = false;
    public bool HAS_LEGS { get; set; } = true;

    // BODY PART ATTITUDES
    public int NOSE { get; set; } = 0;
    public int EYES { get; set; } = 0;
    public int EARS { get; set; } = 0;
    public int HAND { get; set; } = 0;
    public int LEGS { get; set; } = 0;

    // MANOR STATES
    public bool UPSTAIRS { get; set; } = false;
    public bool STEREO_IS_ON { get; set; } = false;

    // Bedroom tutorial scene
    public bool TUTORIAL_OPEN_EYES { get; set; } = false;
    public bool TUTORIAL_GET_UP { get; set; } = false;

    public int TUTORIAL_LISTEN { get; set; } = 0;
    public int TUTORIAL_COUNT_SENSES { get; set; } = 0;

    // Ears Quest
    public bool EARS_TALKED_WITH { get; set; } = false;
    public bool EARS_QUEST_SHOUTED { get; set; } = false;
    public bool EARS_QUEST_EYES_EXAMINED { get; set; } = false;
    public bool EARS_QUEST_NOSE_SNIFFED { get; set; } = false;
    public bool EARS_QUEST_NOSE_PICKUP_ATTEMPT { get; set; } = false;
    public bool EARS_QUEST_LEARNED_OF_STEREO { get; set; } = false;
    public bool EARS_QUEST_LEARNED_OF_WORM { get; set; } = false;
    public bool EARS_QUEST_EYES_SAW_WORM { get; set; } = false;

    public int EARS_QUEST_MADE_CONTACT { get; set; } = 0;
    public int EARS_QUEST_EXAMINATIONS { get; set; } = 0;
    public int EARS_QUEST_LEARNED_OF_SHIVERS { get; set; } = 0;
}
