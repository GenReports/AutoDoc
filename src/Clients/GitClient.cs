using LibGit2Sharp;
using AutoDoc.Models;

namespace AutoDoc.Clients
{
    internal static class GitClient
    {
        public static IEnumerable<MyCommit> GetCommits(
            DateTime start,
            DateTime end,
            AppSettings appSettings)
        {
            using var repo = new Repository(appSettings.RepositoryPath);

            List<MyCommit> commits = [];

            foreach (var commit in repo.Commits
                .Where(c => c.Author.Email == appSettings.OwnerEmail &&
                            c.Author.When.DateTime >= start &&
                            c.Author.When.DateTime <= end)
                .OrderBy(c => c.Author.When.DateTime))
            {
                commits.Add(new MyCommit
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
