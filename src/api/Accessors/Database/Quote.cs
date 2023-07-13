namespace Api.Accessors.Database
{
    public class Quote{
        public required int Id {get;set;}
        public required string Text {get;set;}
        public string? Source{get;set;}
    }
}
