namespace Squadio.Common.Models.Errors
{
    public class ErrorFields
    {
        public static class User
        {
            public const string Id = "user.id";
            public const string Name = "user.name";
            public const string Email = "user.email";
        }

        public static class Company
        {
            public const string Id = "company.id";
            public const string Name = "company.name";
        }

        public static class Team
        {
            public const string Id = "team.id";
            public const string Name = "team.name";
        }


        public static class Project
        {
            public const string Id = "project.id";
            public const string Name = "project.name";
        }
    }
}