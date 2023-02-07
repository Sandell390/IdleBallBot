// See https://aka.ms/new-console-template for more information
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HalconDotNet;
using Newtonsoft.Json;

namespace IdleBallBot
{
    internal class Program
    {
        static string SerialNumber = "478c5f15";

        static bool isSwipingUp = false;

        static List<Ball> balls = new List<Ball>();

        static List<Ball> currentBalls = new List<Ball>();

        static MoneyFormat TotalMoney = new MoneyFormat("0.0");

        static int level = 0;

        static HDevEngine hDevEngine = new HDevEngine();

        static HDevProgram hDevProgram = new HDevProgram();

        static MoneyFormat[] NewBallCost = new MoneyFormat[] { new MoneyFormat("1.0k"), new MoneyFormat("7.50K"), new MoneyFormat("175.00K"), new MoneyFormat("15.00M"), new MoneyFormat("400.00B"), new MoneyFormat("10.00q") };

        static int UpgradeCount = 0;

        static int PrestigeCount = 0;

        static int FoundedDiamonds = 0;

        static Stopwatch WorldStopwatch = new Stopwatch();

        static Stopwatch TotalStopwatch = new Stopwatch();

        static List<long> ClearTimes = new List<long>();

        static long AverageClearTime = 0;


        static List<PrestigeStat> PrestigeStats = new List<PrestigeStat>();

        static Random random = new Random();

        static int Mode = 1;

        static bool BallMenuOpen = false;

        static bool Stopped = false;

        static void Main(string[] args)
        {
            //UpgradeCount = 1;

            Console.Title = "Ball Bot";
            Console.WriteLine("Hello World!");
            hDevProgram.LoadProgram("vision.hdev");

            Task.Run(() =>
            {
                while (true)
                {
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.S:
                            if (Stopped)
                            {
                                Stopped = false;
                            }
                            else
                            {
                                Stopped = true;
                                Console.Clear();
                                Console.WriteLine("Stopped!");
                            }
                            break;
                    }
                }

            });

            WorldStopwatch.Start();
            TotalStopwatch.Start();

            while (true)
            {
                while (Stopped)
                {
                    // Do nothing
                }

                switch (Mode)
                {
                    case 0:
                        StandardRun();
                        break;
                    case 1:
                        DiamondRun();
                        break;
                }
            }
            Console.ReadLine();
        }

        static void DiamondRun() 
        {
            TakeScreenshot();
            RunHdev(5);
            if (BallMenuOpen)
            {
                RunHdev(3);
            }
            else
            {
                RunHdev(7);
            }
            ViewData();
            Thread.Sleep(100);
        }

        static void StandardRun()
        {
            int prestigeReset = 0;

            TakeScreenshot();
            RunHdev(5);
            RunHdev(1);
            RunHdev(2);
            RunHdev(3);
            RunHdev(4);
            ViewData();
            if (level > 50)
            {
                Prestige();
                PrestigeCount++;
                ClearTimes.Add(WorldStopwatch.ElapsedMilliseconds);
                long TotalClearTime = 0;
                ClearTimes.ForEach(t => { TotalClearTime += t; });
                AverageClearTime = (TotalClearTime / ClearTimes.Count) / 1000;
                WorldStopwatch.Restart();
            }
            else
            {

                UpgradeBall();
            }



            //prestigeStat.MinTime = prestigeStat.Times.Min();
            //prestigeStat.MaxTime = prestigeStat.Times.Max();
            //foreach (long time in prestigeStat.Times)
            //{
            //    prestigeStat.TotalTime += time;
            //}
            //prestigeStat.AverageTime = prestigeStat.TotalTime / prestigeStat.Times.Count;

            //PrestigeStats.Add(prestigeStat);

            //var jsonString = JsonConvert.SerializeObject(PrestigeStats);

            //File.WriteAllText("Stats.json", jsonString);

            Thread.Sleep(100);
        }

