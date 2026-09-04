# ST10435609_PROG6212_POE_PART_1

RaceDay is a full-stack web-based event management system built for the South African road running, walking, and cycling community. It replaces the paper-based registration, spreadsheets, and disconnected communication that many local race organisers currently rely on.

The project is being built in three parts:

**Part 1:** (this submission): System planning — ERD, API endpoint plan, and SQL database script.

**Part 2:** RESTful API built with ASP.NET Core Web API (C#), Entity Framework Core, role-based authentication, unit tests, and GitHub Actions CI/CD.

**Part 3:** ASP.NET Core MVC front end consuming the Part 2 API, with Azure Blob Storage for image uploads and Docker containerisation.

## Roles

RaceDay supports two distinct user roles:

**Organiser** — creates, edits, and deletes events; manages event categories; captures participant results; and views all enrolments for the events they manage.

**Participant** — creates an account, browses upcoming events, enters an event by selecting a category, views their own enrolments, and tracks their personal results after events conclude.

## Part 1 Deliverables

The `/docs` folder contains:

- `PROG_ERD.png` (or `.pdf`) — Entity Relationship Diagram with six database entities (Roles, Users, Events, Categories, Enrolments, Results), with primary keys, foreign keys, and cardinality.
- `API_Endpoint_Plan.pdf` — Full endpoint plan covering Authentication, User Profile, Events, Categories, Event Enrolments, and Results.
- `SQLDatabaseScript.sql` — SQL Server script creating the full schema and seeding sample data (2 Organisers, 2 Participants, 3 Events, categories, and enrolments).

## CI/CD

A GitHub Actions workflow (`.github/workflows/validate-docs.yml`) validates that the repository structure is correct — checking that the `/docs` folder exists and contains the ERD, endpoint plan, and SQL script.

**Successful build screenshot:**

<img width="1916" height="1036" alt="CI_CD screenshot" src="https://github.com/user-attachments/assets/45f9d83f-bdea-4299-ba58-ae47347590ac" />

## Video Walkthrough

YouTube Link:  https://youtu.be/UPBPrb7GkD4

The video covers the planning documents, the reasoning behind the ERD design decisions, the endpoint plan choices, and a live run of the SQL script in SSMS.

## AI Tool Disclosure
i used the ai to help configure the github actions then i updated with my file names and how i wanted my files to be structured.
i also used it to help me figure out why my sql scripts were giving me errors on some point and asked it to help me understand the meaning of some errors in ssms

## Reference List 

Coronel, c., Morris, S., Crokett, k. and Blewett, c. (2020) Database Principles: 
Fundamentals of Design, Implementation, and Management. 3rd edn. Andover: Cengage 
Learning.

Troelsen, A. and Japikse, P. (2022) _Pro C 10 with .NET 6: Foundational Principles and Practices in Programming_. 11th edn. Apress.


SQL Server Crash course | Microsoft SQL Server Tutorial 2021. TutorialBrain [video online] Available at: <https://www.youtube.com/watch?v=lo80Q8C0nXk> [Accessed 03 September 
2026].
