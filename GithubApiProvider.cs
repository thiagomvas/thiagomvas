using Octokit;
using System;
using System.Linq;
using System.Threading.Tasks;

public class ApiGithubProvider
{
    private readonly GitHubClient _github;

    public ApiGithubProvider()
    {
        _github = new GitHubClient(new ProductHeaderValue("thiagomvas-readme"));

        // Get the token from environment variable
        string token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

        if (!string.IsNullOrWhiteSpace(token))
        {
            _github.Credentials = new Credentials(token);
        }
        else
        {
            Console.WriteLine("Warning: No GITHUB_TOKEN found. Requests may be rate-limited.");
        }
    }

    public async Task<User> GetUserAsync(string username)
    {
        return await _github.User.Get(username);
    }

    public async Task<Repository[]> GetRepositoriesAsync(string username)
    {
        var repos = await _github.Repository.GetAllForUser(username);
        return repos.ToArray();
    }

    public async Task<int> GetTotalCommitsAsync(string username, Repository[] repos)
    {
        int totalCommits = 0;

        foreach (var repo in repos)
        {
            // Skip forks to avoid double counting
            if (repo.Fork)
                continue;

            try
            {
                var request = new CommitRequest
                {
                    Author = username
                };

                var commits = await _github.Repository.Commit
                    .GetAll(repo.Owner.Login, repo.Name, request);

                Console.WriteLine($"Found {commits.Count} commits on repo {repo.Name}.");

                totalCommits += commits.Count;
            }
            catch
            {
                // Repo might be empty or inaccessible
            }
        }

        return totalCommits;
    }

}

