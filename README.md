# School Management System

A desktop-based School Management System developed using **C#, .NET, Windows Forms, and SQL Server**. The system is designed to centralize and simplify common school administration tasks, including student and teacher management, attendance, examinations, fees, grading, and reporting.

## Features

### Student Management

* Add, update, view, and manage student records.
* Maintain student personal and academic information.
* Organize students according to classes, grades, or academic groups.
* Search and filter student records.
* Maintain student enrollment and related academic information.

### Teacher Management

* Manage teacher profiles and information.
* Add, update, and view teacher records.
* Associate teachers with subjects or classes.
* Maintain teacher-related academic and administrative data.

### Attendance Management

* Record daily student attendance.
* Track attendance by student, class, and date.
* View attendance history.
* Monitor attendance records for individual students.
* Generate attendance-related reports.

### Exams & Assessments

* Manage exams and assessment records.
* Record student marks and assessment results.
* Associate exams with subjects and classes.
* Track academic performance across different assessments.
* Update and review examination results.

### Fees & Payments

* Manage student fee records.
* Record payments and payment history.
* Track outstanding and completed payments.
* Maintain financial information associated with students.
* Support fee-related reporting and monitoring.

### Reports & Grading System

* Generate academic and administrative reports.
* Display student grades and examination results.
* Calculate and organize academic performance information.
* Provide grading information based on recorded marks.
* Support monitoring of student academic progress.

## Technology Stack

* **Programming Language:** C#
* **Framework:** .NET
* **UI Framework:** Windows Forms
* **Database:** Microsoft SQL Server
* **Data Access:** SQL-based database operations
* **Development Environment:** Visual Studio

## Architecture & Code Organization

The application follows a modular structure designed to keep different areas of the system separated and easier to maintain.

### Separation of Concerns

The system separates responsibilities between different parts of the application, including:

* **Presentation Layer:** Windows Forms responsible for the user interface and user interactions.
* **Business Logic:** Application logic responsible for processing operations and enforcing business rules.
* **Data Access:** Database-related operations responsible for communicating with SQL Server.
* **Models/Entities:** Classes representing system data such as students, teachers, attendance, exams, fees, and grades.

This separation helps reduce dependencies between components and makes the application easier to maintain, debug, and extend.

### Modular Structure

The project is organized into modules based on system functionality. Typical modules include:

* Students
* Teachers
* Attendance
* Exams
* Assessments
* Fees & Payments
* Reports
* Grading
* Database/Data Access
* Shared/Common Components

Each module is responsible for a specific part of the application's functionality, making it easier to locate and modify related code.

## Database

The system uses **Microsoft SQL Server** to persist application data.

The database stores information related to:

* Students
* Teachers
* Classes
* Subjects
* Attendance records
* Exams
* Student marks
* Grades
* Fees
* Payments
* Reports and related academic information

Relationships between the database entities are used to maintain data consistency and allow information to be retrieved across different parts of the system.

## Key Objectives

The main objectives of the system are to:

* Reduce manual school administration work.
* Centralize student and teacher information.
* Simplify attendance management.
* Organize examinations and student assessments.
* Track fees and payment records.
* Provide structured academic and administrative reports.
* Maintain data in a centralized SQL Server database.
* Provide a maintainable and modular desktop application architecture.

## Project Focus

This project demonstrates practical experience with:

* C# application development
* .NET development
* Windows Forms
* Object-oriented programming
* SQL Server database design and integration
* CRUD operations
* Relational data management
* Separation of concerns
* Modular application architecture
* Business logic implementation
* Desktop application development

## Project Status

The system is developed as a functional desktop-based school management application. Additional features and improvements can be added as the system evolves.
