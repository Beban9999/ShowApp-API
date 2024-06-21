using System;
namespace AppApi.Models.Chat
{
	public class RoomRequest
	{
		public int receiverId { get; set; }
		public int senderId { get; set; }
	}
}

