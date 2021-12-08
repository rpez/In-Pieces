using System.Collections;
using System.Collections.Generic;

public class GameState
{
    public bool GAME_OVER { get; set; } = false;

    // ROLL_SUCCESS determines if the previous dice roll in conversation was successful or not
    public bool ROLL_SUCCESS { get; set; } = false;

    // BODY PART OWNERSHIP
    public bool HAS_NOSE { get; set; } = false;
    public bool HAS_EYES { get; set; } = false;
    public bool HAS_EARS { get; set; } = false;
    public bool HAS_HAND { get; set; } = false;
    public bool HAS_LEGS { get; set; } = false;

    // BODY PART ATTITUDES
    public int NOSE { get; set; } = 0;
    public int EYES { get; set; } = 0;
    public int EARS { get; set; } = 0;
    public int HAND { get; set; } = 0;
    public int LEGS { get; set; } = 0;

    // MANOR STATES
    public bool UPSTAIRS { get; set; } = true;

    // Intro bar scene
    public bool INTRO_START { get; set; } = false;
    public bool INTRO_LOOKED_AROUND { get; set; } = false;
    public bool INTRO_HEAVY_METAL { get; set; } = false;
    public bool INTRO_THE_MANSION { get; set; } = false;
    public bool INTRO_FINISHED { get; set; } = false;

    public int INTRO_DRUNKENNESS_LEVEL { get; set; } = 0;

    // Bedroom tutorial scene
    public bool TUTORIAL_OPEN_EYES { get; set; } = false;
    public bool TUTORIAL_GET_UP { get; set; } = false;
    public bool TUTORIAL_ASKED_ABOUT_WHADUP { get; set; } = false;
    public bool TUTORIAL_ASKED_ABOUT_FRIEND { get; set; } = false;
    public bool TUTORIAL_ASKED_ABOUT_EXCHANGE { get; set; } = false;
    public bool NEAR_SIGHTED { get; set; } = false;

    public int TUTORIAL_LISTEN { get; set; } = 0;
    public int TUTORIAL_COUNT_SENSES { get; set; } = 0;
    public int TUTORIAL_COUNT_QUESTIONS { get; set; } = 0;

    // Staircase
    public bool UPSTAIRCASE_TALKED_WITH { get; set; } = false;
    public bool UPSTAIRCASE_NOSE_SNIFFED { get; set; } = false;
    public bool UPSTAIRCASE_ATTEMPTED_DESCENT { get; set; } = false;
    public bool UPSTAIRCASE_FINISHED_EXPLANATION { get; set; } = false;

    public bool DOWNSTAIRCASE_TALKED_WITH { get; set; } = false;

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

    // Stereo scene
    public bool STEREO_IS_ON { get; set; } = false;
    public bool STEREO_BASS_BOOST { get; set; } = false;
    public bool STEREO_IS_PLAYING { get; set; } = false;

    public bool STEREO_TALKED_WITH { get; set; } = false;
    public bool STEREO_LEARNED_OF_BUTTONS { get; set; } = false;
    public bool STEREO_LEARNED_OF_DYSLEXIA { get; set; } = false;

    public bool SPEAKER_EYES_EXAMINED { get; set; } = false;
    public bool SPEAKER_NOSE_SNIFFED { get; set; } = false;
    public bool CD_PLAYER_EYES_EXAMINED { get; set; } = false;
}