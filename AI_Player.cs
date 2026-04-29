// HOW TO INTEGRATE (for Eric - doing turn logic):
//
//   1. Create one TroubleAI per AI player, e.g.:
//        TroubleAI redAI = new TroubleAI(1, mainPlayBoardArray,
//                              redStartingArray, redVictoryArray,
//                              new Dictionary<int,int[]> {
//                                  {1, redStartingArray},
//                                  {2, yellowStartingArray},
//                                  {3, greenStartingArray},
//                                  {4, blueStartingArray}
//                              });
//
//   2. On the AI's turn, after rolling the dice call:
//        int pieceToMove = redAI.ChoosePiece(roll, piecePositions);
//        — piecePositions is int[] of length 4, each value is the
//          board index that piece is currently sitting on.
//          Use -1 if a piece is still in the home/start zone.
//
//   3. ChoosePiece returns the INDEX into piecePositions (0-3)
//      of the piece the AI wants to move, or -1 if no move is
//      possible (skip turn).
// ============================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trouble_Group_8_Project
{
    public class AI_Player
    {
        private readonly int colorCode; // 1=Red 2=Green 3=Blue 4=Yellow
        private int[] mainBoard;
        private int[] startZone;
        private int[] victoryLane;
        private int victoryEntryIndex;
        private int startIndex;
        private Dictionary<int, int[]> colorStartZones; //colorCode -> startZone
        private Random rand = new Random();


        public AI_Player(int colorCode, int[] mainBoard, int[] startZone, int[] victoryLane,
     Dictionary<int, int[]> colorStartZones, int victoryEntryIndex, int startIndex)
        {
            this.colorCode = colorCode;
            this.mainBoard = mainBoard;
            this.startZone = startZone;
            this.victoryLane = victoryLane;
            this.colorStartZones = colorStartZones;
            this.victoryEntryIndex = victoryEntryIndex;
            this.startIndex = startIndex;
        }

        // Entry point
        // returns piece inxed 0-3 or -1 = skip
        // piecePosition[i] = board idx of piece i or -1 = home

        public int ChoosePiece(int diceRoll, int[] piecePositions, HashSet<int> opponentSquares, int[] progressValues)
        {
            
            // 1) Can any piece reach a victory square?
            int win = TryWin(diceRoll, piecePositions);
            if (win != -1) return win;
            
            // 2) Can any piece land on an opp?
            int landOpp = TryLandOpponent(diceRoll, piecePositions, opponentSquares);
            if (landOpp != -1) return landOpp;
            
            // 3) Prioritize leaving home when roll a 6
            if (diceRoll == 6)
            {
                int escape = TryEscapeHome(piecePositions);
                if (escape != -1) return escape;
            }

            // 4) Move furthest piece or the pice blocking enterance
            int furthest = TryBestPiece(diceRoll, piecePositions, progressValues);
            if (furthest != -1) return furthest;
            
            // 5) Make a legal random move
            return TryRandom(diceRoll, piecePositions);
        }

        private int TryWin(int roll, int[] positions)
        {
            for (int i = 0; i < 4; i++)
            {
                if (positions[i] == -1)
                {
                    continue;
                }

                int currentPos = Array.IndexOf(mainBoard, positions[i]);
                if (currentPos == -1)
                {
                    continue;
                }

                // steps from current position to the exit point
                int stepsToExit = victoryEntryIndex - currentPos;

                // already passed exit or not close enough
                if (stepsToExit < 0 || stepsToExit >= roll) continue;

                // remaining steps after exit land in victory lane
                int victoryIndex = roll - stepsToExit - 1;
                if (victoryIndex >= 0 && victoryIndex < victoryLane.Length)
                {
                    return i;
                }
            }
            return -1;
        }

        private int TryLandOpponent(int roll, int[] positions, HashSet<int> opponentSquares)
        {
            for (int i = 0; i < 4; i++)
            {
                if (positions[i] == -1) continue;

                int dest = TestMove(roll, positions[i]);
                if (dest == -1) continue;

                if (opponentSquares.Contains(dest))
                    return i;
            }
            return -1;
        }

        private int TryEscapeHome(int[] positions)
        {
            for (int i = 0; i < 4; i++)
            {
                if (positions[i] == -1)  // piece is at home
                {
                    // make sure the start index isn't blocked by own piece
                    bool startBlocked = false;
                    for (int j = 0; j < 4; j++)
                    {
                        if (i != j && positions[j] == mainBoard[startIndex])
                        {
                            startBlocked = true;
                            break;
                        }
                    }
                    if (!startBlocked)
                        return i;
                }
            }
            return -1;
        }

        private int TryBestPiece(int roll, int[] positions, int[] progressValues)
        {
            int bestPiece = -1;
            int furthest = -1;

            bool anyAtHome = positions.Any(p => p == -1);

            for (int i = 0; i < 4; i++)
            {
                if (positions[i] == -1) continue;

                int dest = TestMove(roll, positions[i]);
                if (dest == -1) continue;

                // if pieces are still at home, avoid landing on the entry square
                if (anyAtHome && dest == mainBoard[startIndex])
                    continue;

                if (progressValues[i] > furthest)
                {
                    furthest = progressValues[i];
                    bestPiece = i;
                }
            }

            // if all moves land on entry square, just pick furthest anyway
            // to avoid getting completely stuck
            if (bestPiece == -1)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (positions[i] == -1) continue;
                    int dest = TestMove(roll, positions[i]);
                    if (dest == -1) continue;
                    if (progressValues[i] > furthest)
                    {
                        furthest = progressValues[i];
                        bestPiece = i;
                    }
                }
            }

            return bestPiece;
        }

        private int TryRandom(int roll, int[] positions)
        {
            var legal = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                if (positions[i] == -1)
                {
                    continue;
                }
                if (TestMove(roll, positions[i]) != -1)
                {
                    legal.Add(i);
                }
            }
            if (legal.Count == 0)
            {
                return -1;
            }
            return legal[rand.Next(legal.Count)];
        }

        private int TestMove(int roll, int currIndex)
        {
            int pos = Array.IndexOf(mainBoard, currIndex);
            if (pos == -1) return -1; // not on main board

            int dest = pos + roll;

            // would enter or land in victory lane — still a valid move
            if (dest >= mainBoard.Length)
            {
                int victoryIndex = dest - mainBoard.Length;
                if (victoryIndex < victoryLane.Length)
                    return victoryLane[victoryIndex]; // valid, returns victory square
                return -1; // overshoots victory lane
            }

            return mainBoard[dest];
        }
    }
}
