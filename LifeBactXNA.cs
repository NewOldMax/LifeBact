using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Life
{
    class Sprites
    {
        public Texture2D spriteTexture;
        public Vector2 spritePosition;

        public Sprites()
        { }

        public void LoadContent(ContentManager Content, String texture)
        {
            spriteTexture = Content.Load<Texture2D>(texture);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteTexture, spritePosition, Color.White);
        }
    }

    class Bactery
    {
        public byte HP, hunger;
        public int coord_X, coord_Y;
        public char color;
        public string status;
        public Sprites sprite;
        /*
         * Новая бактерия будет иметь 100 HP, случайный голод не более 50,
         * случайный цвет (1 или 0) и случайные координаты.
         */
        public Bactery(Random rnd)
        {
            HP = 100;
            hunger = (byte)rnd.Next(50);
            coord_X = rnd.Next(20);
            coord_Y = rnd.Next(20);
            color = (rnd.Next(2) == 1 ? '1' : '0');
            sprite = new Sprites();
            ChekCoord();
        }
        public Bactery(int n, int m, Random rnd)
        {
            HP = 100;
            hunger = (byte)rnd.Next(50);
            coord_X = rnd.Next(m);
            coord_Y = rnd.Next(n);
            color = (rnd.Next(2) == 1 ? '1' : '0');
            sprite = new Sprites();
            ChekCoord();
        }
        public Bactery(Random rnd, int y, int x)
        {
            HP = 100;
            hunger = (byte)rnd.Next(50);
            coord_X = x;
            coord_Y = y;
            color = (rnd.Next(2) == 1 ? '1' : '0');
            sprite = new Sprites();
            ChekCoord();
        }
        private void ChekCoord()
        {
            if (coord_X < 0)
                coord_X = 1;
            if (coord_Y < 0)
                coord_Y = 1;
        }
        /*
         * При поглощении другой бактерии, бактерия-охотник утоляет
         * свой голод в размере HP бактерии-жертвы, деленному на 2.
         */
        private int Eat(byte HP_eat)
        {
            hunger -= (byte)(HP_eat / 2);
            if (hunger < 0) hunger = 0;
            status = "Ем";
            return hunger;
        }
        /*
         * При движении бактерия будет голодать на -1 в каждый момент времени.
         * Вектор движения выбирается случайным образом.
         * Также у нее будет восстанавливаться HP на +1 в каждый момент времени,
         * если голод ниже 20.
         * Если голод достигает отметки 70, то бактерия теряет голод/10 HP HP в каждый момент
         * времени (70: -7, 80: -8, 90: -9, 100: -10 и т.д.).
         * Будет и метод, заставляющий передвигаться бактерию к указанной точке.
         */
        private int HungerMove()
        {
            if (hunger < 20 && HP < 100)
            {
                HP++;
                hunger++;
            }
            else if (hunger < 70)
            {
                hunger++;
            }
            else if (hunger >= 70)
            {
                if (hunger / 10 > HP)
                {
                    HP = 0;
                    color = ' ';
                }
                else
                {
                    HP -= (byte)(hunger / 10);
                    if (HP < 0)
                    {
                        HP = 0;
                        color = ' ';
                    }
                    hunger++;
                }
            }
            return 1;
        }
        public int Move(Random rnd)
        {
            coord_X += (rnd.Next(10) % 2 == 0 ? 1 : -1) * rnd.Next(2);
            coord_Y += (rnd.Next(10) % 2 == 0 ? 1 : -1) * rnd.Next(2);
            ChekCoord();
            HungerMove();
            status = "Гуляю";
            return 1;
        }
        /*
         * Бактерия начинает охоту в тот момент, когда голод достигает 50.
         * Охота заключается в следовании к ближайшей бактерии иного цвета.
         */
        public int Hunt(Bactery bact)
        {
            status = "Охочусь";
            int D1 = Math.Abs(coord_X - bact.coord_X);
            int D2 = Math.Abs(coord_Y - bact.coord_Y);
            if (D1 != 0 && D2 != 0)
            {
                if (bact.coord_X > coord_X && bact.coord_Y > coord_Y)
                {
                    coord_X++;
                    coord_Y++;
                }
                else if (bact.coord_X < coord_X && bact.coord_Y > coord_Y)
                {
                    coord_X--;
                    coord_Y++;
                }
                else if (bact.coord_X < coord_X && bact.coord_Y < coord_Y)
                {
                    coord_X--;
                    coord_Y--;
                }
                else if (bact.coord_X > coord_X && bact.coord_Y < coord_Y)
                {
                    coord_X++;
                    coord_Y--;
                }
                HungerMove();
            }
            else if (D1 == 0 && D2 != 0)
            {
                if (bact.coord_Y > coord_Y) coord_Y++;
                else coord_Y--;
                HungerMove();
            }
            else if (D1 != 0 && D2 == 0)
            {
                if (bact.coord_X > coord_X) coord_X++;
                else coord_X--;
                HungerMove();
            }
            else
            {
                status = "В битве";
                Fight(bact);
            }
            ChekCoord();
            return 1;
        }
        /*
         * В схватке побеждает та бактерия, у которой больше или HP (при равенстве атакующая бактерия побеждает).
         * У бактерии-победителя отнимается 30% от HP (потрепали в схватке).
         */
        private int Fight(Bactery bact)
        {
            status = "В битве";
            if (HP >= bact.HP)
            {
                Eat(bact.HP);
                HP -= (byte)((HP / 100) * 30);
                if (HP < 0)
                {
                    HP = 0;
                    color = ' ';
                }
                bact.HP = 0;
                bact.color = ' ';
            }
            else
                bact.Fight(this);
            return 1;
        }
        /*
         * Когда у бактерии 100 HP и меньше 40 голода, она ищет партнера для размножения.
         * Поиск заключается в перемещении к ближайшей бактерии своего цвета.
         * Поиск продолжается до тех пор, пока у бактерии не будет голод 40.
         * После размножения появляется от 1 до 3 новых бактерий.
         * Бактерии-родители истощаются (-50 HP) и могут умереть.
         */
        public int MoveLove(Bactery bact)
        {
            status = "Ищу партнера";
            int D1 = Math.Abs(coord_X - bact.coord_X);
            int D2 = Math.Abs(coord_Y - bact.coord_Y);
            if (D1 != 0 && D2 != 0)
            {
                if (bact.coord_X > coord_X && bact.coord_Y > coord_Y)
                {
                    coord_X++;
                    coord_Y++;
                }
                else if (bact.coord_X < coord_X && bact.coord_Y > coord_Y)
                {
                    coord_X--;
                    coord_Y++;
                }
                else if (bact.coord_X < coord_X && bact.coord_Y < coord_Y)
                {
                    coord_X--;
                    coord_Y--;
                }
                else if (bact.coord_X > coord_X && bact.coord_Y < coord_Y)
                {
                    coord_X++;
                    coord_Y--;
                }
                HungerMove();
            }
            else if (D1 == 0 && D2 != 0)
            {
                if (bact.coord_Y > coord_Y) coord_Y++;
                else coord_Y--;
                HungerMove();
            }
            else if (D1 != 0 && D2 == 0)
            {
                if (bact.coord_X > coord_X) coord_X++;
                else coord_X--;
                HungerMove();
            }
            else
            {
                ChekCoord();
                return 0;
            }
            ChekCoord();
            return 1;
        }
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        static Random rnd = new Random();

        static int n = 100, m = 100;

        char[,] map = new char[n, m];

        List<Bactery> Bact = new List<Bactery>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 600;
            Content.RootDirectory = "Content";
            
            InitMap(map);

            int number = 300;

            for (int i = 0; i < number; i++)
            {
                Bact.Add(new Bactery(n, m, rnd));
                if (map[Bact[i].coord_Y, Bact[i].coord_X] == '1' || map[Bact[i].coord_Y, Bact[i].coord_X] == '0')
                    i--;
            }
            for (int i = 0; i < Bact.Count; i++)
            {
                map[Bact[i].coord_Y, Bact[i].coord_X] = Bact[i].color;
            }
        }


        protected override void Initialize()
        {
            base.Initialize();
        }
      
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            for (int i = 0; i < Bact.Count; i++)
            {
                if (Bact[i].sprite.spriteTexture == null)
                {
                    if (Bact[i].color == '0')
                        Bact[i].sprite.LoadContent(Content, "red");
                    else
                        Bact[i].sprite.LoadContent(Content, "blue");
                }
            }
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (Bact.Count == 0)
                this.Exit();

            for (int i = 0; i < Bact.Count; i++)
            {
                if (Bact[i].HP == 100 && Bact[i].hunger < 40)
                {
                    int D1, D2, min = 900, sqrt;
                    int k = 0;
                    for (int j = 0; j < Bact.Count; j++)
                    {
                        if (i != j && Bact[i].color == Bact[j].color)
                        {
                            D1 = Math.Abs(Bact[i].coord_X - Bact[j].coord_X);
                            D2 = Math.Abs(Bact[i].coord_X - Bact[j].coord_X);
                            if ((sqrt = (int)Math.Sqrt(D1 * D1 + D2 * D2)) < min)
                            {
                                min = sqrt;
                                k = j;
                            }
                        }
                    }
                    map[Bact[i].coord_Y, Bact[i].coord_X] = ' ';
                    if (Bact[i].MoveLove(Bact[k]) == 0)
                    {
                        Bact = LoveBact(Bact, rnd, i, k);
                    }
                    map[Bact[i].coord_Y, Bact[i].coord_X] = Bact[i].color;
                }
                else if (Bact[i].hunger < 50)
                {
                    map[Bact[i].coord_Y, Bact[i].coord_X] = ' ';
                    Bact[i].Move(rnd);
                    while (Bact[i].coord_Y >= n || Bact[i].coord_X >= m || Bact[i].coord_Y <= 0 || Bact[i].coord_X <= 0 || map[Bact[i].coord_Y, Bact[i].coord_X] == '1' || map[Bact[i].coord_Y, Bact[i].coord_X] == '0')
                    {
                        Bact[i].Move(rnd);
                    }
                    map[Bact[i].coord_Y, Bact[i].coord_X] = Bact[i].color;
                }
                else
                {
                    bool flag = false;
                    int D1, D2, min = 900, sqrt;
                    int k = 0;
                    for (int j = 0; j < Bact.Count; j++)
                    {
                        if (i != j && Bact[i].color != Bact[j].color)
                        {
                            D1 = Math.Abs(Bact[i].coord_X - Bact[j].coord_X);
                            D2 = Math.Abs(Bact[i].coord_X - Bact[j].coord_X);
                            if ((sqrt = (int)Math.Sqrt(D1 * D1 + D2 * D2)) < min)
                            {
                                min = sqrt;
                                k = j;
                                flag = true;
                            }
                        }
                    }
                    map[Bact[i].coord_Y, Bact[i].coord_X] = ' ';
                    if (flag == true)
                        Bact[i].Hunt(Bact[k]);
                    else
                    {
                        Bact[i].Move(rnd);
                        while (Bact[i].coord_Y >= n || Bact[i].coord_X >= m || Bact[i].coord_Y <= 0 || Bact[i].coord_X <= 0 || map[Bact[i].coord_Y, Bact[i].coord_X] == '1' || map[Bact[i].coord_Y, Bact[i].coord_X] == '0')
                        {
                            Bact[i].Move(rnd);
                        }
                    }
                    map[Bact[i].coord_Y, Bact[i].coord_X] = Bact[i].color;
                }
            }
            int summary = 0;
            for (int i = 0; i < Bact.Count; i++)
            {
                if (Bact[i].HP < 0 || Bact[i].color == ' ')
                {
                    Bact[i].color = ' ';
                    summary++;
                }
            }
            if (summary != 0)
            {
                Bact = DeleteBact(Bact);
            }

            LoadContent();
            for (int i = 0; i < Bact.Count; i++)
            {
                Bact[i].sprite.spritePosition.X = Bact[i].coord_X;
                Bact[i].sprite.spritePosition.Y = Bact[i].coord_Y;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            for (int i = 0; i < Bact.Count; i++)
            {
                Bact[i].sprite.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

    /*    static void ViewMap(char[,] map)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                    Console.Write(map[i, j]);
                Console.WriteLine();
            }
        }*/

        static char[,] InitMap(char[,] map)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    map[i, j] = ' ';
            return map;
        }

        static private bool FindEmpty(Bactery bact)
        {
            if (bact.color == ' ')
                return true;
            else
                return false;
        }

        static List<Bactery> DeleteBact(List<Bactery> bact)
        {
            do
            {
                bact.RemoveAt(bact.FindIndex(FindEmpty));
            } while (bact.FindIndex(FindEmpty) != -1);
            return bact;
        }

        static List<Bactery> LoveBact(List<Bactery> bact, Random rnd, int i_love, int j_love)
        {
            bact[i_love].HP -= 50;
            if (bact[i_love].HP < 0)
            {
                bact.RemoveAt(i_love);
            }

            bact[j_love].HP -= 50;
            if (bact[j_love].HP < 0)
            {
                bact.RemoveAt(j_love);
            }

            int new_bact = rnd.Next(3) + 1;

            if (bact.Count + new_bact > 1000) return bact;

            int count = bact.Count;

            int x = 0, y = 0;
            for (int i = bact.Count; i < count + new_bact; i++)
            {
                y = bact[i_love].coord_Y + (rnd.Next(2) == 0 ? rnd.Next(10) : -rnd.Next(10));
                x = bact[i_love].coord_Y + (rnd.Next(2) == 0 ? rnd.Next(10) : -rnd.Next(10));
                while (x >= m || y >= n)
                {
                    y = bact[i_love].coord_Y + (rnd.Next(2) == 0 ? rnd.Next(10) : -rnd.Next(10));
                    x = bact[i_love].coord_Y + (rnd.Next(2) == 0 ? rnd.Next(10) : -rnd.Next(10));
                }
                bact.Add(new Bactery(rnd, y, x));
            }

            return bact;
        }
    }
}