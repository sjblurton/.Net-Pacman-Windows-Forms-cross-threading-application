﻿
using System.Windows.Forms;
using System.Drawing;
using System;
using PacmanWinForms;



namespace PacmanWinForms
{
    public class PacmanBoard
    {
        public int Rows;
        public int Cols;
        public Color BgColor;

        public static float cellHeight;
        public static float cellWidth;
        public static float modelCellHeight;
        public static float modelCellWidth;
        private int state = 1;

        private delegate void GhostPaint(Point point, Direction D, GhostColor color, bool sprite1, GhostState state);
        private delegate void FruitPaint(Point point, int picIndex);
        private delegate void MessagePaint(bool visible, string message1, string message2);
        private delegate void BonusScorePaint(Point p, int points, int lblIndex);

        private Panel pnl;
        private Graphics g;

        private PictureBox picRedGhost;
        private PictureBox picBlueGhost;
        private PictureBox picPinkGhost;
        private PictureBox picYellowGhost;
        private PictureBox picFruit;
        private Label lblInfo;
        private Label lblInfoMsg;

        private Label lblBonus1;
        private Label lblBonus2;
        private Label lblBonus3;
        private Label lblBonus4;
        private Label lblBonus5;

        public PacmanBoard(Panel pnl, int rows = 62, int cols = 56, Color? bgColor = null)
        {
            // Color? <=> nullable <Color>
            this.Rows = rows;
            this.Cols = cols;
            this.BgColor = bgColor ?? Color.Black;
            this.pnl = pnl;


            //Resize();
        }

        public void Resize()
        {
            cellHeight = pnl.Height / (float)Rows;
            cellWidth = pnl.Width / (float)Cols;
            modelCellHeight = pnl.Height / (float)31;
            modelCellWidth = pnl.Width / (float)28;
            g = pnl.CreateGraphics();

        }

        public void Clear()
        {
            lock (this)
            {
                g.Clear(BgColor);
            }
        }
        
        public void DrawRect(Point p, Color col)
        {
            Brush b = new SolidBrush(col);
            lock (this)
            {
                g.FillRectangle(b, p.X * cellWidth, p.Y * cellHeight , cellWidth, cellHeight);
            }
        }

        public void DrawDoor(Point p, Color col)
        {
            Brush b = new SolidBrush(col);
            lock (this)
            {
                g.FillRectangle(b, p.X * cellWidth, p.Y * cellHeight + cellHeight/4, cellWidth, cellHeight/2);
            }
        }

        public void DrawDot(Point p, Color col)
        {
            float dotWidth = 2 * cellWidth / 5;
            float dotHeight = 2 * cellHeight / 5;
            Brush b = new SolidBrush(col);
            lock (this)
            {
                g.FillRectangle(b, p.X * cellWidth - dotWidth/2, p.Y * cellHeight - dotHeight/2, dotWidth, dotHeight);
            }
        }

        public void DrawBonus(Point p, Color col, int bonusState)
        {
            float dotWidth; 
            float dotHeight;
            Brush b = new SolidBrush(col);
            if (bonusState == 1)
            {
                dotWidth = cellWidth; dotHeight = cellHeight;
            }
            else if (bonusState == 2)
            {
                dotWidth =(float)1.25*cellWidth; dotHeight = (float)1.25 * cellHeight;
            }
            else if (bonusState == 3)
            {
                dotWidth = (float)2 * cellWidth; dotHeight = (float)2 * cellHeight;
            }
            else
            {
                dotWidth = (float)1.25 * cellWidth; dotHeight = (float)1.25 * cellHeight;
            }
            lock (this)
            {
                g.FillEllipse(b, p.X * cellWidth - dotWidth / 2, p.Y * cellHeight - dotHeight / 2, dotWidth, dotHeight);
            }
        }

        public void ClearBonus(Point p)
        {
            lock (this)
            {
                g.FillRectangle(new SolidBrush(Color.Black), p.X * cellWidth - (float)1.5*cellWidth + 5, p.Y * cellHeight - (float)1.5 * cellHeight +5, 3 * cellWidth -10, 3 * cellHeight -10);
            }
        }

        public void PrintMessage(bool visible, string msg1, string msg2)
        {
            pnl.Invoke(new MessagePaint(messageApears), visible, msg1, msg2);
        }