        static void RunHdev(int mode)
        {
            hDevEngine.SetGlobalCtrlVarTuple("mode", mode);

            hDevProgram.Execute();

            switch (mode)
            {
                case 1:
                    string? _money = hDevEngine.GetGlobalCtrlVarTuple("moneyString").S;
                    if (string.IsNullOrEmpty(_money))
                    {
                        TotalMoney = new MoneyFormat("0");
                    }
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
                    if (string.IsNullOrEmpty(_))
                    {
                        level= 0;
                        return;
                    }
                    level = int.Parse(_);
                    break;
                case 7:
                case 3:
                    HTuple diamondTuple = hDevEngine.GetGlobalCtrlVarTuple("foundDiamond");

                    if (diamondTuple.Length > 0)
                    {
                        ExecuteCommand($"adb -s {SerialNumber} shell input tap {Convert.ToInt32(diamondTuple[1].D)} {Convert.ToInt32(diamondTuple[0].D)}");
                        FoundedDiamonds++;
                    }

                    break;
                case 4:
                    currentBalls.Clear();

                    HTupleVector vector = hDevEngine.GetGlobalCtrlVarVector("balls");
                    HTupleVector ballsUpgradeCoords = hDevEngine.GetGlobalCtrlVarVector("ballsUpgradeCoords");
                    
                    for (int i = 0; i < vector.Length; i++)
                    {
                        for (int k = 0; k < vector[i].Length; k++)
                        {
                            if (vector[i][0].T.Length == 0)
                            {
                                continue;
                            }

                            if (vector[i][0].T.S.ToLower().Contains("l0ck"))
                            {
                                currentBalls.Add(new Ball() { Name = "Unlock new ball"});
                                continue;
                            }

                            if (vector[i][vector[i].Length - 1].T.S == null)
                            {
                                continue;
                            }
                            string? name = vector[i]?.At(vector[i].Length - 1)?.T?.S;
                            if (string.IsNullOrEmpty(name))
                                continue;

                            HTuple tupleAmount = vector[i][0].T;
                            HTuple tupleSpeed = vector[i][1].T;
                            HTuple tuplePower = vector[i][2].T;

                            Upgrades BallAmount = new Upgrades(tupleAmount.ToSArr(), ballsUpgradeCoords[i].T[1].D, ballsUpgradeCoords[i].T[0].D);
                            Upgrades Speed = new Upgrades(tupleSpeed.ToSArr(), ballsUpgradeCoords[i].T[3].D, ballsUpgradeCoords[i].T[2].D);
                            Upgrades Power = new Upgrades(tuplePower.ToSArr(), ballsUpgradeCoords[i].T[5].D, ballsUpgradeCoords[i].T[4].D);


                            Ball ball = new Ball() { Balls = BallAmount, Name = name, Power = Power, Speed = Speed, UpdateTimeout = 10 };

                            currentBalls.Add(ball);

                            if (!balls.Exists(x => x.Name.ToLower() == ball.Name.ToLower()))
                            {
                                balls.Add(ball);
                            }
                            else
                            {
                                balls[balls.FindIndex(x => x.Name.ToLower() == ball.Name.ToLower())].Power = Power;
                                balls[balls.FindIndex(x => x.Name.ToLower() == ball.Name.ToLower())].Balls = BallAmount;
                                balls[balls.FindIndex(x => x.Name.ToLower() == ball.Name.ToLower())].Speed = Speed;
                                balls[balls.FindIndex(x => x.Name.ToLower() == ball.Name.ToLower())].UpdateTimeout = 10;
                            }

                            if (i == 0) {
                                for (int a = 0; a < balls.Count; a++)
                                {
                                    
                                    if (balls[a].Name.ToLower() != ball.Name.ToLower())
                                    {
                                        balls[a].UpdateTimeout--;
                                    }
                                    if (balls[a].UpdateTimeout < 0)
                                    {
                                        balls.RemoveAt(a);
                                    }
                                }

                            }
                            break;
                        }
                        
                    }
                    

                    break;
                case 5:
                    HTuple BallMenu = hDevEngine.GetGlobalCtrlVarTuple("isBallMenuOpen");

                    if (BallMenu.D == 0)
                    {
                        if (Mode == 0)
                            ExecuteCommand($"adb -s {SerialNumber} shell input tap 150 2340");

                        BallMenuOpen = false;
                    }
                    else
                    {
                        BallMenuOpen = true;
                    }

                    break;
            }
        }

