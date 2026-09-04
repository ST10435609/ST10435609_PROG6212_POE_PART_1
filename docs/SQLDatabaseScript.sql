--creating the database
CREATE DATABASE RaceDayDB
GO

USE RaceDayDB
GO

--Creating Users Table

CREATE TABLE Users (
    UserId    INT IDENTITY(1,1) NOT NULL,
    Role      VARCHAR(20) NOT NULL,
    FullName  VARCHAR(100) NOT NULL,
    Email     VARCHAR(100) NOT NULL,
    PasswordHash  VARCHAR(255) NOT NULL,
    ProfilePictureUrl VARCHAR(255) NULL,
    CreatedAt DATETIME  NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_Users PRIMARY KEY (UserId),
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT CK_Users_Role CHECK (Role IN ('Organiser', 'Participant'))
);


--creating the EventTypes table

CREATE TABLE EventTypes (
    EventTypeId    INT IDENTITY(1,1) NOT NULL,
    EventTypeName  VARCHAR(20)  NOT NULL,
    CONSTRAINT PK_EventTypes PRIMARY KEY (EventTypeId),
    CONSTRAINT UQ_EventTypes_Name UNIQUE (EventTypeName)
);


--creating the Events table

CREATE TABLE Events (
    EventId      INT IDENTITY(1,1) NOT NULL,
    OrganiserId  INT NOT NULL,
    EventTypeId  INT NOT NULL,
    Name         VARCHAR(100) NOT NULL,
    Description  VARCHAR(500) NULL,
    EventDate    DATE NOT NULL,
    Location     VARCHAR(150) NOT NULL,
    Distance     DECIMAL(5,2) NOT NULL,
    BannerImageUrl  VARCHAR(255) NULL,
    CONSTRAINT PK_Events PRIMARY KEY (EventId),
    CONSTRAINT FK_Events_Users FOREIGN KEY (OrganiserId) REFERENCES Users (UserId),
    CONSTRAINT FK_Events_EventTypes FOREIGN KEY (EventTypeId) REFERENCES EventTypes (EventTypeId),
    CONSTRAINT CK_Events_Distance CHECK (Distance > 0)
);


--creating the Categories table

CREATE TABLE Categories (
    CategoryId   INT IDENTITY(1,1) NOT NULL,
    EventId      INT NOT NULL,
    CategoryName VARCHAR(50) NOT NULL,
    CONSTRAINT PK_Categories PRIMARY KEY (CategoryId),
    CONSTRAINT FK_Categories_Events FOREIGN KEY (EventId) REFERENCES Events (EventId)
);

--creating the Enrolments table

CREATE TABLE Enrolments (
    EnrolmentId   INT IDENTITY(1,1) NOT NULL,
    ParticipantId INT NOT NULL,
    EventId       INT NOT NULL,
    CategoryId    INT NOT NULL,
    EnrolmentDate  DATETIME NOT NULL DEFAULT GETDATE(),
    EnrolmentStatus VARCHAR(20) NOT NULL DEFAULT 'Confirmed',
    CONSTRAINT PK_Enrolments PRIMARY KEY (EnrolmentId),
    CONSTRAINT FK_Enrolments_Users FOREIGN KEY (ParticipantId) REFERENCES Users (UserId),
    CONSTRAINT FK_Enrolments_Events FOREIGN KEY (EventId) REFERENCES Events (EventId),
    CONSTRAINT FK_Enrolments_Categories FOREIGN KEY (CategoryId) REFERENCES Categories (CategoryId),
    -- a participant can only enrol once per event
    CONSTRAINT UQ_Enrolments_Participant_Event UNIQUE (ParticipantId, EventId)
);

--creating the Results table

CREATE TABLE Results (
    ResultId INT IDENTITY(1,1) NOT NULL,
    EnrolmentId INT  NOT NULL,
    FinishTime TIME  NOT NULL,
    FinishingPosition  INT NOT NULL,
    CONSTRAINT PK_Results PRIMARY KEY (ResultId),
    CONSTRAINT UQ_Results_EnrolmentId UNIQUE (EnrolmentId),
    CONSTRAINT FK_Results_Enrolments FOREIGN KEY (EnrolmentId) REFERENCES Enrolments (EnrolmentId),
    CONSTRAINT CK_Results_FinishingPosition CHECK (FinishingPosition > 0)
);

--adding valules and sample data

--Users sample data 2 Organisers and 2 Participants

INSERT INTO Users (Role, FullName, Email, PasswordHash, ProfilePictureUrl, CreatedAt) VALUES
('Organiser',   'Thabo Nkosi',      'thabo.nkosi@gmail.com',     'Password_Hash_1', NULL, GETDATE()),
('Organiser',   'Lindiwe Mokoena',  'lindiwe.mokoena@gmail.com', 'Password_Hash_2', NULL, GETDATE()),
('Participant', 'Sipho Dlamini',    'sipho.dlamini@gmail.com',     'Password_Hash_3', NULL, GETDATE()),
('Participant', 'Naledi Khumalo',   'naledi.khumalo@gmail.com',    'Password_Hash_4', NULL, GETDATE());

--adding EventTypes

INSERT INTO EventTypes (EventTypeName) VALUES
('Run'),
('Walk'),
('Cycle');

--adding 3 events sample data

INSERT INTO Events (OrganiserId, EventTypeId, Name, Description, EventDate, Location, Distance, BannerImageUrl) VALUES
(1, 1, 'Joburg City 10K',           'A fast, flat 10km road race through the Johannesburg CBD.',   '2026-10-18', 'Johannesburg, Gauteng', 10.00, NULL),
(1, 3, 'Vaal Dam Cycle Challenge',  'A scenic 42km cycling route around the Vaal Dam.',            '2026-11-08', 'Vaal Dam, Gauteng',     42.00, NULL),
(2, 2, 'Soweto Heritage Walk',      'A community charity walk through historic Soweto.',           '2026-09-24', 'Soweto, Gauteng',        5.00, NULL);


--adding Categories sample data

INSERT INTO Categories (EventId, CategoryName) VALUES
(1, 'Under 20'),
(1, 'Senior'),
(1, 'Veteran (50+)'),
(2, '42km Individual'),
(2, '42km Team'),
(3, '5km Walk - Open');


--Enrolments for participants entering events under a category

INSERT INTO Enrolments (ParticipantId, EventId, CategoryId, EnrolmentDate, EnrolmentStatus) VALUES
(3, 1, 2, GETDATE(), 'Confirmed'),  -- Sipho enrols in Joburg City 10K, Senior category
(4, 1, 1, GETDATE(), 'Confirmed'),  -- Naledi enrols in Joburg City 10K, Under 20 category
(3, 3, 6, GETDATE(), 'Confirmed');  -- Sipho enrols in Soweto Heritage Walk


--sample result for a completed enrolment

INSERT INTO Results (EnrolmentId, FinishTime, FinishingPosition) VALUES
(1, '00:42:15', 47);


--viewing the table values
SELECT * FROM  Users
SELECT * FROM  EventTypes
SELECT * FROM  Events
SELECT * FROM  Categories
SELECT * FROM  Enrolments
SELECT * FROM  Results

--using joins

SELECT

u.FullName  AS Participant,
e.Name      AS EventName,
c.CategoryName,
en.EnrolmentStatus,
r.FinishTime,
r.FinishingPosition

FROM Enrolments en
JOIN Users u        ON en.ParticipantId = u.UserId
JOIN Events e       ON en.EventId = e.EventId
JOIN Categories c   ON en.CategoryId = c.CategoryId
LEFT JOIN Results r ON r.EnrolmentId = en.EnrolmentId;