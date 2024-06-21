using Godot;
using KemoCard.Scripts;
using StaticClass;

namespace KemoCard.Pages
{
    public enum BagOpType
    {
        PUT_ON,
        PUT_OFF,
    }
    public partial class BagScene : BaseScene, IEvent
    {
        private PlayerRole playerData;
        [Export] EquipObject HalmetSlot;
        [Export] EquipObject ArmorSlot;
        [Export] EquipObject GloveSlot;
        [Export] EquipObject TrouserSlot;
        [Export] EquipObject ShoeSlot;
        [Export] EquipObject WeaponSlot1;
        [Export] EquipObject WeaponSlot2;
        [Export] EquipObject OtherSlot1;
        [Export] EquipObject OtherSlot2;
        [Export] EquipObject OtherSlot3;
        [Export] EquipObject OtherSlot4;
        [Export] EquipObject OtherSlot5;
        [Export] TextEdit GMEdit;
        [Export] Godot.Button AddBtn;
        [Export] Godot.Button DeleteBtn;
        [Export] public PopupMenu popup;
        [Export] FlowContainer flowContainer;
        [Export] Godot.Button QuitBtn;
        [Export] Control DebugControl;
        private Equip popupdata;
        private uint bagIndex;
        private BagOpType opType = BagOpType.PUT_ON;
        public override void OnAdd(dynamic data)
        {
            if ((data == null && data[0] != null) || data[0] is not PlayerRole)
            {
                StaticInstance.windowMgr.RemoveScene(this);
                return;
            }
            else
            {
                playerData = data[0];
            }
            var prefab = ResourceLoader.Load<PackedScene>("res://Pages/EquipObject.tscn");
            if (prefab != null)
            {
                for (int i = 0; i < StaticEnums.BagCount; i++)
                {
                    var node = prefab.Instantiate<EquipObject>();
                    node.Init(playerData.EquipList[i]);
                    node.AddBagDelegate(1);
                    flowContainer.AddChild(node);
                }
            }
            AddBtn.Pressed += new(() =>
            {
                uint eqid = (uint)GMEdit.Text.ToInt();
                if (Datas.Ins.EquipPool.ContainsKey(eqid))
                {
                    int idx = playerData.GetFirstEmptyBagIndex();
                    if (idx >= 0)
                    {
                        playerData.AddEquipToBag(new(eqid, playerData));
                    }
                    else
                    {
                        StaticInstance.MainRoot.ShowBanner("无空闲空间");
                    }
                }
                else
                {
                    StaticInstance.MainRoot.ShowBanner("无此id装备");
                }
            });

            popup.IndexPressed += new((index) =>
            {
                switch (index)
                {
                    case 0:
                        if (popupdata != null && playerData != null)
                        {
                            switch (opType)
                            {
                                case BagOpType.PUT_ON:
                                    playerData.PutOnEquip(bagIndex);
                                    break;
                                case BagOpType.PUT_OFF:
                                    playerData.PutOffEquip((uint)popupdata.EquipType);
                                    break;
                            }
                        }
                        break;
                    case 1:
                        if (playerData != null)
                        {
                            if (opType == BagOpType.PUT_OFF)
                            {
                                StaticInstance.MainRoot.ShowBanner("已装备的装备无法直接丢弃");
                                return;
                            }
                            var equip = playerData.EquipList[bagIndex];
                            equip.owner = null;
                            equip = null;
                            playerData.EquipList[bagIndex] = null;
                        }
                        break;
                }
                UpdateScene();
            });

            QuitBtn.Pressed += new(() =>
            {
                StaticInstance.windowMgr.RemoveScene(this);
            });

            HalmetSlot.AddBagDelegate(2);
            ArmorSlot.AddBagDelegate(2);
            GloveSlot.AddBagDelegate(2);
            TrouserSlot.AddBagDelegate(2);
            ShoeSlot.AddBagDelegate(2);
            WeaponSlot1.AddBagDelegate(2);
            WeaponSlot2.AddBagDelegate(2);
            OtherSlot1.AddBagDelegate(2);
            OtherSlot2.AddBagDelegate(2);
            OtherSlot3.AddBagDelegate(2);
            OtherSlot4.AddBagDelegate(2);
            OtherSlot5.AddBagDelegate(2);
            DebugControl.Visible = OS.IsDebugBuild();
            UpdateScene();
        }

        public override void OnRemove()
        {
            foreach (EquipObject obj in flowContainer.GetChildren())
            {
                obj.Data = null;
                obj.Dispose();
            }
            popupdata = null;
        }

        void UpdateScene()
        {
            if (playerData != null)
            {
                for (uint i = 1; i <= 12; i++)
                {
                    if (playerData.EquipDic.ContainsKey(i) && playerData.EquipDic[i] != null)
                    {
                        switch (i)
                        {
                            case 1:
                                HalmetSlot.Init(playerData.EquipDic[i]); break;
                            case 2:
                                ArmorSlot.Init(playerData.EquipDic[i]); break;
                            case 3:
                                GloveSlot.Init(playerData.EquipDic[i]); break;
                            case 4:
                                TrouserSlot.Init(playerData.EquipDic[i]); break;
                            case 5:
                                ShoeSlot.Init(playerData.EquipDic[i]); break;
                            case 6:
                                WeaponSlot1.Init(playerData.EquipDic[i]); break;
                            case 7:
                                WeaponSlot2.Init(playerData.EquipDic[i]); break;
                            case 8:
                                OtherSlot1.Init(playerData.EquipDic[i]); break;
                            case 9:
                                OtherSlot2.Init(playerData.EquipDic[i]); break;
                            case 10:
                                OtherSlot3.Init(playerData.EquipDic[i]); break;
                            case 11:
                                OtherSlot4.Init(playerData.EquipDic[i]); break;
                            case 12:
                                OtherSlot5.Init(playerData.EquipDic[i]); break;
                        }
                    }
                }
                for (uint i = 0; i < StaticEnums.BagCount; i++)
                {
                    flowContainer.GetChild<EquipObject>((int)i).Init(playerData.EquipList[i]);
                }
            }
        }

        public void ShowPopMenu(Equip data, uint bagIndex, Vector2 Position, BagOpType type)
        {
            if (data != null)
            {
                popup.Visible = true;
                popup.Position = (Vector2I)Position;
                popupdata = data;
                this.bagIndex = bagIndex;
                opType = type;
            }
        }

        public void ReceiveEvent(string @event, dynamic datas)
        {
            if (@event == "PlayerEquipUpdated")
            {
                UpdateScene();
            }
        }

    }
}