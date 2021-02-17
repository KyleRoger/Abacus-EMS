/// \mainpage Electronic Medical System: Technical Specification
/// \tableofcontents
/// 
///
/// \section intro Program Introduction
/// The <b>Electronic Medical System</b> is a prototype of a revolutionary new medical system aimed at
/// providing ease for receptionists and doctors alike. This system will be capable of registering patients,
/// booking appointments and generating billing information for a typical medical clinic in Ontario.
///
///
/// <hr>
/// \section milestone5 Milestone 5 - Test Plan
///
/// \ref EmsUnitTests.EMS_Billing_UnitTests "Billing"
/// <br>
///
/// \ref Demographics.Tests.EMS_UnitTests "Demographics"
/// <br>
///
/// \ref Login.Tests.EMS_UnitTests "Login"
/// <br>
///
/// \ref MenuNavigationManualTesting
///	<br/>
///
/// \ref Permissions.Tests.EMS_UnitTests "Permissions"
/// <br>
///
/// \ref Scheduling.Tests.EMS_UnitTests "Scheduling"
/// <br>
///
/// \ref SchedulingManualTesting
///	<br/>
///
/// \ref EmsUnitTests.Support_UnitTests "Support"
/// <br>
///
///
/// <hr>
/// \section notes Special Release Notes
/// 
/// The Electronic Medical System is a prototype that will be easy to use for medical staff.
///	To give a brief understanding of the system, the following will guide a user through the general process.
/// \n 
/// The EMS Prototype when initiated will open to a login screen to welcome the user. The user must enter in valid
/// credentials in order to gain access to the root of the EMS. Upon entering the main screen the user will be presented with
/// a calendar that will control their choices. Key input functionality has been developed to enabe hotkey functions for ease of use.
/// A user may select a calendar date to book and appointment or view the current appointments on that date. The doctor will
/// have the option from this page to flag a user for recall. 
/// \n
/// The user will have a choice of hotkeys to move around to different areas of the program. The escape Key will always return the user
/// to the main page. All choices will be easily viewable, with their hotkey beside. Add a Patient will send a user to a new 
/// screen to add a patient to the database for future booking of appointments. The search option will enable a user to search
/// for a specific patient throughout the entire database. The user may input as many or as few fields as they would like and all
/// exact matches will be presented. From here the medical user may book an appointment, view a users appointment,
/// modify appointments and modify the user. The billing option will allow a doctor to view billing codes and add billing codes
/// to each appointment. 
/// \n 
/// All of these features will be part of the EMS prototype and allow for a seamless experience for all medical staff.
/// \n
/// \n
///  
/// 
/// 
/// 
///
///
///
///
///
/// <i>Requirements Summary Brief</i>
/// 
///This Project will has been outsourced to the Abacus from the OmniCorp Corporation. The goal is to develop a 
///successful Electronic Medical System that will allow medical staff to transfer from pencil and paper to an
///easy to understand electronic system.
/// <ul>
/// <li>Goals</li>
/// -	To replace an existing system with a more efficient design.
/// -	To log administration information
/// -	To create monthly billing reports
/// -	To generate a physician schedule
/// -	Registering patients.
/// -	Build a successful prototype.
/// -	Robust GUI front end and a well - designed database back end.
/// <li>Project Boundaries</li>
/// -	Must support billing, scheduling, logging, database functionality.
/// -	Does not support Doctor’s reports.
/// -   Does not facilitate payment, only provides billing information.
/// </ul>
///  
///
/// <hr>
/// \bug [known bugs and limitations within the project]
/// - BUG   : Date/Time issues on a machine to machine basis. - fixed by: Arie
/// - ISSUE : Login Does not differentiate between admin, doctor, recptionist. Currently being fixed by: Bailey
/// - ISSUE : Database Generator does not always Generate valid fields. - Currently being fixed by: Kieron
/// - ISSUE : Redundant unneccessary code in AddPatient. - Currently being fixed by: Kyle
/// 
/// <hr>
/// \section version Current version of the EMS :
/// <ul>
/// <li>\author   <b><i>Abacus</i></b></li>
/// <li>\version   1.1.2.1</li>
/// <li>\date      2018</li>
/// <li>\copyright Abacus</li>
/// </ul>
///