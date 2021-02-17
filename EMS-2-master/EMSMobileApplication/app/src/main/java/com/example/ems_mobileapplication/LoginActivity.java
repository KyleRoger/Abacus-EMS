// file:	LoginActivity.Java
// Author:  Kyle Horsley
// Date:    April 20 ,2019


package com.example.ems_mobileapplication;

import android.app.Activity;
import android.content.Intent;
import android.os.StrictMode;
import android.preference.PreferenceManager;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.List;

import android.content.SharedPreferences;

//Name: Login Activity
//Description: This class contains the methods to make the Login activity function and
//              connects to the SQL database to ensure a valid user is selected.
//Author: Kyle Horsley
//Date: 2019-04-22
public class LoginActivity extends AppCompatActivity {

    private Button loginButton;  //the login button
    private SharedPreferences sharedPreferences; //Connect to the shared preferences.

    //Name: onCreate
    //Description: This creates the initial activity and generates base values.
    //Author: Kyle Horsley
    //Date: 2019-04-22
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.login_main);

        //Access shared preferences
        sharedPreferences = PreferenceManager.getDefaultSharedPreferences(this);

        loginButton = (Button)findViewById(R.id.loginButton); //Set login button to call function to open main page.
        loginButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                EditText hcnEntry = (EditText)findViewById(R.id.HCNEntry); //Get the entries from the user.
                EditText firstName = (EditText)findViewById(R.id.txtName);

                //Query the database
                String query = ("SELECT HCN FROM Patient WHERE HCN = '" + hcnEntry.getText().toString() + "' AND FirstName = '" + firstName.getText().toString() + "';");
                String returnVal = null;

                //Create a new connection
                DatabaseConnection databaseConnection = new DatabaseConnection();
                boolean isValid = false;
                returnVal = databaseConnection.queryDatabase(query);

                //Set validator here with database
                if(returnVal != null) {
                    // Save HCN
                    sharedPreferences.edit().putString("HCN", returnVal).commit();

                    //Intent serviceIntent = new Intent();
                    Intent intent = new Intent(v.getContext(), NotificationService.class);
                    startService(intent);

                    // Load the next page
                    openMainPageActivity();
                }
                else
                {
                    //No valid health card.. Create a toast Message.
                    Toast.makeText(getApplicationContext(), "Name and or health card does not match our records", Toast.LENGTH_SHORT).show();
                }
            }
        });
    }

    //Name: openMainPageActivity
    //Description: Opens the main page activity.
    //Author: Kyle Horsley
    //Date: 2019-04-22
    public void openMainPageActivity(){
        Intent intent = new Intent(this, MainPageActivity.class);
        startActivity(intent);
    }

}
