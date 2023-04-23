using Common;

namespace GameServer.Services
{
/*
 
 对于添加数据可以用地址指针去操作保存
 对于删除数据只能通过操作数据库保存 
 
 */
    internal class DBService : Singleton<DBService>
    {
        private ExtremeWorldEntities entities; // 这个在APPConfig中命名

        public ExtremeWorldEntities Entities => entities;

        public void Init() => entities = new ExtremeWorldEntities();

        public void Save(bool async = false)
        {
            if (async) entities.SaveChangesAsync();
            else entities.SaveChanges();
        }
    }
}