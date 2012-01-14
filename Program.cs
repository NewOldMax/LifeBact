using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Life
{
    class Bactery
    {
        public byte HP, hunger;
        public byte coord_X, coord_Y;
        public char color;
        public string status;
        /*
         * Новая бактерия будет иметь 100 HP, случайный голод не более 50,
         * случайный цвет (1 или 0) и случайные координаты.
         */
        public Bactery(Random rnd)
        {
            HP = 100;
            hunger = (byte)rnd.Next(50);
            coord_X = (byte)rnd.Next(10);
            coord_Y = (byte)rnd.Next(20);
            color = (rnd.Next(2) == 1 ? '1' : '0');
        }
        public Bactery(byte n, byte m, Random rnd)
        {
            HP = 100;
            hunger = (byte)rnd.Next(50);
            coord_X = (byte)rnd.Next(m);
            coord_Y = (byte)rnd.Next(n);
            color = (rnd.Next(2) == 1 ? '1' : '0');
        }
        public Bactery(Random rnd, byte y, byte x)
        {
            HP = 100;
            hunger = (byte)rnd.Next(50);
            coord_X = x;
            coord_Y = y;
            color = (rnd.Next(2) == 1 ? '1' : '0');
        }
        /*
         * При поглощении другой бактерии, бактерия-охотник утоляет
         * свой голод в размере HP бактерии-жертвы, деленному на 2.
         */
        private int Eat(byte HP_eat)
        {
            hunger -= (byte)(HP_eat/2);
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
            coord_X += (byte)((rnd.Next(10) % 2 == 0 ? 1 : -1) * rnd.Next(2));
            coord_Y += (byte)((rnd.Next(10) % 2 == 0 ? 1 : -1) * rnd.Next(2));
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
            byte D1 = (byte)Math.Abs(coord_X - bact.coord_X);
            byte D2 = (byte)Math.Abs(coord_Y - bact.coord_Y);
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
                HP -= (byte)((HP/100)*30);
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
         * Когда у бактерии 100 HP и меньше 20 голода, она ищет партнера для размножения.
         * Поиск заключается в перемещении к ближайшей бактерии своего цвета.
         * Поиск продолжается до тех пор, пока у бактерии не будет голод 20.
         * После размножения появляется от 1 до 3 новых бактерий.
         * Бактерии-родители истощаются (-50 HP) и могут умереть.
         */
        public int MoveLove(Bactery bact)
        {
            status = "Ищу партнера";
            byte D1 = (byte)Math.Abs(coord_X - bact.coord_X);
            byte D2 = (byte)Math.Abs(coord_Y - bact.coord_Y);
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
                return 0;
            }
            return 1;
        }
    }

    class Program
    {
        static Random rnd = new Random();

        static byte n = 24, m = 75;

        static void ViewMap(char[,] map)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                    Console.Write(map[i, j]);
                Console.WriteLine();
            }
        }

        static char[,] InitMap(char[,] map)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    map[i, j] = ' ';
            return map;
        }

        static Bactery[] DeleteBact(Bactery[] bact)
        {
            byte len = 0;
            for (int j = 0; j < bact.Length; j++ )
            {
                for (int i = 0; i < bact.Length - 1; i++)
                {
                    if (bact[i].color == ' ' && bact[i + 1].color != ' ')
                    {
                        Bactery temp = bact[i];
                        bact[i] = bact[i + 1];
                        bact[i + 1] = temp;
                    }
                }
            }
            for (int i = 0; i < bact.Length; i++)
            {
                if (bact[i].color == ' ')
                    len++;
            }
            Bactery[] bact_new = new Bactery[bact.Length - len];
            for (int i = 0; i < bact_new.Length; i++)
            {
                bact_new[i] = bact[i];
            }
            return bact_new;
        }

        static Bactery[] LoveBact(Bactery[] bact, Random rnd)
        {
            int i_love = 0, j_love = 0;
            for (int i = 0; i < bact.Length; i++)
            {
                for (int j = 0; j < bact.Length; j++)
                {
                    if (i != j && bact[i].coord_X == bact[j].coord_X && bact[i].coord_Y == bact[j].coord_Y)
                    {
                        i_love = i;
                        j_love = j;
                    }
                }
            }
            int len = bact.Length;
            bact[i_love].HP -= 50;
            if (bact[i_love].HP < 0)
            {
                bact[i_love].HP = 0;
                bact[i_love].color = ' ';
                len--;
            }

            bact[j_love].HP -= 50;
            if (bact[j_love].HP < 0)
            {
                bact[j_love].HP = 0;
                bact[j_love].color = ' ';
                len--;
            }

            Bactery[] new_bact = new Bactery[len + rnd.Next(3) + 1];

            bact.CopyTo(new_bact, 0);

            byte x = 0, y = 0;
            for (int i = len; i < new_bact.Length; i++)
            {
                y = (byte)(bact[i_love].coord_Y + (rnd.Next(2) == 0 ? rnd.Next(10) : -rnd.Next(10)));
                x = (byte)(bact[i_love].coord_Y + (rnd.Next(2) == 0 ? rnd.Next(10) : -rnd.Next(10)));
                while (x >= m || y >= n)
                {
                    y = (byte)(bact[i_love].coord_Y + (rnd.Next(2) == 0 ? rnd.Next(10) : -rnd.Next(10)));
                    x = (byte)(bact[i_love].coord_Y + (rnd.Next(2) == 0 ? rnd.Next(10) : -rnd.Next(10)));
                }
                new_bact[i] = new Bactery(rnd, y, x);
            }

            return new_bact;
        }

        static void Main(string[] args)
        {
            char[,] map = new char[n,m];

            InitMap(map);
            Console.WriteLine("Программа Жизнь v.1.0.23.1911beta");
            Console.WriteLine("Для начала, введите положительное число бактерий для симмуляции, но учтите:\r\nесли их будет много, корректность симуляции и должная скорость работы не гарантируются.");
            Console.WriteLine("Каждая бактерия обладает базовыми инстинктами, такими как голод и желание\r\nпродолжать свой род, которым старается следовать.");
            Console.Write("Введите число бактерий: ");
            int numb = Convert.ToInt32(Console.ReadLine());

            Bactery[] bact = new Bactery[numb];

            for (int i = 0; i < bact.Length; i++)
            {
                bact[i] = new Bactery(n, m, rnd);
                if (map[bact[i].coord_Y, bact[i].coord_X] == '1' || map[bact[i].coord_Y, bact[i].coord_X] == '0')
                    i--;
            }

            for (int i = 0; i < bact.Length; i++)
                map[bact[i].coord_Y, bact[i].coord_X] = bact[i].color;

            Console.Clear();

            ViewMap(map);

            bool flag_s = true, flag_h = true, flag_a = false;
            int it = 0;
            //Основной цикл программы
            do
            {
                for (int i = 0; i < bact.Length; i++)
                {
                    if (bact[i].HP == 100 && bact[i].hunger < 20)
                    {
                        byte D1, D2, min = 128, sqrt;
                        int k = 0;
                        for (int j = 0; j < bact.Length; j++)
                        {
                            if (i != j && bact[i].color == bact[j].color)
                            {
                                D1 = (byte)Math.Abs(bact[i].coord_X - bact[j].coord_X);
                                D2 = (byte)Math.Abs(bact[i].coord_X - bact[j].coord_X);
                                if ((sqrt = (byte)Math.Sqrt(D1 * D1 + D2 * D2)) < min)
                                {
                                    min = sqrt;
                                    k = j;
                                }
                            }
                        }
                        map[bact[i].coord_Y, bact[i].coord_X] = ' ';
                        if (bact[i].MoveLove(bact[k]) == 0)
                        {
                            bact = LoveBact(bact, rnd);
                        }
                        map[bact[i].coord_Y, bact[i].coord_X] = bact[i].color;
                    }
                    else if (bact[i].hunger < 50)
                    {
                        map[bact[i].coord_Y, bact[i].coord_X] = ' ';
                        bact[i].Move(rnd);
                        while (bact[i].coord_Y >= n || bact[i].coord_X >= m || bact[i].coord_Y < 0 || bact[i].coord_X < 0 || map[bact[i].coord_Y, bact[i].coord_X] == '1' || map[bact[i].coord_Y, bact[i].coord_X] == '0')
                        {
                            bact[i].Move(rnd);
                        }
                        map[bact[i].coord_Y, bact[i].coord_X] = bact[i].color;
                    }
                    else
                    {
                        bool flag = false;
                        byte D1, D2, min = 127, sqrt;
                        int k = 0;
                        for (int j = 0; j < bact.Length; j++)
                        {
                            if (i != j && bact[i].color != bact[j].color)
                            {
                                D1 = (byte)Math.Abs(bact[i].coord_X - bact[j].coord_X);
                                D2 = (byte)Math.Abs(bact[i].coord_X - bact[j].coord_X);
                                if ((sqrt = (byte)Math.Sqrt(D1 * D1 + D2 * D2)) < min)
                                {
                                    min = sqrt;
                                    k = j;
                                    flag = true;
                                }
                            }
                        }
                        map[bact[i].coord_Y, bact[i].coord_X] = ' ';
                        if (flag == true)
                            bact[i].Hunt(bact[k]);
                        else
                        {
                            bact[i].Move(rnd);
                            while (bact[i].coord_Y >= n || bact[i].coord_X >= m || bact[i].coord_Y < 0 || bact[i].coord_X < 0 || map[bact[i].coord_Y, bact[i].coord_X] == '1' || map[bact[i].coord_Y, bact[i].coord_X] == '0')
                            {
                                bact[i].Move(rnd);
                            }
                        }
                        map[bact[i].coord_Y, bact[i].coord_X] = bact[i].color;
                    }
                }
                int summary = 0;
                for (int i = 0; i < bact.Length; i++)
                {
                    if (bact[i].HP < 0 || bact[i].color == ' ')
                    {
                        bact[i].color = ' ';
                        summary++;
                    }
                }
                if (summary != 0)
                {
                    bact = DeleteBact(bact);
                }

                Console.Clear();

                ViewMap(map);

                if (bact.Length == 0)
                {
                    Console.WriteLine("Жизнь вымерла, потратив на это время в количестве "+it+". Нажмите любую кнопку для выхода.");
                    Console.ReadKey();
                    break;
                }

                it++;

                if (flag_s && flag_h)
                    for (int i = 0; i < bact.Length; i++)
                    {
                        Console.WriteLine("Бактерия " + (i + 1) + " x=" + bact[i].coord_X + " y=" + bact[i].coord_Y + " Голод=" + bact[i].hunger + " HP=" + bact[i].HP + " Вид=\'" + bact[i].color + "\' Статус:" + bact[i].status);
                    }

                if (flag_h)
                    Console.WriteLine("\r\ns - вкл\\выкл статистику.\r\nh - убрать все надписи\r\nESC - выход.\r\na - автообновление (неотключаемо)\r\nДля следующего ход нажмите любую клавишу.");

                if (!flag_a)
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.Escape: return;
                        case ConsoleKey.S: flag_s = !flag_s; break;
                        case ConsoleKey.H: flag_h = !flag_h; break;
                        case ConsoleKey.A: flag_a = !flag_a; break;
                    }

            } while (true);
        }
    }
}
