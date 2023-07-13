CREATE TABLE Quotes(
    Id INT NOT NULL Identity,
    Text nvarchar(max) NOT NULL,
    Source nvarchar(255),
    CONSTRAINT PK_Quotes PRIMARY KEY (Id)
)