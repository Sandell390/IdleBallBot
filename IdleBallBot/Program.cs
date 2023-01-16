// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            TakeScreenshot();
            RunHdev(1);
            RunHdev(2);
            RunHdev(4);
            ViewData();
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

                        if (vector[i][0].T.S.ToLower().Contains("l0ck"))
                        {
                            balls.Add(new Ball() { Name = "Unlock new ball"});
                            continue;
                        }

                        string name = vector[i][vector[0].Length - 1].ToString();

                        HTuple tupleAmount = vector[i][0].T;
                        HTuple tupleSpeed = vector[i][1].T;
                        HTuple tuplePower = vector[i][2].T;

                        Upgrades BallAmount = new Upgrades(tupleAmount.ToSArr());
                        Upgrades Speed = new Upgrades(tupleSpeed.ToSArr());
                        Upgrades Power = new Upgrades(tuplePower.ToSArr());
                        

                        Ball ball = new Ball() { Balls = BallAmount, Name = name, Power = Power, Speed = Speed };

                        if (!balls.Exists(x => x.Name == ball.Name))
                        {
                            balls.Add(ball);
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
            ProcessInfo.CreateNoWindow = false;
            ProcessInfo.UseShellExecute = true;

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
                    Console.WriteLine("Amount: " + ball.Balls.LevelPower + " | Cost: " + ball.Balls.MoneyFormat.ToString());
                    Console.WriteLine("Speed: " + ball.Speed.Level + " | Levelpower: " + ball.Speed.LevelPower + " | Cost:" + ball.Speed.MoneyFormat.ToString());
                    Console.WriteLine("Power: " + ball.Power.Level + " | Levelpower: " + ball.Power.LevelPower + " | Cost:" + ball.Power.MoneyFormat.ToString());
                }
                Console.WriteLine();
              
            }

            Console.WriteLine("--------------------------------------------------------------------------");
        }
    }

}











