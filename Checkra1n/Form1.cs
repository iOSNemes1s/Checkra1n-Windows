using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibUsbDotNet.DeviceNotify;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace Checkra1n
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            animationTimer.Tick += AnimationTimer_Tick;

            animationText = label15.Text;
            currentCharIndex = 0;
            isPeriodConverted = false;
        }

        public static IDeviceNotifier deviceNotify = DeviceNotifier.OpenDeviceNotifier();
        public string UCID;
        public string UDID;
        public string ECID;
        public string hardware;
        public string build;
        public string iOS;
        public string SerialNumber;
        public string productType;
        public string name;
        public string OSType;
        public string Model;
        public string CPID;
        public string iBoot;
        public string mode;
        private string animationText;
        private int currentCharIndex;
        private bool isPeriodConverted;


        private static Dictionary<string, string> iDevices = new Dictionary<string, string>
    {
        { "iPhone7,1", "iPhone 6 Plus" },
        { "iPhone7,2", "iPhone 6" },
        { "iPhone8,1", "iPhone 6s" },
        { "iPhone8,2", "iPhone 6s Plus" },
        { "iPhone8,4", "iPhone SE (1st gen)" },
        { "iPhone9,1", "iPhone 7 (Global)" },
        { "iPhone9,2", "iPhone 7 Plus (Global)" },
        { "iPhone9,3", "iPhone 7 (GSM)" },
        { "iPhone9,4", "iPhone 7 Plus (GSM)" },
        { "iPhone10,1", "iPhone 8 (Global)" },
        { "iPhone10,2", "iPhone 8 Plus (Global)" },
        { "iPhone10,3", "iPhone X (Global)" },
        { "iPhone10,4", "iPhone 8 (GSM)" },
        { "iPhone10,5", "iPhone 8 Plus (GSM)" },
        { "iPhone10,6", "iPhone X (GSM)" },
        { "iPod9,1", "iPod Touch (7th gen)" },
        { "iPad5,1", "iPad mini 4 (WiFi)" },
        { "iPad5,2", "iPad mini 4 (Cellular)" },
        { "iPad5,3", "iPad Air 2 (WiFi)" },
        { "iPad5,4", "iPad Air 2 (Cellular)" },
        { "iPad6,3", "iPad Pro 9.7-inch (WiFi)" },
        { "iPad6,4", "iPad Pro 9.7-inch (Cellular)" },
        { "iPad6,7", "iPad Pro 12.9-inch (1st gen, WiFi)" },
        { "iPad6,8", "iPad Pro 12.9-inch (1st gen, Cellular)" },
        { "iPad6,11", "iPad (5th gen, WiFi)" },
        { "iPad6,12", "iPad (5th gen, Cellular)" },
        { "iPad7,1", "iPad Pro 12.9-inch (2nd gen, WiFi)" },
        { "iPad7,2", "iPad Pro 12.9-inch (2nd gen, Cellular)" },
        { "iPad7,3", "iPad Pro 10.5-inch (WiFi)" },
        { "iPad7,4", "iPad Pro 10.5-inch (Cellular)" },
        { "iPad7,5", "iPad (6th gen, WiFi)" },
        { "iPad7,6", "iPad (6th gen, Cellular)" },
        { "iPad7,11", "iPad (7th gen, WiFi)" },
        { "iPad7,12", "iPad (7th gen, Cellular)" }
    };
        private void button2_Click(object sender, EventArgs e)
        {
            //Show options panel
            options.Visible = true;
        }

        public bool enterRecovery()
        {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = Environment.CurrentDirectory + "/ref/ideviceenterrecovery.exe",
                Arguments = UDID,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            process.Start();
            if (!process.StandardOutput.ReadToEnd().Contains("successfully switching to recovery mode."))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //hide options panel
            options.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            deviceNotify.OnDeviceNotify += checkConnection;
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Get the current character in the animation text
            char currentChar = animationText[currentCharIndex];

            // Convert periods to strokes and vice versa
            if (currentChar == '.' && !isPeriodConverted)
                currentChar = '|';
            else if (currentChar == '|' && isPeriodConverted)
                currentChar = '.';

            // Update the label's text with the modified character
            label15.Text = animationText.Substring(0, currentCharIndex) + currentChar +
                                  animationText.Substring(currentCharIndex + 1);

            // Move to the next character for the next tick
            currentCharIndex++;

            // Check if we have finished the conversion for this period
            if (currentCharIndex >= animationText.Length || animationText[currentCharIndex] == ' ')
            {
                isPeriodConverted = !isPeriodConverted;

                // Reset the animation once it reaches the end
                if (isPeriodConverted && currentCharIndex >= animationText.Length)
                {
                    currentCharIndex = 0;
                    isPeriodConverted = false;
                }
            }
            if (currentCharIndex >= animationText.Length)
            {
                currentCharIndex = 0;
            }
        }
            public async Task<bool> checkInfoAsync()
        {
            // start process
            var ideviceinfo = new Process();
            ideviceinfo.StartInfo.FileName = Environment.CurrentDirectory + "/ref/ideviceinfo.exe";
            ideviceinfo.StartInfo.UseShellExecute = false;
            ideviceinfo.StartInfo.RedirectStandardOutput = true;
            ideviceinfo.StartInfo.CreateNoWindow = true;
            ideviceinfo.StartInfo.RedirectStandardError = true;
            ideviceinfo.Start();

            int count = 0;
            //extract content
            while (!ideviceinfo.StandardOutput.EndOfStream)
            {

                string text2 = await ideviceinfo.StandardOutput.ReadLineAsync();
                if (text2.StartsWith("ERROR: "))
                {
                    return false;
                }
                else if (text2.StartsWith("UniqueChipID: "))
                {
                    count += 1;
                    UCID = text2.Replace("UniqueChipID: ", "");
                    long.TryParse(UCID, out long result);
                    ECID = Convert.ToString(result, 16);
                    while (ECID.Length != 16)
                    {
                        ECID = "0" + ECID;
                    }
                    ECID = "0x" + ECID;
                }
                else if (text2.StartsWith("UniqueDeviceID: "))
                {
                    count += 1;
                    UDID = text2.Replace("UniqueDeviceID: ", "");
                }
                else if (text2.StartsWith("SerialNumber: "))
                {
                    count += 1;
                    SerialNumber = text2.Replace("SerialNumber: ", "");
                }
                else if (text2.StartsWith("ProductVersion: "))
                {
                    count += 1;
                    iOS = text2.Replace("ProductVersion: ", "");
                }
                else if (text2.StartsWith("HardwareModel: "))
                {
                    count += 1;
                    hardware = text2.Replace("HardwareModel: ", "");
                }

                else if (text2.StartsWith("BuildVersion: "))
                {
                    count += 1;
                    build = text2.Replace("BuildVersion: ", "");

                }
                else if (text2.StartsWith("ProductType: "))
                {
                    count += 1;
                    productType = text2.Replace("ProductType: ", "");
                    if (productType.StartsWith("iPhone"))
                    {
                        OSType = "iOS";
                    }
                    else if (productType.StartsWith("iPad"))
                    {
                        OSType = "iPadOS";
                    }
                }
            }
            if(count >= 3)
            {
                return true;
            }
            else
            {
                return false;
            }
           
        }

        public static string getKey(string arg, string key, bool ret = true, bool dfu = false)
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Environment.CurrentDirectory + "/ref/irecover.exe",
                    Arguments = arg,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            process.Start();
            if (!ret)
            {
                return "";
            }
            if (dfu)
            {
                if (process.WaitForExit(500) && process.StandardOutput.ReadToEnd().Contains(key))
                {
                    return key;
                }
            }
            else
            {
                string text;
                while ((text = process.StandardOutput.ReadLine()) != null)
                {
                    if (text.Contains(key))
                    {
                        return text;
                    }
                }
            }
            return "";
        }
        public async Task<bool> CheckDeviceAsync()
        {
            var process = new Process();
            process.StartInfo.FileName = Path.Combine(Environment.CurrentDirectory, "ref", "irecovery.exe");
            process.StartInfo.Arguments = "-q";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            int count = 0;
            try
            {
                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();
                var timeoutTask = Task.Delay(1500); // Wait for 1.5 second

                // Wait for either the output or error to complete or the timeout to occur
                var completedTask = await Task.WhenAny(outputTask, errorTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    // Timeout occurred, kill the process and return the error output
                    process.Kill();
                    //var errorOutput = await errorTask;
                    return false;
                }

                var output = await outputTask;
                var error = await errorTask;

                if (error.Length > 0)
                {
                    return false;
                }

                var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    if (line.StartsWith("ECID: "))
                    {
                        count += 1;
                        ECID = line.Replace("ECID: ", "");
                    }
                    else if (line.StartsWith("PRODUCT: "))
                    {
                        count += 1;
                        productType = line.Replace("PRODUCT: ", "");
                        if (productType.StartsWith("iPhone"))
                        {
                            OSType = "iOS";
                        }
                        else if (productType.StartsWith("iPad"))
                        {
                            OSType = "iPadOS";
                        }
                    }
                    else if (line.StartsWith("MODEL: "))
                    {
                        count += 1;
                        Model = line.Replace("MODEL: ", "");
                    }
                    else if (line.StartsWith("CPID: "))
                    {
                        count += 1;
                        CPID = line.Replace("CPID: ", "");
                    }
                    
                }

                process.WaitForExit();

                if (count >= 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                // Handle exceptions
                return false;
            }
            finally
            {
                process.Dispose();
            }

        }
        public async Task<bool> CheckDeviceLoopAsync()
        {
            bool success = false;
            await Task.Run(async () =>
            {

                while (!success)
                {
                    success = await CheckDeviceAsync();
                    if (!success)
                    {
                        await Task.Delay(500); // Delay for .5 seconds before checking again
                    }
                }
            });
            return success;
        }
        public void getiOS(string iBoot)
        {
            if (iBoot.StartsWith("iBoot-4513"))
            {
                iOS = "12.x";
            }
            else if (iBoot.StartsWith("iBoot-5540"))
            {
                iOS = "13.x";
            }
            else if (iBoot.StartsWith("iBoot-6723") || iBoot.StartsWith("iBoot-6603") || iBoot.StartsWith("iBoot-6631") || iBoot.StartsWith("iBoot-6671"))
            {
                iOS = "14.x";
            }
            else if (iBoot.StartsWith("iBoot-7429") || iBoot.StartsWith("iBoot-7459"))
            {
                iOS = "15.x";
            }
            else if (iBoot.StartsWith("iBoot-8419"))
            {
                iOS = "16.x";
            }
            else if (iBoot.StartsWith("iBoot-8422"))
            {
                iOS = "16.4x";
            }
            else
            {
                iOS = "16.4x";
            }
        }
        public async Task<bool> CheckInfoLoopAsync()
        {
            bool success = false;
            await Task.Run(async () =>
            {

                while (!success)
                {
                    success = await checkInfoAsync();
                    if (!success)
                    {
                        await Task.Delay(500); // Delay for .5 seconds before checking again
                    }
                }
            });
            return success;
        }
        public async void checkConnection(object sender, DeviceNotifyEventArgs e)
        {
            string IdProduct = e.Device.IdProduct.ToString();
            string IdVendor = e.Device.IdVendor.ToString();

            //first whether a device is connected
            if (e.EventType.ToString() == "DeviceArrival")
            {
                //We check whether the device is an iDevice
                if(IdVendor == "1452")
                {
                    
                    //We check which mode device is connected in
                    if(IdProduct == "4776" || IdProduct == "4779")
                    {

                        //Connected in Normal mode
                        mode = "Normal";
                        if (await CheckInfoLoopAsync())
                        {
                            //Check if the connected is a supported device
                            if (iDevices.ContainsKey(productType))
                            {
                                name = iDevices[productType];
                                label2.Text = $"{name} ({OSType} {iOS}) connected in Normal mode.\nECID: {ECID}";
                                this.Activate();
                                button1.Enabled = true;
                            }
                            else
                            {
                                label2.Text = "The device connected is unsupported";
                            }

                        }

                    }
                    else if (IdProduct == "4737")
                    {
                        //Connected in Recovery mode
                        mode = "recovery";
                        //await CheckDeviceAsync();
                        if(await CheckDeviceLoopAsync())
                        {
                            if (iDevices.ContainsKey(productType))
                            {
                                name = iDevices[productType];
                                iBoot = getKey("-q", "iBoot").Replace("iBoot: ", "");
                                getiOS(iBoot);
                                this.Activate();
                                label2.Text = $"{name} ({OSType} {iOS}) connected in Recovery mode.\nECID: {ECID}";
                                button1.Enabled = true;

                            }
                            else
                            {
                                label2.Text = "The device connected is unsupported";
                            }
                        }
                        

                    }
                    else if (IdProduct == "4647")
                    {
                        //Connected in DFU mode
                        if (await CheckDeviceLoopAsync())
                        {
                            if (iDevices.ContainsKey(productType))
                            {
                                name = iDevices[productType];
                                this.Activate();
                                label2.Text = $"{name} connected in DFU mode.\nECID: {ECID}\nWARNING: Put device in Normal mode or Recovery mode.";

                            }
                            else
                            {
                                label2.Text = "The device connected is unsupported";
                            }
                        }
                    }
                }

            }
            //When a device gets disconnected
            else if (e.EventType.ToString() == "DeviceRemoveComplete")
            {
                //We check if its an iDevice before we take action
                if (IdVendor == "1452")
                {
                    button1.Enabled = false;
                    label2.Text = "Connect your iPhone, iPod touch, iPad, or AppleTv to begin.";
                }

            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(mode == "Normal")
            {
                options.Visible = true;
                recoveryPanel.Visible = true;
            }
            else
            {
                gotoDFU();
            }
        }

        public void gotoDFU()
        {
            options.Visible = true;
            recoveryPanel.Visible = true;
            dfuPanel.Visible = true;
            devicePicture.Visible = true;
            if (name.Contains("iPhone 6") || name.Contains("iPhone 5s") || name.Contains("iPhone SE") || name.Contains("iPad"))
            {
                devicePicture.Image = Properties.Resources.iphone6;
                label22.Visible = true;
                homeButton.Visible = true;
                pressDFU.Text = "2. Press and hold the Side \r\nand Home buttons \r\ntogether (4)\r\n";
                label20.Text = "3. Release the Side button \r\nBUT KEEP HOLDING the \r\nHome button (10)\r\n";
            }
            else if (name.Contains("iPhone 7") || name.Contains("iPhone 8"))
            {
                devicePicture.Image = Properties.Resources.iphone7_8;
                label22.Visible = true;
                volumeDown.Visible = true;
            }
            else if (name.Contains("iPhone X"))
            {
                devicePicture.Image = Properties.Resources.iphoneX;
                sideButton.Visible = true;
                volumeDown.Visible = true;
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            options.Visible = false;
            recoveryPanel.Visible = false;
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            label14.Visible = true;
            label15.Visible = true;
            button4.Enabled = false;
            button5.Enabled = false;
            animationTimer.Start();

            bool recoveryResult = await Task.Run(() => enterRecovery());
            if (recoveryResult)
            {
                if(await CheckDeviceLoopAsync())
                {
                    animationTimer.Stop();
                    label15.Visible = false;
                    label14.Text = "Device successfully entered recovery mode!";
                    await Task.Delay(1500);

                    gotoDFU();
                }
            }
            else
            {
                animationTimer.Stop();
                label15.Visible = false;
                label14.Text = "Failed to enter recovery mode!";
                button4.Enabled = true;
                button5.Enabled = true;

            }

        }
        public void bootNormal()
        {

            var ideviceinfo = new Process();
            ideviceinfo.StartInfo.FileName = Environment.CurrentDirectory + "/ref/irecovery.exe";
            ideviceinfo.StartInfo.Arguments = "-n";
            ideviceinfo.StartInfo.UseShellExecute = false;
            ideviceinfo.StartInfo.RedirectStandardOutput = true;
            ideviceinfo.StartInfo.CreateNoWindow = true;
            ideviceinfo.StartInfo.RedirectStandardError = true;
            ideviceinfo.Start();

        }

        private void Count1()
        {
            Invoke(new Action(() =>
            {
                startDFU.Enabled = false;
                pressDFU.Enabled = true;
            }));

            int count = 5;
            DateTime startTime = DateTime.Now;

            // Create a timer to execute the process after 2 seconds
            System.Timers.Timer timer = new System.Timers.Timer(2000);
            timer.Elapsed += (sender, e) =>
            {
                // Execute the process
                bootNormal();
                // Stop the timer
                ((System.Timers.Timer)sender).Stop();
            };
            timer.AutoReset = false;
            timer.Start();

            while (true)
            {
                TimeSpan elapsed = DateTime.Now - startTime;

                if (elapsed.TotalSeconds >= 5)
                {
                    // Start the second counting operation once the elapsed time reaches 5 seconds
                    Count2();
                    break;
                }

                count--;

                string txt1;
                if (name.Contains("iPhone 6") || name.Contains("iPhone 5s") || name.Contains("iPhone SE") || name.Contains("iPad"))
                {
                    txt1 = $"2. Press and hold the Side \r\nand Home buttons \r\ntogether ({count})\r\n";
                }
                else
                {
                    txt1 = $"2. Press and hold the Side \r\nand Volume down buttons \r\ntogether ({count})\r\n";
                }
                   
                Invoke(new Action(() => pressDFU.Text = txt1));
                Thread.Sleep(1000);
            }
        }


        private void Count2()
        {
            Invoke(new Action(() =>
            {
                sideButton.Enabled = false;
                label22.Enabled = false;
                pressDFU.Enabled = false;
                label20.Enabled = true;
            }));

            int count = 11;
            DateTime startTime = DateTime.Now;

            while (true)
            {
                TimeSpan elapsed = DateTime.Now - startTime;
                if (elapsed.TotalSeconds >= 11)
                {
                    Invoke(new Action(() =>
                    {
                        label20.Enabled = false;
                        homeButton.Enabled = false;
                        volumeDown.Enabled = false;
                    }));
                    break;
                }

                count--;

                string txt1;
                if (name.Contains("iPhone 6") || name.Contains("iPhone 5s") || name.Contains("iPhone SE") || name.Contains("iPad"))
                {
                    txt1 = $"3. Release the Side button \r\nBUT KEEP HOLDING the \r\nHome button ({count})\r\n";
                }
                else
                {
                    txt1 = $"3. Release the Side button \r\nBUT KEEP HOLDING the \r\nVolume down button ({count})\r\n";
                }

                Invoke(new Action(() => label20.Text = txt1));
                Thread.Sleep(1000);

            }

        }
        private async void button7_Click(object sender, EventArgs e)
        {

            await Task.Run(() =>
            {
                button6.Enabled = false;
                button7.Enabled = false;
                Count1();
                MessageBox.Show("This is not done yet, left with jailbreak.\nHowever you can report any bugs.");
                button6.Enabled = true;
                button7.Enabled = true;
            });
        }
    }
}
