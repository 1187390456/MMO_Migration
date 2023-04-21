using CustomTools;
using Entities;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    /// <summary>
    /// 游戏对象管理器
    /// </summary>

    public class GameObjectManager : MonoSingleton<GameObjectManager>
    {
        private Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>(); // 当前游戏对象字典 实体id 对应 游戏对象

        protected override void OnAwake()
        {
            StartCoroutine(InitGameObjects());
            CharacterManager.Instance.OnCharacterEnter += OnCharacterEnter;
            CharacterManager.Instance.OnCharacterLeave += OnCharacterLeave;
        }

        private void OnDestroy()
        {
            CharacterManager.Instance.OnCharacterEnter -= OnCharacterEnter;
            CharacterManager.Instance.OnCharacterLeave -= OnCharacterLeave;
        }

        // 角色进入回调
        public void OnCharacterEnter(Character cha) => CreateCharacterObject(cha);

        // 角色离开回调
        public void OnCharacterLeave(Character cha)
        {
            if (!Characters.ContainsKey(cha.entityId)) return;
            if (Characters[cha.entityId] != null)
            {
                Destroy(Characters[cha.entityId]);
                Characters.Remove(cha.entityId);
            }
        }

        // 查找所有角色然后创建
        private IEnumerator InitGameObjects()
        {
            foreach (var cha in CharacterManager.Instance.Characters.Values)
            {
                CreateCharacterObject(cha);
                yield return null;
            }
        }

        // 创建角色
        private void CreateCharacterObject(Character character)
        {
            if (!Characters.ContainsKey(character.entityId) || Characters[character.entityId] == null)
            {
                Object obj = Resloader.Load<Object>(character.Define.Resource);
                if (obj == null)
                {
                    Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.", character.Define.TID, character.Define.Resource);
                    return;
                }
                GameObject go = (GameObject)Instantiate(obj, transform);
                go.name = "Character_" + character.Id + "_" + character.Name;
                Characters[character.entityId] = go;

                UIWorldElementManager.Instance.AddCharacterNameBar(go.transform, character);
            }
            InitGameObject(Characters[character.entityId], character);
        }

        //  初始化游戏对象
        private void InitGameObject(GameObject go, Character character)
        {
            // 赋值坐标和方向
            go.transform.position = GameObjectTool.LogicToWorld(character.position);
            go.transform.forward = GameObjectTool.LogicToWorld(character.direction);

            // 判断是否是玩家
            EntityController ec = go.GetComponent<EntityController>();
            if (ec != null)
            {
                ec.entity = character;
                ec.isPlayer = character.IsCurrentPlayer;
            }
            PlayerInputController pc = go.GetComponent<PlayerInputController>();
            if (pc != null)
            {
                if (character.IsCurrentPlayer)
                {
                    User.Instance.CurrentCharacterObject = go;
                    MainPlayerCamera.Instance.player = go;
                    pc.enabled = true;
                    pc.character = character;
                    pc.entityController = ec;
                }
                else pc.enabled = false;
            }
        }
    }
}