        static void TakeScreenshot()
        {
            ExecuteCommand($"adb -s {SerialNumber} exec-out screencap -p > screen.png");
        }

        static void TakeScreenshot(string path, string name)
        {
            ExecuteCommand($"adb -s {SerialNumber} exec-out screencap -p > {path}\\{name}.png");
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

        static void UpgradeBall()
        {
            

            List<Upgrades> upgrades = new List<Upgrades>();

            foreach (Ball ball in currentBalls)
            {
                if (ball.Name != "Unlock new ball")
                {
                    upgrades.Add(ball.Power);
                    upgrades.Add(ball.Balls);
                    upgrades.Add(ball.Speed);

                    /*
                    if (MoneyFormat.Compare(TotalMoney, ball.Speed.MoneyFormat))
                    {
                        //Console.WriteLine("Upgrading");
                        ExecuteCommand($"adb -s {SerialNumber} shell input tap {ball.Speed.X} {ball.Speed.Y}");

                        return;
                    }

                    if (MoneyFormat.Compare(TotalMoney, ball.Power.MoneyFormat))
                    {
                        //Console.WriteLine("Upgrading");
                        ExecuteCommand($"adb -s {SerialNumber} shell input tap {ball.Power.X} {ball.Power.Y}");

                        return;
                    }

                    if (MoneyFormat.Compare(TotalMoney, ball.Balls.MoneyFormat) && ball.Balls.LevelPower < 20)
                    {
                        //Console.WriteLine("Upgrading");
                        ExecuteCommand($"adb -s {SerialNumber} shell input tap {ball.Balls.X} {ball.Balls.Y}");

                        return;
                    }
                    */
                }
                //Console.WriteLine("Not upgrade");
            }

            int currentUpgradeIndex = 0;

            for (int i = 0; i < upgrades.Count; i++)
            {
                if (i == currentUpgradeIndex)
                {
                    continue;
                }

                if (MoneyFormat.Compare(upgrades[currentUpgradeIndex].MoneyFormat, upgrades[i].MoneyFormat))
                {
                    currentUpgradeIndex = i;
                    i = 0;
                }
            }

            if (upgrades.Count != 0 && MoneyFormat.Compare(TotalMoney, upgrades[currentUpgradeIndex].MoneyFormat))
            {
                ExecuteCommand($"adb -s {SerialNumber} shell input tap {upgrades[currentUpgradeIndex].X} {upgrades[currentUpgradeIndex].Y}");
                return;
            }

            if (currentBalls.Exists(x => x.Name == "Unlock new ball"))
            {
                if (MoneyFormat.Compare(TotalMoney, NewBallCost[UpgradeCount]))
                {
                    BuyNewBall();
                    UpgradeCount++;
                    return;
                }

                ExecuteCommand($"adb -s {SerialNumber} shell input swipe 250 1876 250 2000");
                isSwipingUp = true;
                return;
            }

            if (currentBalls.Exists(x => x.Name.ToLower() == "basic"))
            {
                ExecuteCommand($"adb -s {SerialNumber} shell input swipe 250 2000 250 1876");

                isSwipingUp = false;
                return;
            }

            if (isSwipingUp)
            {
                ExecuteCommand($"adb -s {SerialNumber} shell input swipe 250 1876 250 2000");

            }
            else
            {
                ExecuteCommand($"adb -s {SerialNumber} shell input swipe 250 2000 250 1876");
            }

        }

        static void BuyNewBall()
        {
            Thread.Sleep(800);
            ExecuteCommand($"adb -s {SerialNumber} shell input tap 500 2150");
            Thread.Sleep(800);
            ExecuteCommand($"adb -s {SerialNumber} shell input tap 800 1250");
            Thread.Sleep(800);
        }

        static void Prestige()
        {
            ExecuteCommand($"adb -s {SerialNumber} shell input tap 960 2333");
            Thread.Sleep(1000);
            ExecuteCommand($"adb -s {SerialNumber} shell input tap 200 1480");
            Thread.Sleep(1000);
            ExecuteCommand($"adb -s {SerialNumber} shell input tap 350 1670");
            Thread.Sleep(1000);
            ExecuteCommand($"adb -s {SerialNumber} shell input tap 700 1530");
            Thread.Sleep(1000);
            ExecuteCommand($"adb -s {SerialNumber} shell input tap 150 2340");

            UpgradeCount = 0;
            balls.Clear();
        }

        static void Prestige(string path, string name)
        {
            ExecuteCommand($"adb -s {SerialNumber} shell input tap 960 2333");
            Thread.Sleep(1000);
            ExecuteCommand($"adb -s {SerialNumber} shell input tap 200 1480");

            TakeScreenshot(path, name);

            Thread.Sleep(1000);
            ExecuteCommand($"adb -s {SerialNumber} shell input tap 350 1670");
            Thread.Sleep(1000);
            ExecuteCommand($"adb -s {SerialNumber} shell input tap 700 1530");
            Thread.Sleep(1000);
            ExecuteCommand($"adb -s {SerialNumber} shell input tap 150 2340");

            UpgradeCount = 0;
            balls.Clear();
        }

        static void ViewData()
        {
            if (Stopped)
                return;
            Console.Clear();

            Console.WriteLine("Ball Bot");
            Console.WriteLine();
            Console.WriteLine("Total Time: " + TotalStopwatch.ElapsedMilliseconds / 1000 + " Seconds");
            Console.WriteLine("Money: " + TotalMoney.ToString());
            Console.WriteLine("Level: " + level.ToString());
            Console.WriteLine("Founded Diamonds: " + FoundedDiamonds.ToString());
            Console.WriteLine("Upgrade count: " + UpgradeCount.ToString());
            Console.WriteLine("Prestige count: " + PrestigeCount.ToString());
            Console.WriteLine("Current world time: " + WorldStopwatch.ElapsedMilliseconds / 1000 + " Seconds");
            Console.WriteLine("Average Clear time: " + AverageClearTime.ToString("00.00"));
            Console.WriteLine();
            Console.WriteLine("Balls: ");
            foreach (var ball in balls)
            {
                Console.WriteLine("--------------------------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine("Name: " + ball.Name + " | Updates Cycles lefted: " + ball.UpdateTimeout);
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

                    Console.Write("Power: " + ball.Power.Level + " | Levelpower: " + ball.Power.LevelPower + " | Cost: ");
                    Console.ForegroundColor = MoneyFormat.Compare(TotalMoney, ball.Power.MoneyFormat) ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.Write(ball.Power.MoneyFormat.ToString());
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.White;
                }
                
                Console.WriteLine();


            }



            Console.WriteLine("--------------------------------------------------------------------------");
            if (currentBalls.Exists(x => x.Name == "Unlock new ball"))
            {
                Console.WriteLine();
                Console.WriteLine("Name: Unlock ball");
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------------------------------");

            }
        }
    }

}






