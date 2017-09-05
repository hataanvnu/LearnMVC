--create schema Quiz
--go

create table Quiz.Category
(
	CategoryID int identity primary key not null,
	Title nvarchar(30) not null
)

create table Quiz.QuizUnit
(
	QuizUnitID int identity primary key not null,
	InfoTextHeader nvarchar(MAX),
	InfoTextContent nvarchar(MAX),
	[Order] int unique not null,
	CategoryID int foreign key references Quiz.Category(CategoryID)
)

create table Quiz.Question
(
	QuestionID int identity primary key not null,
	QuestionText nvarchar(MAX) not null,
	QuizUnitID int foreign key references Quiz.QuizUnit(QuizUnitID)
)

create table Quiz.Answer
(
	AnswerID int identity primary key not null,
	AnswerText nvarchar(MAX) not null,
	IsCorrect bit not null,
	QuestionID int foreign key references Quiz.Question(QuestionID)
)

create table Quiz.Progress
(
	ProgressID int identity primary key not null,
	MemberID nvarchar(450),
	QuestionID int foreign key references Quiz.Question(QuestionID)
)