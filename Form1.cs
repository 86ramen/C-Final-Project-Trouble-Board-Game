
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace Trouble_Group_8_Project
{
    public partial class Form1 : Form
    {
        private AI_Player Ai;

        private (int Color, bool Functional, Color OutlineColor)[] boardCells;
        private Random dice = new Random();
        private int[] mainPlayBoardArray;          // All functional slots
        private int[] redStartingArray;       // Red start positions
        private int[] yellowStartingArray;    // Yellow start positions
        private int[] greenStartingArray;     // Green start positions
        private int[] blueStartingArray;      // Blue start positions
        private int[] redVictoryArray;        // Red victory lane outlines
        private int[] yellowVictoryArray;     // Yellow victory lane outlines
        private int[] greenVictoryArray;      // Green victory lane outlines
        private int[] blueVictoryArray;       // Blue victory lane outlines
        
        public Form1()
        {
            InitializeComponent();
            ConfigureTableLayoutPanel();
            InitializeBoard();
            LoadBoardIntoTableLayout();
        }

        //hard coded so we Dont muck up the board by acadentaly changing the Form1.cs[design]
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
        //-----------------------------------------------------------------------------------------------------board game size
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

            // Functional boxes 
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
            //main game board array
            mainPlayBoardArray = FUNCTIONALBOARDVALUES;

            // Starting position arrays
            redStartingArray = new int[] { 1, 2, 3, 4 };
            yellowStartingArray = new int[] { 17, 26, 35, 44 };    
            blueStartingArray = new int[] { 36, 45, 54, 63 };        
            greenStartingArray = new int[] { 76, 77, 78, 79 };

            // Victory arrays 
            redVictoryArray = new int[] { 20, 21, 22, 23 };
            yellowVictoryArray = new int[] { 24, 33, 42, 51 };
            greenVictoryArray = new int[] { 60, 59, 58, 57 };
            blueVictoryArray = new int[] { 56, 47, 38, 29 };
        }
        //------------------------------------------------------------------------------------------------------------------Board has been initialized
        private void LoadBoardIntoTableLayout()
        {
            tableLayoutPanel1.Controls.Clear();// for loading boards to make sure that the old board is gone
            tableLayoutPanel1.SuspendLayout();//stops the board from being changed until all the computation has gone through to make it faster and have the board load in one go

            for (int i = 0; i < 81; i++)
            {
                Panel cell = CreateCell(i);
                tableLayoutPanel1.Controls.Add(cell, i % 9, i / 9);
            }

            tableLayoutPanel1.ResumeLayout();// allowes the board to load all at once
        }

        //text is hard coded as well 
        private Panel CreateCell(int index)
        {
            var cell = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(1),
                Tag = index,
                BackColor = GetColorFromCode(boardCells[index].Color)
            };

            string displayText = GetDisplayText(index);

            var label = new Label
            {
                Text = displayText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 26, FontStyle.Bold),
                ForeColor = SystemColors.ActiveCaptionText,   // text color wont work, if enabled is true clicking stops working
                BackColor = Color.Transparent,
                Enabled = false
            };

            cell.Controls.Add(label);
            cell.Click += Form1_Click;
            cell.Paint += victoryBoxBorders;

            return cell;
        }

        private string GetDisplayText(int index)
        {   
            if (index == 0)
                return "Quit";
            else if (index == 7)
                return "Save";
            else if (index == 8)
                return "Load";
            else if (index == 40)
                return "Roll Dice";
            else if (index == 18)
                return "→";      // Right
            else if (index == 62)
                return "←";      // Left
            else if (index == 74)
                return "↑";      // Up
            else if (index == 6)
                return "↓";      // Down
            else
                return index.ToString(); // used to make debugging easier so u can see the empty values~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //return "";// will be used for actual implementation
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

        // is used when assigning the background color of the cells it takes the color code and returns the actual color to be used
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


        private void Form1_Click(object sender, EventArgs e)
        {
            PrintBoardArray2D();

            Panel clickedPanel = sender as Panel;
            if (clickedPanel == null) return; // makes sure something was pressed

            int index = (int)clickedPanel.Tag;

            if (index == 0) // exists and stops the program when quit is pressed at index 0
            {
                Application.Exit();
                return;
            }
            else if (index == 7) // when the save game button is pressed at inderx 7
            {
                SaveGame();
                MessageBox.Show("Game saved", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (index == 8) //when load game is pressed at index 8
            {
                LoadGame();
                LoadBoardIntoTableLayout();
                MessageBox.Show("Game loaded", "Load", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (index == 40) // dice role 
            {
                int roll = dice.Next(1, 7);
                MessageBox.Show($"You rolled a {roll}!", "Dice Roll", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var cell = boardCells[index];
            string colorName = GetColorName(cell.Color);
            ///////////////////////////////////logic for when the player makes a move
            ///




            //message box is for debugging
            MessageBox.Show(
                $"Index: {index}\nColor: {colorName} (code {cell.Color})\nFunctional: {cell.Functional}",
                "Cell Info",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            ///////////////////////////////////////////////////////////////////////////////
        }

        //for testing and debuging makes sure that all valkues are properly reported
        private string GetColorName(int code)
        {
            if (code == 1)
                return "Red";
            else if (code == 2)
                return "Yellow";
            else if (code == 3)
                return "Green";
            else if (code == 4)
                return "Blue";
            else if (code == 5)
                return "Gray";
            else if (code == 6)
                return "White";
            else
                return "Unknown";
        }

        private void SaveGame()
        {
            ;//save game goes here-----------------------------------------------------------------------------------------------------------------------------
        }

        private void LoadGame()
        {

            string[] lines = File.ReadAllLines("savegame.txt");
            if (lines.Length != 81)
            {
                MessageBox.Show("Save file corrupted.", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

          ;// load game goes here------------------------------------------------------------------------------------------------------------------------------
        }

        // used for debugging--------------------------------------------------------------
        private void PrintBoardArray2D()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("=== Board Array (Colour Codes) ===");
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    int index = row * 9 + col;
                    int code = boardCells[index].Color;
                    sb.Append(code + " ");
                }
                sb.AppendLine();
            }
            MessageBox.Show(sb.ToString(), "2D Board Array", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        
        //makes it so whenn you change the values of the board that its refreshed so the correct colors are reflected, should be done at the end of every interaction logic
        private void RefreshBoardColors()
        {
            foreach (Control c in tableLayoutPanel1.Controls)
            {
                if (c is Panel panel)
                {
                    int index = (int)panel.Tag;
                    panel.BackColor = GetColorFromCode(boardCells[index].Color);
                }
            }
        }
        
        //-----------------------------------------------------------------------------------------------------------ALL LOGIC THAT DOESNT DIRECTLY MAKE THE BOARD OR IOTS INTERACTIONS GOES BELOW HERE


    }


}