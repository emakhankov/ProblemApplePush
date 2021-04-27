using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PushSharp.Apple;

namespace SendPushTest
{
	public class PushNotificator
	{
		
		public void checkedUnavailableDevices()
		{
			var config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Production,
				"push-cert.p12", "");

			var fbs = new FeedbackService(config);
			fbs.FeedbackReceived += (string deviceToken, DateTime timestamp) => {

				Console.WriteLine($"need remove: ID={deviceToken}, timeStamp={timestamp}");
				// Remove the deviceToken from your database
				// timestamp is the time the token was reported as expired
			};
			fbs.Check();
		}



		public void sendTestNotification()
		{

			// Configuration (NOTE: .pfx can also be used here)
			var config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Production,
				"push-cert.p12", "", false);

			// Create a new broker
			var apnsBroker = new ApnsServiceBroker(config);

			// Wire up events
			apnsBroker.OnNotificationFailed += (notification, aggregateEx) =>
			{

				aggregateEx.Handle(ex =>
				{

					// See what kind of exception it was to further diagnose
					if (ex is ApnsNotificationException)
					{
						var notificationException = (ApnsNotificationException)ex;

						// Deal with the failed notification
						var apnsNotification = notificationException.Notification;
						var statusCode = notificationException.ErrorStatusCode;

						Console.WriteLine($"Apple Notification Failed: ID={apnsNotification.Identifier}, Code={statusCode}");

					}
					else
					{
						// Inner exception might hold more useful information like an ApnsConnectionException			
						Console.WriteLine($"Apple Notification Failed for some unknown reason : {ex.InnerException}");
					}

					// Mark it as handled
					return true;
				});
			};

			apnsBroker.OnNotificationSucceeded += (notification) =>
			{
				Console.WriteLine("Apple Notification Sent!");
			};


			// Start the broker
			apnsBroker.Start();


			// Queue a notification to send
			var aps = new Aps();
			aps.aps = new Aps2() { alert = "TestForYou", badge = 0 };
			var json = JsonConvert.SerializeObject(aps);
			apnsBroker.QueueNotification(new ApnsNotification
			{
				DeviceToken = "MY VALID TOKEN",
				Payload = JObject.Parse(json)
			});
			//}

			// Stop the broker, wait for it to finish   
			// This isn't done after every message, but after you're
			// done with the broker

			apnsBroker.Stop();

			Console.WriteLine($"APNSBroker stopped");
		}

	}
}
