using System.Diagnostics;
using Camera.MAUI;
using Camera.MAUI.ZXingHelper;
using ZXing.QrCode;
using Trak_IT.Scripts;
using Camera.MAUI.ZXing;
using static System.Collections.Specialized.BitVector32;
using System.Threading;
namespace Trak_IT
{
    public partial class MainPage : ContentPage
    {
        //int count = 0;
        public readonly LocalDBServer _dbServer;
        public readonly UsersProfile _usersProfile;

        public MainPage(LocalDBServer dbServer, UsersProfile usersProfile)
        {
            try
            {
                InitializeComponent();
                var profile = new ProfileBind
                {
                    ProfileImageFilePath = usersProfile.Photostring
                };
                _dbServer = dbServer;
                _usersProfile = usersProfile;
                BindingContext = profile;

                cameraView.BarCodeDecoder = new ZXingBarcodeDecoder();
                cameraView.BarCodeOptions = new BarcodeDecodeOptions
                {
                    AutoRotate = true,
                    PossibleFormats = { BarcodeFormat.QR_CODE },
                    ReadMultipleCodes = false,
                    TryHarder = true,
                };
                cameraView.BarCodeDetectionFrameRate = 10;
                cameraView.BarCodeDetectionMaxThreads = 5;
                cameraView.ControlBarcodeResultDuplicate = true;
                cameraView.BarCodeDetectionEnabled = true;
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.Source);
            }

        }

