using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client.Exceptions;
//using Windows.Security.Cryptography;
//using Windows.Security.Cryptography.Core;
//using Windows.Storage.Streams;

namespace AzureConnections
{
    public static partial class MyConnections
    {
   

        //private static IBuffer GetMD5Hash(string key)
        //{
        //    IBuffer bufferUTF8Msg = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
        //    HashAlgorithmProvider hashAlgorithmProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
        //    IBuffer hashBuffer = hashAlgorithmProvider.HashData(bufferUTF8Msg);
        //    if (hashBuffer.Length != hashAlgorithmProvider.HashLength)
        //    {
        //        throw new Exception("There was an error creating the hash");
        //    }
        //    return hashBuffer;
        //}    

        //public static string GenerateKeyWithPassword(string password, int resultKeyLength = 68)
        //{
        //    if (password.Length < 6)
        //        throw new ArgumentException("password length must atleast 6 characters or above");
        //    string key = "";

        //    var hashKey = GetMD5Hash(password);
        //    var decryptBuffer = CryptographicBuffer.ConvertStringToBinary(password, BinaryStringEncoding.Utf8);
        //    var AES = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
        //    var symmetricKey = AES.CreateSymmetricKey(hashKey);
        //    var encryptedBuffer = CryptographicEngine.Encrypt(symmetricKey, decryptBuffer, null);
        //    key = CryptographicBuffer.EncodeToBase64String(encryptedBuffer);
        //    string cleanKey = key.Trim(new char[] { ' ', '\r', '\t', '\n', '/' });
        //    cleanKey = cleanKey.Replace("/", string.Empty); //.Replace("+", string.Empty).Replace("=", string.Empty);
        //    key = cleanKey;
        //    if (key.Length > resultKeyLength)
        //    {
        //        key = key.Substring(0,  resultKeyLength);
        //    }
        //    if (key.Length == resultKeyLength)
        //    {
        //        return key;
        //    }
        //    else  
        //    {
        //        key = GenerateKeyWithPassword(key, resultKeyLength);
        //    }
        //    return key;

        //}
        public static string GenerateKeyUsingListOfChars(int letterCount = 44, char[] Letters=null, bool AppendEqualsAtEnd = true)
        {
            // Get the number of words and letters per word.
            int num_letters = letterCount;
            if (AppendEqualsAtEnd)
                num_letters--;
            // Make an array of the letters we will use.
            char[] letters = Letters;
            if (Letters == null)
                letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrsruvwxyz+".ToCharArray();
            int lettersLength =  letters.Length;

            // Make a word.
            string word = "";

            //Use Cryptography to generate random numbers rather than Psuedo Random Rand
            // Deliberate overkill here
            byte[] randomBytes = new byte[num_letters*256];
            List<int> rands = new List<int>();
            do
            {
                using (System.Security.Cryptography.RNGCryptoServiceProvider rngCsp = new
                            System.Security.Cryptography.RNGCryptoServiceProvider())
                {
                    // Fill the array with a random value.
                    rngCsp.GetBytes(randomBytes);
                }


                // Truncate the set of random bytes to being in range 0 .. (lettersLength-1)
                // Nb Using mod of randomBytes will reduce entropy of the set

                foreach (var x in randomBytes)
                {
                    if (x < num_letters)
                        rands.Add((int)x);
                    if (rands.Count() >= num_letters)
                        break;
                }
            }
            while (rands.Count < letterCount);

            int[] randsArray = rands.ToArray();

            // Get random selection of characters from letters
            for (int j = 0; j < num_letters; j++)
            {
                int letter_num = randsArray[j]; ;
                // Append the letter.
                word += letters[letter_num];
            }
            if (AppendEqualsAtEnd)
                word += "=";
                return word;
        }



        public static string AddDeviceAsync(string IoTHubOwnerconnectionString, string deviceId)
        {
            RegistryManager registryManager = RegistryManager.CreateFromConnectionString(IoTHubOwnerconnectionString);
            registryManager.OpenAsync().GetAwaiter().GetResult();
            Device device = null;
           

            try
            {
                device = registryManager.AddDeviceAsync(new Device(deviceId)).GetAwaiter().GetResult();
                if (device != null)
                    OnStatusUpdateD?.Invoke("AddDeviceAsync: Device created OK.");
                else
                    OnStatusUpdateD?.Invoke("AddDeviceAsync: Unable to create Device.");
            }
            catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceAlreadyExistsException)
            {
                device = registryManager.GetDeviceAsync(deviceId).GetAwaiter().GetResult();
                if (device != null)
                    OnStatusUpdateD?.Invoke("AddDeviceAsync: Device already exists but has been found OK.");
                else
                    OnStatusUpdateD?.Invoke("AddDeviceAsync: Device already exists but HASN'T been found.");

            }
            catch (Exception)
            {
                OnStatusUpdateD?.Invoke("AddDeviceAsync: Device creation failed and/or device doesn't already exist.");
                System.Diagnostics.Debug.WriteLine("AddDeviceAsync: Device creation failed and/or device doesn't already exist.");
                device = null;
            }
            //Console.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);

