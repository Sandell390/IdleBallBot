// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using HalconDotNet;

namespace IdleBallBot
{
    internal class Program
    {
        static string SerialNumber = "478c5f15";

        static List<Ball> balls = new List<Ball>();

        static MoneyFormat TotalMoney;

        static int level = 0;

        static HDevEngine hDevEngine = new HDevEngine();

        static HDevProgram hDevProgram = new HDevProgram();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            hDevProgram.LoadProgram("vision.hdev");


            while (true)
            {
                TakeScreenshot();
                RunHdev(1);
                RunHdev(2);
                RunHdev(4);
                ViewData();

                Thread.Sleep(1000);
            }
            Console.ReadLine();
        }

        static void RunHdev(int mode)
        {
            hDevEngine.SetGlobalCtrlVarTuple("mode", mode);

            hDevProgram.Execute();

            switch (mode)
            {
                case 1:
                    string _money = hDevEngine.GetGlobalCtrlVarTuple("moneyString");
                    TotalMoney = new MoneyFormat(_money);
                    break;
                case 2:
                    string _level = hDevEngine.GetGlobalCtrlVarTuple("levelString");

                    string _ = "";
                    for (int i = 0; i < _level.Length; i++)
                    {
                        if (int.TryParse(_level[i].ToString(), out int res))
                        {
                            _ += res;
                        }
                    }
                    level = int.Parse(_);
                    break;
                case 3:
                case 4:
                    HTupleVector vector = hDevEngine.GetGlobalCtrlVarVector("balls");

                    for (int i = 0; i < vector.Length; i++)
                    {
                        for (int k = 0; k < vector[i].Length; k++)
                        {
                            if (vector[i][0].T.S.ToLower().Contains("l0ck"))
                            {
                                //balls.Add(new Ball() { Name = "Unlock new ball"});
                                continue;
                            }

                            string name = vector[i][vector[0].Length - 1].T.S;
                            if (string.IsNullOrEmpty(name))
                                continue;

                            HTuple tupleAmount = vector[i][0].T;
                            HTuple tupleSpeed = vector[i][1].T;
                            HTuple tuplePower = vector[i][2].T;

                            Upgrades BallAmount = new Upgrades(tupleAmount.ToSArr());
                            Upgrades Speed = new Upgrades(tupleSpeed.ToSArr());
                            Upgrades Power = new Upgrades(tuplePower.ToSArr());


                            Ball ball = new Ball() { Balls = BallAmount, Name = name, Power = Power, Speed = Speed };


                            if (!balls.Exists(x => x.Name.ToLower() == ball.Name.ToLower()))
                            {
                                balls.Add(ball);
                            }
                            else
                            {
                                balls[balls.FindIndex(x => x.Name.ToLower() == ball.Name.ToLower())].Power = Power;
                                balls[balls.FindIndex(x => x.Name.ToLower() == ball.Name.ToLower())].Balls = BallAmount;
                                balls[balls.FindIndex(x => x.Name.ToLower() == ball.Name.ToLower())].Speed = Speed;
                            }
                            break;
                        }
                        
                    }
                    break;
            }
        }

        static void TakeScreenshot()
        {
            ExecuteCommand($"adb -s {SerialNumber} exec-out screencap -p > screen.png");
        }

        static void ExecuteCommand(string Command)
        {
            ProcessStartInfo ProcessInfo;
            Process Process;

            ProcessInfo = new ProcessStartInfo("cmd.exe", "/c " + Command);
            ProcessInfo.CreateNoWindow = true;
            ProcessInfo.UseShellExecute = false;

            Process = Process.Start(ProcessInfo);
            Process.WaitForExit();
        }

        static void ViewData()
        {
            Console.Clear();
            Console.WriteLine("Ball Bot");
            Console.WriteLine();
            Console.WriteLine("Money: " + TotalMoney.ToString());
            Console.WriteLine("Level: " + level.ToString());
            Console.WriteLine();
            Console.WriteLine("Balls: ");
            foreach (var ball in balls)
            {
                Console.WriteLine("--------------------------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine("Name: " + ball.Name);
                if (!ball.Name.Contains("Unlock"))
                {
                    Console.Write("Amount: " + ball.Balls.LevelPower + " | Cost: ");
                    Console.ForegroundColor = MoneyFormat.Compare(TotalMoney , ball.Balls.MoneyFormat) ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.Write(ball.Balls.MoneyFormat.ToString());
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.White;

                    Console.Write("Speed: " + ball.Speed.Level + " | Levelpower: " + ball.Speed.LevelPower + " | Cost: ");
                    Console.ForegroundColor = MoneyFormat.Compare(TotalMoney, ball.Speed.MoneyFormat) ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.Write(ball.Speed.MoneyFormat.ToString());
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.White;

                    Console.Write("Power: " + ball.Speed.Level + " | Levelpower: " + ball.Power.LevelPower + " | Cost: ");
                    Console.ForegroundColor = MoneyFormat.Compare(TotalMoney, ball.Power.MoneyFormat) ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.Write(ball.Power.MoneyFormat.ToString());
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine();

                
            }

            Console.WriteLine("--------------------------------------------------------------------------");
        }
    }

}