        private void sendData_Clicked(object sender, EventArgs e)
        {
            SheetService server = new SheetService();

            //var newData = new List<IList<object>>
            //{
            //new List<object> { "Username", "Email", "Phone", "Address"}
            //};

            //await server.PostStudent("your-spreadsheet-id", "Sheet1!A2:D2", newData);
        }
        private async void cameraView_CamerasLoaded(object sender, EventArgs e)
        {
            if (cameraView.NumCamerasDetected > 0)
            {
                // Select the first available camera
                cameraView.Camera = cameraView.Cameras.First();

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    var result = await cameraView.StartCameraAsync();

                    if (result != CameraResult.Success)
                    {
                        // Handle unsuccessful camera start, e.g., log error or show alert
                        await DisplayAlert("Failed", "Failed to start camera", "OK");
                    }
                });
            }

            else
            {
                await DisplayAlert("Error", "No camera detected", "OK");
            }
        }

        private async void cameraView_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
        {

            //byte[] code = System.Text.Encoding.UTF8.GetBytes($"{args.Result[0].BarcodeFormat}:{args.Result[0].Text}");
            
            SheetService service = new SheetService();
            Decryption decryption = new Decryption();
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    // Decrypt the provided data
                    string base64EncryptedData = $"{args.Result[0].Text}";
                    byte[] data = Convert.FromBase64String(base64EncryptedData);
                    Debug.WriteLine(data);
                    string decryptedId = decryption.DecryptMain(data);

                    // Retrieve student by decrypted ID
                    StudentModel student = await _dbServer.GetByStudent(decryptedId);
                    if (student == null)
                    {
                        await DisplayAlert("No student", "No student found with the given ID.", "OK");
                    }

                    DateTime now = DateTime.Now;

                    // Check if the student has a Timein already
                    if (string.IsNullOrEmpty(student.Timein))
                    {
                        // First entry - set Timein
                        student.Timein = now.ToString("h:mm tt");

                        await _dbServer.UpdateStudent(student); // Save Timein
                        // Re-fetch to ensure data is updated in DB
                        StudentModel studentUpdated = await _dbServer.GetByStudent(decryptedId);
                        if (studentUpdated == null || string.IsNullOrEmpty(studentUpdated.Timein))
                        {
                            Debug.WriteLine("Failed to update Timein in the database.");
                            return;
                        }

                        // Update UI for Timein
                        studentName.Text = studentUpdated.Name;
                        studentSection.Text = studentUpdated.Section;
                        bsitID.Text = studentUpdated.Bsitid;
                        clock_In.Text = studentUpdated.Timein;
                        clock_Out.Text = "00:00";

                        // Post to Google Sheets
                        var newData = new List<IList<object>>
                        {
                            new List<object> { studentUpdated.Id, studentUpdated.Bsitid, studentUpdated.Name, studentUpdated.Section, studentUpdated.Event, studentUpdated.Timein,"00:00", _usersProfile.Name }
                        };
                        await service.PostStudent("1uX6fxS6ocZ8Tgtcm_jyUqP2UlZDuaMArDzoapktKi8Q", $"TEAMBUILDING!A:H", newData);

                    }
                    else
                    {
                        // Second entry - set Timeout
                        student.Timeout = now.ToString("h:mm tt");
                        await _dbServer.UpdateStudent(student); // Save Timeout

                        // Re-fetch to ensure data is updated in DB
                        student = await _dbServer.GetByStudent(decryptedId);
                        if (student == null || string.IsNullOrEmpty(student.Timeout))
                        {
                            Debug.WriteLine("Failed to update Timeout in the database.");
                            return;
                        }

                        // Update UI for Timeout
                        studentName.Text = student.Name;
                        studentSection.Text = student.Section;
                        bsitID.Text = student.Bsitid;
                        clock_In.Text = student.Timein;
                        clock_Out.Text = student.Timeout;

                        // Update Google Sheets
                        var newData = new List<object> { student.Timeout };

                        await service.FindAndUpdateRowAsync("1uX6fxS6ocZ8Tgtcm_jyUqP2UlZDuaMArDzoapktKi8Q", student.Bsitid, newData);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error", $"{ex.Message}", "OK"); ;
                    Debug.WriteLine(ex.StackTrace);
                }
            });
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            studentName.Text = "Name";
            studentSection.Text = "Section";
            bsitID.Text = "BSIT ID";
            clock_In.Text = "0:00";
            clock_Out.Text = "0:00";
            await cameraView.StopCameraAsync();
            await Task.Delay(2000);
            if (cameraView.NumCamerasDetected > 0)
            {
                // Select the first available camera
                cameraView.Camera = cameraView.Cameras.First();

                // Start the camera asynchronously on the main thread
                var result = await MainThread.InvokeOnMainThreadAsync(() => cameraView.StartCameraAsync());

                if (result == CameraResult.Success)
                {
                    Console.WriteLine("Camera started successfully.");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to start camera.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "No camera detected.", "OK");
            }
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            SheetService service = new SheetService();
            Decryption decryption = new Decryption();
            try
            {
                // Decrypt the provided data
                string base64EncryptedData = "I0VHs1SeGmTgd49fZQlRYy2qVo/IfRoil5evxYAiw54=";
                byte[] data = Convert.FromBase64String(base64EncryptedData);
                string decryptedId = decryption.DecryptMain(data);

                // Retrieve student by decrypted ID
                StudentModel student = await _dbServer.GetByStudent(decryptedId);

                if (student == null)
                {
                    Debug.WriteLine("No student found with the given ID.");
                    return;
                }

                DateTime now = DateTime.Now;

                // Check if the student has a Timein already
                if (string.IsNullOrEmpty(student.Timein))
                {
                    // First entry - set Timein
                    student.Timein = now.ToString("h:mm tt");
                    await _dbServer.UpdateStudent(student); // Save Timein

                    // Re-fetch to ensure data is updated in DB
                    student = await _dbServer.GetByStudent(decryptedId);

                    // Update UI for Timein
                    //UpdateUIElements(student, timeIn: student.Timein, timeOut: "0:00");

                    // Post to Google Sheets
                    var newData = new List<IList<object>>
            {
                new List<object> { student.Id, student.Bsitid, student.Name, student.Section, student.Event, student.Timein, student.Timeout }
            };
                    await service.PostStudent("1uX6fxS6ocZ8Tgtcm_jyUqP2UlZDuaMArDzoapktKi8Q", "'TEAMBUILDING'!A:H", newData);
                }
                else
                {
                    // Second entry - set Timeout
                    student.Timeout = now.ToString("h:mm tt");
                    await _dbServer.UpdateStudent(student); // Save Timeout

                    // Re-fetch to ensure data is updated in DB
                    student = await _dbServer.GetByStudent(decryptedId);
                    // Update UI for Timeout
                    //UpdateUIElements(student, timeIn: student.Timein, timeOut: student.Timeout);

                    // Update Google Sheets
                    var newData = new List<object> { student.Id, student.Bsitid, student.Name, student.Section, student.Event, student.Timein, student.Timeout };
                    await service.FindAndUpdateRowAsync("1uX6fxS6ocZ8Tgtcm_jyUqP2UlZDuaMArDzoapktKi8Q", student.Bsitid, newData);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception occurred: {ex.Message}");
            }
        }
    }

}
