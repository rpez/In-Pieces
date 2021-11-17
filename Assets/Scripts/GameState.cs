using System.Collections;
using System.Collections.Generic;

public class GameState
{
    /* BOOLEANS */

    // ROLL_SUCCESS determines if the previous dice roll in conversation was successful or not
    public bool ROLL_SUCCESS { get; set; } = false;

    // Bedroom tutorial scene
    public bool TUTORIAL_LISTEN { get; set; } = false;
    public bool TUTORIAL_OPEN_EYES { get; set; } = false;
    public bool TUTORIAL_GET_UP { get; set; } = false;

    /* INTEGERS */

    // BODY PART ATTITUDES
    public int NOSE { get; set; } = 0;
    public int EYES { get; set; } = 0;
    public int EARS { get; set; } = 0;
    public int HAND { get; set; } = 0;
    public int LEGS { get; set; } = 0;

    // Bedroom tutorial scene
    public int TUTORIAL_COUNT_SENSES { get; set; } = 0;
}
