using System.Collections.Generic;
using Trouble_Group_8_Project;

public static class AI_Player_Test
{
    public static void Run(int[] mainPlayBoardArray, int[] redStartingArray,
                           int[] redVictoryArray, Dictionary<int, int[]> allStarts)
    {
        System.Diagnostics.Debug.WriteLine("=== AI TEST STARTING ===");

        AI_Player redAI = new AI_Player(
            colorCode: 1,
            mainBoard: mainPlayBoardArray,
            startZone: redStartingArray,
            victoryLane: redVictoryArray,
            colorStartZones: allStarts,
            victoryEntryIndex: 1,
            startIndex: 0
        );

        // Test 1: all home, roll 6 → escape home (expect 0-3)
        int[] pos1 = { -1, -1, -1, -1 };
        Log("Test 1 - Roll 6 all home (expect 0-3):", redAI.ChoosePiece(6, pos1));

        // Test 2: all home, roll 3 → no legal move (expect -1)
        Log("Test 2 - Roll 3 all home (expect -1):", redAI.ChoosePiece(3, pos1));

        // Test 3: piece 0 at first path stop, roll 2 → advance (expect 0)
        int[] pos3 = { mainPlayBoardArray[0], -1, -1, -1 };
        Log("Test 3 - Roll 2 one piece on board (expect 0):", redAI.ChoosePiece(2, pos3));

        // Test 4: piece 1 further along, roll 1 → pick furthest (expect 1)
        int[] pos4 = { mainPlayBoardArray[0], mainPlayBoardArray[5], -1, -1 };
        Log("Test 4 - Roll 1 pick furthest (expect 1):", redAI.ChoosePiece(1, pos4));

        // Test 5: piece at victoryEntryIndex-1, roll 2 → win move (expect 0)
        // red victoryEntryIndex is 1, so piece is at index 0, roll 2 → 
        // stepsToExit=1, victoryIndex=0 → lands in victory slot 0
        int[] pos5 = { mainPlayBoardArray[0], -1, -1, -1 };
        Log("Test 5 - Roll 2 near victory (expect 0):", redAI.ChoosePiece(2, pos5));

        // Test 6: piece would overshoot victory lane, roll 6 → no win move (expect -1 or advance)
        int[] pos6 = { mainPlayBoardArray[0], -1, -1, -1 };
        Log("Test 6 - Roll 6 would overshoot victory (expect -1 or advance):", redAI.ChoosePiece(6, pos6));

        // Test 7: two pieces on board, one can win → pick the winner (expect 0)
        int[] pos7 = { mainPlayBoardArray[0], mainPlayBoardArray[5], -1, -1 };
        Log("Test 7 - Roll 2 one can win (expect 0):", redAI.ChoosePiece(2, pos7));

        // Test 8: piece at home, roll 6, start not blocked → escape (expect 2)
        int[] pos8 = { mainPlayBoardArray[3], mainPlayBoardArray[5], -1, -1 };
        Log("Test 8 - Roll 6 one home not blocked (expect 2):", redAI.ChoosePiece(6, pos8));

        System.Diagnostics.Debug.WriteLine("=== AI TEST COMPLETE ===");
    }

    private static void Log(string label, int value)
    {
        System.Diagnostics.Debug.WriteLine($"{label} → {value}");
    }
}