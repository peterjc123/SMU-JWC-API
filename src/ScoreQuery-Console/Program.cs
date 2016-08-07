using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScoreQuery.ViewModels;
using ScoreQuery.Models;
using System.Threading;
using System.Text;

namespace ScoreQuery_Console
{
    public class Program
    {

        public static void Main(string[] args)
        {
            Process();
            Thread.Sleep(-1);
        }

        private static async Task<LoginWorkerViewModel> Init()
        {
            LoginWorkerViewModel login = new LoginWorkerViewModel();

            await login.Init(true);

            while (!login.WorkerModel.Inited)
            {
                Console.WriteLine("Init failed, press y to retry,otherwise to stop");
                char ch = Console.ReadKey().KeyChar;
                if (ch == 'y')
                {
                    await login.Init(true);
                }
                else
                {
                    Environment.Exit(0);
                }
            }

            return login;
        }

        private static async Task<LoginWorkerViewModel> Login(LoginWorkerViewModel login)
        {
            var user = new UserInfo();

            Console.WriteLine("Please input your student ID:");
            var username = Console.ReadLine();
            Console.WriteLine("Please input your password:");

            StringBuilder sb = new StringBuilder();
            char ch = Console.ReadKey(true).KeyChar;
            while (ch != '\n' && ch != '\r')
            {
                sb.Append(ch);
                ch = Console.ReadKey(true).KeyChar;
            }
            var password = sb.ToString();

            user.UserName = username;
            user.Password = password;

            await login.Login(user);

            while (!login.WorkerModel.Logined)
            {
                Console.WriteLine("Login failed, press y to retry,otherwise to stop");
                ch = Console.ReadKey().KeyChar;
                if (ch == 'y')
                {
                    await login.Login(user);
                }
                else
                {
                    Environment.Exit(0);
                }
            }

            return login;
        }

        private static async Task<PlanWorkerViewModel> GetPlan(LoginWorkerViewModel login)
        {
            var plan = new PlanWorkerViewModel(login);
            await plan.Query();

            var info = plan.WorkerModel.Info;
            Console.WriteLine($"专业:{info.Major}");
            Console.WriteLine($"表现:{info.Performance}");
            Console.WriteLine($"已修学分/应修学分:{info.CreditRatio}");

            return plan;
        }

        private static int GetLetterCount(string str)
        {
            int i = 0;
            foreach(var ch in str)
            {
                var cd = (int)ch;
                if (cd < 127 && cd > 0)
                {
                    i++;
                }
            }
            return i;
        }

        private static void PrintPerformance(QueryWorkerViewModel perf)
        {
            Console.WriteLine($"该学期:{perf.WorkerModel.CurrentSemesterText}");
            Console.WriteLine($"该学期{perf.WorkerModel.GpaText}");
            var list = perf.WorkerModel.ScoreItems;
            var maxLength = list.Max(t => t.CourseName.Length);
            foreach (var item in list)
            {
                Console.WriteLine(string.Format("{0,-" + (maxLength + 4) + "}", item.CourseName) +
                    new string(' ', maxLength - item.CourseName.Length + GetLetterCount(item.CourseName)) +
                $"{item.Credit}学分\t\t{item.Score}\t{item.Grade}");
            }
        }

        private static async Task<QueryWorkerViewModel> GetPerformance(LoginWorkerViewModel login)
        {
            var perf = new QueryWorkerViewModel(login);
            await perf.GetInfo();

            PrintPerformance(perf);

            return perf;
        }

        private static async void Process()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.WriteLine("Welcome to use the JWC-API written by PJC");

            Console.WriteLine("Ready to init");

            var login = await Init();

            Console.WriteLine("Init complete, ready to login");

            login = await Login(login);

            Console.WriteLine("Login complete, ready to get plan");

            var plan = await GetPlan(login);

            Console.WriteLine("Get plan complete, ready to get performance");

            var pref = await GetPerformance(login);

            Console.WriteLine("Get performance complete, press p to get previous, press n to get next, otherwise to shutdown");

            char ch = Console.ReadKey(true).KeyChar;
            while (true)
            {
                if (ch == 'p')
                {
                    await pref.PreviousSemester();
                    PrintPerformance(pref);
                }
                else if (ch == 'n')
                {
                    await pref.NextSemester();
                    PrintPerformance(pref);
                }
                else
                {
                    break;
                }
                Console.WriteLine("Get performance complete, press p to get previous, press n to get next, otherwise to shutdown");
                ch = Console.ReadKey(true).KeyChar;
            }


            Environment.Exit(0);
        }
    }
}
