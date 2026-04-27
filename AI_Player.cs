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
        private Dictionary<int, int[]> colorStartZones; //colorCode -> startZone
        private Random rand = new Random();


        public AI_Player(int colorCode, int[] mainBoard, int[] startZone, int[] victoryLane, Dictionary<int, int[]> colorStartZones)
        {
            colorCode = this.colorCode;
            mainBoard = this.mainBoard;
            startZone = this.startZone;
            victoryLane = this.victoryLane;
            colorStartZones = this.colorStartZones;
        }

        // Entry point
        // returns piece inxed 0-3 or -1 = skip
        // piecePositions[i] = board idx of piece i or -1 = home

        public int ChoosePiece(int diceRoll, int[] piecePosition)
        {
            // 1) Can any piece reach a victory square?
            int win = TryWin(diceRoll, piecePosition);
            if (win != -1) return win;

            // 2) Can any piece land on an opp?
            int landOpp = TryLandOpponent(diceRoll, piecePosition);
            if (landOpp != -1) return landOpp;

            // 3) Prioritize leaving home when roll a 6
            if (diceRoll == 6)
            {
                int escape = TryEscapeHome(piecePosition);
                if (escape != -1) return escape;
            }

            // 4) Move furthest piece
            int furthest = TryFurthestPiece(diceRoll, piecePosition);
            if (furthest != -1) return furthest;

            // 5) Make a legal random move
            return TryRandom(diceRoll, piecePosition);
        }

        private int TryWin(int roll, int[] positions)
        {
            return -1;
        }

        private int TryLandOpponent(int roll, int[] positions)
        {
            return -1;
        }

        private int TryEscapeHome(int[] positions)
        {
            return -1;
        }

        private int TryFurthestPiece(int roll, int[] positions)
        {
            return -1;
        }

        private int TryRandom(int roll, int[] positions)
        {
            return -1;
        }


    }


}
