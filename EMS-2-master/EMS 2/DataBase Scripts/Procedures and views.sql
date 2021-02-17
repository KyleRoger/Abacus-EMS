/*
**	File			: Procedures and iews.sql
**	Project			: EMS 2
**	Programmers		: Arie Kraayenbrink, Kieron Higgs
**	First Version	: Feb. 2019
**	Descripton		: 
*/




/* Store billing codes */
CREATE PROCEDURE SaveBillCodes
-- Parameter list:
--		BillCode - the 4 digit code
--		EffectDate - the date the code became active
--		Fee	- the doctor fee for the bill code
@BillCode char(4),
@EffectDate datetime,
@Fee varchar(11)
AS
BEGIN
	BEGIN TRY
		INSERT INTO Billcode (Code, Date, DollarAmount) 
			VALUES (@BillCode, @EffectDate, @Fee);
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



-- Funtion
-- Get all bill codes, HCN and gender within date range
CREATE FUNCTION BillReport 
-- Parameter list:
--		startDate - the first day to include in the report
--		endDate - the last day to include in the report
(
	@startDate DATE, 
	@endDate DATE
)
RETURNS TABLE
AS 
RETURN
(
	SELECT attendee_hcn, appointment_billcode.code, gender, appointment.date, DollarAmount
	FROM appointment_billcode 
	INNER JOIN appointment ON appointment.appointmentid = appointment_billcode.appointmentid 
	INNER JOIN Patient ON attendee_hcn = HCN
	INNER JOIN BillCode on appointment_billcode.code = BillCode.code
	WHERE  Appointment.Date >= @startDate AND Appointment.Date <= @endDate
);
GO



-- Turns the ministry flag off in the appointment table
-- This procedure will be called by a trigger when a new record that has failed is inserted into the ResponseRecords table.
CREATE PROCEDURE SetMinistryFlagOn
-- Parameter list:
--		BillCode - the 4 digit code
--		HCN - the health card number
--		appDate - the appointment date
	@billCode CHAR(4),
	@HCN CHAR(12),
	@appDate DATETIME
AS
BEGIN
	BEGIN TRY
		-- Set a flag for review that indicates the MOH file had a problem with this record
		UPDATE Appointment
			SET MinistryFlag = 1
			WHERE AppointmentID = (SELECT AppointmentID FROM Appointment_Billcode WHERE Code = @billCode AND Attendee_HCN = @HCN) AND date = @appDate;
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



-- Turns the ministry flag off in the appointment table
-- This procedure will be called by a trigger.
CREATE PROCEDURE SetMinistryFlagOff
-- Parameter list:
--		BillCode - the 4 digit code
--		HCN - the health card number
--		appDate - the appointment date
	@billCode CHAR(4),
	@HCN CHAR(12),
	@appDate DATETIME
AS
BEGIN
	BEGIN TRY
		UPDATE Appointment
			SET MinistryFlag = 0
			WHERE AppointmentID = (SELECT AppointmentID FROM Appointment_Billcode WHERE Code = @billCode AND Attendee_HCN = @HCN) AND date = @appDate;
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



-- Views

-- Returns all bill codes from the billCodes table
CREATE VIEW viewBillCodes
AS
	SELECT [Code], [Date], [DollarAmount] 
		FROM [dbo].[Billcode]
