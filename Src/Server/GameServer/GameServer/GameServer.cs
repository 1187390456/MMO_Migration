using GameServer.Managers;
using GameServer.Services;
using Network;
using System.Threading;

namespace GameServer
{
    internal class GameServer
    {
        private Thread thread;
        private bool running = false;
        private NetService network;

        public bool Init()
        {
            int Port = Properties.Settings.Default.ServerPort;
            network = new NetService();
            network.Init(Port);

            DBService.Instance.Init();
            DataManager.Instance.Load();
            MapService.Instance.Init();
            UserSerevice.Instance.Init();
            ItemService.Instance.Init();
            QuestService.Instance.Init();
            FriendService.Instance.Init();
            TeamService.Instance.Init();
            GuildService.Instance.Init();

            thread = new Thread(new ThreadStart(this.Update));
            return true;
        }

        public void Start()
        {
            network.Start();
            running = true;
            thread.Start();
        }

        public void Stop()
        {
            running = false;
            thread.Join();
            network.Stop();
        }

        public void Update()
        {
            while (running)
            {
                TimeUtil.Tick();
                Thread.Sleep(100);
                //Console.WriteLine("{0} {1} {2} {3} {4}", Time.deltaTime, Time.frameCount, Time.ticks, Time.time, Time.realtimeSinceStartup);

                MapService.Instance.Update();
            }
        }
    }
}