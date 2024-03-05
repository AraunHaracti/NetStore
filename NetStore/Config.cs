using System;
using System.Collections.Generic;
using System.ComponentModel;
using Avalonia.Controls;
using MySqlConnector;
using NetStore.Models;

namespace NetStore;

public static class Config
{
    public static MySqlConnectionStringBuilder ConnectionStringBuilder_ = new MySqlConnectionStringBuilder()
    {
        Server = "192.168.0.13",
        Port = 3306,
        Database = "net-store",
        UserID = "root",
        Password = "rtc48EN"
    };
    
    public static MySqlConnectionStringBuilder ConnectionStringBuilder = new MySqlConnectionStringBuilder()
    {
        Server = "2.tcp.eu.ngrok.io",
        Port = 15256,
        Database = "net-store",
        UserID = "root",
        Password = "rtc48EN"
    };
    
    private static List<ContentControl> Windows { get; set; } = new List<ContentControl>();
    
    public static User CurrentUser { get; set; }
    
    public static void AddWindow(ContentControl window)
    {
        Windows.Add(window);
        OnListUpdate();
    }

    public static void SubWindow()
    {
        if (Windows.Count > 0)
            Windows.RemoveAt(Windows.Count - 1);
        OnListUpdate();
    }

    public static ContentControl? GetLastWindow()
    {
        if (Windows.Count > 0)
        {
            return Windows[^1];
        }

        return null;
    }

    private static void OnListUpdate()
    {
        WindowsListChanged?.Invoke(null, EventArgs.Empty);
    }

    public static event EventHandler WindowsListChanged;
}