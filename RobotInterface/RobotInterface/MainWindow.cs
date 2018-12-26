﻿using System;
using System.IO.Ports;
using System.Collections.Generic;
using Gtk;
using RobotInterface;

public partial class MainWindow : Gtk.Window
{

    #region FIELDS

    private ListStore framesListStore;
    private Robot robot = new Robot(
            new Servo(10, 170, 90),
            new Servo(10, 170, 170),
            new Servo(10, 170, 35),
            new Servo(10, 170, 90),
            new Servo(10, 170, 90),
            new Servo(10, 170, 90),
            new Servo(0, 180, 0)
        );
    private List<Gtk.HScale> actuatorScales = new List<Gtk.HScale>();

    #endregion


    #region CONSTRUCTORS/DESCTRUCTORS

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();

        //Init tree frame view.
        this.InitFrameTreeView();

        //Init actuator scales.
        this.InitActuatorScales();

        //Load available serial ports.
        this.LoadAvailableSerialPorts();

        //Update baud rate and serial port.
        this.OnBaudRateDropdownChanged(this.BaudRateDropdown, null);
        this.OnSerialPortDropdownChanged(this.SerialPortDropdown, null);
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    #endregion


    #region METHODS

    private void InitFrameTreeView()
    {
        this.framesListStore = new ListStore(
                typeof(string), 
                typeof(string),
                typeof(float),
                typeof(float),
                typeof(float),
                typeof(float),
                typeof(float),
                typeof(float),
                typeof(float),
                typeof(int)
            );

        this.FrameTreeView.Model = this.framesListStore;

        var cellView = new CellRendererText();

        this.FrameTreeView.AppendColumn("ID", cellView, "text", 0);
        this.FrameTreeView.AppendColumn("Name", cellView, "text", 1);
        this.FrameTreeView.AppendColumn("Actuator 0", cellView, "text", 2);
        this.FrameTreeView.AppendColumn("Actuator 1", cellView, "text", 3);
        this.FrameTreeView.AppendColumn("Actuator 2", cellView, "text", 4);
        this.FrameTreeView.AppendColumn("Actuator 3", cellView, "text", 5);
        this.FrameTreeView.AppendColumn("Actuator 4", cellView, "text", 6);
        this.FrameTreeView.AppendColumn("Actuator 5", cellView, "text", 7);
        this.FrameTreeView.AppendColumn("Actuator 6", cellView, "text", 8);
        this.FrameTreeView.AppendColumn("Time (Milliseconds)", cellView, "text",  9);
    }

    private void InitActuatorScales()
    {
        //Clear actuator scales list.
        this.actuatorScales.Clear();

        //Add actuators to actuator scales list.
        this.actuatorScales.Add(this.ActuatorScale);
        this.actuatorScales.Add(this.ActuatorScale1);
        this.actuatorScales.Add(this.ActuatorScale2);
        this.actuatorScales.Add(this.ActuatorScale3);
        this.actuatorScales.Add(this.ActuatorScale4);
        this.actuatorScales.Add(this.ActuatorScale5);
        this.actuatorScales.Add(this.ActuatorScale6);

        //Set values of scales.
        for(int i = 0; i < actuatorScales.Count; i++) 
        {
            this.actuatorScales[i].Adjustment.Lower = this.robot.Servos[i].MinAngle;
            this.actuatorScales[i].Adjustment.Upper = this.robot.Servos[i].MaxAngle;
            this.actuatorScales[i].Adjustment.Value = this.robot.Servos[i].Angle;
        }
    }

    private void LoadAvailableSerialPorts()
    {

        //Add serial ports to dropdown.
        foreach (string portName in Serial.Instance.GetPortNames())
        {
            this.SerialPortDropdown.AppendText(portName); 
        }

        //Set active serial port.
        this.SerialPortDropdown.Active = 0;

    }

    private void AddToSerialTerminal(string text)
    {
        //Add time to text.
        DateTime dateTime = DateTime.Now;
        text = string.Format(
                "[{0}:{1}:{2}.{3}] {4}",
                dateTime.Hour.ToString().PadLeft(2, '0'),
                dateTime.Minute.ToString().PadLeft(2, '0'),
                dateTime.Second.ToString().PadLeft(2, '0'),
                dateTime.Millisecond.ToString().PadLeft(3, '0'),
                text
            );

        //Add text.
        var iter = this.SerialTerminal.Buffer.GetIterAtLine(this.SerialTerminal.Buffer.LineCount);
        this.SerialTerminal.Buffer.Insert(ref iter, text + "\n");
    }

    #endregion


    #region EVENTS

    private void OnConnectSerial()
    {
        this.AddToSerialTerminal("Device connected successfully.");

        //Set connect serial action icon.
        this.connectSerialAction.StockId = Stock.Connect;

        //Disable baudrate and port dropdowns.
        this.BaudRateDropdown.Sensitive = false;
        this.SerialPortDropdown.Sensitive = false;

        //Delay
        System.Threading.Thread.Sleep(3000);

        //Init servo angles.
        this.robot.InitializeServoAngles();

    }

    private void OnDisconnectSerial()
    {
        this.AddToSerialTerminal("Device disconnected successfully.");

        //Set connect serial action icon.
        this.connectSerialAction.StockId = Stock.Disconnect;

        //Enable baudrate and port dropdowns.
        this.BaudRateDropdown.Sensitive = true;
        this.SerialPortDropdown.Sensitive = true;

    }

    protected void OnSerialPortDropdownChanged(object sender, EventArgs e)
    {

        //Set serial port.
        Serial.Instance.SetSerialPort(((ComboBox)sender).ActiveText);

    }

    protected void OnBaudRateDropdownChanged(object sender, EventArgs e)
    {

        int baudRate = 0;

        try
        {
            baudRate = Convert.ToInt32(((ComboBox)sender).ActiveText);
        }
        catch(Exception)
        {
            Console.WriteLine("Could not convert dropdown baud rate to int.");
        }

        //Set baud rate.
        Serial.Instance.SetBaudRate(baudRate);

    }

    protected void OnConnectSerialActivated(object sender, EventArgs e)
    {

        if (!Serial.Instance.IsOpen())
        {
            if (Serial.Instance.Open()) this.OnConnectSerial();
            else 
            {
                MessageDialog dialog = new MessageDialog(
                        null,
                        DialogFlags.Modal,
                        MessageType.Info,
                        ButtonsType.Ok,
                        "Could not open serial port."
                    );
                dialog.Run();
                dialog.Destroy();
            }
        }
        else
        {
            if (Serial.Instance.Close()) this.OnDisconnectSerial();
            else
            {
                MessageDialog dialog = new MessageDialog(
                            null,
                            DialogFlags.Modal,
                            MessageType.Info,
                            ButtonsType.Ok,
                            "Could not close serial port."
                        );
                dialog.Run();
                dialog.Destroy();
            }
        }

    }

    protected void OnActuatorScaleChanged(object sender, EventArgs e)
    {
        //Get sender name.
        string senderName = ((Gtk.Widget)sender).Name;

        //Check what scale to update.
        for(int i = 0; i < this.actuatorScales.Count; i++) 
        {
            Gtk.HScale actuatorScale  = this.actuatorScales[i];

            //If actuator name not sender name, continue.
            if (actuatorScale.Name != senderName) continue;

            //Update servo connected to actuator.
            this.robot.SetServoAngle(i, (float)actuatorScale.Adjustment.Value);

            break;
        }
    }

    #endregion
}
