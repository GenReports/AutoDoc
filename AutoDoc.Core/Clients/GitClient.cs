using AutoDoc.Core.Models;
using LibGit2Sharp;

namespace AutoDoc.Core.Clients
{
    public static class GitClient
    {
        public static IEnumerable<Models.Commit> GetCommits(
            DateTime start,
            DateTime end,
            AppSettings appSettings)
        {
            using var repo = new LibGit2Sharp.Repository(appSettings.RepositoryPath);

            List<Models.Commit> commits = [];

            foreach (var commit in repo.Commits
                .Where(c => c.Author.Email == appSettings.OwnerEmail &&
                            c.Author.When.DateTime >= start &&
                            c.Author.When.DateTime <= end)
                .OrderBy(c => c.Author.When.DateTime))
            {
                commits.Add(new Models.Commit
                {
                    Message = commit.Message,
                    Tittle = commit.MessageShort,
                    CreatedAt = commit.Author.When.DateTime
                });
            }

            return commits;
        }
    }
}