        public void PrintBonus(Point p, int score, int lblIndex)
        {
            pnl.Invoke(new BonusScorePaint(bonusPrint), p, score, lblIndex);
        } 

        public void GhostMove(Point P, Direction D, GhostColor color, bool sprite1, GhostState state)
        {
            pnl.Invoke(new GhostPaint(changePic), P, D, color, sprite1, state);
        }

        public void DrawFruit(Point P, int index)
        {
            pnl.Invoke(new FruitPaint(changeFruitPos), P, index);
        }

        public void CleanFruit(Point P, int index)
        {
            pnl.Invoke(new FruitPaint(CleanFruitPos), P, index);
        }

        private void CleanFruitPos(Point P, int index)
        {
            if (picFruit != null) pnl.Controls.Remove(picFruit);

        }

        public void CleanBonus(Point P, int score, int lblIndex)
        {
            pnl.Invoke(new BonusScorePaint(cleanBonus), P, score, lblIndex);
        }

        private void cleanBonus(Point P, int score, int lblIndex)
        {
            switch (lblIndex)
            {
                case 1:
                    if (lblBonus1 != null) pnl.Controls.Remove(lblBonus1);
                    break;
                case 2:
                    if (lblBonus2 != null) pnl.Controls.Remove(lblBonus2);
                    break;
                case 3:
                    if (lblBonus3 != null) pnl.Controls.Remove(lblBonus3);
                    break;
                case 4:
                    if (lblBonus4 != null) pnl.Controls.Remove(lblBonus4);
                    break;
                case 5:
                    if (lblBonus5 != null) pnl.Controls.Remove(lblBonus5);
                    break;
                default:
                    if (lblBonus1 != null) pnl.Controls.Remove(lblBonus1);
                    if (lblBonus2 != null) pnl.Controls.Remove(lblBonus2);
                    if (lblBonus3 != null) pnl.Controls.Remove(lblBonus3);
                    if (lblBonus4 != null) pnl.Controls.Remove(lblBonus4);
                    if (lblBonus5 != null) pnl.Controls.Remove(lblBonus5);
                    break;
            }
        }

        private void bonusPrint(Point P, int score, int lblIndex)
        {

            Label lblTemp = new Label();
            lblTemp.AutoSize = true;
            lblTemp.Font = new Font("Courier New", 13F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(161)));
            lblTemp.BackColor = Color.Transparent;
            lblTemp.ForeColor = Color.MediumSeaGreen;
            lblTemp.Location = new Point((int)(P.X * cellWidth ), (int)((P.Y -1) * cellHeight));
            lblTemp.Size = new Size((int)(4 * cellWidth) - 13, (int)(4 * cellHeight) - 13);
            lblTemp.Text = score.ToString();

            switch (lblIndex)
            {
                case 1:
                    if (lblBonus1 != null) pnl.Controls.Remove(lblBonus1);
                    lblTemp.ForeColor = Color.MediumSeaGreen;
                    lblBonus1 = lblTemp;
                    pnl.Controls.Add(lblBonus1);
                    break;
                case 2:
                    if (lblBonus2 != null) pnl.Controls.Remove(lblBonus2);
                    lblTemp.ForeColor = Color.Lime;
                    lblBonus2 = lblTemp;
                    pnl.Controls.Add(lblBonus2);
                    break;
                case 3:
                    if (lblBonus3 != null) pnl.Controls.Remove(lblBonus3);
                    lblTemp.ForeColor = Color.Chartreuse; 
                    lblBonus3 = lblTemp;
                    pnl.Controls.Add(lblBonus3);
                    break;
                case 4:
                    if (lblBonus4 != null) pnl.Controls.Remove(lblBonus4);
                    lblTemp.ForeColor = Color.DarkTurquoise;
                    lblBonus4 = lblTemp;
                    pnl.Controls.Add(lblBonus4);
                    break;
                case 5:
                    if (lblBonus5 != null) pnl.Controls.Remove(lblBonus5);
                    lblTemp.ForeColor = Color.DeepPink;
                    lblBonus5 = lblTemp;
                    pnl.Controls.Add(lblBonus5);
                    break;
                default:
                    if (lblBonus1 != null) pnl.Controls.Remove(lblBonus1);
                    lblTemp.ForeColor = Color.Aqua;
                    lblBonus1 = lblTemp;
                    pnl.Controls.Add(lblBonus1);
                    break;
            }
            
        }
        
