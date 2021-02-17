/*
* 	Author:			Arie Kraaynbrink, Kieron Higgs
*	File:			Create DB.sql
*	Project:		EMS 2
*	Date:			January 2019
*	Description:	This file is used to create the database for the EMS 2 project.
*/


/* DEBUG: Set @testData to '1' to enable insertion of demo test data via this script */
DECLARE @testData int = 0;
/* DEBUG */

DROP DATABASE IF EXISTS ems_data;
CREATE DATABASE ems_data;
USE ems_data;

DROP TABLE IF EXISTS healthCardNum;
CREATE TABLE healthCardNum (
	
	);


DROP TABLE IF EXISTS Patient;
CREATE TABLE Patient (
	HCN char(14) NOT NULL,
	LastName varchar(255) NOT NULL,
	FirstName varchar(255) NOT NULL,
	DOB DATETIME NOT NULL,
	Gender char NOT NULL,
	HoH char(14),
	AddressLine1 varchar(255),
	AddressLine2 varchar(255),
	City varchar(255),
	PostalCode char(6),
	Prov char(2),
	Phone char(10),
	PRIMARY KEY (HCN)
	);

DROP TABLE IF EXISTS Appointments;
CREATE TABLE Appointment (
	AppointmentID int NOT NULL,
	AppointmentDate DATETIME NOT NULL,
	PrimaryHCN char(14) NOT NULL,
	PrimaryBillCodes varchar(255),
	SecondaryHCN char (14),
	SecondaryBillCodes varchar(255),
	PRIMARY KEY (AppointmentID),
	FOREIGN KEY (PrimaryHCN) REFERENCES Patient(HCN),
	FOREIGN KEY (SecondaryHCN) REFERENCES Patient(HCN)
	);
	
/* DEBUG */
IF (@testData = 0)
BEGIN
	INSERT INTO Patient (HCN, LastName, FirstName, DoB, Gender, HoH, AddressLine1, AddressLine2, City, PostalCode, Prov, Phone)
    VALUES
	  ('12345678901AAA', 'Smith', 'John', '19850101', 'M', null, '123 Normal Street', null, 'Citytown','A1A1A1', 'ON', '1234567890'),
	  ('23456789012BBB', 'Joe', 'Average', '19900202', 'M', null, '234 Pavement Road', 'Apt. A1', 'Village 1A', 'B2B2B2', 'ON', '2345678901'),
	  ('34567890123CCC', 'Doe', 'Jane', '19950303', 'F', null, '345 Long Drive', null, 'The Suburbs', 'C3C3C3', 'ON', '3456789012'),
	  ('45678901234DDD', 'Luck', 'Lady', '20000404', 'F', null, '456 Drury Lane', null, 'Muffinland', 'D4D4D4', 'ON', '4567890123'),
	  ('56789012345EEE', 'Aphrodite', 'Herman', '20050505', 'I', '45678901234DDD',  null, null, null, null, null, null);
	
	INSERT INTO Appointment (AppointmentID, AppointmentDate, PrimaryHCN, PrimaryBillCodes, SecondaryHCN, SecondaryBillCodes)
    VALUES
	  (1000000, '20190227', '12345678901AAA', null, null, null),
	  (1000001, '20190227', '23456789012BBB', null, null, null),
	  (1000002, '20190228', '34567890123CCC', null, null, null),
	  (1000003, '20190228', '56789012345EEE', null, '45678901234DDD', null);
END
/* DEBUG*/