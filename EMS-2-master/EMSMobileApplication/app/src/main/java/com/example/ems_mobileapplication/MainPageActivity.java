// file:	MainPageActivity.Java
// Author:  Kyle Horsley
// Date:    April 20 ,2019


package com.example.ems_mobileapplication;

import android.content.Intent;
import android.content.SharedPreferences;
import android.preference.PreferenceManager;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;

import java.text.DateFormat;
import java.util.Date;

//Name: MainPageActivity
//Description: Allows the user to pick between booking an appointment and checking into the database.
//Author: Kyle Horsley
//Date: 2019-04-22
public class MainPageActivity extends AppCompatActivity {

    private Button checkinbutton; //The check in button
    private Button bookapptbutton; //The book appointment button
    private SharedPreferences sharedPreferences; //The shared preferences.

    //Name: onCreate
    //Description: This creates the initial activity and generates base values.
    //Author: Kyle Horsley
    //Date: 2019-04-22
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main_page);

        sharedPreferences = PreferenceManager.getDefaultSharedPreferences(this);

        //Set the on click listeners.
        checkinbutton = (Button)findViewById(R.id.checkinbutton);
        checkinbutton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                openCheckInActivity();
            }
        });

        bookapptbutton = (Button)findViewById(R.id.bookapptbutton);
        bookapptbutton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                openBookApptActivity();
            }
        });
    }

    //Name: openCheckInActivity
    //Description: Opens the check in page, either the appointment today one if they have an appointment,
    //             or the no appointment today if the user does not have an appointment on todays date.
    //Author: Kyle Horsley
    //Date: 2019-04-22
    public void openCheckInActivity(){
        //Create a new database connection
        DatabaseConnection databaseConnection = new DatabaseConnection();
        String date = (DateFormat.getDateInstance(DateFormat.SHORT).format(new Date()));
        String returnVal = null;
        //Query the database.
        String HCN =  sharedPreferences.getString("HCN",""); //Needs to be replaced with initial entered hcn..
        String query = "SELECT Appointment.AppointmentID FROM Appointment INNER JOIN Appointment_Attendee ON Appointment.AppointmentID = Appointment_Attendee.AppointmentID WHERE Attendee_HCN = '" + HCN + "' AND Date = '" + date +"';";
        returnVal = databaseConnection.queryDatabase(query);

        //Insert the appointmentID into shared preferences
        sharedPreferences.edit().putString("AppointmentID", returnVal).commit();

        //If that database returned a value, there is an appointment today,
        if(returnVal != null) {
            Intent intent = new Intent(this, apptScheduledCheckIn.class);
            startActivity(intent);
            //Check in activity.
        }
        else //No appointmenr check in.
        {
            Intent intent = new Intent(this, noApptCheckIn.class);
            startActivity(intent);
        }
    }

    //Name: openBookApptActivity
    //Description: Opens the book appointment page if selected.
    //Author: Kyle Horsley
    //Date: 2019-04-22
    public void openBookApptActivity(){
        Intent intent = new Intent(this, book_appt_activity.class);
        startActivity(intent);
    }


}
