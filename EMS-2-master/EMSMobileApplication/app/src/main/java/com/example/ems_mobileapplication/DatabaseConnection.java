// file:	DatabaseConnection.Java
// Author:  Kyle Horsley
// Date:    April 20 ,2019


package com.example.ems_mobileapplication;

import android.os.StrictMode;
import android.util.Log;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;

//Name: DatabaseConnection
//Description: This class contains information to connect to the database and execute queries.
//Author: Kyle Horsley
//Date: 2019-04-22
public class DatabaseConnection {

    //Name: executeNonQuery
    //Description: Executes a query with no return value.
    //Author: Bailey Mills
    //Date: 2019-04-22
    public void executeNonQuery (String query)
    {
        Connection conn;
        try {
            //Get a connection and execute the query.
            conn = connectionClass();
            if (conn == null) {
            } else {
                Statement statement = conn.createStatement();
                statement.execute(query);
                statement.close();
            }
        }
        catch(SQLException sql)
        {
            Log.e("error 1: ", sql.getMessage());
        }
        catch (Exception s)
        {
            Log.e("error 1: ", s.getMessage());
        }
    }

    //Name: queryDatabase
    //Description: This queries the database to return a string
    //Author: Kyle Horsley
    //Date: 2019-04-22
    public String queryDatabase(String query)
    {
        Connection conn;
        String result = null;
        try {
            conn = connectionClass();
            if (conn == null) {
            } else {
                //Get the query in a string.
                Statement statement = conn.createStatement();
                ResultSet resultat = statement.executeQuery(query);
                while (resultat.next()) {

                    result = resultat.getString(1);

                }
                //Close the database.
                resultat.close();
                statement.close();
            }
        }
        catch(SQLException sql)
        {
            Log.e("error 1: ", sql.getMessage());
        }
        catch (Exception s)
        {
            Log.e("error 1: ", s.getMessage());
        }
        return result;
    }

    //Name: connection Class
    //Description: Creates the initial connection to the database.
    //Author: Kyle Horsley
    //Date: 2019-04-22
    public Connection connectionClass()
    {
        Connection connection = null;
        String result = null;
        StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().permitAll().build();
        StrictMode.setThreadPolicy(policy);


        //The connection URL and password.
        String connectionURL = null;
        String password  = "7HQabV9&At#u";
        try
        {
            Class.forName("net.sourceforge.jtds.jdbc.Driver");
            //Where the connection needs to go.
            connectionURL = "jdbc:jtds:sqlserver://abacus-ems2.database.windows.net:1433;DatabaseName=EMS2;user=abacus@abacus-ems2;password=" + password + ";encrypt=true;trustServerCertificate=false;hostNameInCertificate=*.database.windows.net;loginTimeout=30";
            connection = DriverManager.getConnection(connectionURL);

        }
        catch(SQLException sql)
        {
            Log.e("error 1: ", sql.getMessage());
        }
        catch (ClassNotFoundException e)
        {
            Log.e("error 1: ", e.getMessage());
        }
        catch (Exception s)
        {
            Log.e("error 1: ", s.getMessage());
        }

        //Return the connection.
        return connection;
    }
}
