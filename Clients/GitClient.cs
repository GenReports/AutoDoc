using LibGit2Sharp;
using AutoDoc.Models;

namespace AutoDoc.Clients
{
    internal static class GitClient
    {
        public const string BaseRepo = @"C:\Users\eduar\Desktop\DoroTech\Nice Acesso\Work\nice-acesso-api";
        public const string Email = "dudumoises2005@gmail.com";

        public static IEnumerable<MyCommit> GetCommits(DateTime start, DateTime end)
        {
            using var repo = new Repository(BaseRepo);

            List<MyCommit> commits = [];

            foreach (var commit in repo.Commits
                .Where(c => c.Author.Email == Email &&
                            c.Author.When.DateTime >= start &&
                            c.Author.When.DateTime <= end))
            {
                commits.Add(new MyCommit
                {
                    Message = commit.Message,
                    Tittle = commit.MessageShort,
                    CreatedAt = commit.Author.When.DateTime
                });
            }

            return commits.OrderBy(c => c.CreatedAt);
        }
    }
}
