// file:	noApptCheckIn.Java
// Author:  Kyle Horsley
// Date:    April 20 ,2019


package com.example.ems_mobileapplication;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;

import java.text.DateFormat;
import java.util.Date;

//Name: noApptCheckIn
//Description: This creates the page if no appointment is scheduled on todays date.
//Author: Kyle Horsley
//Date: 2019-04-22
public class noApptCheckIn extends AppCompatActivity {

    private Button bookApptButton;

    //Name: onCreate
    //Description: This creates the initial activity and generates base values.
    //Author: Kyle Horsley
    //Date: 2019-04-22
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_no_appt_checkin);

        //Get current date.
        String currentDateTimeString = DateFormat.getDateInstance().format(new Date());
        TextView text = findViewById(R.id.noApptDate);
        text.setText(currentDateTimeString);

        //Book appointment buttton.
        bookApptButton = (Button)findViewById(R.id.bookapptbutton);
        bookApptButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                openBookApptActivity();
            }
        });

    }

    //Name: openBookApptActivity
    //Description: opens the book appointment activity.
    //Author: Kyle Horsley
    //Date: 2019-04-22
    public void openBookApptActivity()
    {
        Intent intent = new Intent(this, book_appt_activity.class);
        startActivity(intent);
    }

}