        private void messageApears(bool visible, string msg1, string msg2)
        {
            if (lblInfo != null) pnl.Controls.Remove(lblInfo);
            if (lblInfoMsg != null) pnl.Controls.Remove(lblInfoMsg);

            lblInfo = new Label();
            lblInfoMsg = new Label();

            lblInfo.Text = msg1;

            lblInfoMsg.Text = msg2;

            lblInfo.AutoSize = true;
            lblInfo.Font = new Font("Courier New", 18F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(161)));
            lblInfo.BackColor = Color.Transparent;
            lblInfo.ForeColor = Color.Red;
            lblInfo.Location = new Point((pnl.Width / 2) - 80, (pnl.Height - lblInfo.Height) / 2 + 30) ;
            lblInfo.Size = new Size();
            lblInfo.Visible = visible;

            lblInfoMsg.AutoSize = true;
            lblInfoMsg.Font = new Font("Courier New", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(161)));
            lblInfoMsg.BackColor = Color.Transparent;
            lblInfoMsg.ForeColor = Color.Red;
            lblInfoMsg.Size = new Size(150, 20);
            lblInfoMsg.Location = new Point((pnl.Width ) / 2 - 175, (pnl.Height) / 2 + 40);
            lblInfoMsg.Visible = visible;

           

            if (visible)
            {
                pnl.Controls.Add(lblInfoMsg);
                pnl.Controls.Add(lblInfo);
            }
            else
            {

                pnl.Controls.Remove(lblInfo);
                pnl.Controls.Remove(lblInfoMsg);
            }

        }

        private void changeFruitPos(Point P, int index)
        {
            if (picFruit != null) pnl.Controls.Remove(picFruit);
            picFruit = new PictureBox();
            picFruit.BackgroundImageLayout = ImageLayout.Zoom;
            picFruit.Location = new Point((int)(P.X * cellWidth + 11), (int)(P.Y * cellHeight + 11));
            picFruit.Size = new Size((int)(4 * cellWidth) - 13, (int)(4 * cellHeight) - 13);
            switch (index)
            {
                case 1:
                    picFruit.BackgroundImage = Properties.Resources.Cherry;
                    break;
                case 2:
                    picFruit.BackgroundImage = Properties.Resources.Strawberry;
                    break;
                case 3:
                    picFruit.BackgroundImage = Properties.Resources.orange;
                    break;
                case 4:
                    picFruit.BackgroundImage = Properties.Resources.Apple;
                    break;
                case 5:
                    picFruit.BackgroundImage = Properties.Resources.Melon;
                    break;
                case 6:
                    picFruit.BackgroundImage = Properties.Resources.Galaxian_Boss;
                    break;
                case 7:
                    picFruit.BackgroundImage = Properties.Resources.Bell;
                    break;
                case 8:
                    picFruit.BackgroundImage = Properties.Resources.Key;
                    break;
                default:
                    picFruit.BackgroundImage = Properties.Resources.Strawberry;
                    break;
            }
            pnl.Controls.Add(picFruit);

        }

        private Bitmap findImage(Direction D, GhostColor color, bool sprite1, GhostState state)
        {
            if (state == GhostState.BONUSEND)
            {
                if (sprite1) return new Bitmap(Properties.Resources.BonusWGhost1);
                else return new Bitmap(Properties.Resources.BonusWGhost2);
            }
            else if (state == GhostState.BONUS)
            {
                if (sprite1) return new Bitmap(Properties.Resources.BonusBGhost1);
                else return new Bitmap(Properties.Resources.BonusBGhost2);
            }
            else if (state == GhostState.EATEN)
            {
                switch (D)
                {
                    case Direction.DOWN: return new Bitmap(Properties.Resources.eyesDown);
                    case Direction.UP: return new Bitmap(Properties.Resources.eyesUp);
                    case Direction.RIGHT: return new Bitmap(Properties.Resources.eyesRight);
                    case Direction.LEFT: return new Bitmap(Properties.Resources.eyesLeft);
                    default: return null;
                }
            }
            else
            {
                switch (color)
                {
                    case GhostColor.RED:
                        switch (D)
                        {
                            case Direction.DOWN: if (sprite1) return new Bitmap(Properties.Resources.RedGhostDown1);
                                                else return new Bitmap(Properties.Resources.RedGhostDown2);
                            case Direction.UP: if (sprite1) return new Bitmap(Properties.Resources.RedGhostUp1);
                                              else return new Bitmap(Properties.Resources.RedGhostUp2);
                            case Direction.RIGHT:if(sprite1) return new Bitmap(Properties.Resources.RedGhostRight1);
                                                 else return new Bitmap(Properties.Resources.RedGhostRight2);
                            case Direction.LEFT: if (sprite1) return new Bitmap(Properties.Resources.RedGhostLeft1);
                                                else return new Bitmap(Properties.Resources.RedGhostLeft2);
                            default: return null;
                        }

                    case GhostColor.BLUE:
                        switch (D)
                        {
                            case Direction.DOWN: if (sprite1) return new Bitmap(Properties.Resources.BlueGhostDown1);
                                                 else return new Bitmap(Properties.Resources.BlueGhostDown2);
                            case Direction.UP: if (sprite1) return new Bitmap(Properties.Resources.BlueGhostUp1);
                                               else return new Bitmap(Properties.Resources.BlueGhostUp2);
                            case Direction.RIGHT: if (sprite1) return new Bitmap(Properties.Resources.BlueGhostRight1);
                                                 else return new Bitmap(Properties.Resources.BlueGhostRight2);
                            case Direction.LEFT: if (sprite1) return new Bitmap(Properties.Resources.BlueGhostLeft1);
                                                 else return new Bitmap(Properties.Resources.BlueGhostLeft2);

                            default: return null;
                        }

                    case GhostColor.PINK:
                        switch (D)
                        {
                            case Direction.DOWN:
                                if (sprite1) return new Bitmap(Properties.Resources.PinkGhostDown1);
                                else return new Bitmap(Properties.Resources.PinkGhostDown2);
                            case Direction.UP:
                                if (sprite1) return new Bitmap(Properties.Resources.PinkGhostUp1);
                                else return new Bitmap(Properties.Resources.PinkGhostUp2);
                            case Direction.RIGHT:
                                if (sprite1) return new Bitmap(Properties.Resources.PinkGhostRight1);
                                else return new Bitmap(Properties.Resources.PinkGhostRight2);
                            case Direction.LEFT:
                                if (sprite1) return new Bitmap(Properties.Resources.PinkGhostLeft1);
                                else return new Bitmap(Properties.Resources.PinkGhostLeft2);
                            default: return null;
                        }

                    case GhostColor.YELLOW:
                        switch (D)
                        {
                            case Direction.DOWN:
                                if (sprite1) return new Bitmap(Properties.Resources.YellowGhostDown1);
                                else return new Bitmap(Properties.Resources.YellowGhostDown2);
                            case Direction.UP:
                                if (sprite1) return new Bitmap(Properties.Resources.YellowGhostUp1);
                                else return new Bitmap(Properties.Resources.YellowGhostUp2);
                            case Direction.RIGHT:
                                if (sprite1) return new Bitmap(Properties.Resources.YellowGhostRight1);
                                else return new Bitmap(Properties.Resources.YellowGhostRight2);
                            case Direction.LEFT:
                                if (sprite1) return new Bitmap(Properties.Resources.YellowGhostLeft1);
                                else return new Bitmap(Properties.Resources.YellowGhostLeft2);
                            default: return null;
                        }
                    default: return null;
                }
            }


        }

        private void changePic(Point P, Direction D, GhostColor color, bool sprite1, GhostState state)
        {
            switch (color)
            {
                case GhostColor.RED:
                    if (picRedGhost != null) pnl.Controls.Remove(picRedGhost);
                    picRedGhost = new PictureBox();
                    picRedGhost.BackgroundImageLayout = ImageLayout.Zoom;
                    picRedGhost.Location = new Point((int)(P.X * cellWidth + 4 ), (int)(P.Y * cellHeight +4));
                    picRedGhost.Size = new Size((int)(4 * cellWidth) - 6, (int)(4 * cellHeight) - 6);
                    picRedGhost.BackgroundImage = findImage(D, color, sprite1, state);
                    pnl.Controls.Add(picRedGhost);
                    break;
                case GhostColor.BLUE:
                    if (picBlueGhost != null) pnl.Controls.Remove(picBlueGhost);
                    picBlueGhost = new PictureBox();
                    picBlueGhost.BackgroundImageLayout = ImageLayout.Zoom;
                    picBlueGhost.Location = new Point((int)(P.X * cellWidth + 4), (int)(P.Y * cellHeight + 4));
                    picBlueGhost.Size = new Size((int)(4 * cellWidth) - 6, (int)(4 * cellHeight) - 6);
                    picBlueGhost.BackgroundImage = findImage(D, color, sprite1, state);
                    pnl.Controls.Add(picBlueGhost);
                    break;
                case GhostColor.PINK:
                    if (picPinkGhost != null) pnl.Controls.Remove(picPinkGhost);
                    picPinkGhost = new PictureBox();
                    picPinkGhost.BackgroundImageLayout = ImageLayout.Zoom;
                    picPinkGhost.Location = new Point((int)(P.X * cellWidth + 4), (int)(P.Y * cellHeight + 4));
                    picPinkGhost.Size = new Size((int)(4 * cellWidth) - 6, (int)(4 * cellHeight) - 6);
                    picPinkGhost.BackgroundImage = findImage(D, color, sprite1, state);
                    pnl.Controls.Add(picPinkGhost);
                    break;
                case GhostColor.YELLOW:
                    if (picYellowGhost != null) pnl.Controls.Remove(picYellowGhost);
                    picYellowGhost = new PictureBox();
                    picYellowGhost.BackgroundImageLayout = ImageLayout.Zoom;
                    picYellowGhost.Location = new Point((int)(P.X * cellWidth + 4), (int)(P.Y * cellHeight + 4));
                    picYellowGhost.Size = new Size((int)(4 * cellWidth) - 6, (int)(4 * cellHeight) - 6);
                    picYellowGhost.BackgroundImage = findImage(D, color, sprite1, state);
                    pnl.Controls.Add(picYellowGhost);
                    break;
            }

        }
        
        public void DrawPacMan(int x, int y, Color color, Direction dir)
        {
            Brush b = new SolidBrush(color);
            Rectangle rect = new Rectangle((int)(x * cellWidth + 4), (int)(y * cellHeight + 4), (int)(cellWidth * 4 - 8), (int)(cellHeight * 4 - 8));
            int startAngle, sweepAngle;
            calculateAngles(dir, out startAngle, out sweepAngle);
            lock (this)
            {
                g.FillPie(b, rect, startAngle, sweepAngle);

            }
        }
        
        private void calculateAngles(Direction dir, out int startAngle, out int sweepAngle)
        {
            int stAngle, swAngle ;
            if(state == 1)
            {
                stAngle = 0; swAngle = 380;
                state++;
            }
            else if(state == 2)
            {
                stAngle = 25; swAngle = 310;
                state++;
            }
            else if (state == 3)
            {
                stAngle = 58; swAngle = 240;
                state++;
            }
            else 
            {
                stAngle = 25; swAngle = 310;
                state = 1;
            }



            startAngle = stAngle; sweepAngle = swAngle;
            switch (dir)
            {
                case Direction.RIGHT:
                    
                    break;
                case Direction.LEFT:
                    startAngle += 180;
                    break;
                case Direction.UP:
                    startAngle -= 90;
                    break;
                case Direction.DOWN:
                    startAngle += 90;
                    break;
                case Direction.STOP:
                    startAngle = 0; sweepAngle = 380;
                    break;
            }

        }

        public void DrawPacMan(Point p, Color color, Direction dir)
        {
            DrawPacMan(p.X, p.Y, color, dir);
        }
        
        public void ClearPacMan(Point p)
        {
            
            Brush b = new SolidBrush(BgColor);
            lock (this)
            {
                g.FillEllipse(b, p.X * cellWidth, p.Y * cellHeight, cellWidth * 4, cellHeight * 4);
            }
        }

        //public void ClearRedGhost()
        //{
        //    lock (this)
        //    {
        //        c.Remove(redGhost);
        //    }
        //}
    }
}
