using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationWidget : MonoBehaviour
{

    public Text NotificationText;
    public float NotificationDuration = 4;
    private List<Notification> activeNotifications;


    // Use this for initialization
    void Start()
    {
        activeNotifications = new List<Notification>();
        RedrawNotifications();
    }

    // Update is called once per frame
    void Update()
    {
        // count down and clean notes
        for (int i = 0; i < activeNotifications.Count; i++)
        {
            activeNotifications[i].TimeLeft -= Time.deltaTime;
            if (activeNotifications[i].TimeLeft <= 0)
            {
                activeNotifications.RemoveAt(i);
                i--;
            }

        }

        // drawn notes
        RedrawNotifications();
    }

    private void RedrawNotifications()
    {
        string msg = string.Join("\n", activeNotifications.Select(n => n.Message).ToArray());
        NotificationText.text = msg;
    }

    public void Notify(string msg)
    {
        Notification newNote = new Notification()
        {
            TimeLeft = NotificationDuration,
            Message = msg
        };

        activeNotifications.Add(newNote);
    }

    private class Notification
    {
        public float TimeLeft;
        public string Message;
    }
}
