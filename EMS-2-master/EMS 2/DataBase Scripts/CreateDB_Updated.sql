DROP TABLE IF EXISTS Account;
DROP TABLE IF EXISTS ResponseRecord;
DROP TABLE IF EXISTS Appointment_Billcode;
DROP TABLE IF EXISTS Appointment_Attendee;
DROP TABLE IF EXISTS Appointment;
DROP TABLE IF EXISTS Billcode;
DROP TABLE IF EXISTS MobileRequest;
DROP TABLE IF EXISTS MobileResponse;
DROP TABLE IF EXISTS ResponseRecord;
DROP TABLE IF EXISTS Patient_Dependant;
DROP TABLE IF EXISTS Patient_HoH;
DROP TABLE IF EXISTS Patient;

CREATE TABLE Patient (
	HCN char(12) NOT NULL,
	LastName varchar(255) NOT NULL,
	FirstName varchar(255) NOT NULL,
	MInitial varchar(1),
	DoB DATETIME NOT NULL,
	Gender char NOT NULL,
	PRIMARY KEY (HCN)
);

CREATE TABLE Patient_HoH (
	HoH_HCN char (12),
	AddressLine1 varchar(255),
	AddressLine2 varchar(255),
	City varchar(255),
	PostalCode varchar(7),
	Prov char(2),
	Phone varchar(14),
	PRIMARY KEY (HoH_HCN),
	FOREIGN KEY (HoH_HCN) REFERENCES Patient(HCN)
);

CREATE TABLE Patient_Dependant (
	Dependant_HCN char(12) NOT NULL
		CONSTRAINT FK__Patient_Dependant__Dependant_HCN__Patient__HCN
        FOREIGN KEY (Dependant_HCN) REFERENCES Patient(HCN),
	HoH_HCN char (12) NOT NULL
		CONSTRAINT FK__Patient_Dependant__HoH_HCN__Patient_HoH__HoH_HCN
        FOREIGN KEY (HoH_HCN) REFERENCES Patient_HoH(HoH_HCN),
	PRIMARY KEY (Dependant_HCN)
);


CREATE TABLE Billcode (
	Code char(4) NOT NULL,
	"Date" DATETIME NOT NULL,
	DollarAmount varchar(11) NOT NULL,
	PRIMARY KEY (Code)
);


CREATE TABLE Appointment (
	AppointmentID int IDENTITY (1,1) PRIMARY KEY,
	"Date" DATETIME NOT NULL,
	RecallFlag int NOT NULL,
	MinistryFlag int NOT NULL,
	MobileFlag int NOT NULL
);

CREATE TABLE Appointment_Attendee (
	Attendee_HCN char(12) NOT NULL
		CONSTRAINT FK__Appointment_Attendee__Attendee_HCN__Patient__HCN
        FOREIGN KEY (Attendee_HCN) REFERENCES Patient(HCN),
	AppointmentID int NOT NULL,
	PRIMARY KEY (Attendee_HCN, AppointmentID),
	FOREIGN KEY (AppointmentID) REFERENCES Appointment(AppointmentID)
);

CREATE TABLE Appointment_Billcode (
	AppointmentID int NOT NULL,
	Attendee_HCN char(12) NOT NULL
		CONSTRAINT FK__Appointment_Billcode__Attendee_HCN__Patient__HCN
        FOREIGN KEY (Attendee_HCN) REFERENCES Patient(HCN),
	Code char(4) NOT NULL
	FOREIGN KEY (AppointmentID) REFERENCES Appointment(AppointmentID),
	FOREIGN KEY (Code) REFERENCES Billcode(Code)
);

CREATE TABLE MobileRequest (
	HCN char (12) NOT NULL,
	"Date" DATETIME NOT NULL,
	PRIMARY KEY (HCN),
	FOREIGN KEY (HCN) REFERENCES Patient(HCN)
);

CREATE TABLE MobileResponse (
	HCN char (12) NOT NULL,
	"Status" varchar(255) NOT NULL,
	PRIMARY KEY (HCN),
	FOREIGN KEY (HCN) REFERENCES Patient(HCN)
);

-- Holds the response records from the MOH
-- There is also a trigger that fires after insert.
-- The trigger sets the MinistryFlag in the Appointment table.
CREATE TABLE ResponseRecord (
	appointmentDate DATETIME NOT NULL,
	HCN char(12) NOT NULL,
	gender char NOT NULL,
	Code char(4) NOT NULL,
	fee varchar(11) NOT NULL,
	encounterState CHAR(4) NOT NULL,
	PRIMARY KEY (appointmentDate, HCN, Code),
	FOREIGN KEY (Code) REFERENCES Billcode(Code),
	FOREIGN KEY (HCN) REFERENCES Patient(HCN)
);



