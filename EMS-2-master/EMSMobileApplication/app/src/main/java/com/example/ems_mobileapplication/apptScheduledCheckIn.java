// file:	apptScheduledCheckIn.Java
// Author:  Kyle Horsley
// Date:    April 20 ,2019

package com.example.ems_mobileapplication;

import android.preference.PreferenceManager;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.Gravity;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.content.SharedPreferences;
import android.widget.Toast;

import java.text.DateFormat;
import java.util.Date;

//Name: apptScheduleCheckIn
//Description: This class contains the java to check  into an appointment.
//Author: Kyle Horsley
//Date: 2019-04-22
public class apptScheduledCheckIn extends AppCompatActivity {

    private Button checkInButton; //Button to check in
    private SharedPreferences sharedPreferences;

    //Name: onCreate
    //Description: This creates the initial activity and generates base values.
    //Author: Kyle Horsley
    //Date: 2019-04-22
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_appt_scheduled_check_in);

        //Get the shared preferences
        sharedPreferences = PreferenceManager.getDefaultSharedPreferences(this);

        //Get the string date
        String currentDateTimeString = DateFormat.getDateInstance().format(new Date());
        TextView text = findViewById(R.id.apptDate);
        text.setText(currentDateTimeString); //Set the text to the current date.

        //Set the button listener
        checkInButton = (Button)findViewById(R.id.checkInButton);
        checkInButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                appointmentCheckIn();
            }
        });

    }

    //Name: appointmentCheckIn
    //Description: Queries the database to set a mobile flag to inform desktop user that the user has checked in.
    //Author: Kyle Horsley
    //Date: 2019-04-22
    public void appointmentCheckIn()
    {
        //Create database connection
        DatabaseConnection databaseConnection = new DatabaseConnection();
        String apptID = sharedPreferences.getString("AppointmentID","");
        String query = "UPDATE Appointment" +
                " SET MobileFlag = 1" +
                " WHERE AppointmentID = '" + apptID + "' AND MobileFlag = 0;";

        //Query the database.
        databaseConnection.executeNonQuery(query);

        //Toast that it was sent
        Toast toast = Toast.makeText(getApplicationContext(), "Thank You, The Doctor Will Be With You Shortly!", Toast.LENGTH_SHORT);

        TextView v = (TextView) toast.getView().findViewById(android.R.id.message);
        if( v != null) v.setGravity(Gravity.CENTER);
        toast.show();
        //Check into appt here..
        //Toast message for confirmation.
    }

}
