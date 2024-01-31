using System;
namespace AppApi.Models.Chat
{
	public class Room
	{
        public Room(string roomName, string avatar, int roomId, string senderId, string senderuser, string receiverId, string receiveruser)
        {
            this.roomName = roomName;
            this.avatar = avatar;
            this.roomId = roomId;
            users = new List<RoomUser>
            {
                new RoomUser(senderId, senderuser),
                new RoomUser(receiverId, receiveruser)
            };
        }

        public string roomName { get; set; }
		public string avatar { get; set; }
		public int roomId { get; set; }
        public List<RoomUser> users { get; set; }
    }
}

