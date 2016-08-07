
# SMU-JWC-API
SMU-JWC-API is short for the simplified API for accessing SMU-JWC. You can not only log into the SMU-JWC , but you can also query how you perform in your college years when you study in SMU. It used the .NET Core, which means the code can run on major OS, such as Windows 7 or up, Ubuntu 14.04 or up and OS X 10.10 or up.

# Release
You can also download the latest version for Window, Ubuntu and OS X via [this link](https://github.com/peterjc123/SMU-JWC-API/releases).

# Build
To build this project, you must have the Visual Studio 2015 (only on Windows) or Visual Studio Code and the .Net Core SDK to be installed.

You could just type the command below to make the project work:

```
dotnet restore
dotnet build -c release
dotnet run
```

# Example
It's very easy to use. Suppose you want to log into SMU-JWC:

```C#
LoginWorkerViewModel login = new LoginWorkerViewModel();
await login.Init(true);

var user = new UserInfo() { UserName = "2013111", Password = "xxx"};
await login.Login(user);
```

With these four lines, we're able to access the info of the SMU-JWC.

But if you wonder about how you perform when you're in college?

```c#
var plan = new PlanWorkerViewModel(login);
await plan.Query();

var info = plan.WorkerModel.Info;
Console.WriteLine($"Major:{info.Major}");
Console.WriteLine($"Performance:{info.Performance}");
Console.WriteLine($"Credits got/Credits required:{info.CreditRatio}");
```

  Also you can get your performance term by term.

```C#
var perf = new QueryWorkerViewModel(login);
await perf.GetInfo();

Console.WriteLine($"This semester:{perf.WorkerModel.CurrentSemesterText}");
Console.WriteLine($"This semester {perf.WorkerModel.GpaText}");
var list = perf.WorkerModel.ScoreItems;
var maxLength = list.Max(t => t.CourseName.Length);
foreach (var item in list)
{
	Console.WriteLine($"{item.CourseName}\t{item.Credit}Credit(s)\t{item.Score}\t{item.Grade}");
}
```

For more usage, please go to the [project wiki](https://github.com/peterjc123/SMU-JWC-API/wiki) for more information.