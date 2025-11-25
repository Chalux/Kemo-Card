using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Godot;
using KemoCard.Scripts.Equips;
using static KemoCard.Scripts.Datas;
using static KemoCard.Scripts.StaticEnums;

namespace KemoCard.Scripts
{
    public class Equip
    {
        // 临时的属性修改都放在这里面，只有永久修改的才写代码改属性值
        public Dictionary<string, float> Symbol { get; } = new();

        public string Id { get; }

        public string Uuid { get; private set; }
        [JsonIgnore] public BaseRole Owner;
        [JsonIgnore] public EquipType EquipType { get; private set; } = EquipType.Other;

        public Equip(string id, BaseRole owner = null)
        {
            Owner = owner;
            Id = id;
            InitScript();
        }

        public Equip(string id, string uuid, Dictionary<string, float> symbol)
        {
            Id = id;
            Uuid = uuid;
            if (Ins.EquipPool.TryGetValue(Id, out var value))
            {
                _struct = value;
                EquipType = (EquipType)_struct.EquipType;
            }

            Symbol = symbol;
        }

        [JsonIgnore] public EquipImplBase EquipScript { get; private set; } = new();

        [JsonIgnore] private EquipStruct _struct;

        private void InitScript()
        {
            EquipScript = new EquipImplBase();
            EquipScript.Binder = this;
            if (Ins.EquipPool.TryGetValue(Id, out var value))
            {
                _struct = value;
                var script = EquipFactory.CreateEquip(Id);
                script?.OnEquipInit(EquipScript);

                EquipType = (EquipType)_struct.EquipType;
            }
            else
            {
                var errorLog = "未在脚本库中找到对应装备id，请检查Mod配置。id:" + Id;
                StaticInstance.MainRoot.ShowBanner(errorLog);
                GD.PrintErr(errorLog);
            }

            EquipScript.OnCreated?.Invoke();
            Uuid = Guid.NewGuid().ToString();
        }

        ~Equip()
        {
            Owner = null;
        }

        public override string ToString()
        {
            var str = "";
            if (EquipScript == null) return str;
            str += EquipScript.Name != "" ? EquipScript.Name + "\n" : "";
            str += EquipScript.Desc != "" ? EquipScript.Desc + "\n" : "";
            foreach (var cardId in EquipScript.CardDic.Keys)
            {
                if (!Ins.CardPool.TryGetValue(cardId, out var cardInfo)) continue;
                str += StaticUtils.MakeColorString(cardInfo.Alias, StaticUtils.GetFrameColorByRare(cardInfo.Rare));
                str += "\n";
                str += cardInfo.Desc;
                str += "\n";
            }

            return str;
        }
    }
}