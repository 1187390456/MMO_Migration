using GameServer;
using GameServer.Entities;
using GameServer.Services;

//using GameServer.Entities;
using SkillBridge.Message;

namespace Network
{
    public class NetSession : INetSession
    {
        public TUser User { get; set; }

        public Character Character { get; set; }
        public NEntity Entity { get; set; }

        public IPostResponser PostResponser { get; set; }

        public void Disconnected()
        {
            PostResponser = null;
            if (Character != null) UserSerevice.Instance.CharacterLeave(Character);
        }

        private NetMessage response;

        public NetMessageResponse Response
        {
            get
            {
                if (response == null)
                {
                    response = new NetMessage();
                }
                if (response.Response == null)
                    response.Response = new NetMessageResponse();
                return response.Response;
            }
        }

        public byte[] GetResponse()
        {
            if (response != null)
            {
                if (PostResponser != null) PostResponser.PostProcess(Response);

                byte[] data = PackageHandler.PackMessage(response);
                response = null;
                return data;
            }
            return null;
        }
    }
}