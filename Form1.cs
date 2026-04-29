using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Media;
using WMPLib;


namespace Trouble_Group_8_Project
{
    public partial class Form1 : Form
    {
        private WindowsMediaPlayer backgroundMusic = new WindowsMediaPlayer();

        private (int Color, bool Functional, Color OutlineColor)[] boardCells;
        private Random dice = new Random(); // For Dice value 1-6

        private int[] mainPlayBoardArray; // All Functional slots

        // Colors Starting Position
        private int[] redStartingArray;
        private int[] yellowStartingArray;
        private int[] greenStartingArray;
        private int[] blueStartingArray;

        // Color Victory Lanes Outline 
        private int[] redVictoryArray;
        private int[] yellowVictoryArray;
        private int[] greenVictoryArray;
        private int[] blueVictoryArray;

        // To know which color is choosen by the player //
        private bool[] isPlayer = new bool[5];

        // Variables for current color turn and roll logic //
        private int currentTurnColor = 1;
        private int currentDiceRoll = 0;
        private bool hasRolled = false;

        // The area where the color pieces are (Outside of the main board, not in 0 to ....). //
        private int[] redPieces = { -1, -1, -1, -1 };
        private int[] yellowPieces = { -1, -1, -1, -1 };
        private int[] greenPieces = { -1, -1, -1, -1 };
        private int[] bluePieces = { -1, -1, -1, -1 };

        // Letting the game know where each color enters the board path of number cell //
        private int[] entryProgress = new int[5];