//for (int i = 0; i < 18; i++)
//{
//    PrestigeStat prestigeStat = new PrestigeStat() { Prestige = (i * 10) + 160};
//    prestigeStat.Times = new List<long>();
//    Directory.CreateDirectory("Prestige" + prestigeStat.Prestige);

//    do
//    {
//        TakeScreenshot();
//        RunHdev(5);
//        RunHdev(1);
//        RunHdev(2);
//        RunHdev(3);
//        RunHdev(4);
//        ViewData();
//        if (level > prestigeStat.Prestige)
//        {
//            Prestige("Prestige" + prestigeStat.Prestige, prestigeStat.Times.Count + "screen");
//            PrestigeCount++;
//            ClearTimes.Add(WorldStopwatch.ElapsedMilliseconds);
//            long TotalClearTime = 0;
//            ClearTimes.ForEach(t => { TotalClearTime += t; });
//            AverageClearTime = (TotalClearTime / ClearTimes.Count) / 1000;

//            prestigeStat.Times.Add(WorldStopwatch.ElapsedMilliseconds / 1000);

//            WorldStopwatch.Restart();

//        }
//        else
//        {
//            UpgradeBall();
//        }

//        Thread.Sleep(1500);
//        GC.Collect();
//        GC.WaitForPendingFinalizers();

//    } while (prestigeStat.Times.Count <= 9);




//}




