namespace Api.Accessors{
    public interface IQuoteAccessor{
        Quote[] GetQuotes();
    }
    public class QuoteAccessor : IQuoteAccessor {
        private readonly Configuration.DatabaseConfiguration _databaseConfiguration;
        public QuoteAccessor(Configuration.DatabaseConfiguration databaseConfiguration){
            _databaseConfiguration = databaseConfiguration;
        }
        public Quote[] GetQuotes(){
            using var db = new Database.QuoteContext(_databaseConfiguration);
            return db.Quotes
                .AsEnumerable()
                .Select(a=> new Quote {Source = a.Source ?? "Anonymous", Text=a.Text})
                .ToArray();
        }
    }
}
