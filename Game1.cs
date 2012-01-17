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
        public int HP, Hunger, HungerLimit, HPLimit;
        public int Coord_X, Coord_Y;
        public char Color;
        public string Status;
        public Sprites Sprite;
        public int LVL;
        public int Speed;

        //Создание новой бактерии 1 уровня (100 HP) со случайным голодом (до 50) и случайным порогом голода (70-100).
        //Бактерия будет создана в пределах координат x=20, y=20. Порог HP=100.
        public Bactery(Random rnd)
        {
            Speed = rnd.Next(3) + 1;
            LVL = 1;
            HP = 100;
            HPLimit = 100;
            Hunger = rnd.Next(50);
            HungerLimit = rnd.Next(70, 100);
            Coord_X = rnd.Next(20);
            Coord_Y = rnd.Next(20);
            Color = (rnd.Next(2) == 1 ? '1' : '0');
            Sprite = new Sprites();
            Status = "Рождение";
            ChekCoord();
        }

        //Создание новой бактерии 1 уровня (100 HP) со случайным голодом (до 50) и случайным порогом голода (70-100).
        //Бактерия будет создана в пределах координат x=m, y=n. Порог HP=100.
        public Bactery(int n, int m, Random rnd)
        {
            Speed = rnd.Next(3) + 1;
            LVL = 1;
            HP = 100;
            HPLimit = 100;
            Hunger = rnd.Next(50);
            HungerLimit = rnd.Next(70, 100);
            Coord_X = rnd.Next(m);
            Coord_Y = rnd.Next(n);
            Color = (rnd.Next(2) == 1 ? '1' : '0');
            Sprite = new Sprites();
            ChekCoord();
        }

        //Создание новой бактерии 1 уровня (100 HP) со случайным голодом (до 50) и случайным порогом голода (70-100).
        //Бактерия будет создана в координатах x=x, y=y. Порог HP=100.
        public Bactery(Random rnd, int y, int x)
        {
            Speed = rnd.Next(3) + 1;
            LVL = 1;
            HP = 100;
            HPLimit = 100;
            Hunger = rnd.Next(50);
            HungerLimit = rnd.Next(70, 100);
            Coord_X = x;
            Coord_Y = y;
            Color = (rnd.Next(2) == 1 ? '1' : '0');
            Sprite = new Sprites();
            ChekCoord();
        }

        //Создание новой бактерии 10/20 уровня (200/300 HP) со случайным голодом (до 10) и случайным порогом голода (70-100).
        //Бактерия будет создана в координатах x=x, y=y. Порог HP=200/300.
        public Bactery(Random rnd, int y, int x, char ch)
        {
            if (ch == '2')
            {
                LVL = 10;
                HP = 200;
                HPLimit = 200;
                Speed = 2;
            }
            else
            {
                LVL = 20;
                HP = 300;
                HPLimit = 300;
                Speed = 1;
            }
            Hunger = rnd.Next(10);
            HungerLimit = rnd.Next(70, 100);
            Coord_X = x;
            Coord_Y = y;
            Color = ch;
            Sprite = new Sprites();
            ChekCoord();
        }

        //Проверка выхода за пределы карты.
        private void ChekCoord()
        {
            if (Coord_X < 0)
                Coord_X = 1;
            if (Coord_Y < 0)
                Coord_Y = 1;
            if (Coord_X > 600)
                Coord_X = 599;
            if (Coord_Y > 600)
                Coord_Y = 599;
        }
        
        //Поедание бактерии с целью утоления голода.
        private int Eat(int HP_eat)
        {
            Hunger -= HP_eat;
            if (Hunger < 0) Hunger = 0;
            HP += HP_eat / 2;
            if (HP > HPLimit)
                HP = HPLimit;
            Status = "Ем";
            return Hunger;
        }
        
        //Проверка голода бактерии и соответствующие действия с ее HP.
        private int HungerMove()
        {
            if (Hunger < ((HungerLimit/100)*80) && HP < HPLimit)
            {
                HP++;
                Hunger++;
            }
            else if (Hunger < HungerLimit)
            {
                Hunger++;
            }
            else if (Hunger >= HungerLimit)
            {
                HP--;
                if (HP <= 0)
                {
                    HP = 0;
                    Color = ' ';
                }
                Hunger++;
            }
            return 1;
        }

        //Движение в случайном направлении.
        public int Move(Random rnd)
        {
            Status = "Гуляю";
            Coord_X += (rnd.Next(10) % 2 == 0 ? 1 : -1) * Speed;
            Coord_Y += (rnd.Next(10) % 2 == 0 ? 1 : -1) * Speed;
            ChekCoord();
            HungerMove();  
            return 1;
        }
        
        //Движение к ближайшей бактерии при охоте/размножении.
        public int MoveToBact(Bactery bact, byte Action)
        {
            Status = (Action == 0 ? "Охочусь" : "Ищу партнера");
            int D1 = Math.Abs(Coord_X - bact.Coord_X);
            int D2 = Math.Abs(Coord_Y - bact.Coord_Y);
            if (D1 != 0 && D2 != 0)
            {
                if (bact.Coord_X > Coord_X && bact.Coord_Y > Coord_Y)
                {
                    Coord_X++;
                    Coord_Y++;
                }
                else if (bact.Coord_X < Coord_X && bact.Coord_Y > Coord_Y)
                {
                    Coord_X--;
                    Coord_Y++;
                }
                else if (bact.Coord_X < Coord_X && bact.Coord_Y < Coord_Y)
                {
                    Coord_X--;
                    Coord_Y--;
                }
                else if (bact.Coord_X > Coord_X && bact.Coord_Y < Coord_Y)
                {
                    Coord_X++;
                    Coord_Y--;
                }
                HungerMove();
            }
            else if (D1 == 0 && D2 != 0)
            {
                if (bact.Coord_Y > Coord_Y) Coord_Y++;
                else Coord_Y--;
                HungerMove();
            }
            else if (D1 != 0 && D2 == 0)
            {
                if (bact.Coord_X > Coord_X) Coord_X++;
                else Coord_X--;
                HungerMove();
            }
            else
            {
                if (Action == 0)
                {
                    ChekCoord();
                    Fight(bact);
                }
                else
                {
                    ChekCoord();
                    return 0;
                }
            }
            ChekCoord();
            return 1;
        }

        //Битва двух бактерий при охоте.
        private int Fight(Bactery bact)
        {
            if (HP >= bact.HP)
            {
                Eat(bact.HP);
                HP -= (HP / 100) * 30;
                if (HP <= 0)
                {
                    HP = 0;
                    Color = ' ';
                }
                else
                {
                    LVL++;
                    HP += HP;
                    if (HP > HPLimit)
                        HP = HPLimit;
                }
                bact.HP = 0;
                bact.Color = ' ';
            }
            else
                bact.Fight(this);
            return 1;
        }
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        static Random rnd = new Random();

        static int n = 600, m = 600;

        char[,] map = new char[n, m];

        List<Bactery> Bact = new List<Bactery>();

        KeyboardState Key = new KeyboardState();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 600;
            Content.RootDirectory = "Content";
            
            InitMap(map);

            int number = 100;

            for (int i = 0; i < number; i++)
            {
                Bact.Add(new Bactery(n, m, rnd));
                if (map[Bact[i].Coord_Y, Bact[i].Coord_X] == '1' || map[Bact[i].Coord_Y, Bact[i].Coord_X] == '0')
                    i--;
            }
            for (int i = 0; i < Bact.Count; i++)
            {
                map[Bact[i].Coord_Y, Bact[i].Coord_X] = Bact[i].Color;
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
                if (Bact[i].Sprite.spriteTexture == null && Bact[i].Color != '2' && Bact[i].Color != '3')
                {
                    if (Bact[i].Color == '0')
                        Bact[i].Sprite.LoadContent(Content, "red");
                    else if (Bact[i].Color == '1')
                        Bact[i].Sprite.LoadContent(Content, "blue");
                }
                else if (Bact[i].Color == '2')
                    Bact[i].Sprite.LoadContent(Content, "black");
                else if (Bact[i].Color == '3')
                    Bact[i].Sprite.LoadContent(Content, "green");
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

            Key = Keyboard.GetState();
            if (Key.IsKeyDown(Keys.Space))
                Bact = AddBact(Bact, rnd);

            GameStadeOne(Bact, map);
            LoadContent();
            for (int i = 0; i < Bact.Count; i++)
            {
                Bact[i].Sprite.spritePosition.X = Bact[i].Coord_X;
                Bact[i].Sprite.spritePosition.Y = Bact[i].Coord_Y;
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            for (int i = 0; i < Bact.Count; i++)
            {
                Bact[i].Sprite.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        /////////////////////////////////////////////////////////////
        //Главный метод первой стадии игры (развитие бактерий)
        //и его вспомогательные методы.
        /////////////////////////////////////////////////////////////
        static void GameStadeOne(List<Bactery> Bact, char[,] map)
        {
            for (int i = 0; i < Bact.Count; i++)
            {
                IfLVLup(Bact[i]);

                if (IfLove(Bact, i, map) == 0)
                    if (IfHunt(Bact, i, map) == 0)
                        IfMove(Bact, i, map);

                Bact = ChekForDelete(Bact);
            }    
        }

        //Получение расстояния между бактериями.
        static int GetLine(List<Bactery> Bact, int i, byte Action)
        {
            int D1, D2, min = 900, sqrt;
            int k = 0;
            for (int j = 0; j < Bact.Count; j++)
            {
                if (Action == 1)
                {
                    if (i != j && Bact[i].Color == Bact[j].Color)
                    {
                        D1 = Math.Abs(Bact[i].Coord_X - Bact[j].Coord_X);
                        D2 = Math.Abs(Bact[i].Coord_X - Bact[j].Coord_X);
                        if ((sqrt = (int)Math.Sqrt(D1 * D1 + D2 * D2)) < min)
                        {
                            min = sqrt;
                            k = j;
                        }
                    }
                }
                else
                {
                    if (i != j && Bact[i].Color != Bact[j].Color)
                    {
                        D1 = Math.Abs(Bact[i].Coord_X - Bact[j].Coord_X);
                        D2 = Math.Abs(Bact[i].Coord_X - Bact[j].Coord_X);
                        if ((sqrt = (int)Math.Sqrt(D1 * D1 + D2 * D2)) < min)
                        {
                            min = sqrt;
                            k = j;
                        }
                    }
                }
            }
            return k;
        }

        //Проверка на удаление "отработавших" бактерий.
        static List<Bactery> ChekForDelete(List<Bactery> bact)
        {
            int summary = 0;
            for (int i = 0; i < bact.Count; i++)
            {
                if (bact[i].HP <= 0 || bact[i].Color == ' ')
                {
                    bact[i].Color = ' ';
                    summary++;
                }
            }
            if (summary != 0)
            {
                bact = DeleteBact(bact);
            }
            return bact;
        }

        //Первоначальная инициализация карты-маски.
        static char[,] InitMap(char[,] map)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    map[i, j] = ' ';
            return map;
        }

        //Нахождение "отработавшей" бактерии.
        static private bool FindEmpty(Bactery bact)
        {
            if (bact.Color == ' ')
                return true;
            else
                return false;
        }

        //Удаление бактерии.
        static List<Bactery> DeleteBact(List<Bactery> bact)
        {
            do
            {
                bact.RemoveAt(bact.FindIndex(FindEmpty));
            } while (bact.FindIndex(FindEmpty) != -1);
            return bact;
        }

        //Размножение бактерий.
        static List<Bactery> LoveBact(List<Bactery> bact, Random rnd, int i_love, int j_love)
        {
            int new_bact = rnd.Next(3) + 1;

            int count = bact.Count;

            int x = 0, y = 0;
            for (int i = bact.Count; i < count + new_bact; i++)
            {
                y = bact[i_love].Coord_Y + (rnd.Next(2) == 0 ? rnd.Next(10) : -rnd.Next(5));
                x = bact[i_love].Coord_Y + (rnd.Next(2) == 0 ? rnd.Next(10) : -rnd.Next(5));
                while (x >= m || y >= n || x <= 0 || y <= 0)
                {
                    y = bact[i_love].Coord_Y + (rnd.Next(2) == 0 ? rnd.Next(10) : -rnd.Next(5));
                    x = bact[i_love].Coord_Y + (rnd.Next(2) == 0 ? rnd.Next(10) : -rnd.Next(5));
                }
                if (bact[i_love].LVL < 10)
                    bact.Add(new Bactery(rnd, y, x));
                else
                {
                    bact.Add(new Bactery(rnd, y, x, bact[i_love].Color));
                }
            }

            bact[i_love].HP -= 50;
            if (bact[i_love].HP <= 0)
            {
                bact.RemoveAt(i_love);
            }

            bact[j_love].HP -= 50;
            if (bact[j_love].HP <= 0)
            {
                bact.RemoveAt(j_love);
            }

            return bact;
        }

        //Проверка необходимости размножения и само размножение.
        static int IfLove(List<Bactery> Bact, int i, char[,] map)
        {
            if (Bact[i].HP >= ((Bact[i].HPLimit/100)*60) && Bact[i].Hunger < Bact[i].HungerLimit && Bact.Count < 300)
            {
                int k = GetLine(Bact, i, 1);

                map[Bact[i].Coord_Y, Bact[i].Coord_X] = ' ';
                if (Bact[i].MoveToBact(Bact[k], 1) == 0)
                {
                    Bact = LoveBact(Bact, rnd, i, k);
                }
                map[Bact[i].Coord_Y, Bact[i].Coord_X] = Bact[i].Color;

                return 1;
            }
            else
            {
                return 0;
            }
        }

        //Проверка необходимости охоты и сама охота.
        static int IfHunt(List<Bactery> Bact, int i, char[,] map)
        {
            if (Bact[i].Hunger >= Bact[i].HungerLimit)
            {
                bool flag = false;

                int k = GetLine(Bact, i, 0);

                if (k > 0)
                    flag = true;

                map[Bact[i].Coord_Y, Bact[i].Coord_X] = ' ';
                if (flag == true)
                {
                    Bact[i].MoveToBact(Bact[k], 0);
                }
                else
                {
                    Bact[i].Move(rnd);
                    int it = 0;
                    while (Bact[i].Coord_Y >= n || Bact[i].Coord_X >= m || Bact[i].Coord_Y <= 0 || Bact[i].Coord_X <= 0 || map[Bact[i].Coord_Y, Bact[i].Coord_X] == '1' || map[Bact[i].Coord_Y, Bact[i].Coord_X] == '0')
                    {
                        if (it > 10)
                            break;
                        Bact[i].Move(rnd);
                        it++;
                    }
                }
                map[Bact[i].Coord_Y, Bact[i].Coord_X] = Bact[i].Color;

                return 1;
            }
            else
            {
                return 0;
            }
        }

        //Проверка необходимости эволюции и сама эволюция.
        static int IfLVLup(Bactery Bact)
        {
            if (Bact.LVL >= 3 && Bact.LVL < 5 && (Bact.Sprite.spriteTexture == null || Bact.Sprite.spriteTexture.Width == 3))
            {
                Bact.Color = '2';
                Bact.Speed = 2;
                return 1;
            }
            else if (Bact.LVL >= 5 && (Bact.Sprite.spriteTexture == null || Bact.Sprite.spriteTexture.Width == 6))
            {
                Bact.Color = '3';
                Bact.Speed = 1;
                return 2;
            }
            return 0;
        }

        //Проверка необходимости погулять и собственно гуляние.
        static int IfMove(List<Bactery> Bact, int i, char[,] map)
        {
            if (Bact[i].Hunger < Bact[i].HungerLimit)
            {
                map[Bact[i].Coord_Y, Bact[i].Coord_X] = ' ';
                Bact[i].Move(rnd);
                while (Bact[i].Coord_Y >= n || Bact[i].Coord_X >= m || Bact[i].Coord_Y <= 0 || Bact[i].Coord_X <= 0 || map[Bact[i].Coord_Y, Bact[i].Coord_X] == '1' || map[Bact[i].Coord_Y, Bact[i].Coord_X] == '0')
                {
                    Bact[i].Move(rnd);
                }
                map[Bact[i].Coord_Y, Bact[i].Coord_X] = Bact[i].Color;
                return 1;
            }
            else
            {
                return 0;
            }
        }

        //Добавть еще бактерий
        static List<Bactery> AddBact(List<Bactery> Bact, Random rnd)
        {
                int k = rnd.Next(10, 30);
                for (int i = 0; i < k; i++)
                {
                    Bact.Add(new Bactery(n, m, rnd));
                }
                return Bact;
        }
    }
}