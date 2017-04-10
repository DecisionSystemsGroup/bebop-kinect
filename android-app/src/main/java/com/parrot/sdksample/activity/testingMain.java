///*package com.parrot.sdksample.activity;
//
//import java.io.DataInputStream;
//import java.io.IOException;
//import java.net.ServerSocket;
//import java.net.Socket;
//
///**
// * Created by Acetaminophen on 13/3/2017.
// */
//
//public class testingMain {
//   static byte buffer[];
//    static ServerSocket listener;
//   static DataInputStream din;
//   static String received;
//
//
//    public static void main (String args[]) throws Exception{
///*
//        connectionThread ct = new connectionThread();
//        Thread myThread = new Thread(ct);
//
//        myThread.start();*/
//
//        Thread t = new Thread(new Runnable() {
//            @Override
//            public void run() {
//                try {
//                    listener = new ServerSocket(100);
//                    Socket sock = listener.accept();
//                    while(true){
//                        //System.out.println ("mpika mesa ");
//                        din = new DataInputStream(sock.getInputStream());
//                        buffer = new byte[1];
//                        din.read(buffer, 0, 1);
//                        received = new String(buffer, 0, 1);
//                        System.out.println("mesa sto thread" + received);
////                        if (sock.isConnected())
////                            listener.close();
//                        if (sock.isClosed()) {
//                            sock.close();
//                            break;
//                        }
//                    }
//                } catch (IOException e) {
//                    e.printStackTrace();
//                }
//            }
//        });
//        t.start();
//
//
//
//
//        while(t.isAlive()) {
//
//
//            System.out.println("auto pou elava apo to thread einai :" + received);
//            break;
//
//        }
//
//
//    }
//
//
//    private void setIt( String received){
//        this.received = received;
//    }
//
//    public String getIt(){
//        return received;
//    }
//}
//*/