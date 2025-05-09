CREATE TABLE Currency (
Id INT PRIMARY KEY,
Name VARCHAR(100) NOT NULL,
Rate FLOAT(3) NOT NULL
)
CREATE TABLE Country (
Id INT PRIMARY KEY,
Name VARCHAR (100)
)
CREATE TABLE Currency_Country (
Country_Id INT PRIMARY KEY,
Currency_Id INT PRIMARY KEY,
FOREIGN KEY (Country_Id) REFERENCES Country(Id),
FOREIGN KEY (Currency_Id) REFERENCES Currency(Id),
)