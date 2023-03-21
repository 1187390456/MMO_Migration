using Common;
using log4net;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CustomTools
{
    /// <summary>
    /// 通用日志输出类 重写Unity日志
    /// </summary>
    public class UnityLogger
    {
        public static void Init()
        {
            Application.logMessageReceived += OnLogMessageReceived;
            Log.Init("Unity");
        }

        private static ILog log = LogManager.GetLogger("Unity");

        private static void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    log.ErrorFormat("{0}\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                    break;

                case LogType.Assert:
                    log.DebugFormat("{0}\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                    break;

                case LogType.Exception:
                    log.FatalFormat("{0\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                    break;

                case LogType.Warning:
                    log.WarnFormat("{0}\r\n{1}", condition, stackTrace.Replace("\n", "\r\n"));
                    break;

                default:
                    log.Info(condition);
                    break;
            }
        }
    }

    /// <summary>
    /// 通用工具
    /// </summary>
    public static class CommonTools
    {
        /// <summary>
        ///  封装的字符串判断 并弹窗
        /// </summary>
        public static bool TestString(string targetValue, string message, string title = "", MessageBoxType type = MessageBoxType.Information, string btnOK = "", string btnCancel = "")
        {
            if (string.IsNullOrEmpty(targetValue))
            {
                TipsConfig.Instance.ShowSystemTips(message, 2);
                return false;
            }
            return true;
        }

        /// <summary>
        ///  比较字符串判断 并弹窗
        /// </summary>
        public static bool CompareString(string value1, string value2, string message, string title = "", MessageBoxType type = MessageBoxType.Information, string btnOK = "", string btnCancel = "")
        {
            if (value1 != value2)
            {
                MessageBox.Show(message, title, type, btnOK, btnCancel);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 设置某个对象下的子对象状态
        /// </summary>
        public static void SetGameObjeActive(Transform transform, bool value)
        {
            for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(value);
        }

        /// <summary>
        /// 设置某个对象下 子对象 唯一激活状态
        /// </summary>
        public static void SetSoleActive(Transform transform, int activeIndex)
        {
            SetGameObjeActive(transform, false);
            transform.GetChild(activeIndex).gameObject.SetActive(true);
        }

        /// <summary>
        ///  设置某个集合对象下的 子对象 唯一激活状态
        /// </summary>
        public static void SetSoleActive(GameObject[] gobjs, int activeIndex)
        {
            for (int i = 0; i < gobjs.Length; i++) gobjs[i].SetActive(false);
            gobjs[activeIndex].SetActive(true);
        }

        /// <summary>
        ///  设置某个对象下的某些子对象的激活状态 互斥唯一 激活名称必须包含在数组中
        /// </summary>
        public static void SetActiveByName(Transform transform, string activeName, params string[] names)
        {
            for (int i = 0; i < names.Length; i++) transform.Find(names[i]).gameObject.SetActive(false);
            transform.Find(activeName).gameObject.SetActive(true);
        }

        /// <summary>
        /// 销毁指定父级的所有子级
        /// </summary>
        public static void DestoryAllChild(Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++) Object.Destroy(transform.GetChild(i).gameObject);
        }

        /// <summary>
        /// 销毁指定父级的所有子级
        /// </summary>
        public static void DestoryAllChild(List<GameObject> gos)
        {
            for (int i = 0; i < gos.Count; i++) Object.Destroy(gos[i]);
            gos.Clear();
        }

        /// <summary>
        /// 销毁指定游戏对象数组的全部
        /// </summary>
        public static void DestoryAllChild(GameObject[] target)
        {
            for (var i = 0; i < target.Length; i++) Object.Destroy(target[i].gameObject);
        }

        /// <summary>
        /// 强制自适应指定布局容器
        /// </summary>
        public static IEnumerator ForceUpdataContent(Transform target)
        {
            foreach (var fitter in target.GetComponentsInChildren<ContentSizeFitter>()) fitter.SetLayoutVertical();
            target.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            yield return new WaitForEndOfFrame();
            target.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    /// <summary>
    /// 游戏对象工具
    /// </summary>

    public class GameObjectTool
    {
        public static Vector3 LogicToWorld(NVector3 vector) => new Vector3(vector.X / 100f, vector.Z / 100f, vector.Y / 100f);

        public static Vector3 LogicToWorld(Vector3Int vector) => new Vector3(vector.x / 100f, vector.z / 100f, vector.y / 100f);

        public static float LogicToWorld(int val) => val / 100f;

        public static int WorldToLogic(float val) => Mathf.RoundToInt(val * 100f);

        public static NVector3 WorldToLogicN(Vector3 vector) => new NVector3()
        {
            X = Mathf.RoundToInt(vector.x * 100),
            Y = Mathf.RoundToInt(vector.z * 100),
            Z = Mathf.RoundToInt(vector.y * 100)
        };

        public static Vector3Int WorldToLogic(Vector3 vector) => new Vector3Int()
        {
            x = Mathf.RoundToInt(vector.x * 100),
            y = Mathf.RoundToInt(vector.z * 100),
            z = Mathf.RoundToInt(vector.y * 100)
        };

        public static bool EntityUpdate(NEntity entity, UnityEngine.Vector3 position, Quaternion rotation, float speed)
        {
            NVector3 pos = WorldToLogicN(position);
            NVector3 dir = WorldToLogicN(rotation.eulerAngles);
            int spd = WorldToLogic(speed);
            bool updated = false;
            if (!entity.Position.Equal(pos))
            {
                entity.Position = pos;
                updated = true;
            }
            if (!entity.Direction.Equal(dir))
            {
                entity.Direction = dir;
                updated = true;
            }
            if (entity.Speed != spd)
            {
                entity.Speed = spd;
                updated = true;
            }
            return updated;
        }
    }

    /// <summary>
    /// 其他工具 场景
    /// </summary>
    public class GameUtil
    {
        public static bool InScreen(Vector3 position) => Screen.safeArea.Contains(Camera.main.WorldToScreenPoint(position));
    }

    /// <summary>
    /// 其他工具 字符转换
    /// </summary>
    public class EnumUtil
    {
        public static string GetEnumDescription(Enum enumValue)
        {
            string str = enumValue.ToString();
            System.Reflection.FieldInfo field = enumValue.GetType().GetField(str);
            object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (objs == null || objs.Length == 0) return str;
            System.ComponentModel.DescriptionAttribute da = (System.ComponentModel.DescriptionAttribute)objs[0];
            return da.Description;
        }
    }

    /// <summary>
    /// Mono单例
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public bool global = true;
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null) instance = (T)FindObjectOfType<T>();
                return instance;
            }
        }

        private void Awake()
        {
            if (global)
            {
                if (instance != null && instance != gameObject.GetComponent<T>())
                {
                    Destroy(gameObject);
                    return;
                }
                DontDestroyOnLoad(gameObject);
                instance = gameObject.GetComponent<T>();
            }
            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }
    }

    /// <summary>
    /// 单例
    /// </summary>
    public class Singleton<T> where T : new()
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }
    }
}