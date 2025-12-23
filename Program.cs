using Octokit;
using System;
using System.Linq;
using System.Threading.Tasks;

string userName = "thiagomvas";
const int TextWidth = 55;

var provider = new ApiGithubProvider();

// Get data
User user = await provider.GetUserAsync(userName);
Repository[] repos = await provider.GetRepositoriesAsync(userName);

// Totals
int totalStars = repos.Sum(r => r.StargazersCount);
int totalRepos = repos.Length;
int totalFollowers = user.Followers;
int totalCommits = await provider.GetTotalCommitsAsync(userName, repos);
//int totalCommits = 2222;
// Output

string[] lines =
[
    "Thiago Vasconcelos ".PadRight(TextWidth, '-'),
    Helpers.DotLine(TextWidth, ("OS", "Nobara Linux 42, Ubuntu Server 24.02")),
    Helpers.DotLine(TextWidth, ("Uptime", Helpers.GetElapsedYMD(new DateTime(2005, 02, 24), DateTime.UtcNow))),
    "",

    Helpers.DotLine(TextWidth, ("Specialties", "DevOps, Full-Stack")),
    Helpers.DotLine(TextWidth, ("Stack", "C#, .NET, JS/TS, React, Docker")),
    Helpers.DotLine(TextWidth, ("Cloud", "AWS, Azure")),
    Helpers.DotLine(TextWidth, ("Data", "PostgreSQL, Redis, RabbitMQ")),
    Helpers.DotLine(TextWidth, ("Arch", "Microservices, CQRS")),
    "",

    Helpers.DotLine(TextWidth, ("Education", "BSc Computer Science, UFS")),
    Helpers.DotLine(TextWidth, ("Career.Interests", "DevOps Engineer, Full-Stack Developer")),

    Helpers.DotLine(TextWidth, ("Hobbies", "Homelabbing, Building Dev Tools, Gaming")),
    "",

    "Contact Info ".PadRight(TextWidth, '-'),
    Helpers.DotLine(TextWidth, ("Email", "thiagomvas@gmail.com")),
    Helpers.DotLine(TextWidth, ("LinkedIn", "thiago-m-vasconcelos")),
    "",

    "GitHub Stats ".PadRight(TextWidth, '-'),
    Helpers.DotLine(TextWidth, ("Repos", totalRepos), ("Stars", totalStars)),
    Helpers.DotLine(TextWidth, ("Commits", totalCommits), ("Followers", totalFollowers)),
];

var ascii = File.ReadAllLines("ascii.txt");
File.WriteAllText("card-dark.svg",
    Helpers.GenerateSvg(ascii, lines, SvgTheme.Dark));

File.WriteAllText("card-light.svg",
    Helpers.GenerateSvg(ascii, lines, SvgTheme.Light));
