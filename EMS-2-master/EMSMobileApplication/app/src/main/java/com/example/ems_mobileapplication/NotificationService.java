// file:	NotificationService.Java
// Author:  Kyle Horsley
// Date:    April 20 ,2019


package com.example.ems_mobileapplication;

import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.Service;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Build;
import android.os.CountDownTimer;
import android.os.IBinder;
import android.os.SystemClock;
import android.preference.PreferenceManager;
import android.view.Gravity;
import android.widget.TextView;
import android.widget.Toast;

import android.os.Handler;
import java.lang.Runnable;

import android.support.v4.app.NotificationCompat;
import android.app.PendingIntent;

import android.os.Looper;

//Name: NotificationService
//Description: Starts the service to check for notifications from the desktop application
//Author: Kyle Horsley
//Date: 2019-04-22
public class NotificationService extends Service {
    SharedPreferences sharedPreferences;
    DatabaseConnection databaseConnection = null;

    //Name: onCreate
    //Description: This creates the initial activity and generates base values.
    //Author: Kyle Horsley
    //Date: 2019-04-22
    @Override
    public void onCreate() {
        super.onCreate();

        sharedPreferences = PreferenceManager.getDefaultSharedPreferences(this);
        databaseConnection = new DatabaseConnection();
    }

    //Name: onCreate
    //Description: This Runs the service continuously
    //Author: Kyle Horsley
    //Date: 2019-04-22
    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        //Creating new thread for my service
        //Always write your long running tasks in a separate thread, to avoid ANR
        new Thread(new Runnable() {
            @Override
            public void run() {
                while (true) {
                    try {
                        Thread.sleep(1000);

                        // Do Stuff
                        String HCN = sharedPreferences.getString("HCN", "");

                        if (HCN != null) {

                            String query = "SELECT Status FROM MobileResponse WHERE HCN='" + HCN + "';";
                            String response = databaseConnection.queryDatabase(query);
                            //SystemClock.sleep(500);

                            int icon = R.drawable.mapmark;

                            if (response != null){
                                // Delete the response from db
                                String command = "DELETE FROM MobileResponse WHERE HCN='" + HCN + "';";
                                databaseConnection.executeNonQuery(command);
                                //SystemClock.sleep(500);

                                Intent intent = new Intent(NotificationService.this, NotificationService.class);
                                PendingIntent pIntent = PendingIntent.getActivity(NotificationService.this, 0, intent, 0);

                                NotificationCompat.Builder mBuilder = new NotificationCompat.Builder(NotificationService.this, "ems")
                                        .setSmallIcon(icon)
                                        .setContentTitle("EMS-Mobile")
                                        .setContentText(response)
                                        .setPriority(NotificationCompat.PRIORITY_DEFAULT)
                                        .setAutoCancel(true);

                                //SystemClock.sleep(500);
                                NotificationManager manager = (NotificationManager) getSystemService(NOTIFICATION_SERVICE);
                                //SystemClock.sleep(500);

                                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
                                    //Android 8.0 stuff. Probably a good idea to use with the latest SDK
                                    //If you need more, this URL has a good explanation of new things
                                    //https://developer.android.com/training/notify-user/build-notification.html

                                    CharSequence name = "My Channel Name";
                                    String description = "My much larger channel description";
                                    NotificationChannel channel = new NotificationChannel("ems", name, NotificationManager.IMPORTANCE_DEFAULT);
                                    channel.setDescription(description);
                                    // Register the channel with the system
                                    manager.createNotificationChannel(channel);
                                }
                                //SystemClock.sleep(500);
                                manager.notify(1,mBuilder.build());
                            }
                        }
                    } catch (Exception e) {
                    }
                }
            }
        }).start();

        return Service.START_STICKY;
    }


    @Override
    public IBinder onBind(Intent intent) {
        // TODO: Return the communication channel to the service.
        throw new UnsupportedOperationException("Not yet implemented");
    }
}
