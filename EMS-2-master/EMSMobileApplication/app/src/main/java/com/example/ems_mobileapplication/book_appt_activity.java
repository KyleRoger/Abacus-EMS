// file:	book_appt_activity.java
// Author:  Kyle Horsley
// Date:    April 20 ,2019


package com.example.ems_mobileapplication;

import android.content.SharedPreferences;
import android.preference.PreferenceManager;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.Gravity;
import android.view.View;
import android.widget.Button;
import android.widget.CalendarView;
import android.widget.DatePicker;
import android.widget.TextView;
import android.widget.Toast;

import java.sql.Time;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;


//Name: book_appt_activity
//Description: This class contains the java to book an appointment to be looked at by the desktop user..
//Author: Kyle Horsley
//Date: 2019-04-22
public class book_appt_activity extends AppCompatActivity {

    private CalendarView calendarView;
    private Button apptButton;
    private SharedPreferences sharedPreferences;

    //Name: onCreate
    //Description: This creates the initial activity and generates base values.
    //Author: Kyle Horsley
    //Date: 2019-04-22
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_book_appt_activity);

        //Create an instance of a shared preference.
        sharedPreferences = PreferenceManager.getDefaultSharedPreferences(this);

        //Get todats date.
        String currentDateTimeString = DateFormat.getDateInstance(DateFormat.SHORT).format(new Date());
        TextView text = findViewById(R.id.dateText);
        text.setText(currentDateTimeString);

        //Set the min date of the calendar to todays date.
        calendarView = (CalendarView) findViewById(R.id.calendarView);
        calendarView.setMinDate(Calendar.getInstance().getTimeInMillis());

        //Set the text for the date.
        calendarView.setOnDateChangeListener(new CalendarView.OnDateChangeListener() {
            @Override
            public void onSelectedDayChange(CalendarView calendarView, int year, int month, int dayOfMonth) {
                String date;

                if(month < 10)
                {
                  date = year + "-0" + (month + 1) + "-" + dayOfMonth;
                }
                else {
                    date = year + "-" + (month + 1) + "-" + dayOfMonth;
                }

                TextView text = findViewById(R.id.dateText);

                text.setText(date);

            }
        });

        //Select the appointment button lostiner to send appointment
        apptButton = (Button)findViewById(R.id.apptButton);
        apptButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                openSendAppt();
                //Toast.

                //Create a toast message.
                Toast toast = Toast.makeText(getApplicationContext(), "You Will Be Notified Shortly On The Status Of Your Appointment!", Toast.LENGTH_SHORT);

                TextView view = (TextView) toast.getView().findViewById(android.R.id.message);
                if( view != null) view.setGravity(Gravity.CENTER);
                toast.show();
                //Add database send here!!! with the date!!!
            }
        });
    }

    public void openSendAppt()
    {
        //Create a database connection
        DatabaseConnection databaseConnection = new DatabaseConnection();

        //Get the date to send to the desktop app
        TextView date = findViewById(R.id.dateText);
        String dateToSend = date.getText().toString();
        //Set the query string
        String HCN = sharedPreferences.getString("HCN",""); //Health card here.
        String query = "INSERT INTO MobileRequest(HCN, Date) " +
                "VALUES ('" + HCN + "', '" + dateToSend + "')";
        //Execute the query
        databaseConnection.executeNonQuery(query);    }
}
