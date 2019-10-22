using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System;


public class TestSpCon : MonoBehaviour
{
    public Text ReadData;
    public Text DataInput;
    public Toggle bytetoggle;
    public Toggle stringtoggle;
    public Dropdown ByteChoose;
    public Dropdown PortNameChoose;
    public Dropdown PortBaudrate;
    public GameObject TextInput;
    /*public byte[] test;
    public byte aa;*/

    SerialPort VMSerialPort;

    public string[] portbaudrate;
    public string[] ports;

    // Start is called before the first frame update
    void Start()
    {
        VMSerialPort = new SerialPort();
        ChoosePortName();
    }

    // Update is called once per frame
    void Update()
    {
        if(stringtoggle.isOn)
        {
            TextInput.SetActive(true);
        }
        else
        {
            TextInput.SetActive(false);
        }
    }

    public void OpenPort()
    {
        VMSerialPort.PortName = PortNameChoose.options[PortNameChoose.value].text;
        VMSerialPort.BaudRate = Convert.ToInt32(PortBaudrate.options[PortBaudrate.value].text);
        VMSerialPort.Parity = Parity.None;
        VMSerialPort.DataBits = 8;
        VMSerialPort.Open();

        Debug.Log("Serial Port Opened: " + VMSerialPort.ToString());

        System.Threading.Thread.Sleep(2000);
    }

    public void PortNameOnChange()
    {
        Debug.Log("Chosen port name: " + PortNameChoose.options[PortNameChoose.value].text);
    }

    public void BaudRateOnChange()
    {
        Debug.Log("Chosen baud rate: " + PortBaudrate.options[PortBaudrate.value].text);
    }

    public void MotorChoiceOnChange()
    {
        Debug.Log("Chosen motor: " + ByteChoose.value);
    }

    public void SendToPort()
    {
        OpenPort();

        if (ByteChoose.value == 0)
        {
            Debug.Log("Running motor 1");
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x00, 0x00, 0x35 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 1)
        {
            Debug.Log("Running motor 2");
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x01, 0x00, 0x36 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 2)
        {
            Debug.Log("Running motor 3");
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x02, 0x00, 0x37 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 3)
        {
            Debug.Log("Running motor 4");
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x03, 0x00, 0x38 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 4)
        {
            Debug.Log("Running motor 5");
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x04, 0x00, 0x39 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 5)
        {
            Debug.Log("Running motor 6");
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x05, 0x00, 0x3A };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 6)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x06, 0x00, 0x3B };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 7)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x0A, 0x00, 0x3F };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 8)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x0B, 0x00, 0x40 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 9)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x0C, 0x00, 0x41 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 10)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x0D, 0x00, 0x42 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 11)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x0E, 0x00, 0x43 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 12)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x0F, 0x00, 0x44 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 13)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x10, 0x00, 0x45 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 14)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x14, 0x00, 0x49 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 15)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x15, 0x00, 0x4A };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 16)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x16, 0x00, 0x4B };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 17)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x17, 0x00, 0x4C };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 18)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x18, 0x00, 0x4D };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 19)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x19, 0x00, 0x4E };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 20)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x1A, 0x00, 0x4F };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 21)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x1E, 0x00, 0x53 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 22)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x1F, 0x00, 0x54 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 23)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x20, 0x00, 0x55 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 24)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x21, 0x00, 0x56 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 25)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x22, 0x00, 0x57 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 26)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x23, 0x00, 0x58 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 27)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x24, 0x00, 0x59 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 28)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x28, 0x00, 0x5D };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 29)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x29, 0x00, 0x5E };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 30)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x2A, 0x00, 0x5F };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 31)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x2B, 0x00, 0x60 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 32)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x2C, 0x00, 0x61 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 33)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x2D, 0x00, 0x62 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }
        else if (ByteChoose.value == 34)
        {
            PortReset();
            if (bytetoggle.isOn)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x2E, 0x00, 0x63 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (stringtoggle.isOn)
            {
                VMSerialPort.Write(DataInput.text);
            }
        }

        ClosePort();
    }

    void ChoosePortName()
    {
        ports = SerialPort.GetPortNames();
        PortNameChoose.options.Clear();
        foreach(string cc in ports)
        {
            PortNameChoose.options.Add(new Dropdown.OptionData() { text = cc });
        }
        PortNameChoose.value = 1;
        PortNameChoose.value = 0;
    }

    void ClosePort()
    {
        VMSerialPort.Close();
    }

    void PortReset()
    {
        byte[] SendingBytes = { 0xFF, 0x01, 0x01, 0x01, 0x01, 0x01, 0x03 };
        VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);

        System.Threading.Thread.Sleep(2000);
    }
}
