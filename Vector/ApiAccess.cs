using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using static Anki.Vector.ExternalInterface.ExternalInterface;
using Anki.Vector.ExternalInterface;
using Google.Protobuf;
using System.Linq;

namespace Vector
{
	public static class ApiAccess
	{
		const string AnkiAppKey = "aung2ieCho3aiph7Een3Ei";

		/// <summary>
		/// Required for first time use.  Used to grant this device access to a Vector robot.  The robot must be joined to the wifi network using the Vector Companion App before running this.
		/// </summary>
		/// <param name="robotName">Find your robot name (ex. Vector-A1B2) by placing Vector on the charger and double-clicking Vector's backpack button.</param>
		/// <param name="ipAddress">Find your robot ip address (ex. 192.168.42.42) by placing Vector on the charger, double-clicking Vector's backpack button, then raising and lowering his arms.If you see XX.XX.XX.XX on his face, reconnect Vector to your WiFi using the Vector Companion App.</param>
		/// <param name="serialNumber">Please find your robot serial number (ex. 00e20100) located on the underside of Vector, or accessible from Vector's debug screen.</param>
		/// <param name="userName">Enter your email. Make sure to use the same account that was used to set up your Vector through the Companion app.</param>
		/// <param name="password">Enter your password. Make sure to use the same account that was used to set up your Vector through the Companion app.</param>
		/// <returns>used to connect to the robot</returns>
		public static async Task<RobotConnectionInfo> GrantAsync(string robotName, string ipAddress, string serialNumber, string userName, string password)
		{
			var result = new RobotConnectionInfo();
			robotName = FormatRobotName(robotName);
			serialNumber = FormatSerialNumber(serialNumber);

			//get the certificate
			var client = new HttpClient();
			var response = await client.GetAsync(@"https://session-certs.token.global.anki-services.com/vic/" + serialNumber);
			if (!response.IsSuccessStatusCode)
			{
				throw new VectorAuthorizationException("invalid serial number");
			}
			result.Certificate = await response.Content.ReadAsStringAsync();

			//validate robot name
			var cert = new X509Certificate(Encoding.ASCII.GetBytes(result.Certificate));
			var certSubject = cert.Subject.Split(',').Select(i => new KeyValuePair<string, string>(i.Split('=')[0].Trim(), i.Split('=')[1].Trim()));
			var commonName = certSubject.First(i => i.Key == "CN").Value;
			if (commonName != robotName)
			{
				throw new VectorAuthorizationException($"The name of the certificate ({commonName}) does not match the name provided robotName ({robotName}) Please verify the name, and try again.");
			}
			result.RobotName = robotName;

			//get session token
			response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Post, @"https://accounts.api.anki.com/1/sessions")
			{
				Headers = { { "User-Agent", "Vector-sdk/0.5.1" }, { "Anki-App-Key", AnkiAppKey } },
				Content = new FormUrlEncodedContent(new Dictionary<string, string>() { { "username", userName }, { "password", password } }),
			});
			if (!response.IsSuccessStatusCode)
				throw new VectorAuthorizationException("invalid userName and password");
			var userSession = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()) as JObject;
			var sessionToken = userSession["session"]["session_token"].Value<string>();

			//have robot authenticate with session token and retreive the robots client token
			var ssl = new SslCredentials(result.Certificate);
			var channel = new Channel(ipAddress, 443, ssl, new ChannelOption[] { new ChannelOption("grpc.ssl_target_name_override", robotName) });
			try
			{
				await channel.ConnectAsync(DateTime.UtcNow.AddSeconds(10));
			}
			catch (TaskCanceledException ex)
			{
				throw new VectorAuthorizationException("Timeout.  Could not connect to vector.  insure IP address is correct", ex);
			}
			var robotClient = new ExternalInterfaceClient(channel);
			var hostname = System.Net.Dns.GetHostName();
			var authRequest = new UserAuthenticationRequest() { ClientName = ByteString.CopyFromUtf8(hostname), UserSessionId = ByteString.CopyFromUtf8(sessionToken) };
			var authResult = await robotClient.UserAuthenticationAsync(authRequest);
			if (authResult.Code == UserAuthenticationResponse.Types.Code.Unauthorized)
				throw new VectorAuthorizationException("unauthorized");
			result.Token = authResult.ClientTokenGuid.ToStringUtf8();
			result.IpAddress = ipAddress;
			await channel.ShutdownAsync();

			return result;
		}

		static internal string FormatRobotName(string robotName)
		{
			if (robotName.Length == 4)
				return $"Vector-{robotName.ToUpper()}";
			if (robotName.StartsWith("Vector ", StringComparison.OrdinalIgnoreCase))
				return $"Vector-{robotName.Split(' ').Last().ToUpper()}";
			return robotName;
		}

		static internal string FormatSerialNumber(string serialNumber)
		{
			return serialNumber.ToLower();
		}
	}
}
