using LanguageExt;
using LanguageExt.Common;
using LanguageExtExtensions;
using static LanguageExtExtensions.OutExtensions;

namespace EitherExample
{
    public class UserRepository
    {
        private readonly IDictionary<string, User> users = new Dictionary<string, User>();
        private readonly Database database = new Database();

        public User FindDummy(string userId) => database.GetUser(userId);

        public Either<RepositryFailure, User> FindFromRepository(string userId) => 
        Prelude.Try(() => database
            .GetUser(userId))
            .ToEither()
            .MapLeft(f => UserFailure.DatabaseFailure(f));

        public Either<BusinessLogicFailure, User> FindFromCache(string userId) => users
            .TryGetValue2(Key: userId)
            .ToEither(UserFailure.ObjectNotFound(Error.New("Not f")));
    }

    class Database
    {
        public User GetUser(string userId) => new("3", "name");
    }

    public record User(string Id, string Name) { }

    public static class UserFailure
    {
        public static RepositryFailure Fetch(Error error) => new RepositryFailure.Fetch(error);

        public static RepositryFailure DatabaseFailure(Error error) => new RepositryFailure.DatabaseFailure(error);

        public static BusinessLogicFailure ObjectNotFound(Error error) => new BusinessLogicFailure.ObjectNotFound(error);
    }

    public abstract class RepositryFailure
    {
        public Error Error { get; }
        public RepositryFailure(Error error) => Error = error;
        public class Fetch : RepositryFailure { public Fetch(Error error) : base(error) { } }
        public class DatabaseFailure : RepositryFailure { public DatabaseFailure(Error error) : base(error) { } }
        public class ObjectNotFound : RepositryFailure { public ObjectNotFound(Error error) : base(error) { } }
    }
    public abstract class BusinessLogicFailure
    {
        public Error Error { get; }
        public BusinessLogicFailure(Error error) => Error = error;
        public class ObjectNotFound : BusinessLogicFailure { public ObjectNotFound(Error error) : base(error) { } }
    }
}
