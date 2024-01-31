using System;
namespace AppApi.Models.Chat
{
	public class Message
	{
        public Message(int roomId, string senderId, string receiverId, string content, string date, string timestamp)
        {
            this.roomId = roomId;
            this.senderId = senderId;
            this.receiverId = receiverId;
            this.content = content;
            this.date = date;
            this.timestamp = timestamp;
        }

        public int roomId { get; set; }
		public string senderId { get; set; }
		public string receiverId { get; set; }
		public string content { get; set; }
		public string date { get; set; }
		public string timestamp { get; set; }

	}
}