-- This trigger sets a flag for records that have a problem in the ResponseRecord table
-- The are the records returned from the MOH.
CREATE TRIGGER checkRecord
	ON ResponseRecord
	AFTER INSERT
AS

BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		DECLARE @currState char(4) = (SELECT encounterState FROM inserted);
		DECLARE @currCode CHAR(4) = (SELECT Code FROM inserted);
		DECLARE @currHCN CHAR(12) = (SELECT HCN FROM inserted);
		DECLARE @currDate DATETIME = (SELECT appointmentDate FROM inserted);

		-- Set flag based on record state
		IF (@currState = 'FHCV')
		BEGIN
			EXEC [dbo].[SetMinistryFlagOn]
				@billCode = @currCode,
				@HCN = @currHCN,
				@appDate = @currDate
		END
		ELSE IF (@currState = 'CMOH')
		BEGIN
			EXEC [dbo].[SetMinistryFlagOn]
				@billCode = @currCode,
				@HCN = @currHCN,
				@appDate = @currDate
		END

		-- Doesn't require the ministry flag to be set
		ELSE
		BEGIN
			EXEC [dbo].[SetMinistryFlagOff]
				@billCode = @currCode,
				@HCN = @currHCN,
				@appDate = @currDate
		END
	END TRY
	BEGIN CATCH
		SELECT 
			ErrorLine = ERROR_LINE(), 
			ErrorMessage = ERROR_MESSAGE(), 
			ErrorNumber = ERROR_NUMBER(), 
			ErrorProcedure = ERROR_PROCEDURE(), 
			ErrorSeverity = ERROR_SEVERITY(), 
			ErrorState = ERROR_STATE()
	END CATCH
END
GO



CREATE TABLE Account (
	"user" varchar(255),
	password varchar(255)
);

INSERT INTO Account ("user", password)
VALUES
	('receptionist', 'password'),
	('admin', 'iambatman');
	
INSERT INTO Patient (HCN, LastName, FirstName, MInitial, DoB, Gender)
VALUES
	('0000000000AA', 'Wayne', 'Bruce', 'A', '1939-05-01 12:00:00 AM', 'M'),
	('1111111111BB', 'Grayson', 'Dick', 'B', '1940-02-01 12:00:00 AM', 'M'),
	('2222222222CC', 'Richards', 'Reed', 'C', '1962-11-01 12:00:00 AM', 'M'),
	('3333333333DD', 'Richards', 'Susan', 'S', '1962-11-01 12:00:00 AM', 'F'),
    ('4444444444EE', 'Grimm', 'Benjamin', 'E', '1962-11-01 12:00:00 AM', 'M'),
    ('5555555555FF', 'Storm', 'Jonathan', 'F', '1962-11-01 12:00:00 AM', 'M'),
	('6666666666GG', 'Thanos', 'Mr.', 'A', '1962-11-01 12:00:00 AM', 'M');

INSERT INTO Patient_HoH (HoH_HCN, AddressLine1, AddressLine2, City, PostalCode, Prov, Phone)
VALUES
	('0000000000AA', '100 Bruce Manor Drive', 'The Cave', 'Gotham', 'A1A1A1', 'ON', '0000000000'),
	('2222222222CC', '42 Fantastic Four HQ', 'New York City', '4th Floor', 'B2B2B2', 'ON', '1111111111'),
	('6666666666GG', '1 Million Trillium Drive', 'Thanaria', 'Retirement', 'C3C3C3', 'ON', '2222222222');

INSERT INTO Patient_Dependant (Dependant_HCN, HoH_HCN)
VALUES
	('1111111111BB', '0000000000AA'),
	('3333333333DD', '2222222222CC'),
	('4444444444EE', '2222222222CC'),
	('5555555555FF', '2222222222CC');

INSERT INTO Appointment ("Date", RecallFlag, MinistryFlag, MobileFlag)
VALUES
	('20190226 12:00:00 AM', 0, 0, 0),
	('20190227 12:00:00 AM', 0, 0, 0),
	('20190228 12:00:00 AM', 0, 0, 0);

INSERT INTO Appointment_Attendee (Attendee_HCN, AppointmentID)
VALUES
	('0000000000AA', 1),
	('4444444444EE', 2),
	('3333333333DD', 3),
	('2222222222CC', 3);