        public Form1()
        {
            // Structure of the board / Player Color and Message Logic
            InitializeComponent();

            backgroundMusic.URL = "Sounds/pripac-soft-chill-vibes-323673.wav";
            backgroundMusic.settings.setMode("loop", true);
            backgroundMusic.settings.volume = 25;
            backgroundMusic.controls.play();

            ConfigureTableLayoutPanel();
            InitializeBoard();
            AskPlayerColor();
            LoadBoardIntoTableLayout();

            this.Shown += (s, e) =>
            {
                ShowTurnMessage(); // Showing who's turn is it //
                if (!isPlayer[currentTurnColor])     // if first turn is AI, kick it off automatically
                {

                    AITurn();
                }
            };
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // Method in setting up the board by code instead of doing it manually in the actual Form to prevent errors
        private void ConfigureTableLayoutPanel()
        {
            tableLayoutPanel1.RowCount = 9;
            tableLayoutPanel1.ColumnCount = 9;
            tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel1.Dock = DockStyle.Fill;

            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.ColumnStyles.Clear();

            for (int i = 0; i < 9; i++)
            {
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 11.11F));
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.11F));
            }
        }
        //----------------------------------------------------------------------------------------------------------board game size

        private void InitializeBoard()
        {
            boardCells = new (int Color, bool Functional, Color OutlineColor)[81];

            for (int i = 0; i < 81; i++)
                boardCells[i] = (5, false, Color.Empty);

            // Red = 1
            for (int i = 1; i <= 4; i++)
                boardCells[i].Color = 1;

            // White = 6
            int[] whiteBoardValues = {
                10,11,12,13,14,15,16,
                19,20,21,22,23,24,25,
                28,29,         33,34,
                37,38,         42,43,
                46,47,         51,52,
                55,56,57,58,59,60,61,
                64,65,66,67,68,69,70
            };

            foreach (int i in whiteBoardValues)
                boardCells[i].Color = 6;

            // Yellow = 2
            int[] yellowBoardValues = { 17, 26, 35, 44 };
            foreach (int i in yellowBoardValues)
                boardCells[i].Color = 2;

            // Blue = 4
            int[] blueBoardValues = { 36, 45, 54, 63 };
            foreach (int i in blueBoardValues)
                boardCells[i].Color = 4;

            // Green = 3
            for (int i = 76; i <= 79; i++)
                boardCells[i].Color = 3;

            /*
            // Functional game boxes 
            int[] FUNCTIONALBOARDVALUES = {
                 10,11,12,13,14,15,16,
                 19,               25,
                 28,               34,
                 37,               43,
                 46,               52,
                 55,               61,
                 64,65,66,67,68,69,70
             };
          
            foreach (int i in FUNCTIONALBOARDVALUES)
                boardCells[i].Functional = true;
            */


            // Outlines
            (int Index, Color Outline)[] outlineValues = {
                (18, Color.Red),(20, Color.Red), (21, Color.Red), (22, Color.Red), (23, Color.Red),
                (6, Color.Yellow),(24, Color.Yellow), (33, Color.Yellow), (42, Color.Yellow), (51, Color.Yellow),
                (62, Color.Green),(60, Color.Green), (59, Color.Green), (58, Color.Green), (57, Color.Green),
                (74, Color.Blue),(56, Color.Blue), (47, Color.Blue), (38, Color.Blue), (29, Color.Blue)
            };

            foreach (var val in outlineValues)
            {
                boardCells[val.Index].OutlineColor = val.Outline;
            }

            // Main game board array
            //mainPlayBoardArray = FUNCTIONALBOARDVALUES;
            mainPlayBoardArray = new int[]
            {
                10, 11, 12, 13, 14, 15, 16,  // top row,    index 0-6
                25, 34, 43, 52, 61,          // right side, index 7-11
                70, 69, 68, 67, 66, 65, 64,  // bottom row, index 12-18
                55, 46, 37, 28, 19           // left side,  index 19-23
            };


            int[] FUNCTIONALBOARDVALUES = mainPlayBoardArray;


            foreach (int i in FUNCTIONALBOARDVALUES)
                boardCells[i].Functional = true;

            // Starting positions 
            redStartingArray = new int[] { 1, 2, 3, 4 };
            yellowStartingArray = new int[] { 17, 26, 35, 44 };
            blueStartingArray = new int[] { 36, 45, 54, 63 };
            greenStartingArray = new int[] { 76, 77, 78, 79 };

            // Victory Arrays 
            redVictoryArray = new int[] { 20, 21, 22, 23 };
            yellowVictoryArray = new int[] { 24, 33, 42, 51 };
            greenVictoryArray = new int[] { 60, 59, 58, 57 };
            blueVictoryArray = new int[] { 56, 47, 38, 29 };

            // Starting index for each color so the pieces enter at their designed color area //
            entryProgress[1] = 0;
            entryProgress[2] = 6;
            entryProgress[3] = 12;
            entryProgress[4] = 18;
        }



        //-------------------------------------------------------- Pre-Game Set up for color
        // Ask player which color they want to be //
        private void AskPlayerColor()
        {
            // Only want will actually be the player //
            isPlayer[1] = true;
            isPlayer[2] = false;
            isPlayer[3] = false;
            isPlayer[4] = false;

            //  A window pops out and asks the player to choose a color ///
            Form setup = new Form();
            setup.Text = "Player Color";
            setup.Size = new Size(300, 260);
            setup.StartPosition = FormStartPosition.CenterScreen;

            CheckBox red = new CheckBox();
            red.Text = "Red";
            red.Location = new Point(30, 60);
            red.Checked = true;

            CheckBox yellow = new CheckBox();
            yellow.Text = "Yellow";
            yellow.Location = new Point(30, 90);

            CheckBox green = new CheckBox();
            green.Text = "Green";
            green.Location = new Point(30, 120);

            CheckBox blue = new CheckBox();
            blue.Text = "Blue";
            blue.Location = new Point(30, 150);

            // Gets the choosen option of the player and the not choosen into an array //
            CheckBox[] boxes = { red, yellow, green, blue };

            // Goes through the array //
            foreach (CheckBox box in boxes)
            {
                // Whenever a box gets any changes//
                box.CheckedChanged += (s, e) =>
                {
                    // Check what checkbox was clicked //
                    CheckBox clicked = s as CheckBox;

                    // Check if it's not bull and it was checked //
                    if (clicked != null && clicked.Checked)
                    {
                        // If the choosen option is valid, uncheck the others //
                        // So only one can be seleted // 
                        foreach (CheckBox other in boxes)
                        {
                            if (other != clicked)
                                other.Checked = false;
                        }
                    }
                };
            }

            // Button to start the game  //
            Button start = new Button();
            start.Text = "Start Game";
            start.Location = new Point(30, 180);
            start.Width = 200;


            // Following code makes sure that when starting the game, the player actually choose a color.
            // If not a show a message and show the other window again with the colors set up // 
            start.Click += (s, e) =>
            {
                if (!red.Checked && !yellow.Checked && !green.Checked && !blue.Checked)
                {
                    MessageBox.Show("Please Pick a Color.");
                    return;
                }

                isPlayer[1] = red.Checked;
                isPlayer[2] = yellow.Checked;
                isPlayer[3] = green.Checked;
                isPlayer[4] = blue.Checked;

                setup.Close();
            };

            setup.Controls.Add(red);
            setup.Controls.Add(yellow);
            setup.Controls.Add(green);
            setup.Controls.Add(blue);
            setup.Controls.Add(start);

            setup.ShowDialog();
        }

        //----------------------------------------------------------------------------------------------------Initialized Board
        private void LoadBoardIntoTableLayout()
        {
            tableLayoutPanel1.Controls.Clear(); // Loading boards to make sure the old one is gone
            tableLayoutPanel1.SuspendLayout();  // Stops board from being changed until all computation goes through to be faster and have the board load in one go

            for (int i = 0; i < 81; i++)
            {
                Panel cell = CreateCell(i);
                tableLayoutPanel1.Controls.Add(cell, i % 9, i / 9);
            }

            tableLayoutPanel1.ResumeLayout(); // Allows board to lad all at once
        }

        private void RefreshBoard()
        {
            foreach (Control c in tableLayoutPanel1.Controls)
            {
                if (!(c is Panel panel)) continue;
                int index = (int)panel.Tag;
                panel.BackColor = GetDisplayColor(index);

                if (panel.Controls.Count > 0 && panel.Controls[0] is Label label)
                    label.Text = GetDisplayText(index);

                panel.Invalidate(); // redraws victory borders
            }
        }

        // Text for the cells that is hardcoded
        private Panel CreateCell(int index)
        {
            Panel cell = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(1),
                Tag = index,
                BackColor = GetDisplayColor(index)
            };

            string displayText = GetDisplayText(index);

            Label label = new Label
            {
                Text = displayText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.Black,
                BackColor = Color.Transparent,
                Enabled = false
            };

            // Adding stuff for the labels, the actions of the players/AI, and the victory borders 
            cell.Controls.Add(label);
            cell.Click += Form1_Click;
            cell.Paint += victoryBoxBorders;

            return cell;
        }

        // Display the text / color at index of the board 
        private string GetDisplayText(int index)
        {
            int pieceColor = GetPieceColorAtBoardIndex(index);

            if (pieceColor != 0)
                return GetColorLetter(pieceColor);

            if (index == 0)
                return "Quit";
            else if (index == 7)
                return "Save";
            else if (index == 8)
                return "Load";
            else if (index == 40)
                return "Roll\nDice";
            else if (index == 18)
                return "→";      // Right 
            else if (index == 62)
                return "←";      // Left 
            else if (index == 74)
                return "↑";     // Up
            else if (index == 6)
                return "↓";    // Down
            else
                //return index.ToString(); // used to make deubbuging easier so u can see the empty values------- 
                return "";
        }

        // Letter for the color and place of the starting spots for the pieces 
        private string GetColorLetter(int color)
        {
            if (color == 1)
                return "R";
            else if (color == 2)
                return "Y";
            else if (color == 3)
                return "G";
            else if (color == 4)
                return "B";
            else
                return "";
        }

        private Color GetDisplayColor(int index)
        {
            int pieceColor = GetPieceColorAtBoardIndex(index);

            if (pieceColor != 0)
                return GetColorFromCode(pieceColor);

            return GetColorFromCode(boardCells[index].Color);
        }

        private void victoryBoxBorders(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel == null) return;

            int index = (int)panel.Tag;
            var cell = boardCells[index];

            if (cell.OutlineColor != Color.Empty)
            {
                using (Pen pen = new Pen(cell.OutlineColor, 4))
                {
                    Rectangle rect = panel.ClientRectangle;
                    rect.Inflate(-2, -2);
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }
        }

        // Used when assigning the background color of the cells it takes the color code and returns the actual color to be used 
        private Color GetColorFromCode(int colorValue)
        {
            if (colorValue == 1)
                return Color.Red;
            else if (colorValue == 2)
                return Color.Yellow;
            else if (colorValue == 3)
                return Color.Green;
            else if (colorValue == 4)
                return Color.Blue;
            else if (colorValue == 5)
                return Color.Gray;
            else if (colorValue == 6)
                return Color.White;
            else
                return Color.Gray;
        }


        //Actions of the Player 
        // EXIT, ROLL, SAVE, LOAD

        private void Form1_Click(object sender, EventArgs e)
        {
            Panel clickedPanel = sender as Panel;
            if (clickedPanel == null) return; // Makes sure something was pressed 

            int index = (int)clickedPanel.Tag;

            if (index == 0) // Exists and stops the program when quit is pressed at index 0
            {
                SoundPlayer soundQuit = new SoundPlayer("Sounds/universfield-computer-mouse-click-352734.wav");
                soundQuit.PlaySync();
                Application.Exit();
                return;
            }
            else if (index == 7) // Saves game when saved is pressed at index 7
            {
                SaveGame();
                SoundPlayer soundSave = new SoundPlayer("Sounds/universfield-computer-mouse-click-352734.wav");
                soundSave.PlaySync();
                MessageBox.Show("Game saved", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (index == 8) // Load when the load is pressed at index 8
            {     
                
                SoundPlayer soundLoad = new SoundPlayer("Sounds/universfield-computer-mouse-click-352734.wav");
                soundLoad.PlaySync();
                LoadGame();
                LoadBoardIntoTableLayout();

                MessageBox.Show("Game loaded", "Load", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (index == 40) // Index at 40 is pressed, call the function to roll the dice
            {
                RollDiceForPlayer();
                return;
            }

            // If it's not the player current turn based on the color, message about that it is the AI next
            if (!isPlayer[currentTurnColor])
            {
                MessageBox.Show("It is the AI's turn.");
                return;
            }

            // Makes sure that the player rolls
            if (!hasRolled)
            {
                MessageBox.Show("Roll the dice first.");
                return;
            }


            // Makes sure that the player clicks one of their own color pieces at i
            int clickedColor = GetPieceColorAtBoardIndex(index);

            if (clickedColor != currentTurnColor)
            {
                MessageBox.Show("Click one of your own pieces.");
                return;
            }

            // Gets index of the piece that the player is tring to get. If the IF statement triggers, then 
            // they are trying to get a piece from the starting array that is maybe not there anymore
            int pieceIndex = GetPieceIndexAtBoardIndex(currentTurnColor, index);

            if (pieceIndex == -1)
            {
                MessageBox.Show("Piece not found.");
                return;
            }

            // Looks into the current color, index, and current dice roll to make sure it is an allowed move for the player 
            bool moved = TryMovePiece(currentTurnColor, pieceIndex, currentDiceRoll);

            if (!moved)
            {
                MessageBox.Show("That move is not allowed.");
                return;
            }

            // Loads board to continue and safety
            RefreshBoard();

            // Checks if the current color has won after everymove
            if (CheckWinner(currentTurnColor))
            {
                backgroundMusic.controls.stop();

                SoundPlayer soundWin = new SoundPlayer("Sounds/puyopuyomegafan1234-winner-game-sound-404167.wav");
                soundWin.Play();
                MessageBox.Show(GetColorName(currentTurnColor) + " wins!!!");

                foreach(Control c in tableLayoutPanel1.Controls)
                {
                    if((int)c.Tag == 40)
                    {
                        c.Enabled = false;
                    }
                }

                return;
            }

            // End the current turn of the color 
            EndTurn();
        }

        // Action of the Player when pressing ROLL  -----------
        private void RollDiceForPlayer()
        {
            // If it's not the player color turn, then it's the AI's
            if (!isPlayer[currentTurnColor])
            {
                AITurn();
                return;
            }

            // Checks if the player wants to re-roll, but can't because they need to move a piece first
            if (hasRolled)
            {
                MessageBox.Show("You already rolled. Move a piece.");
                return;
            }

            // The value of the current dice roll, and makes sure that the player has rolled, making the bool val true
            currentDiceRoll = dice.Next(1, 7);
            hasRolled = true;

            // Shows who rolled what 
            SoundPlayer rollDice = new SoundPlayer("Sounds/freesound_community-dice_roll-96878.wav");
            rollDice.Play();
            MessageBox.Show(GetColorName(currentTurnColor) + " rolled a " + currentDiceRoll);


            // Cheeking if any move is possible, like not rolling a 6 to get out 
            List<int> validMoves = GetValidPieces(currentTurnColor, currentDiceRoll);

            // If no valid moves, then message about not being able to leave the starting area.
            if (validMoves.Count == 0)
            {
                SoundPlayer soundWrongPlayerMove = new SoundPlayer("Sounds/lesiakower-error-mistake-sound-effect-incorrect-answer-437420.wav");
                soundWrongPlayerMove.Play();
                MessageBox.Show(GetColorName(currentTurnColor) + " (Player) has no valid moves. Turn skipped.");
                EndTurn();
                return;
            }
        }

        // TURN for the AI
        private void AITurn()
        {
            // Roll for AI
            currentDiceRoll = dice.Next(1, 7);
            hasRolled = true;
            SoundPlayer AiRollDice = new SoundPlayer("Sounds/freesound_community-dice_roll-96878.wav");
            AiRollDice.Play();
            MessageBox.Show(GetColorName(currentTurnColor) + " (AI) rolled a " + currentDiceRoll);

            int[] dbgPieces = GetPiecesArray(currentTurnColor);
            

            // Check if any moves are possible
            List<int> validMoves = GetValidPieces(currentTurnColor, currentDiceRoll);

            if (validMoves.Count == 0)
            {
                SoundPlayer soundAiWrongMove = new SoundPlayer("Sounds/lesiakower-error-mistake-sound-effect-incorrect-answer-437420.wav");
                soundAiWrongMove.Play();
                MessageBox.Show(GetColorName(currentTurnColor) + " (AI) has no valid moves. Turn skipped.");
                hasRolled = false;
                currentDiceRoll = 0;
                EndTurn();
                return;
            }

            // Build board position array from his progress system for AI_Player
            int[] pieces = GetPiecesArray(currentTurnColor);
            int[] boardPositions = new int[4];
            int[] progressValues = new int[4]; // for ai to see furthest
            for (int i = 0; i < 4; i++)
            {
                progressValues[i] = pieces[i]; // raw progress, -1 if home
                if (pieces[i] == -1)
                    boardPositions[i] = -1; // still at home
                else
                    boardPositions[i] = GetBoardIndexFromProgress(currentTurnColor, pieces[i]);
            }


            var opponentPositions = new HashSet<int>();
            for (int c = 1; c <= 4; c++)
            {
                if (c == currentTurnColor) continue;
                int[] oppPieces = GetPiecesArray(c);
                for (int i = 0; i < oppPieces.Length; i++)
                {
                    if (oppPieces[i] == -1) continue;
                    opponentPositions.Add(GetBoardIndexFromProgress(c, oppPieces[i]));
                }
            }

            // exitIndices and entryIndices per color (index 0 unused, colors are 1-4)
            int[] exitIndices = { 0, 23, 5, 11, 17 };
            int[] entryIndices = { 0, 0, 6, 12, 18 };

            // Build AI instance for current color
            var allStarts = new Dictionary<int, int[]>
            {
                { 1, redStartingArray },
                { 2, yellowStartingArray },
                { 3, greenStartingArray },
                { 4, blueStartingArray }
            };

            AI_Player ai = new AI_Player(
                colorCode: currentTurnColor,
                mainBoard: mainPlayBoardArray,
                startZone: GetStartingArray(currentTurnColor),
                victoryLane: GetVictoryArray(currentTurnColor),
                colorStartZones: allStarts,
                victoryEntryIndex: exitIndices[currentTurnColor],
                startIndex: entryIndices[currentTurnColor]
            );

            // Ask AI which piece to move
            int chosenPiece = ai.ChoosePiece(currentDiceRoll, boardPositions, opponentPositions, progressValues);
           
            // Fallback to first valid move if AI returns -1
            if (chosenPiece == -1 || !validMoves.Contains(chosenPiece))
                chosenPiece = validMoves[0];

            // Execute move using his existing movement system
            bool moved = TryMovePiece(currentTurnColor, chosenPiece, currentDiceRoll);

            RefreshBoard();

            if (CheckWinner(currentTurnColor))
            {

                backgroundMusic.controls.stop();

                SoundPlayer soundAIWin = new SoundPlayer("Sounds/puyopuyomegafan1234-winner-game-sound-404167.wav");
                soundAIWin.Play();
                MessageBox.Show(GetColorName(currentTurnColor) + " (AI) wins!!!");

                foreach(Control c in tableLayoutPanel1.Controls)
                {
                    if((int)c.Tag == 40)
                    {
                        c.Enabled = false;
                    }
                }
                return;
            }

            EndTurn();
        }

        // Cheks all 4 pieces of the color and returns the ones possible to move
        private List<int> GetValidPieces(int color, int roll)
        {
            List<int> validPieces = new List<int>();
            int[] pieces = GetPiecesArray(color);

            // Through each of the pieces that can possibly move 
            for (int i = 0; i < pieces.Length; i++)
            {
                if (CanMovePiece(color, i, roll))
                    validPieces.Add(i);
            }

            return validPieces;
        }

        // Method to check if a piece can move 
        private bool CanMovePiece(int color, int pieceIndex, int roll)
        {
            // Gets the array pieces of the current color into another array
            int[] pieces = GetPiecesArray(color);

            int currentProgress = pieces[pieceIndex]; // Gets where the piece (start, main board, or on the finish area)

            int newProgress;

            // If it's still at the starting line, it can't move until the roll value is a 6
            if (currentProgress == -1)
            {
                if (roll != 6)
                    return false;

                newProgress = 0; // If rolled a 6, the piece moves out to the first board spot available
            }
            else
            {
                newProgress = currentProgress + roll; // If it's out in the board, move towards the finish area
            }                                         // by the dice amount

            // Prevents a piece from going over the finish line 
            if (newProgress > 27)
            {
                return false;
            }

            // Make progress number into the real board square number 
            int targetBoardIndex = GetBoardIndexFromProgress(color, newProgress);

            // Invalid move if the square doesn't exist
            if (targetBoardIndex == -1)
            {
                return false;
            }

            // Cheeking if there is a piece there on thae square board at index
            int occupantColor = GetOccupantColorOnBoard(targetBoardIndex);

            // Cannot put another piece on your own color 
            if (occupantColor == color)
            {
                return false;
            }

            // If no weird scenarios, then move is valid
            return true;
        }

        // Moving the piece 
        private bool TryMovePiece(int color, int pieceIndex, int roll)
        {
            // Cheking if the move is valid 
            if (!CanMovePiece(color, pieceIndex, roll))
            {
                return false;
            }

            // Gets the pieces of the current color and the progress
            int[] pieces = GetPiecesArray(color);
            int currentProgress = pieces[pieceIndex];

            int newProgress;

            if (currentProgress == -1)
            {
                if (roll != 6) // Home base check rolling 6
                {
                    return false;
                }

                newProgress = 0;
            }
            else
            {
                newProgress = currentProgress + roll; // Calculates where the piece should go
            } // So, where you are right now PLUS the value of the roll.

            int targetBoardIndex = GetBoardIndexFromProgress(color, newProgress); // Looking where the piece will land at the square index on the board
            int occupantColor = GetOccupantColorOnBoard(targetBoardIndex); // Checking if that sqaure spit 

            // If color piece at the square is not empty and not the PLAYER color, 
            if (occupantColor != 0 && occupantColor != color)
            {
                SendEnemyPieceHome(occupantColor, targetBoardIndex); // Sned the other color at the start
            }

            pieces[pieceIndex] = newProgress; // Updating the piece's position at the board

            SoundPlayer soundMove = new SoundPlayer("Sounds/freesound_community-moving-with-table-105076.wav");
            soundMove.Play();  
            return true;
        }

        // Method when interacting with other colors 
        private void SendEnemyPieceHome(int enemyColor, int boardIndex)
        {
            // For enemy pieces
            int[] enemyPieces = GetPiecesArray(enemyColor);

            // Looping through all 4 enemy pieces 
            for (int i = 0; i < enemyPieces.Length; i++)
            {
                if (enemyPieces[i] == -1) continue; // skip pieces already home

                int enemyBoardIndex = GetBoardIndexFromProgress(enemyColor, enemyPieces[i]);

                if (enemyBoardIndex == boardIndex)
                {
                    enemyPieces[i] = -1;
                    SoundPlayer soundSentHome = new SoundPlayer("Sounds/freesound_community-jump-sound-14839.wav");
                    soundSentHome.Play();
                    MessageBox.Show(GetColorName(enemyColor) + " piece sent home!");
                    return;
                }
            }
        }

        // Converts progress into real board square number
        private int GetBoardIndexFromProgress(int color, int progress)
        {
            if (progress >= 0 && progress <= 23) // Piece on the main board
            {
                int position = (entryProgress[color] + progress) % 24; // The board position loop for each color
                return mainPlayBoardArray[position]; // returning the square number of the board
            }

            if (progress >= 24 && progress <= 27) // Current piece is at the victory lane area.
            {
                int victoryIndex = progress - 24; // The four victory positions for the 4 pieces of the color 
                return GetVictoryArray(color)[victoryIndex]; // Returning the color's victory lane square
            }

            return -1; // Return -1 if invalid progress
        }

        // Checks what color is at the current square at i
        private int GetPieceColorAtBoardIndex(int boardIndex)
        {
            for (int color = 1; color <= 4; color++)
            {
                int[] pieces = GetPiecesArray(color);
                for (int i = 0; i < pieces.Length; i++)
                {
                    if (pieces[i] == -1)
                    {
                        // check home position
                        if (GetHomeIndexForPiece(color, i) == boardIndex)
                            return color;
                    }
                    else
                    {
                        // check board position
                        if (GetBoardIndexFromProgress(color, pieces[i]) == boardIndex)
                            return color;
                    }
                }
            }
            return 0;
        }

        private int GetOccupantColorOnBoard(int boardIndex)
        {
            for (int color = 1; color <= 4; color++)
            {
                int[] pieces = GetPiecesArray(color);
                for (int i = 0; i < pieces.Length; i++)
                {
                    if (pieces[i] == -1) continue;
                    if (GetBoardIndexFromProgress(color, pieces[i]) == boardIndex)
                        return color;
                }
            }
            return 0;
        }

        // Finds the piece that was clocked 
        private int GetPieceIndexAtBoardIndex(int color, int boardIndex)
        {
            int[] pieces = GetPiecesArray(color); // Get currrent color's pieces

            for (int i = 0; i < pieces.Length; i++) // Checking all 4 pieces 
            {
                int pieceBoardIndex;

                // Locating the piece at home start
                if (pieces[i] == -1)
                {
                    pieceBoardIndex = GetHomeIndexForPiece(color, i);
                }
                else
                {
                    pieceBoardIndex = GetBoardIndexFromProgress(color, pieces[i]); // Else at the game board lane
                }

                if (pieceBoardIndex == boardIndex) // Returns the piece number (0 - 3)
                {
                    return i;
                }
            }

            return -1; // No matching piece found
        }

        private int GetHomeIndexForPiece(int color, int pieceIndex) // Locating where a piece is at in the start area 
        {
            int[] homeArray = GetStartingArray(color); // Starting position array
            return homeArray[pieceIndex]; // Returns suqare for a piece of it
        }

        private int[] GetPiecesArray(int color) // Returns the pieces of the color based on the number
        {

            switch (color)
            {
                case 1:
                    return redPieces;
                case 2:
                    return yellowPieces;
                case 3:
                    return greenPieces;
                case 4:
                    return bluePieces;
            }

            return null; // Null if no correct color 

        }

        // Gets the arrays of the colors at their start positions 
        private int[] GetStartingArray(int color)
        {

            switch (color)
            {
                case 1:
                    return redStartingArray;
                case 2:
                    return yellowStartingArray;
                case 3:
                    return greenStartingArray;
                case 4:
                    return blueStartingArray;
            }

            return null; // Null if no color or trying to acces 5th color
        }

        // Gettign the array of the victory area lane of the current color 
        private int[] GetVictoryArray(int color)
        {

            switch (color)
            {
                case 1:
                    return redVictoryArray;
                case 2:
                    return yellowVictoryArray;
                case 3:
                    return greenVictoryArray;
                case 4:
                    return blueVictoryArray;
            }

            return null; // Null if no correct color of an unormal array
        }

        private void EndTurn()
        {
            // Roll a 6 = roll again, don't advance turn
            if (currentDiceRoll == 6)
            {
                hasRolled = false;
                currentDiceRoll = 0;
                MessageBox.Show(GetColorName(currentTurnColor) + " rolled a 6 - roll again!");
                if (!isPlayer[currentTurnColor])
                    AITurn();  // AI gets its bonus roll automatically
                return;  // don't advance currentTurnColor
            }
            // Reseting dice for the next player 
            hasRolled = false;
            currentDiceRoll = 0;
            currentTurnColor++; // Moving to next plauer 

            if (currentTurnColor > 4) // Looping throuhg all 4 players, not 5
            {
                currentTurnColor = 1;
            }


            ShowTurnMessage(); // Showing who's turn is it

            if (!isPlayer[currentTurnColor]) // If it's not the PLAYER'S turn it's the Ai's
            {
                AITurn();
            }
        }

        // Showing who's turn is it and the color 
        private void ShowTurnMessage()
        {
            string playerType;

            if (isPlayer[currentTurnColor])
                playerType = "Player";
            else
                playerType = "AI";

            MessageBox.Show("Current Turn: " + GetColorName(currentTurnColor) + " (" + playerType + ")");
        }

        // Cheeking for the winner 
        private bool CheckWinner(int color)
        {
            int[] pieces = GetPiecesArray(color); // Getting the pieces of that color 

            foreach (int piece in pieces)
            {
                if (piece < 24)
                    return false; // if not all pieces are in the victory area, no win
            }

            return true; // Only true if all pieces are in the victory area
        }

        // Makes the numbers into readable names of colors for messafes and UI
        private string GetColorName(int code)
        {

            switch (code)
            {
                case 1:
                    return "Red";
                case 2:
                    return "Yellow";
                case 3:
                    return "Green";
                case 4:
                    return "Blue";
                case 5:
                    return "Gray";
                case 6:
                    return "White";
            }

            return null; // If no color exists
        }

        private void SaveGame()
        {
            using (StreamWriter sw = new StreamWriter("savegame.txt"))
            {
                sw.WriteLine(currentTurnColor);
                sw.WriteLine(currentDiceRoll);
                sw.WriteLine(hasRolled);
                // save piece progress for all 4 colors
                sw.WriteLine(string.Join(",", redPieces));
                sw.WriteLine(string.Join(",", yellowPieces));
                sw.WriteLine(string.Join(",", greenPieces));
                sw.WriteLine(string.Join(",", bluePieces));
                // save which color is player
                for (int i = 1; i <= 4; i++)
                    sw.WriteLine(isPlayer[i]);
            }
        }

        private void LoadGame()
        {
            if (!File.Exists("savegame.txt"))
            {
                MessageBox.Show("No save file found.", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] lines = File.ReadAllLines("savegame.txt");

            if (lines.Length < 11)
            {
                MessageBox.Show("Save file corrupted.", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            currentTurnColor = int.Parse(lines[0]);
            currentDiceRoll = int.Parse(lines[1]);
            hasRolled = bool.Parse(lines[2]);

            redPieces = lines[3].Split(',').Select(int.Parse).ToArray();
            yellowPieces = lines[4].Split(',').Select(int.Parse).ToArray();
            greenPieces = lines[5].Split(',').Select(int.Parse).ToArray();
            bluePieces = lines[6].Split(',').Select(int.Parse).ToArray();

            for (int i = 1; i <= 4; i++)
                isPlayer[i] = bool.Parse(lines[6 + i]);
        }
    }
}
