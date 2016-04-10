using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace _2dGameLanguage
{


    public partial class BGL : Form
    {
        //Instance Variables
        #region
        int offsetFieldFigure = 29;
        int boardOffsetX = 350, boardOffsetY = 730;
        int tempLineOffset = 5;
        int moves = 0;
        int numOfPlayers = 0;
        string[] players = { "Crveni", "Žuti", "Zeleni", "Plavi" };
        int activePlayer = 0; //vezano za listu igrača koji se prikazuju, 0 - Crveni ... 3 - Plavi
        int NumOfThrows = 3;
        int ThrownSoFar = 0;
        int useFigure = -1;
        bool FigureWait = false;
        int WaitMessage = 0;
        Sprite[] activeFigures;

        double lastTime, thisTime, diff;
        Sprite[] sprites = new Sprite[1000];
        Sprite[] redFigures = new Sprite[8];
        Sprite[] yellowFigures = new Sprite[8];
        Sprite[] greenFigures = new Sprite[8];
        Sprite[] blueFigures = new Sprite[8];
        Sprite[] redHomes = new Sprite[8];
        Sprite[] yellowHomes = new Sprite[8];
        Sprite[] greenHomes = new Sprite[8];
        Sprite[] blueHomes = new Sprite[8];
        Path[] pathCoordinates = new Path[40];  //koordinate po kojima ćemo se kretati
        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        int spriteCount = 0, soundCount = 0;
        string inkey;
        int mouseKey, mouseXp, mouseYp;
        Rectangle Collision;
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;

        #endregion

        //Structs
        #region
        public struct Sprite
        {
            public string image;
            public Bitmap bmp;
            public int x, y, width, height, start_x, start_y;
            public bool show;
            public bool moves;
            public int pathPosition;
            public int pathOffset;
            public bool saved;
            public bool near_house;

            public Sprite(string images, int p1, int p2)
            {
                bmp = new Bitmap(images);
                image = images;
                x = p1;
                y = p2;
                start_x = p1;
                start_y = p2;
                width = bmp.Width;
                height = bmp.Height;
                show = true;
                moves = false;
                saved = false;
                near_house = false;
                pathPosition = -1;
                pathOffset = -1;
            }

            public Sprite(string images, int p1, int p2, int w, int h)
            {
                bmp = new Bitmap(images);
                image = images;
                x = p1;
                y = p2;
                start_x = p1;
                start_y = p2;
                width = w;
                height = h;
                show = true;
                moves = false;
                saved = false;
                near_house = false;
                pathPosition = -1;
                pathOffset = -1;
            }
        }

        public struct Path
        {
            public int x, y;
            public Path(int p1, int p2)
            {
                x = p1;
                y = p2;
            }
        }

        #endregion

        public BGL()
        {
            InitializeComponent();
        }

        public void Init()
        {
            while (numOfPlayers < 2 || numOfPlayers > 4)
            {
                try
                {
                    numOfPlayers = Convert.ToInt16(ShowDialog("Broj igrača (2-4):", "Pitanje"));

                }
                catch { }
            }

            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;


            setTitle("Čovječe ne ljuti se!!!"); // postavi naziv prozora
            setStatus("Baci kocku!");
            setNumOfThrows(NumOfThrows - ThrownSoFar);

            setBackgroundColour(209, 182, 137);
            loadSound(1, "roll-dice.wav"); // učitaj zvukove isto kao i sprite-ove
          
            SetBoard(); //postavlja ploču
            SetHomes(numOfPlayers); // postavlja kucice i figure na njih sa brojevima

            // postavlja i rotira startne pozivije onih strelica
            loadSprite("direction.png", 91, spriteX(0), spriteY(0) - tempLineOffset);
            loadSprite("direction.png", 92, spriteX(20) + tempLineOffset, spriteY(20));
            rotateSprite(92, 90);
            loadSprite("direction.png", 93, spriteX(40), spriteY(40) + tempLineOffset);
            rotateSprite(93, 180);
            loadSprite("direction.png", 94, spriteX(60) - tempLineOffset, spriteY(60));
            rotateSprite(94, 270);

            //postavlja aktivnog igrača na labelu
            setPlayer(activePlayer);

        }

        //Set board i Set path u kombinaciji idu.
        public void SetBoard()
        {

            int tempOffsetX = 0;
            int tempOffsetY = 0;

            //60px ima svaki field - učitaj sprite sa identifikatorom na određenu koordinatu

            for (int i = 0; i < 4; i++)
            {

                if (i == 0)
                {
                    tempOffsetX = boardOffsetX;
                    tempOffsetY = boardOffsetY;
                }
                if (i == 1)
                {
                    tempOffsetX = boardOffsetX - 315;
                    tempOffsetY = boardOffsetY - 441;
                }
                else if (i == 2)
                {
                    tempOffsetX = boardOffsetX + 127;
                    tempOffsetY = boardOffsetY - 756;
                }
                else if (i == 3)
                {
                    tempOffsetX = boardOffsetX + 442;
                    tempOffsetY = boardOffsetY - 315;
                }

                SetPath(tempOffsetX, tempOffsetY, offsetFieldFigure, "WhiteField.png", "StartField.png", "v_line.png", "h_line.png", i);
            }

            //Postavljanje kockica
            SetDice(3);
        }


        public int RollDice(int x)
        {
            if (s.ElapsedMilliseconds > 2000)
            {
                s.Stop();
                setStatus("Baci kocku!");
                ThrownSoFar++;
                setNumOfThrows(NumOfThrows - ThrownSoFar);
            }
            else if (s.ElapsedMilliseconds % 500 == 0)
            {
                Random rnd = new Random();
                x = rnd.Next(1, 7);
                SetDice(x);
            }
            return x;
        }

        public void SetDice(int x)
        {
            loadSprite("dice-" + x.ToString() + ".png", 81, boardOffsetX + 525, boardOffsetY - 110);
        }

        public void SetPath(int offsetX, int offsetY, int offsetFieldFigure, string whitePath, string startPath, string verticalLine, string horizontalLine, int rotation)
        {
            int tempX = offsetX;
            int tempY = offsetY;
            int tempOffset = 2 * offsetFieldFigure + tempLineOffset;


            string spriteName = "";

            for (int i = (0 + rotation * 20); i < (20 + rotation * 20); i++)
            {
                if (i % 20 == 0) spriteName = startPath;
                else spriteName = whitePath;


                if ((i >= (0 + rotation * 20) && i < (10 + rotation * 20)) || (i >= (18 + rotation * 20) && i < (20 + rotation * 20)))
                {
                    if (i == (9 + rotation * 20))
                    {

                        spriteName = horizontalLine;
                        tempX -= tempLineOffset;
                        tempY += offsetFieldFigure;
                        if (rotation == 1)
                        {
                            spriteName = verticalLine;
                            tempX += (offsetFieldFigure + tempLineOffset);
                            tempY -= (offsetFieldFigure + tempLineOffset);
                        }
                        else if (rotation == 2)
                        {
                            tempX += (offsetFieldFigure * 2 + tempLineOffset);
                        }
                        else if (rotation == 3)
                        {
                            spriteName = verticalLine;
                            tempX += (offsetFieldFigure + tempLineOffset);
                            tempY += offsetFieldFigure;
                        }

                    }
                    else if (i % 2 == 1 & i != (18 + rotation * 20))
                    {
                        spriteName = verticalLine;
                        tempX += offsetFieldFigure;
                        tempY -= tempLineOffset;
                        if (rotation == 1)
                        {
                            spriteName = horizontalLine;
                            tempX += offsetFieldFigure;
                            tempY += offsetFieldFigure + tempLineOffset;
                        }
                        else if (rotation == 2)
                        {
                            tempY += (offsetFieldFigure * 2 + tempLineOffset);
                        }
                        else if (rotation == 3)
                        {
                            spriteName = horizontalLine;
                            tempX -= (offsetFieldFigure + tempLineOffset);
                            tempY += (offsetFieldFigure + tempLineOffset);
                        }
                    }
                    else
                    {
                        if (rotation == 0)
                            tempY -= tempOffset;
                        else if (rotation == 1)
                            tempX += tempOffset;
                        else if (rotation == 2)
                            tempY += tempOffset;
                        else
                            tempX -= tempOffset;
                    }
                }
                else if (i >= (10 + rotation * 20) && i < (20 + rotation * 20))
                {
                    if (i == (17 + rotation * 20))
                    {
                        spriteName = verticalLine;
                        tempX += offsetFieldFigure;
                        tempY -= tempLineOffset;
                        if (rotation == 1)
                        {
                            spriteName = horizontalLine;
                            tempX += offsetFieldFigure;
                            tempY += offsetFieldFigure + tempLineOffset;
                        }
                        else if (rotation == 2)
                        {
                            tempY += (offsetFieldFigure * 2 + tempLineOffset);
                        }
                        else if (rotation == 3)
                        {
                            spriteName = horizontalLine;
                            tempX -= (offsetFieldFigure + tempLineOffset);
                            tempY += (offsetFieldFigure + tempLineOffset);
                        }
                    }
                    else if (i % 2 == 1)
                    {
                        spriteName = horizontalLine;
                        tempX -= tempLineOffset;
                        tempY += offsetFieldFigure;
                        if (rotation == 1)
                        {
                            spriteName = verticalLine;
                            tempX += offsetFieldFigure + tempLineOffset;
                            tempY -= (offsetFieldFigure + tempLineOffset);
                        }
                        else if (rotation == 2)
                        {
                            tempX += (2 * offsetFieldFigure + tempLineOffset);
                        }
                        else if (rotation == 3)
                        {
                            spriteName = verticalLine;
                            tempX += offsetFieldFigure + tempLineOffset;
                            tempY += offsetFieldFigure;
                        }
                    }
                    else
                    {
                        if (rotation == 0)
                            tempX -= tempOffset;
                        else if (rotation == 1)
                            tempY -= tempOffset;
                        else if (rotation == 2)
                            tempX += tempOffset;
                        else
                            tempY += tempOffset;
                    }
                }

                //crtanje puta i upisivanje korditana u listu
                loadSprite(spriteName, i, tempX - offsetFieldFigure, tempY - offsetFieldFigure);
                if (i % 2 == 0)
                {

                    loadPathCoordinates(i / 2, tempX - offsetFieldFigure, tempY - offsetFieldFigure);
                }

                //ispravke offseta nazad u crtanju linija između polja
                if ((i >= (0 + rotation * 20) && i < (10 + rotation * 20)) || (i >= (18 + rotation * 20) && i < (20 + rotation * 20)))
                {
                    if (i == (9 + rotation * 20))
                    {
                        tempX += tempLineOffset;
                        tempY -= offsetFieldFigure;
                        if (rotation == 1)
                        {
                            tempX -= (offsetFieldFigure + tempLineOffset);
                            tempY += (offsetFieldFigure + tempLineOffset);
                        }
                        else if (rotation == 2)
                        {
                            tempX -= (offsetFieldFigure * 2 + tempLineOffset);
                        }
                        else if (rotation == 3)
                        {
                            tempX -= (offsetFieldFigure + tempLineOffset);
                            tempY -= offsetFieldFigure;
                        }
                    }
                    else if (i % 2 == 1 & i != (18 + rotation * 20))
                    {
                        tempX -= offsetFieldFigure;
                        tempY += tempLineOffset;
                        if (rotation == 1)
                        {
                            tempX -= offsetFieldFigure;
                            tempY -= (offsetFieldFigure + tempLineOffset);
                        }
                        else if (rotation == 2)
                        {
                            tempY -= (offsetFieldFigure * 2 + tempLineOffset);
                        }
                        else if (rotation == 3)
                        {
                            tempX += (offsetFieldFigure + tempLineOffset);
                            tempY -= (offsetFieldFigure + tempLineOffset);
                        }
                    }
                }
                else if (i >= (10 + rotation * 20) && i < (20 + rotation * 20))
                {
                    if (i == (17 + rotation * 20))
                    {
                        tempX -= offsetFieldFigure;
                        tempY += tempLineOffset;
                        if (rotation == 1)
                        {
                            tempX -= offsetFieldFigure;
                            tempY -= (offsetFieldFigure + tempLineOffset);
                        }
                        else if (rotation == 2)
                        {
                            tempY -= (offsetFieldFigure * 2 + tempLineOffset);
                        }
                        else if (rotation == 3)
                        {
                            tempX += (offsetFieldFigure + tempLineOffset);
                            tempY -= (offsetFieldFigure + tempLineOffset);
                        }
                    }
                    else if (i % 2 == 1)
                    {
                        tempX += tempLineOffset;
                        tempY -= offsetFieldFigure;
                        if (rotation == 1)
                        {
                            tempX -= offsetFieldFigure + tempLineOffset;
                            tempY += (offsetFieldFigure + tempLineOffset);
                        }
                        else if (rotation == 2)
                        {
                            tempX -= (2 * offsetFieldFigure + tempLineOffset);
                        }
                        else if (rotation == 3)
                        {
                            tempX -= offsetFieldFigure + tempLineOffset;
                            tempY -= offsetFieldFigure;
                        }
                    }
                }

            }
        }

        //set homes postavlja igrače također
        public void SetHomes(int numOfPlayers)
        {

            loadRedHomeSprite("RedField.png", 2, spriteX(14), spriteY(0));
            loadRedHomeSprite("RedField.png", 3, spriteX(16), spriteY(0));
            loadRedHomeSprite("RedField.png", 0, spriteX(14), spriteY(2));
            loadRedHomeSprite("RedField.png", 1, spriteX(16), spriteY(2));
            loadRedHomeSprite("RedField.png", 4, spriteX(78), spriteY(2));
            loadRedHomeSprite("RedField.png", 5, spriteX(78), spriteY(4));
            loadRedHomeSprite("RedField.png", 6, spriteX(78), spriteY(6));
            loadRedHomeSprite("RedField.png", 7, spriteX(78), spriteY(8));
            if (numOfPlayers > 0 && numOfPlayers < 5)
            {
                loadRedFigureSprite("RedFigure.png", 2, spriteX(14), spriteY(0));
                loadRedFigureSprite("RedFigure.png", 3, spriteX(16), spriteY(0));
                loadRedFigureSprite("RedFigure.png", 0, spriteX(14), spriteY(2));
                loadRedFigureSprite("RedFigure.png", 1, spriteX(16), spriteY(2));
                loadRedFigureSprite("Three.png", 6, spriteX(14), spriteY(0));
                loadRedFigureSprite("Four.png", 7, spriteX(16), spriteY(0));
                loadRedFigureSprite("One.png", 4, spriteX(14), spriteY(2));
                loadRedFigureSprite("Two.png", 5, spriteX(16), spriteY(2));

            }

            loadYellowHomeSprite("YellowField.png", 1, spriteX(20), spriteY(34));
            loadYellowHomeSprite("YellowField.png", 0, spriteX(22), spriteY(34));
            loadYellowHomeSprite("YellowField.png", 3, spriteX(20), spriteY(36));
            loadYellowHomeSprite("YellowField.png", 2, spriteX(22), spriteY(36));
            loadYellowHomeSprite("YellowField.png", 4, spriteX(14), spriteY(18));
            loadYellowHomeSprite("YellowField.png", 5, spriteX(12), spriteY(18));
            loadYellowHomeSprite("YellowField.png", 6, spriteX(10), spriteY(18));
            loadYellowHomeSprite("YellowField.png", 7, spriteX(8), spriteY(18));
            if (numOfPlayers > 1 && numOfPlayers < 5)
            {
                loadYellowFigureSprite("YellowFigure.png", 1, spriteX(20), spriteY(34));
                loadYellowFigureSprite("YellowFigure.png", 0, spriteX(22), spriteY(34));
                loadYellowFigureSprite("YellowFigure.png", 3, spriteX(20), spriteY(36));
                loadYellowFigureSprite("YellowFigure.png", 2, spriteX(22), spriteY(36));
                loadYellowFigureSprite("Two.png", 5, spriteX(20), spriteY(34));
                loadYellowFigureSprite("One.png", 4, spriteX(22), spriteY(34));
                loadYellowFigureSprite("Four.png", 7, spriteX(20), spriteY(36));
                loadYellowFigureSprite("Three.png", 6, spriteX(22), spriteY(36));
            }

            loadGreenHomeSprite("GreenField.png", 0, spriteX(54), spriteY(34));
            loadGreenHomeSprite("GreenField.png", 1, spriteX(56), spriteY(34));
            loadGreenHomeSprite("GreenField.png", 2, spriteX(54), spriteY(36));
            loadGreenHomeSprite("GreenField.png", 3, spriteX(56), spriteY(36));
            loadGreenHomeSprite("GreenField.png", 4, spriteX(78), spriteY(34));
            loadGreenHomeSprite("GreenField.png", 5, spriteX(78), spriteY(32));
            loadGreenHomeSprite("GreenField.png", 6, spriteX(78), spriteY(30));
            loadGreenHomeSprite("GreenField.png", 7, spriteX(78), spriteY(28));
            if (numOfPlayers > 2 && numOfPlayers < 5)
            {
                loadGreenFigureSprite("GreenFigure.png", 0, spriteX(54), spriteY(34));
                loadGreenFigureSprite("GreenFigure.png", 1, spriteX(56), spriteY(34));
                loadGreenFigureSprite("GreenFigure.png", 2, spriteX(54), spriteY(36));
                loadGreenFigureSprite("GreenFigure.png", 3, spriteX(56), spriteY(36));
                loadGreenFigureSprite("One.png", 4, spriteX(54), spriteY(34));
                loadGreenFigureSprite("Two.png", 5, spriteX(56), spriteY(34));
                loadGreenFigureSprite("Three.png", 6, spriteX(54), spriteY(36));
                loadGreenFigureSprite("Four.png", 7, spriteX(56), spriteY(36));
            }

            loadBlueHomeSprite("BlueField.png", 2, spriteX(54), spriteY(0));
            loadBlueHomeSprite("BlueField.png", 3, spriteX(56), spriteY(0));
            loadBlueHomeSprite("BlueField.png", 0, spriteX(54), spriteY(2));
            loadBlueHomeSprite("BlueField.png", 1, spriteX(56), spriteY(2));
            loadBlueHomeSprite("BlueField.png", 4, spriteX(54), spriteY(18));
            loadBlueHomeSprite("BlueField.png", 5, spriteX(52), spriteY(18));
            loadBlueHomeSprite("BlueField.png", 6, spriteX(50), spriteY(18));
            loadBlueHomeSprite("BlueField.png", 7, spriteX(48), spriteY(18));
            if (numOfPlayers == 4)
            {
                loadBlueFigureSprite("BlueFigure.png", 2, spriteX(54), spriteY(0));
                loadBlueFigureSprite("BlueFigure.png", 3, spriteX(56), spriteY(0));
                loadBlueFigureSprite("BlueFigure.png", 0, spriteX(54), spriteY(2));
                loadBlueFigureSprite("BlueFigure.png", 1, spriteX(56), spriteY(2));
                loadBlueFigureSprite("Three.png", 6, spriteX(54), spriteY(0));
                loadBlueFigureSprite("Four.png", 7, spriteX(56), spriteY(0));
                loadBlueFigureSprite("One.png", 4, spriteX(54), spriteY(2));
                loadBlueFigureSprite("Two.png", 5, spriteX(56), spriteY(2));
            }

        }

        private Stopwatch s = new Stopwatch();

        public int GetNumOfThrows(int actPlayer)
        {
            if (actPlayer == 0)
            {
                activeFigures = redFigures;
            }
            else if (actPlayer == 1)
            {
                activeFigures = yellowFigures;
            }
            else if (actPlayer == 2)
            {
                activeFigures = greenFigures;
            }
            else if (actPlayer == 3)
            {
                activeFigures = blueFigures;
            }

            int tempNumOfMoves = 3;

            foreach (Sprite figure in activeFigures)
            {
                if (figure.moves)
                {
                    tempNumOfMoves = 1;
                    break;
                }
            }
            return tempNumOfMoves;
        }

        //odabir igrača za igrati nakon bacanja kockice
        public int ChoosePlayer()
        {
            int pl = 0;

            while (pl < 1 || pl > 4)
            {
                try
                {
                    pl = Convert.ToInt16(ShowDialog("Broj igrača (1-4):", "Pitanje"));
                }
                catch { }
            }

            return pl;
        }

        //cekamo li za odabir igrača ili ne
        public bool checkFigureWait(int activePlayer, int moves)
        {
            if (moves == 0)
                return false;
            else if (moves == 6)
                return true;
            else
            {
                if (activePlayer == 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (redFigures[i].moves)
                        {   
                                return true;
                        }
                    }
                }
                else if (activePlayer == 1)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (yellowFigures[i].moves)
                        {
                                return true;
                        }
                    }
                }
                else if (activePlayer == 2)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (greenFigures[i].moves)
                        {
                                return true;
                        }
                    }
                }
                else if (activePlayer == 3)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (blueFigures[i].moves)
                        {
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool CheckWin(Sprite[] sprites)
        {
            for (int i = 0; i < 4; i++)
            {
                if (sprites[i].saved == false)
                {
                    return false;
                }
            }

            return true;
        }

        //update se stalno vrti i pokreće program
        private void Update(object sender, EventArgs e)
        {
            // ponašanje figura 
            //startna pozicija sprite[0] crveni, sprite[20] zuti, sprite [40] zeleni, sprite [60] plavi

            if (CheckWin(redFigures))
            {
                timer1.Stop();
                MessageBox.Show("Pobjedio je crveni igrač!", "Čestitamo", MessageBoxButtons.OK);
                SetHomes(4);
                numOfPlayers = Convert.ToInt16(ShowDialog("Broj igrača (2-4):", "Pitanje"));
                setPlayer(0);
                setStatus("Baci kocku!");
                NumOfThrows = GetNumOfThrows(activePlayer);
                ThrownSoFar = 0;
                moves = 0;
                timer1.Start();
            }
            if (CheckWin(blueFigures))
            {
                timer1.Stop();
                MessageBox.Show("Pobjedio je plavi igrač!", "Čestitamo", MessageBoxButtons.OK);
                SetHomes(4);
                numOfPlayers = Convert.ToInt16(ShowDialog("Broj igrača (2-4):", "Pitanje"));
                setPlayer(0);
                setStatus("Baci kocku!");
                NumOfThrows = GetNumOfThrows(activePlayer);
                ThrownSoFar = 0;
                moves = 0;
                timer1.Start();
            }
            if (CheckWin(yellowFigures))
            {
                timer1.Stop();
                MessageBox.Show("Pobjedio je žuti igrač!", "Čestitamo", MessageBoxButtons.OK);
                SetHomes(4);
                numOfPlayers = Convert.ToInt16(ShowDialog("Broj igrača (2-4):", "Pitanje"));
                setPlayer(0);
                setStatus("Baci kocku!");
                NumOfThrows = GetNumOfThrows(activePlayer);
                ThrownSoFar = 0;
                moves = 0;
                timer1.Start();
            }
            if (CheckWin(greenFigures))
            {
                timer1.Stop();
                MessageBox.Show("Pobjedio je zeleni igrač!", "Čestitamo", MessageBoxButtons.OK);
                SetHomes(4);
                numOfPlayers = Convert.ToInt16(ShowDialog("Broj igrača (2-4):", "Pitanje"));
                setPlayer(0);
                setStatus("Baci kocku!");
                NumOfThrows = GetNumOfThrows(activePlayer);
                ThrownSoFar = 0;
                moves = 0;
                timer1.Start();
            }


            NumOfThrows = GetNumOfThrows(activePlayer);

            if (isKeyDown(Keys.Enter) && !FigureWait)
            {
                s = new Stopwatch();
                s.Start();
                playSound(1);
                setStatus("Čekaj red!");
                setNumOfThrows(NumOfThrows - ThrownSoFar);
    
            }

            if (isKeyDown(Keys.Space))
            {
                moveSprite(redFigures, 0, sprites[78].x, sprites[78].y);
                moveSprite(redFigures, 4, sprites[78].x, sprites[78].y);
                redFigures[0].moves = true;
                redFigures[0].near_house = true;
                redFigures[0].pathPosition = 78;
                moveSprite(redFigures, 1, sprites[76].x, sprites[76].y);
                moveSprite(redFigures, 5, sprites[76].x, sprites[76].y);
                redFigures[1].moves = true;
                redFigures[1].pathPosition = 76;
                redFigures[1].near_house = true;

                moveSprite(yellowFigures, 0, sprites[18].x, sprites[18].y);
                moveSprite(yellowFigures, 4, sprites[18].x, sprites[18].y);
                yellowFigures[0].moves = true;
                yellowFigures[0].near_house = true;
                yellowFigures[0].pathPosition = 18;
                moveSprite(yellowFigures, 1, sprites[16].x, sprites[16].y);
                moveSprite(yellowFigures, 5, sprites[16].x, sprites[16].y);
                yellowFigures[1].moves = true;
                yellowFigures[1].pathPosition = 16;
                yellowFigures[1].near_house = true;

                moveSprite(greenFigures, 0, sprites[38].x, sprites[38].y);
                moveSprite(greenFigures, 4, sprites[38].x, sprites[38].y);
                greenFigures[0].moves = true;
                greenFigures[0].near_house = true;
                greenFigures[0].pathPosition = 38;
                moveSprite(greenFigures, 1, sprites[36].x, sprites[36].y);
                moveSprite(greenFigures, 5, sprites[36].x, sprites[36].y);
                greenFigures[1].moves = true;
                greenFigures[1].pathPosition = 36;
                greenFigures[1].near_house = true;

                moveSprite(blueFigures, 0, sprites[58].x, sprites[58].y);
                moveSprite(blueFigures, 4, sprites[58].x, sprites[58].y);
                blueFigures[0].moves = true;
                blueFigures[0].near_house = true;
                blueFigures[0].pathPosition = 58;
                moveSprite(blueFigures, 1, sprites[56].x, sprites[56].y);
                moveSprite(blueFigures, 5, sprites[56].x, sprites[56].y);
                blueFigures[1].moves = true;
                blueFigures[1].pathPosition = 56;
                blueFigures[1].near_house = true;

                setPlayer(activePlayer);
                setStatus("Baci kocku!");
                NumOfThrows = GetNumOfThrows(activePlayer);
                ThrownSoFar = 0;
                moves = 0;
                setNumOfThrows(NumOfThrows - ThrownSoFar);
            }
            if (s.IsRunning)
            {
                moves = RollDice(moves);
            }
            else if (moves > 0)
            {
                FigureWait = checkFigureWait(activePlayer, moves);

                if (FigureWait)
                {
                    if (WaitMessage == 0)
                    {
                        setStatus("Figura 1-4?");
                    }
                    else if (WaitMessage == 1)
                    {
                        setStatus("Krivi odabir!");
                    }
                    else
                    {
                        setStatus("Ne mogu dalje!");
                    }
                    if (isKeyDown(Keys.NumPad1) || isKeyDown(Keys.D1))
                    {
                        useFigure = 0;
                    }
                    if (isKeyDown(Keys.NumPad2) || isKeyDown(Keys.D2))
                    {
                        useFigure = 1;
                    }
                    if (isKeyDown(Keys.NumPad3) || isKeyDown(Keys.D3))
                    {
                        useFigure = 2;
                    }
                    if (isKeyDown(Keys.NumPad4) || isKeyDown(Keys.D4))
                    {
                        useFigure = 3;
                    }

                }

                if (useFigure > -1)
                {

                    if (activePlayer == 0)
                    {
                            moveFigureToSprite(redFigures, useFigure, moves, redHomes);                      
                    }
                    else if (activePlayer == 1)
                    {
                            moveFigureToSprite(yellowFigures, useFigure, moves, yellowHomes);
                    }
                    else if (activePlayer == 2)
                    {
                            moveFigureToSprite(greenFigures, useFigure, moves, greenHomes);
                    }
                    else if (activePlayer == 3)
                    {
                            moveFigureToSprite(blueFigures, useFigure, moves, blueHomes);
                    }

                }

                if (!FigureWait)
                {
                    if (NumOfThrows <= ThrownSoFar)
                    {
                        activePlayer = (activePlayer + 1) % numOfPlayers;
                        setPlayer(activePlayer);
                        setStatus("Baci kocku!");
                        NumOfThrows = GetNumOfThrows(activePlayer);
                        ThrownSoFar = 0;
                        moves = 0;
                    }
                    
                }

                if (ThrownSoFar != NumOfThrows)
                {
                    setNumOfThrows(NumOfThrows - ThrownSoFar);
                }
                else
                {
                    setNumOfThrows((activePlayer + 1) % numOfPlayers);
                }

            }

            this.Refresh();
        }

        // Start of Game Methods
            

        #region

        //This is the beginning of the setter methods

        private void startTimer(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            Init();
        }

        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        //postavlja naslov igrice
        public void setTitle(string title)
        {
            this.Text = title;
        }

        public void setStatus(string status)
        {
            lblStatus.Text = status;
        }

        public void setNumOfThrows(int numOfThrowes)
        {
            lblNumOfMoves.Text = "Bacanja - " + numOfThrowes.ToString();
        }
        public void setPlayer(int pl)
        {
            lblPlayerInfo.Text = "Igrač - " + players[pl];
        }

        public void setBackgroundColour(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }

        public void setBackgroundColour(Color col)
        {
            this.BackColor = col;
        }

        public void setBackgroundImage(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        public void setBackgroundImageLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        public void loadSprite(string file, int spriteNum)
        {
            spriteCount++;
            sprites[spriteNum] = new Sprite(file, 0, 0);
        }

        public void loadSprite(string file, int spriteNum, int x, int y)
        {
            spriteCount++;
            sprites[spriteNum] = new Sprite(file, x, y);
        }
        public void loadRedFigureSprite(string file, int spriteNum, int x, int y)
        {
            redFigures[spriteNum] = new Sprite(file, x, y);
            redFigures[spriteNum].pathOffset = 0;
            redFigures[spriteNum].moves = false;
            redFigures[spriteNum].saved = false;
            redFigures[spriteNum].pathPosition = -1;
            redFigures[spriteNum].near_house = false;
            redFigures[spriteNum].x = redFigures[spriteNum].start_x;
            redFigures[spriteNum].y = redFigures[spriteNum].start_y;
        }
        public void loadYellowFigureSprite(string file, int spriteNum, int x, int y)
        {
            yellowFigures[spriteNum] = new Sprite(file, x, y);
            yellowFigures[spriteNum].pathOffset = 20;
            yellowFigures[spriteNum].moves = false;
            yellowFigures[spriteNum].saved = false;
            yellowFigures[spriteNum].near_house = false;
            yellowFigures[spriteNum].pathPosition = -1;
            yellowFigures[spriteNum].x = yellowFigures[spriteNum].start_x;
            yellowFigures[spriteNum].y = yellowFigures[spriteNum].start_y;
        }
        public void loadGreenFigureSprite(string file, int spriteNum, int x, int y)
        {
            greenFigures[spriteNum] = new Sprite(file, x, y);
            greenFigures[spriteNum].pathOffset = 40;
            greenFigures[spriteNum].moves = false;
            greenFigures[spriteNum].saved = false;
            greenFigures[spriteNum].near_house = false;
            greenFigures[spriteNum].pathPosition = -1;
            greenFigures[spriteNum].x = greenFigures[spriteNum].start_x;
            greenFigures[spriteNum].y = greenFigures[spriteNum].start_y;
        }
        public void loadBlueFigureSprite(string file, int spriteNum, int x, int y)
        {
            blueFigures[spriteNum] = new Sprite(file, x, y);
            blueFigures[spriteNum].pathOffset = 60;
            blueFigures[spriteNum].moves = false;
            blueFigures[spriteNum].saved = false;
            blueFigures[spriteNum].near_house = false;
            blueFigures[spriteNum].pathPosition = -1;
            blueFigures[spriteNum].x = blueFigures[spriteNum].start_x;
            blueFigures[spriteNum].y = blueFigures[spriteNum].start_y;
        }
        public void loadRedHomeSprite(string file, int spriteNum, int x, int y)
        {
            redHomes[spriteNum] = new Sprite(file, x, y);
        }
        public void loadYellowHomeSprite(string file, int spriteNum, int x, int y)
        {
            yellowHomes[spriteNum] = new Sprite(file, x, y);
        }
        public void loadGreenHomeSprite(string file, int spriteNum, int x, int y)
        {
            greenHomes[spriteNum] = new Sprite(file, x, y);
        }
        public void loadBlueHomeSprite(string file, int spriteNum, int x, int y)
        {
            blueHomes[spriteNum] = new Sprite(file, x, y);
        }

        //ucitavamo kordinate po kojima se krećemo kasnije
        public void loadPathCoordinates(int pathNum, int x, int y)
        {
            pathCoordinates[pathNum] = new Path(x, y);
        }

        public void loadSprite(string file, int spriteNum, int x, int y, int w, int h)
        {
            spriteCount++;
            sprites[spriteNum] = new Sprite(file, x, y, w, h);
        }

        public void rotateSprite(int spriteNum, int angle)
        {
            if (angle == 90)
                sprites[spriteNum].bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            if (angle == 180)
                sprites[spriteNum].bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
            if (angle == 270)
                sprites[spriteNum].bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
        }

        public void scaleSprite(int spriteNum, int scale)
        {
            float sx = float.Parse(sprites[spriteNum].width.ToString());
            float sy = float.Parse(sprites[spriteNum].height.ToString());
            float nsx = ((sx / 100) * scale);
            float nsy = ((sy / 100) * scale);

            sprites[spriteNum].width = Convert.ToInt32(nsx);
            sprites[spriteNum].height = Convert.ToInt32(nsy);
        }

        public void moveSprite(Sprite[] tempSprites, int spriteNum, int x, int y)
        {
            tempSprites[spriteNum].x = x;
            tempSprites[spriteNum].y = y;
        }

        //pomocanje figura
        public void moveFigureToSprite(Sprite[] figures, int numOfFigure, int numOfMoves, Sprite[] homes)
        {

            //ako se pomiče iz kućice postaviti na offset
            useFigure = -1;
            setStatus("Baci kocku!");
            FigureWait = false;
            if (!figures[numOfFigure].moves && numOfMoves == 6)
            {
                testPojede(figures[numOfFigure], figures[numOfFigure].pathOffset, figures[numOfFigure].moves);
                figures[numOfFigure].x = sprites[figures[numOfFigure].pathOffset].x;
                figures[numOfFigure].y = sprites[figures[numOfFigure].pathOffset].y;
                figures[numOfFigure + 4].x = sprites[figures[numOfFigure].pathOffset].x;
                figures[numOfFigure + 4].y = sprites[figures[numOfFigure].pathOffset].y;
                figures[numOfFigure].moves = true;
                figures[numOfFigure].pathPosition = figures[numOfFigure].pathOffset;
                
            
                NumOfThrows = 1;
                ThrownSoFar = 0;
                moves = 0;
                WaitMessage = 0;
            }
            else if (figures[numOfFigure].moves)
            {
                
                int newPosition = figures[numOfFigure].pathPosition + numOfMoves * 2;
                if (numOfMoves == 6)
                {
                    NumOfThrows = 1;
                    ThrownSoFar = 0;
                }

                if (figures[numOfFigure].near_house && newPosition > (78 + figures[numOfFigure].pathOffset) % 80)
                {
                        //ulazi u kućicu indexi kućica su 4, 5, 6, 7 offset je 4

                    int lastPositionInHouse = (newPosition / 10)*10 + 8;


                    if(activePlayer == 0)
                        if (lastPositionInHouse > 88) lastPositionInHouse = 88;
                    if (activePlayer == 1)
                        if (lastPositionInHouse > 28) lastPositionInHouse = 28;
                    if (activePlayer == 2)
                        if (lastPositionInHouse > 48) lastPositionInHouse = 48;
                    if (activePlayer == 3)
                        if (lastPositionInHouse > 68) lastPositionInHouse = 68;

                    int firstPositionInHouse = GetFirstPositionInHouse(figures[numOfFigure], figures, homes, lastPositionInHouse);


                    if (newPosition >= firstPositionInHouse)
                    {
                        FigureWait = true;
                        WaitMessage = 2;
                        bool moveNext = true;
                        

                        for (int k = 0; k < 4; k++)
                        {
                            int newTempPosition = figures[k].pathPosition + numOfMoves * 2;
                            int newTempFirstPositionInHouse = GetFirstPositionInHouse(figures[k], figures, homes, lastPositionInHouse);
                            if ((k != numOfFigure && figures[k].moves == true) || numOfMoves == 6)
                            {
                                if (newTempPosition <= newTempFirstPositionInHouse)
                                {
                                    moveNext = false;
                                }

                            }
                        }


                        if (moveNext)
                        {
                            activePlayer = (activePlayer + 1) % numOfPlayers;
                            setPlayer(activePlayer);
                            setStatus("Baci kocku!");
                            NumOfThrows = GetNumOfThrows(activePlayer);
                            ThrownSoFar = 0;
                            WaitMessage = 0;
                            moves = 0;
                            FigureWait = false;
                        }

                    }
                    else
                    {
                        // modulo mora ici u ovisnosti o figurama
                        // zuti 20, zeleni 40, plavi 60, crveni 80
                        int modulo = (newPosition / 10)*10;
                        testPojede(figures[numOfFigure], newPosition, figures[numOfFigure].moves);
                        figures[numOfFigure].x = homes[(newPosition % modulo) / 2 + 4].x;
                        figures[numOfFigure].y = homes[(newPosition % modulo) / 2 + 4].y;
                        figures[numOfFigure + 4].x = homes[(newPosition % modulo) / 2 + 4].x;
                        figures[numOfFigure + 4].y = homes[(newPosition % modulo) / 2 + 4].y;
                        figures[numOfFigure].saved = true;
                        figures[numOfFigure].pathPosition = newPosition;
                        moves = 0;
                        WaitMessage = 0;
                        useFigure = -1;
                    }
                }
                else
                {
                    testPojede(figures[numOfFigure], newPosition, figures[numOfFigure].moves);
                    figures[numOfFigure].x = sprites[newPosition % 80].x;
                    figures[numOfFigure].y = sprites[newPosition % 80].y;
                    figures[numOfFigure + 4].x = sprites[newPosition % 80].x;
                    figures[numOfFigure + 4].y = sprites[newPosition % 80].y;
                    if (activePlayer == 0 && newPosition % 80 > 68 && newPosition % 80 < 80)
                        figures[numOfFigure].near_house = true;
                    else if (activePlayer == 1 && newPosition % 80 > 8 && newPosition % 80 < 20)
                        figures[numOfFigure].near_house = true;
                    else if (activePlayer == 2 && newPosition % 80 > 28 && newPosition % 80 < 40)
                        figures[numOfFigure].near_house = true;
                    else if (activePlayer == 3 && newPosition % 80 > 48 && newPosition % 80 < 60)
                        figures[numOfFigure].near_house = true;
                    figures[numOfFigure].pathPosition = newPosition % 80;

                   

                    moves = 0;
                    WaitMessage = 0;
                    useFigure = -1;
                }
            }
            else
            {
                FigureWait = true;
                WaitMessage = 1;
            }
        }

        public int GetFirstPositionInHouse(Sprite activeFigure, Sprite[] figures, Sprite[] homes, int position) 
        {
            //positon mora ici također u ovisnosti o figurama
            // zuti je 28, zeleni je 48, plavi je 68, crveni 88
            int figureCoordinate = 0;
            int activeFigureCoordinate = 0;


            for (int i = 0; i < 4; i++)
            {
                if (activePlayer == 0 || activePlayer == 2)
                {
                    figureCoordinate = figures[i].y;
                    activeFigureCoordinate = activeFigure.y;
                }
                else if (activePlayer == 1 || activePlayer == 3)
                {
                    figureCoordinate = figures[i].x;
                    activeFigureCoordinate = activeFigure.x;
                }


                if (figureCoordinate != activeFigureCoordinate && spriteCollision(figures[i], homes[7]))
                {
                    position -= 2;
                }
                if (figureCoordinate != activeFigureCoordinate && spriteCollision(figures[i], homes[6]))
                {
                    if (activeFigure.pathPosition < position - 4 )
                    {
                        position -= 4;
                    }
                }
                if (figureCoordinate != activeFigureCoordinate && spriteCollision(figures[i], homes[5]))
                {
                    if (activeFigure.pathPosition < position - 6)
                    {
                        position -= 6;
                    }
                }
                if (figureCoordinate != activeFigureCoordinate && spriteCollision(figures[i], homes[4]))
                {
                    if (activeFigure.pathPosition < position - 8)
                    {
                        position -= 8;
                    }
                }
                
            }

            return position;
        }


        public void SendHome(Sprite[] figures, int i) 
        {
            figures[i].moves = false;
            figures[i].x = figures[i].start_x;
            figures[i].y = figures[i].start_y;
            figures[i].near_house = false;
            figures[i + 4].x = figures[i].start_x;
            figures[i + 4].y = figures[i].start_y;

        }

        //provjeravamo ocemo li pojest nekoga u hodu
        public void testPojede(Sprite figure, int newPosition, bool moves)
        {
            if (activePlayer == 1 || activePlayer == 2 || activePlayer == 3)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (!redFigures[i].saved)
                    {
                        if (!moves && redFigures[i].pathPosition == newPosition)
                        {
                            SendHome(redFigures, i);
                        }
                        else if (moves && redFigures[i].moves & redFigures[i].pathPosition > figure.pathPosition & redFigures[i].pathPosition <= newPosition)
                        {
                            SendHome(redFigures, i);
                        }
                    }
                }
            }
            if (activePlayer == 0 || activePlayer == 2 || activePlayer == 3)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (!yellowFigures[i].saved)
                    {
                        if (!moves && yellowFigures[i].pathPosition == newPosition)
                        {
                            SendHome(yellowFigures, i);
                        }
                        else if (moves && yellowFigures[i].moves & yellowFigures[i].pathPosition > figure.pathPosition & yellowFigures[i].pathPosition <= newPosition)
                        {
                            SendHome(yellowFigures, i);
                        }
                    }
                }
            }
            if (activePlayer == 0 || activePlayer == 1 || activePlayer == 3)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (!greenFigures[i].saved)
                    {
                        if (!moves && greenFigures[i].pathPosition == newPosition)
                        {
                            SendHome(greenFigures, i);
                        }
                        else if (moves && greenFigures[i].moves & greenFigures[i].pathPosition > figure.pathPosition & greenFigures[i].pathPosition <= newPosition)
                        {
                            SendHome(greenFigures, i);
                        }
                    }
                }
            }
            if (activePlayer == 0 || activePlayer == 1 || activePlayer == 2)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (!blueFigures[i].saved)
                    {
                        if (!moves && blueFigures[i].pathPosition == newPosition)
                        {
                            SendHome(blueFigures, i);
                        }
                        else if (moves && blueFigures[i].moves & blueFigures[i].pathPosition > figure.pathPosition & blueFigures[i].pathPosition < newPosition)
                        {
                            SendHome(blueFigures, i);
                        }
                    }
                }
            }
   
        }


        public void setImageColorKey(int spriteNum, int r, int g, int b)
        {
            sprites[spriteNum].bmp.MakeTransparent(Color.FromArgb(r, g, b));
        }

        public void setImageColorKey(int spriteNum, Color col)
        {
            sprites[spriteNum].bmp.MakeTransparent(col);
        }

        public void setSpriteVisible(int spriteNum, bool ans)
        {
            sprites[spriteCount].show = ans;
        }

        public void hideSprite(int spriteNum)
        {
            sprites[spriteCount].show = false;
        }


        public void flipSprite(int spriteNum, string fliptype)
        {
            if (fliptype.ToLower() == "none")
                sprites[spriteNum].bmp.RotateFlip(RotateFlipType.RotateNoneFlipNone);

            if (fliptype.ToLower() == "horizontal")
                sprites[spriteNum].bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);

            if (fliptype.ToLower() == "horizontalvertical")
                sprites[spriteNum].bmp.RotateFlip(RotateFlipType.RotateNoneFlipXY);

            if (fliptype.ToLower() == "vertical")
                sprites[spriteNum].bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }

        public void changeSpriteImage(int spriteNum, string file)
        {
            sprites[spriteNum] = new Sprite(file, sprites[spriteNum].x, sprites[spriteNum].y);
        }


        //koristili smo ovo
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        public void hideMouse()
        {
            Cursor.Hide();
        }

        public void showMouse()
        {
            Cursor.Show();
        }



        //This is the beginning of the getter methods

        public bool spriteExist(int spriteNum)
        {
            if (sprites[spriteNum].bmp != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int spriteX(int spriteNum)
        {
            return sprites[spriteNum].x;
        }

        public int spriteY(int spriteNum)
        {
            return sprites[spriteNum].y;
        }

        public int spriteWidth(int spriteNum)
        {
            return sprites[spriteNum].width;
        }

        public int spriteHeight(int spriteNum)
        {
            return sprites[spriteNum].height;
        }

        public bool spriteVisible(int spriteNum)
        {
            return sprites[spriteNum].show;
        }

        public string spriteImage(int spriteNum)
        {
            return sprites[spriteNum].bmp.ToString();
        }

        public bool isKeyPressed(string key)
        {
            if (inkey == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isKeyPressed(Keys key)
        {
            if (inkey == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //provjera key down
        public bool isKeyDown(Keys key)
        {
            if (inkey == key.ToString())
            {
                inkey = "";
                return true;
            }
            else
            {
                return false;
            }
        }
        //koristili
        public bool spriteCollision(Sprite spriteNum1, Sprite spriteNum2)
        {
            Rectangle sp1 = new Rectangle(spriteNum1.x, spriteNum1.y, spriteNum1.width, spriteNum1.height);
            Rectangle sp2 = new Rectangle(spriteNum2.x, spriteNum2.y, spriteNum2.width, spriteNum2.height);
            Collision = Rectangle.Intersect(sp1, sp2);

            if (!Collision.IsEmpty)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        public bool isMousePressed()
        {
            if (mouseKey == 1) return true;
            else return false;
        }

        public int mouseX()
        {
            return mouseXp;
        }

        public int mouseY()
        {
            return mouseYp;
        }

        #endregion


        //Game Update and Input
        #region
        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            foreach (Sprite sprite in sprites)
            {
                if (sprite.bmp != null && sprite.show == true)
                    g.DrawImage(sprite.bmp, new Rectangle(sprite.x, sprite.y, sprite.width, sprite.height));
            }
            //crtanje kuća ispred figurica tako da budu ispod
            foreach (Sprite sprite in redHomes)
            {
                if (sprite.bmp != null && sprite.show == true)
                    g.DrawImage(sprite.bmp, new Rectangle(sprite.x, sprite.y, sprite.width, sprite.height));
            }
            foreach (Sprite sprite in greenHomes)
            {
                if (sprite.bmp != null && sprite.show == true)
                    g.DrawImage(sprite.bmp, new Rectangle(sprite.x, sprite.y, sprite.width, sprite.height));
            }
            foreach (Sprite sprite in yellowHomes)
            {
                if (sprite.bmp != null && sprite.show == true)
                    g.DrawImage(sprite.bmp, new Rectangle(sprite.x, sprite.y, sprite.width, sprite.height));
            }
            foreach (Sprite sprite in blueHomes)
            {
                if (sprite.bmp != null && sprite.show == true)
                    g.DrawImage(sprite.bmp, new Rectangle(sprite.x, sprite.y, sprite.width, sprite.height));
            }
            //crtanje figurica
            foreach (Sprite sprite in redFigures)
            {
                if (sprite.bmp != null && sprite.show == true)
                    g.DrawImage(sprite.bmp, new Rectangle(sprite.x, sprite.y, sprite.width, sprite.height));
            }
            foreach (Sprite sprite in greenFigures)
            {
                if (sprite.bmp != null && sprite.show == true)
                    g.DrawImage(sprite.bmp, new Rectangle(sprite.x, sprite.y, sprite.width, sprite.height));
            }
            foreach (Sprite sprite in yellowFigures)
            {
                if (sprite.bmp != null && sprite.show == true)
                    g.DrawImage(sprite.bmp, new Rectangle(sprite.x, sprite.y, sprite.width, sprite.height));
            }
            foreach (Sprite sprite in blueFigures)
            {
                if (sprite.bmp != null && sprite.show == true)
                    g.DrawImage(sprite.bmp, new Rectangle(sprite.x, sprite.y, sprite.width, sprite.height));
            }
        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            inkey = e.KeyCode.ToString();
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            inkey = "";
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
            mouseKey = 1;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            mouseKey = 1;
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            mouseKey = 0;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseXp = e.X;
            mouseYp = e.Y;
        }

        #endregion


        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 200,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedToolWindow,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen

            };
            Label textLabel = new Label() { Left = 20, Top = 23, Text = text };
            TextBox textBox = new TextBox() { Left = 105, Top = 20, Width = 55, TextAlign = HorizontalAlignment.Center };
            textBox.Text = "2";
            Button confirmation = new Button() { Text = "Potvrdi", Left = 20, Width = 140, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            
            // provjera još za text
            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "0";
        }
    }
}