            if (device != null)
            {
                string[] conparts = IoTHubOwnerconnectionString.Split(new char[] { ';' });
                if (conparts.Length == 3)
                {
                    DeviceConnectionString = string.Format("{0};DeviceId={1};SharedAccessKey={2}",
                     conparts[0], deviceId, device.Authentication.SymmetricKey.PrimaryKey);
                    IoTHubConnectionString = IoTHubOwnerconnectionString;
                    DeviceId = deviceId;
                    return DeviceConnectionString;
                }
            }
            return "";
        }

        public static string RemoveDeviceAsync(string IoTHubOwnerconnectionString, string deviceId)
        {
            if (deviceId=="")
            {
                OnStatusUpdateD?.Invoke(string.Format("RemoveDeviceAsync: Device Not deleted. Blank DeviceId."));

                return "";
            }
            else if (IoTHubOwnerconnectionString == "")
            {
                OnStatusUpdateD?.Invoke(string.Format(string.Format("RemoveDeviceAsync: Device {0} Not deleted. Blank IoTHubOwnerconnectionString.", deviceId)));

                return "";
            }
            RegistryManager registryManager = RegistryManager.CreateFromConnectionString(IoTHubOwnerconnectionString);
            registryManager.OpenAsync().GetAwaiter().GetResult();
            Device device = null;
            try
            {
                device = registryManager.GetDeviceAsync(deviceId).GetAwaiter().GetResult();
                if (device != null)
                {
                    registryManager.RemoveDeviceAsync(device).GetAwaiter().GetResult();
                }
                else
                {
                    OnStatusUpdateD?.Invoke(string.Format("RemoveDeviceAsync: Device {0} Not deleted. Unable to find Device.", deviceId));
                    return "";
                }

                device = null;
                OnStatusUpdateD?.Invoke(string.Format("RemoveDeviceAsync: Device {0} Deleted.", deviceId));

            }
            catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
            {
                OnStatusUpdateD?.Invoke(string.Format("RemoveDeviceAsync: Device {0} Not deleted. Unable to find Device.", deviceId));
            }
            catch (Exception ex)
            {
                OnStatusUpdateD?.Invoke(string.Format("RemoveDeviceAsync: Device {0} deletion failed: {1} " , deviceId, ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("RemoveDeviceAsync: Device {0} deletion failed: {1} ", deviceId, ex.Message));
                device = null;
            }
            
            DeviceId = "";
            DeviceConnectionString = "";
            return "";
        }

        public static string GetDeviceCSAsync(string IoTHubOwnerconnectionString, string deviceId)
        {
            RegistryManager registryManager = RegistryManager.CreateFromConnectionString(IoTHubOwnerconnectionString);
            registryManager.OpenAsync().GetAwaiter().GetResult();
            Device device = null;

            try
            {
                device = registryManager.GetDeviceAsync(deviceId).GetAwaiter().GetResult();
                if (device != null)
                    OnStatusUpdateD?.Invoke("AddDeviceAsync: Device exists and has been found OK.");
                else
                    OnStatusUpdateD?.Invoke("AddDeviceAsync: Device not been found.");
            }
            catch (Exception ex)
            {
                OnStatusUpdateD?.Invoke("AddDeviceAsync: Failed to get device- " + ex.Message);
                System.Diagnostics.Debug.WriteLine("AddDeviceAsync: Failed to get device: ", ex.Message);
                device = null;
            }
            //Console.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);

            if (device != null)
            {
                string[] conparts = IoTHubOwnerconnectionString.Split(new char[] { ';' });
                if (conparts.Length == 3)
                {
                    DeviceConnectionString = string.Format("{0};DeviceId={1};SharedAccessKey={2}",
                     conparts[0], deviceId, device.Authentication.SymmetricKey.PrimaryKey);
                    IoTHubConnectionString = IoTHubOwnerconnectionString;
                    DeviceId = deviceId;
                    return DeviceConnectionString;
                }
            }
            return "";
        }
    }
}
